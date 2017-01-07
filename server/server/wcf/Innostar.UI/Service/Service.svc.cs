using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml.Linq;
using Application.Infrastructure.NotificationsManagement;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using Innostar.Dal.Infrastructure;
using Innostar.Dal.Repositories;
using Innostar.Models;
using Innostar.UI.Helpers;
using Microsoft.Ajax.Utilities;
using WebMatrix.WebData;

namespace Innostar.UI.Service
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed), ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Service : IService
    {
        public const string DATE_FORMAT = "dd-MM-yyyy-HH-mm";

        #region Authentication

        public AuthData UserAuth(string login, string password, string devicetoken)
        {
            using (var context = new InnostarModelsContext())
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                ChatUserRepository userRepository = new ChatUserRepository(context);
                ChatUser user = userRepository._Get(e => e.Login == login).FirstOrDefault();

                if (user == null || user.Disabled || user.Login != login)
                {
                    return new AuthData() { code = "404", message = "Пользователь не найден" };
                }

                PasswordUtility pu = new PasswordUtility(user.PasswordSalt, user.PasswordHash);
                if (!pu.Verify(password.ToCharArray()))
                {
                    return new AuthData() { code = "403", message = "Неверные данные для авторизации." };
                }
                string token = GetOrCreateAuthToken(user.Id);
                SetDeviceToken(user.XmppLogin, devicetoken);
                string xmppPassword = Guid.NewGuid().ToString().Replace("-", "");
                string xmppResult = UpdateOrAddXmppUser("update", user.XmppLogin, xmppPassword, null);
                if (xmppResult != null)
                {
                    return new AuthData()
                    {
                        code = "403",
                        message = string.Format("Проблема на XMPP сервере {0}", xmppResult)
                    };
                }

                userRepository._Update(user);
                userRepository._Save();

                var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
                var currentTime = DateTime.Now.Ticks / 10000;

                AuthData authData = new AuthData();
                authData.code = "200";
                authData.message = "OK";
                authData.xmpp_password = xmppPassword;
                authData.access_token = token;
                authData.user_data = GetDataFromUserProfile(user);
                authData.current_time = (currentTime - startTime).ToString(CultureInfo.InvariantCulture);
                authData.admin_jid = string.Format("{0}@{1}",
                    ConfigurationManager.AppSettings["XmppAdminLogin"],
                    ConfigurationManager.AppSettings["XmppServerName"]);
                return authData;
            }
        }

        private static bool IsAuthorized()
        {
            return true;
            //UserTokenRepository tokenRepository = new UserTokenRepository();
            //string token = WebOperationContext.Current.IncomingRequest.Headers["Authorization"];
            //try
            //{
            //    Guid guid = new Guid(token);
            //    return tokenRepository.Get(x => x.Token == guid).Any();
            //}
            //catch
            //{
            //    return false;
            //}
        }

        public static bool IsAuthorized(CommonDataContract dataToReturn)
        {
            if (!IsAuthorized())
            {
                dataToReturn.code = "401";
                dataToReturn.message = "Требуется авторизация";
                return false;
            }
            return true;
        }

        private static string GetOrCreateAuthToken(int userId)
        {
            using (var context = new InnostarModelsContext())
            {
                UserTokenRepository tokenRepository = new UserTokenRepository(context);
                var exist = tokenRepository._Get(x => x.UserId == userId).FirstOrDefault();
                if (exist != null)
                {
                    return exist.Token.ToString();
                }
                Guid guid = Guid.NewGuid();
                UserToken token = new UserToken();
                token.UserId = userId;
                token.Token = guid;
                token.CreatedOn = DateTime.Now;
                tokenRepository._Insert(token);
                tokenRepository._Save();
                return guid.ToString();
            }
        }

        private static void SetDeviceToken(string xmppLogin, string devicetoken)
        {
            List<string> rosterUsers = new List<string>();
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["OpenfireConnection"];

            SetDeviceTokenService(xmppLogin, devicetoken);

            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();

                var existFewDeviceTokensCommand = connection.CreateCommand();
                existFewDeviceTokensCommand.CommandText = string.Format(@"select * from devicetokens where devicetoken = '{0}'", devicetoken);
                var existFewReader = existFewDeviceTokensCommand.ExecuteReader();
                if (existFewReader.HasRows)
                {
                    existFewReader.Close();
                    using (var updateDeviceTokensCommand = connection.CreateCommand())
                    {
                        updateDeviceTokensCommand.CommandText = string.Format(@"update devicetokens set devicetoken = '0' where devicetoken = '{0}'", devicetoken);
                        updateDeviceTokensCommand.ExecuteNonQuery();
                    }
                }
                existFewReader.Close();

                var existDeviceTokensCommand = connection.CreateCommand();
                existDeviceTokensCommand.CommandText = string.Format(@"select * from devicetokens where username = '{0}'", xmppLogin);

                var existReader = existDeviceTokensCommand.ExecuteReader();
                if (existReader.HasRows)
                {
                    existReader.Close();
                    var updateDeviceTokensCommand = connection.CreateCommand();
                    updateDeviceTokensCommand.CommandText = string.Format(@"update devicetokens set devicetoken = '{1}' where username = '{0}'", xmppLogin, devicetoken);
                    updateDeviceTokensCommand.ExecuteNonQuery();
                }
                else
                {
                    existReader.Close();
                    var insertDeviceTokensCommand = connection.CreateCommand();
                    insertDeviceTokensCommand.CommandText = string.Format(@"insert into devicetokens values ('{0}','{1}')", xmppLogin, devicetoken);
                    insertDeviceTokensCommand.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        private static void SetDeviceTokenService(string xmppLogin, string devicetoken)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["OpenfireConnection"];
                using (var openfireConn = new SqlConnection(connectionString.ConnectionString))
                {
                    openfireConn.Open();

                    App42API.Initialize(ConfigurationManager.AppSettings["PushServiceKey"], ConfigurationManager.AppSettings["PushServiceSecretKey"]);
                    var pushNotificationService = App42API.BuildPushNotificationService();

                    var sqlCommand = openfireConn.CreateCommand();
                    sqlCommand.CommandText =
                        "SELECT username, devicetoken FROM devicetokens " +
                        "where username = @username or devicetoken = @devicetoken";
                    sqlCommand.Parameters.Add("@username", SqlDbType.NVarChar, 1024).Value = xmppLogin;
                    sqlCommand.Parameters.Add("@devicetoken", SqlDbType.NVarChar, 1024).Value = devicetoken;
                    sqlCommand.Prepare();

                    using (var dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            //pushNotificationService.DeleteDeviceToken(Convert.ToString(dataReader[0]), Convert.ToString(dataReader[1]));
                            DeleteTokenFromService(pushNotificationService, Convert.ToString(dataReader[0]),
                                Convert.ToString(dataReader[1]));
                        }
                    }

                    pushNotificationService.StoreDeviceToken(xmppLogin, devicetoken, DeviceType.iOS);
                    openfireConn.Close();
                }
            }
            catch (Exception ex)
            {
                //to do something
            }
        }

        static private void DeleteTokenFromService(PushNotificationService service, string user, string token)
        {
            try
            {
                service.DeleteDeviceToken(user, token);
            }
            catch (Exception)
            {
                // to do something
            }
        }

        private static string UpdateOrAddXmppUser(string requestType, string login, string password, string name)
        {
            try
            {
                string requestStr =
                    string.Format("{0}/plugins/userService/userservice?type={1}&secret={2}&username={3}",
                        ConfigurationManager.AppSettings["XmppServerUrl"],
                        requestType,
                        ConfigurationManager.AppSettings["UserServiceBigSecret"],
                        login);

                if (!string.IsNullOrEmpty(password))
                {
                    requestStr += string.Format("&password={0}", password);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    requestStr += string.Format("&name={0}", name);
                }

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestStr);
                WebResponse response = req.GetResponse();
                XDocument document = XDocument.Load(response.GetResponseStream());
                var resultResp = document.Element("result");
                if (resultResp != null && resultResp.Value.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                var errResp = document.Element("error");
                if (errResp != null)
                {
                    return errResp.Value;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "unknown error";
        }

        private static string GetXmppLogin(string login)
        {
            return string.Format("xmpp_{0}", login).ToLower(CultureInfo.InvariantCulture);
        }

        //public AuthData Register(string login, string password, string name, string email, string phone, string recovertype, string question, string answer, string imageTitle, string imageContent)
        //{
        //    return Service.RegisterUser(login, password, name, email, phone, recovertype, question, answer, imageTitle, imageContent);
        //}

        public AuthData Register(string login, string password, string name, string email, string phone, string recovertype, string question, string answer, string imageTitle, string imageContent)
        {
            AuthData result = new AuthData();
            string xmppLogin = GetXmppLogin(login);
            string xmppPassword = Guid.NewGuid().ToString().Replace("-", "");

            try
            {
                using (var context = new InnostarModelsContext())
                {
                    ChatUserRepository userRepository = new ChatUserRepository(context);
                    var uploadService = new UploadService();
                    ChatUser user = userRepository._Get(u => u.Login.Equals(login)).FirstOrDefault();
                    if (user == null)
                    {
                        string xmppResult = UpdateOrAddXmppUser("add", xmppLogin, xmppPassword, name ?? xmppLogin);

                        if (xmppResult == null)
                        {
                            user = new ChatUser();
                            user.Login = login;
                            user.Name = name ?? login;
                            user.Email = email;
                            user.Phone = phone;
                            user.XmppLogin = xmppLogin;
                            if (!string.IsNullOrEmpty(imageContent))
                            {
                                byte[] buffer = Convert.FromBase64String(imageContent);
                                Stream stream = new MemoryStream(buffer);
                                int imageType = 0;
                                var extension = Path.GetExtension(imageTitle);
                                if (extension != null)
                                {
                                    switch (extension.ToUpper())
                                    {
                                        case ".PNG":
                                        case ".JPG":
                                        case ".JPEG":
                                        case ".GIF":
                                        case ".BMP":
                                        case ".TIFF":
                                            imageType = 1;
                                            break;
                                        case ".MP3":
                                        case ".M4A":
                                            imageType = 2;
                                            break;
                                    }
                                }
                                UploadData uploadResult = uploadService.UploadFile(imageTitle, imageType, stream);
                                if (uploadResult.code == "200")
                                {
                                    user.ImageId = uploadResult.id;
                                }
                            }

                            user.Password = password;
                            PasswordUtility pu = new PasswordUtility(password.ToCharArray());
                            user.PasswordHash = pu.Hash;
                            user.PasswordSalt = pu.Salt;
                            var rnd = new Random();
                            user.Uuid = Guid.NewGuid().ToString().Replace("-", string.Empty) +
                                        rnd.Next(9999).ToString(CultureInfo.InvariantCulture);

                            user.PasswordRecoveryAnswer = answer;
                            user.PasswordRecoveryQuestion = question;
                            user.PasswordRecoveryType = int.Parse(recovertype);
                            user.RegistrationDate = DateTime.Now;
                            user.LastActivityTime = DateTime.Now;
                            user.SafeModeStartDate = new DateTime(1800, 1, 1);
                            user.SafeModeEndDate = new DateTime(1800, 1, 1);
                            user.MessageStorageType = (int)MessageStorageTypeEnum.Forever;
                            user.IsMessagesArchived = true;
                            user.TimeoutTypeClosedConference = (int)TimeoutClosedRoomEnum.Minutes10;
                            user.IsConfirmedDelivery = true;
                            userRepository._Insert(user);
                            userRepository._Save();

                            result.code = "200";
                            result.message = "OK";
                            result.xmpp_password = xmppPassword;
                            result.access_token = GetOrCreateAuthToken(user.Id);
                            result.user_data = GetDataFromUserProfile(user);

                            return result;

                            //if (user.PasswordRecoveryType == 1)
                            //{
                            //    result.mail_data = SendConfirmationMailToUser(context, user);
                            //    return result;
                            //}
                            //else
                            //{
                            //    return result;
                            //}                
                        }
                        else
                        {
                            result.message = string.Format("Пользователя создать не удалось на сервере XMPP. {0}",
                                xmppResult);
                            result.code = "403";
                            return result;
                        }
                    }
                    else
                    {
                        result.code = "409";
                        result.message = "Такой пользователь существует";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.message = string.Format("Пользователя создать не удалось. {0}", ex.Message);
                result.code = "403";
                return result;
            }
        }

        public CommonDataContract DeleteUser(string uid)
        {

            string message = string.Empty;
            string code = "200";

            try
            {
                using (var context = new InnostarModelsContext())
                {
                    int userId = int.Parse(uid);
                    var userRepository = new ChatUserRepository(context);

                    var user = userRepository._Find(userId);

                    string xmppResult = UpdateOrAddXmppUser("delete", user.XmppLogin, null, null);

                    if (xmppResult == null)
                    {
                        message += "remove xmpp user succesfull. ";
                    }
                    else
                    {
                        code = "500";
                        message += xmppResult;
                    }

                    user.Disabled = true;

                    userRepository._Update(user);
                    userRepository._Save();
                    message += "remove service user succesfull. ";
                }
            }
            catch (Exception ex)
            {
                code = "500";
                message += ex.Message;
            }
            return new CommonDataContract
            {
                code = code,
                message = message
            };
        }

        public CommonDataContract AddRoomKey(string roomJid, string key)
        {
            try
            {
                using (var context = new InnostarModelsContext())
                {
                    RoomKeyRepository repository = new RoomKeyRepository(context);
                    int existingRoomsCount = repository._All.Count(e => e.RoomId == roomJid);
                    if (existingRoomsCount == 0)
                    {
                        var roomKey = new RoomKey
                        {
                            RoomId = roomJid,
                            RoomPasswordKey = key,
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now
                        };

                        repository._Insert(roomKey);
                        repository._Save();

                        return new CommonDataContract
                        {
                            code = "200",
                            message = "success"
                        };
                    }
                    else
                    {
                        throw new Exception("jid room exists");
                    }
                }
            }
            catch (Exception ex)
            {
                return new CommonDataContract
                {
                    code = "500",
                    message = ex.Message
                };
            }
        }

        public CommonDataContract GetRoomKey(string roomJid)
        {
            try
            {
                using (var context = new InnostarModelsContext())
                {
                    RoomKeyRepository repository = new RoomKeyRepository(context);
                    int existingRoomsCount = repository._All.Count(e => e.RoomId == roomJid);
                    if (existingRoomsCount > 0)
                    {
                        var key = repository._All.FirstOrDefault(e => e.RoomId == roomJid).RoomPasswordKey;
                        return new CommonDataContract
                        {
                            code = "200",
                            message = "success",
                            response_date = key
                        };
                    }
                    else
                    {
                        throw new Exception("this room doesn't exists");
                    }
                }
            }
            catch (Exception ex)
            {
                return new CommonDataContract
                {
                    code = "500",
                    message = ex.Message
                };
            }
        }

        public RecoverPasswordData RecoverPassword(string info)
        {
            var result = new RecoverPasswordData
            {
                code = "404"
            };

            try
            {
                using (var dataContext = new InnostarModelsContext())
                {
                    Match match = Regex.Match(info,
                        @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$",
                        RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        result.IsEmail = 1;

                        var repository = new ChatUserRepository(dataContext);
                        var users = repository._All.Where(e => e.Email == info);
                        if (users.Any())
                        {
                            result.ExistEmail = 1;
                            var user = users.FirstOrDefault();
                            if (user.PasswordRecoveryType == 1)
                            {
                                return SendRecoverMailToUser(dataContext, user);
                            }
                        }
                    }

                    result.IsLogin = 1;

                    var usersRepository = new ChatUserRepository(dataContext);
                    var usersByLogin = usersRepository._All.Where(e => e.Login == info);
                    if (usersByLogin.Any())
                    {
                        result.ExistLogin = 1;
                        var user = usersByLogin.FirstOrDefault();
                        if (user.PasswordRecoveryType == 2)
                        {
                            result.TypeQuestion = 1;

                            result.code = "200";
                            result.message = "question";
                            result.login = user.Login;
                            result.question = user.PasswordRecoveryQuestion;
                            return result;
                        }
                        else if (user.PasswordRecoveryType == 1)
                        {
                            return SendRecoverMailToUser(dataContext, user);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.code = "500";
                result.message = "server error";
                result.response_date = ex.Message;
                return result;
            }


            return result;
        }

        private RecoverPasswordData SendRecoverMailToUser(InnostarModelsContext dataContext, ChatUser user)
        {
            var result = new RecoverPasswordData
            {
                code = "404"
            };

            try
            {
                //if (user.Disabled)
                //{
                //    return new RecoverPasswordData() { code = "403", message = "Email пользователя не активирован" };
                //}
                result.TypeEmail = 1;

                var recoverRepository = new RecoverPasswordRequestRepository(dataContext);
                var rnd = new Random();
                var requestKey = rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                 + Guid.NewGuid().ToString().Replace("-", string.Empty)
                                 + rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                 + Guid.NewGuid().ToString().Replace("-", string.Empty);

                var request = new RecoverPasswordRequest
                {
                    IsActive = true,
                    RequestAcceptedTime = DateTime.Now,
                    RequestKey = requestKey,
                    User = user
                };

                recoverRepository._Insert(request);
                recoverRepository._Save();

                var templateRepository = new MessageTemplateRepository(dataContext);
                var template = templateRepository._Get(e => e.SpecialTemplate == 1).FirstOrDefault().Message;
                var message = template.Replace("%userEmail%", user.Email)
                    .Replace("%userPasswordRequest%", GetRecoverLink(requestKey))
                    .Replace("%userDisplayName%", user.Name);

                MailSender.SendMail(user.Email, "Восстановление пароля пользователя MegaTalk", message);

                result.code = "200";
                result.message = "success";
                return result;
            }
            catch (Exception ex)
            {
                result.code = "500";
                result.message = "server error";
                result.response_date = ex.Message;
                return result;
            }
        }

        private CommonDataContract SendConfirmationMailToUser(InnostarModelsContext dataContext, ChatUser user)
        {
            var result = new CommonDataContract
            {
                code = "404"
            };

            try
            {
                var repository = new ConfirmEmailRequestRepository(dataContext);
                var rnd = new Random();
                var requestKey = rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                 + Guid.NewGuid().ToString().Replace("-", string.Empty)
                                 + rnd.Next(99).ToString(CultureInfo.InvariantCulture)
                                 + Guid.NewGuid().ToString().Replace("-", string.Empty);

                var request = new ConfirmEmailRequest
                {
                    IsActive = true,
                    RequestAcceptedTime = DateTime.Now,
                    RequestKey = requestKey,
                    User = user
                };

                repository._Insert(request);
                repository._Save();

                using (var mail = new MailMessage(
                        ConfigurationManager.AppSettings["PasswordSenderAddress"], user.Email))
                {
                    mail.Subject = "Подтверждение почтового адреса пользователя MegTalk";

                    var templateRepository = new MessageTemplateRepository(dataContext);
                    var template = templateRepository._Get(e => e.SpecialTemplate == 2).FirstOrDefault().Message;
                    var message = template.Replace("%userEmail%", user.Email)
                        .Replace("%userConfirmRequest%", GetConfirmLink(requestKey))
                        .Replace("%userDisplayName%", user.Name);

                    mail.Body = message;

                    using (var sc = new SmtpClient(ConfigurationManager.AppSettings["SmtpClient"],
                        Convert.ToInt32(ConfigurationManager.AppSettings["SmtpClientPort"])))
                    {
                        sc.EnableSsl = true;
                        sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                        sc.UseDefaultCredentials = false;
                        sc.Credentials =
                            new NetworkCredential(
                                ConfigurationManager.AppSettings["PasswordSenderAddress"],
                                ConfigurationManager.AppSettings["PasswordSenderPassword"]);
                        sc.Send(mail);
                    }
                }

                result.code = "300";
                result.message = "success";
                result.response_date = "Письмо с ссылкой для подтверждения email выслано";
                return result;
            }
            catch (Exception ex)
            {
                result.code = "500";
                result.message = "server error";
                result.response_date = ex.Message;
                return result;
            }
        }

        public CommonDataContract RecoverPasswordByAnswer(string login, string answer)
        {
            try
            {
                using (var datacontext = new InnostarModelsContext())
                {
                    var usersRepository = new ChatUserRepository(datacontext);
                    var userByLogin = usersRepository._All.Where(e => e.Login == login);
                    if (userByLogin.Any())
                    {
                        var user = userByLogin.FirstOrDefault();
                        if (user.PasswordRecoveryType == 2)
                        {
                            if (user.PasswordRecoveryAnswer == answer)
                            {
                                var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                                var newPassword = guid.Substring(0, 6);

                                user.Password = newPassword;
                                var pu = new PasswordUtility(newPassword.ToCharArray());
                                user.PasswordHash = pu.Hash;
                                user.PasswordSalt = pu.Salt;

                                usersRepository._Update(user);
                                usersRepository._Save();

                                return new CommonDataContract
                                {
                                    code = "200",
                                    message = "success",
                                    response_date = newPassword
                                };
                            }
                            else
                            {
                                return new CommonDataContract
                                {
                                    code = "404",
                                    message = "Неверный ответ"
                                };
                            }
                        }
                        else
                        {
                            return new CommonDataContract
                            {
                                code = "404",
                                message = "Данный пользователь указал другой способ восстановления пароля"
                            };
                        }
                    }
                    else
                    {
                        return new CommonDataContract
                        {
                            code = "404",
                            message = "Нет такого пользователя"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CommonDataContract
                {
                    code = "500",
                    message = "server error"
                };
            }
        }

        public BoolData IsUniqueEmail(string email)
        {
            try
            {
                using (var context = new InnostarModelsContext())
                {
                    var userRepository = new ChatUserRepository(context);
                    return new BoolData { Value = !userRepository._Get(chatUser => chatUser.Email.Equals(email)).Any() };
                }
            }
            catch (Exception)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = "status error";
            }
            return new BoolData();
        }

        public CommonDataContract ChangeSafeMode(string login, string activate)
        {
            using (var context = new InnostarModelsContext())
            {
                int number = Convert.ToInt32(activate);
                ChatUserRepository userRepository = new ChatUserRepository(context);
                int count = userRepository._Get(chatUser => chatUser.Login.Equals(login)).Count();
                if (count != 0)
                {
                    var user = userRepository._Get(chatUser => chatUser.Login.Equals(login)).FirstOrDefault();
                    user.SafeModeActivated = number > 0;
                    if (user.SafeModeActivated)
                    {
                        user.SafeModeStartDate = DateTime.Now;
                        user.SafeModeEndDate = DateTime.Now.AddYears(1);
                    }
                    else
                    {
                        user.SafeModeEndDate = DateTime.Now;
                    }
                    userRepository._Update(user);
                    userRepository._Save();
                    return new CommonDataContract()
                    {
                        code = "200",
                        message = "success",
                        response_date = activate
                    };
                }
                else
                {
                    return new CommonDataContract()
                    {
                        code = "400",
                        message = "не существует такого пользователя",
                        response_date = count.ToString(CultureInfo.InvariantCulture)
                    };
                }
            }
        }

        public CommonDataContract ClearDeviceToken(string xmpplogin)
        {
            try
            {
                SetDeviceToken(xmpplogin, "0");
                return new CommonDataContract()
                {
                    code = "200",
                    message = "success",
                    response_date = "0"
                };
            }
            catch (Exception)
            {
                return new CommonDataContract()
                {
                    code = "500",
                    message = "error",
                    response_date = "0"
                };
            }
        }

        #endregion

        #region Fill methods

        private static ChatUserData GetDataFromUserProfile(ChatUser user)
        {
            ChatUserData userData = new ChatUserData();
            userData.email = user.Email;
            userData.id = user.Id;
            userData.image_id = user.ImageId;
            userData.login = user.Login;
            userData.xmpp_login = string.Format("{0}@{1}", user.XmppLogin, ConfigurationManager.AppSettings["XmppServerName"]);
            userData.uuid = user.Uuid;
            userData.safe_mode_activated = user.SafeModeActivated && DateTime.Now < user.SafeModeEndDate ? 1 : 0;
            userData.MessageStorageType = user.MessageStorageType;
            userData.IsConfirmedDelivery = user.IsConfirmedDelivery;
            userData.IsMessagesArchived = user.IsMessagesArchived;
            userData.IsPasswordDeleteAllEnabled = user.IsPasswordDeleteAllEnabled;
            userData.TimeoutTypeClosedConference = user.TimeoutTypeClosedConference;
            userData.LastActivityTime = UnixTimeHelper.GetLongFromDate(user.LastActivityTime).ToString(CultureInfo.InvariantCulture);
            userData.name = user.Name;
            userData.phone = user.Phone;
            return userData;
        }

        private string GetRecoverLink(string requestKey)
        {
            return string.Format("{0}/Recover/Password?key={1}",
                ConfigurationManager.AppSettings["MailSenderHost"],
                requestKey);
        }

        private static string GetConfirmLink(string requestKey)
        {
            return string.Format("{0}/Home/ConfirmEmail?key={1}",
                ConfigurationManager.AppSettings["MailSenderHost"],
                requestKey);
        }

        #endregion

        #region User
        public ChatUserData GetUserProfile(string uid)
        {
            using (var context = new InnostarModelsContext())
            {
                ChatUserData result = new ChatUserData();
                if (!IsAuthorized(result))
                {
                    return result;
                }

                int userId = int.Parse(uid);

                ChatUserRepository userRepository = new ChatUserRepository(context);
                ChatUser user = userRepository._Find(userId);
                if (user == null)
                {
                    result.message = "Пользователь не найден";
                    result.code = "404";
                    return result;
                }
                result = GetDataFromUserProfile(user);
                result.message = "OK";
                result.code = "200";
                return result;
                ;
            }
        }

        public ChatUserData UpdateUser(string uid, string password, string name, string email,
            string phone, string recovertype, string question, string answer, string imageTitle,
            string messageStorageType, string isMessagesArchived, string isPasswordDeleteAllEnabled, 
            string timeoutTypeClosedConference, string isConfirmedDelivery, string imageContent, 
            string passwordDeleteAll, string lastActivityTime)
        {
            try
            {
                using (var context = new InnostarModelsContext())
                {
                    ChatUserData result = new ChatUserData();
                    if (!IsAuthorized(result))
                    {
                        return result;
                    }

                    int userId = int.Parse(uid);

                    ChatUserRepository userRepository = new ChatUserRepository(context);
                    var uploadService = new UploadService();
                    ChatUser user = userRepository._Find(userId);
                    if (user == null)
                    {
                        result.message = "Пользователь не найден";
                        result.code = "404";
                        return result;
                    }
                    else
                    {
                        string xmppResult = UpdateOrAddXmppUser("update", user.XmppLogin, null, name);
                        if (xmppResult != null)
                        {
                            result.message = string.Format("Пользователя обновить не удалось на сервере XMPP. {0}",
                                xmppResult);
                            result.code = "403";
                            return result;
                        }

                        if (!string.IsNullOrEmpty(name) && name != "0")
                            user.Name = name;
                        if (!string.IsNullOrEmpty(email) && email != "0")
                            user.Email = email;
                        if (!string.IsNullOrEmpty(phone) && phone != "0")
                            user.Phone = phone;

                        if (!string.IsNullOrEmpty(messageStorageType))
                        {
                            user.MessageStorageType = Convert.ToInt32(messageStorageType);
                        }

                        if (!string.IsNullOrEmpty(timeoutTypeClosedConference))
                        {
                            user.TimeoutTypeClosedConference = Convert.ToInt32(timeoutTypeClosedConference);
                        }

                        if (!string.IsNullOrEmpty(isConfirmedDelivery))
                        {
                            user.IsConfirmedDelivery = isConfirmedDelivery == "1" || isConfirmedDelivery == "true";
                        }

                        if (!string.IsNullOrEmpty(isMessagesArchived))
                        {
                            user.IsMessagesArchived = isMessagesArchived == "1" || isMessagesArchived == "true";
                        }

                        if (!string.IsNullOrEmpty(isPasswordDeleteAllEnabled))
                        {
                            user.IsPasswordDeleteAllEnabled = isPasswordDeleteAllEnabled == "1" || isPasswordDeleteAllEnabled == "true";
                        }

                        if (!string.IsNullOrEmpty(lastActivityTime))
                        {
                            user.LastActivityTime = UnixTimeHelper.GetDateFromLong(Convert.ToInt64(lastActivityTime));
                        }

                        user.Password = "1111111111";

                        if (!string.IsNullOrEmpty(imageContent))
                        {
                            byte[] buffer = Convert.FromBase64String(imageContent);
                            Stream stream = new MemoryStream(buffer);
                            int imageType = 0;
                            var extension = Path.GetExtension(imageTitle);
                            if (extension != null)
                            {
                                switch (extension.ToUpper())
                                {
                                    case ".PNG":
                                    case ".JPG":
                                    case ".JPEG":
                                    case ".GIF":
                                    case ".BMP":
                                    case ".TIFF":
                                        imageType = 1;
                                        break;
                                    case ".MP3":
                                    case ".M4A":
                                        imageType = 2;
                                        break;
                                }
                            }
                            UploadData uploadResult = uploadService.UploadFile(imageTitle, imageType, stream);
                            if (uploadResult.code == "200")
                            {
                                user.ImageId = uploadResult.id;
                            }
                        }

                        if (!string.IsNullOrEmpty(password) && password != "0")
                        {
                            PasswordUtility pu = new PasswordUtility(password.ToCharArray());
                            user.PasswordHash = pu.Hash;
                            user.PasswordSalt = pu.Salt;
                        }

                        if (!string.IsNullOrEmpty(passwordDeleteAll) && passwordDeleteAll != "0")
                        {
                            var pu = new PasswordUtility(passwordDeleteAll.ToCharArray());
                            user.PasswordDeleteAllHash = pu.Hash;
                            user.PasswordDeleteAllSalt = pu.Salt;
                        }

                        if (!string.IsNullOrEmpty(answer) && answer != "0")
                            user.PasswordRecoveryAnswer = answer;
                        if (!string.IsNullOrEmpty(question) && question != "0")
                            user.PasswordRecoveryQuestion = question;
                        if (!string.IsNullOrEmpty(recovertype) && recovertype != "0")
                            user.PasswordRecoveryType = int.Parse(recovertype);
                        userRepository._Update(user);
                        userRepository._Save();
                    }
                    result = GetDataFromUserProfile(user);
                    result.message = "OK";
                    result.code = "200";
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new ChatUserData { code = "500", message = ex.Message };
            }
        }

        public ChatUsersData GetRoster(string uid)
        {
            using (var context = new InnostarModelsContext())
            {
                ChatUsersData result = new ChatUsersData();
                if (!IsAuthorized(result))
                {
                    return result;
                }
                int userId = int.Parse(uid);

                List<string> rosterUsers = new List<string>();
                ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["OpenfireConnection"];

                ChatUserRepository userRepository = new ChatUserRepository(context);
                ChatUser user = userRepository._Find(userId);
                if (user == null)
                {
                    result.message = "Пользователь не найден";
                    result.code = "404";
                    return result;
                }

                using (SqlConnection connection = new SqlConnection(connectionString.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command =
                        new SqlCommand(string.Format(@"SELECT *  FROM ofRoster WHERE username = '{0}' AND sub = 3",
                            user.XmppLogin));
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string jid = reader["jid"].ToString();
                        rosterUsers.Add(jid);
                    }
                    connection.Close();

                }
                var chatUsers = rosterUsers.Join(userRepository._All, s => s, chatUser => chatUser.XmppLogin,
                    (s, chatUser) => chatUser);
                result.users = chatUsers.Select(GetDataFromUserProfile).ToList();
                result.message = "OK";
                result.code = "200";
                return result;
            }

        }

        public ChatUsersData GetProfilesByLogins(string logins)
        {
            using (var context = new InnostarModelsContext())
            {
                var result = new ChatUsersData();
                if (!IsAuthorized(result))
                {
                    return result;
                }
                var loginsList = logins.ToStringListFromDelimitedString(true).ToList();
                var userRepository = new ChatUserRepository(context);
                var chatUsers = loginsList.Join(userRepository._All.Where(e => !e.Disabled).ToList(), s => s, chatUser => chatUser.Login,
                    (s, chatUser) => chatUser);

                result.users = chatUsers.Select(GetDataFromUserProfile).ToList();
                result.message = "OK";
                result.code = "200";
                return result;
            }

        }

        public ChatUsersData GetProfilesByXmppLogins(string xmpplogins)
        {
            using (var context = new InnostarModelsContext())
            {
                ChatUsersData result = new ChatUsersData();
                if (!IsAuthorized(result))
                {
                    return result;
                }

                var loginsList = xmpplogins.ToStringListFromDelimitedString(true);
                ChatUserRepository userRepository = new ChatUserRepository(context);
                var chatUsers = loginsList.Join(userRepository._All.Where(e => !e.Disabled).ToList(), s => s, chatUser => chatUser.XmppLogin,
                    (s, chatUser) => chatUser);

                result.users = chatUsers.Select(GetDataFromUserProfile).ToList();
                result.message = "OK";
                result.code = "200";
                return result;
            }
        }

        public CommonDataContract SetUserDeviceToken(string xmppLogin, string devicetoken)
        {
            try
            {
                SetDeviceToken(xmppLogin, devicetoken);
                return new CommonDataContract
                {
                    code = "200",
                    message = "success"
                };
            }
            catch (Exception ex)
            {
                return new CommonDataContract
                {
                    code = "500",
                    message = "server error",
                    response_date = ex.Message
                };
            }

        }

        public UserSettingsData GetUserSettings(string id)
        {
            try
            {
                using (var context = new InnostarModelsContext())
                {
                    var userRepository = new ChatUserRepository(context);
                    var idValue = Convert.ToInt32(id);
                    if (userRepository._Get(chatUser => chatUser.Id == idValue).Any())
                    {
                        var user = userRepository._Get(chatUser => chatUser.Id == idValue).First();
                        return new UserSettingsData
                        {
                            MessageStorageType = user.MessageStorageType,
                            IsMessagesArchived = user.IsMessagesArchived,
                            IsConfirmedDelivery = user.IsConfirmedDelivery,
                            TimeoutTypeClosedConference = user.TimeoutTypeClosedConference
                        };
                    }
                    else
                    {
                        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                        WebOperationContext.Current.OutgoingResponse.StatusDescription = "no such user";
                    }
                }
            }
            catch (Exception)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = "error";
            }
            return new UserSettingsData();
        }

        public StringData SetLastReadMessageTime(string username, string jid, string utcTime)
        {
            long time = 0;
            try
            {
                time = Convert.ToInt64(utcTime);
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Invalid value";
                return new StringData { Value = "Error" };
            }

            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["OpenfireConnection"];

                using (var connection = new SqlConnection(connectionString.ConnectionString))
                {
                    connection.Open();

                    var existUserCommand = connection.CreateCommand();
                    existUserCommand.CommandText = string.Format(@"select * from ofUser where username = '{0}'", username);
                    var existUserReader = existUserCommand.ExecuteReader();
                    if (existUserReader.HasRows)
                    {
                        var existCommand = connection.CreateCommand();
                        existCommand.CommandText = string.Format(@"select lastReadTime, lastDeliveredTime from ofLastMessageTime where username = '{0}' and jid = '{1}'", username, jid);
                        var existReader = existCommand.ExecuteReader();
                        if (existReader.HasRows)
                        {
                            existReader.Read();
                            var existReadTime = Convert.ToInt64(existReader[0]);
                            var existDeliveredTime = Convert.ToInt64(existReader[1]);
                            existReader.Close();

                            if (time <= existReadTime || time > existDeliveredTime)
                            {
                                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Invalid value";
                                return new StringData { Value = "Error" };
                            }

                            using (var updateTimeCommand = connection.CreateCommand())
                            {
                                updateTimeCommand.CommandText = string.Format(@"update ofLastMessageTime set lastReadTime = '{2}' where username = '{0}' and jid = '{1}'", username, jid, utcTime);
                                updateTimeCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            existReader.Close();
                            using (var insertTimeCommand = connection.CreateCommand())
                            {
                                insertTimeCommand.CommandText = string.Format(@"insert into ofLastMessageTime values ('{0}','{1}', {2}, {2})", username, jid, utcTime);
                                insertTimeCommand.ExecuteNonQuery();
                            }
                        }
                        return new StringData { Value = "Success" };
                    }
                    else
                    {
                        existUserReader.Close();
                        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                        WebOperationContext.Current.OutgoingResponse.StatusDescription = "Invalid username";
                        return new StringData { Value = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Server error";
                return new StringData { Value = "Error" };
            }
        }

        public StringData SetLastDeliveredMessageTime(string username, string jid, string utcTime)
        {
            long time = 0;
            try
            {
                time = Convert.ToInt64(utcTime);
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Invalid value";
                return new StringData { Value = "Error" };
            }

            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["OpenfireConnection"];

                using (var connection = new SqlConnection(connectionString.ConnectionString))
                {
                    connection.Open();

                    var existUserCommand = connection.CreateCommand();
                    existUserCommand.CommandText = string.Format(@"select * from ofUser where username = '{0}'", username);
                    var existUserReader = existUserCommand.ExecuteReader();
                    if (existUserReader.HasRows)
                    {
                        var existCommand = connection.CreateCommand();
                        existCommand.CommandText = string.Format(@"select lastDeliveredTime, lastReadTime from ofLastMessageTime where username = '{0}' and jid = '{1}'", username, jid);
                        var existReader = existCommand.ExecuteReader();
                        if (existReader.HasRows)
                        {
                            existReader.Read();
                            var existDeliveredTime = Convert.ToInt64(existReader[0]);
                            var existReadTime = Convert.ToInt64(existReader[1]);
                            existReader.Close();

                            if (time <= existDeliveredTime || time < existReadTime)
                            {
                                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Invalid value";
                                return new StringData { Value = "Error" };
                            }

                            using (var updateTimeCommand = connection.CreateCommand())
                            {
                                updateTimeCommand.CommandText = string.Format(@"update ofLastMessageTime set lastDeliveredTime = '{2}' where username = '{0}' and jid = '{1}'", username, jid, utcTime);
                                updateTimeCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            existReader.Close();
                            using (var insertTimeCommand = connection.CreateCommand())
                            {
                                insertTimeCommand.CommandText = string.Format(@"insert into ofLastMessageTime values ('{0}','{1}', {2}, 0)", username, jid, utcTime);
                                insertTimeCommand.ExecuteNonQuery();
                            }
                        }
                        return new StringData { Value = "Success" };
                    }
                    else
                    {
                        existUserReader.Close();
                        WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                        WebOperationContext.Current.OutgoingResponse.StatusDescription = "Invalid username";
                        return new StringData { Value = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Server error";
                return new StringData { Value = "Error" };
            }
        }

        public CommonDataContract SaveConferenceConfiguration(string jid, string isMessagesArchivedAtServer, string isClearedByPasswordDeleteAll)
        {
            try
            {
                using (var context = new InnostarModelsContext())
                {
                    var repository = new ConferenceConfigurationRepository(context);
                    int existingConferenceConfigurationsCount = repository._All.Count(e => e.ConferenceJid == jid);
                    if (existingConferenceConfigurationsCount == 0)
                    {
                        var conferenceConfiguration = new ConferenceConfiguration
                        {
                            ConferenceJid = jid,
                            IsMessagesArchivedAtServer = isMessagesArchivedAtServer == "1" || isMessagesArchivedAtServer == "true",
                            IsClearedByPasswordDeleteAll = isClearedByPasswordDeleteAll == "1" || isClearedByPasswordDeleteAll == "true"
                        };

                        repository._InsertOrUpdate(conferenceConfiguration);
                        repository._Save();

                        return new CommonDataContract
                        {
                            code = "200",
                            message = "success"
                        };
                    }
                    else
                    {
                        var conferenceConfiguration = repository._All.FirstOrDefault(e => e.ConferenceJid == jid);
                        conferenceConfiguration.IsClearedByPasswordDeleteAll = isMessagesArchivedAtServer == "1" || isMessagesArchivedAtServer == "true";
                        conferenceConfiguration.IsMessagesArchivedAtServer = isClearedByPasswordDeleteAll == "1" || isClearedByPasswordDeleteAll == "true";
                        repository._InsertOrUpdate(conferenceConfiguration);
                        repository._Save();

                        return new CommonDataContract
                        {
                            code = "200",
                            message = "success"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CommonDataContract
                {
                    code = "500",
                    message = ex.Message
                };
            }
        }

        public ConferenceConfigurationData GetConferenceConfiguration(string jid)
        {
            try
            {
                using (var context = new InnostarModelsContext())
                {
                    var repository = new ConferenceConfigurationRepository(context);
                    var conferenceConfiguration = repository._All.FirstOrDefault(e => e.ConferenceJid == jid);

                    if (conferenceConfiguration != null)
                    {
                        return new ConferenceConfigurationData
                        {
                            code = "200",
                            message = "success",
                            IsMessagesArchivedAtServer = conferenceConfiguration.IsMessagesArchivedAtServer ? 1 : 0,
                            IsCleanedByPasswordDeleteAll = conferenceConfiguration.IsClearedByPasswordDeleteAll ? 1 : 0,
                        };
                    }

                    return new ConferenceConfigurationData
                    {
                        code = "404",
                        message = "this conference not exist",
                    };
                }
            }
            catch (Exception ex)
            {
                return new ConferenceConfigurationData
                {
                    code = "500",
                    message = ex.Message
                };
            }
        }

        public BoolData IsPasswordDeleteAll(string login, string password)
        {
            try
            {
                using (var context = new InnostarModelsContext())
                {
                    var userRepository = new ChatUserRepository(context);
                    ChatUser user = userRepository._Get(chatUser => chatUser.Login.ToLower().Equals(login.ToLower())).FirstOrDefault();
                    if (user == null || user.PasswordDeleteAllHash == null || user.PasswordDeleteAllSalt == null || !user.IsPasswordDeleteAllEnabled)
                    {
                        return new BoolData
                        {
                            Value = false
                        };
                    }

                    var pu = new PasswordUtility(user.PasswordDeleteAllSalt, user.PasswordDeleteAllHash);
                    if (pu.Verify(password.ToCharArray()))
                    {
                        return new BoolData
                        {
                            Value = true
                        };
                    }
                    return new BoolData
                    {
                        Value = false
                    };
                }
            }
            catch
            {
                return new BoolData
                {
                    Value = false
                };
            }
        }

        public ChatUsersData SearchProfiles(string profileInfo)
        {
            var result = new ChatUsersData();
            result.message = "OK";
            result.code = "200";

            if (profileInfo.Length > 2)
            {
                using (var context = new InnostarModelsContext())
                {
                    var userRepository = new ChatUserRepository(context);
                    var chatUsers = userRepository._All.Where(e => !e.Disabled).ToList().Where(e => 
                        e.Login.ToLowerInvariant().Contains(profileInfo.ToLowerInvariant())
                        || (!string.IsNullOrEmpty(e.Name) &&e.Name.ToLowerInvariant().Contains(profileInfo.ToLowerInvariant()))
                        || (!string.IsNullOrEmpty(e.Phone) && e.Phone.ToLowerInvariant() == profileInfo.ToLowerInvariant()))
                        .ToList();

                    result.users = chatUsers.Select(GetDataFromUserProfile).ToList();
                    return result;
                }
            }
            else
            {
                result.users = new List<ChatUserData>();
                return result;
            }
        }

        public CommonDataContract GetInvitationMessageTemplate()
        {
            var result = new CommonDataContract();
            result.code = "400";

            try
            {
                using (var context = new InnostarModelsContext())
                {
                    var templateRepository = new MessageTemplateRepository(context);
                    var template = templateRepository._All.FirstOrDefault(e => e.SpecialTemplate == 3);
                    result.code = template != null && !string.IsNullOrEmpty(template.Message) ? "200" : "400";
                    result.message = template != null && !string.IsNullOrEmpty(template.Message) ? template.Message : string.Empty;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new CommonDataContract { code = "500", message = ex.Message };
            }
        }

        #endregion

        #region Data

        #endregion
    }
}

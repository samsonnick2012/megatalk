using System;
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
using System.Web.Mvc;
using System.Xml.Linq;
using XChat.Helpers;
using XChat.Models;
using XChat.Models.DB;
using XChat.Models.ViewModels;

namespace XChat.Service
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed), ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class Service : IService
    {
        public const string DATE_FORMAT = "dd-MM-yyyy-HH-mm";

        #region Authentication

        public AuthData UserAuth(string login, string password, string devicetoken)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
            ChatUserRepository userRepository = new ChatUserRepository();
            ChatUser user = userRepository.Get(chatUser => chatUser.Login.Equals(login)).FirstOrDefault();
            if (user == null)
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
            string xmppPassword = new string(PasswordUtility.Generate());
            string xmppResult = UpdateOrAddXmppUser("update", user.XmppLogin, xmppPassword, null);
            if (xmppResult != null)
            {
                return new AuthData() { code = "403", message = string.Format("Проблема на XMPP сервере {0}", xmppResult) };
            }

            userRepository.Update(user);
            userRepository.Save();

            AuthData authData = new AuthData();
            authData.code = "200";
            authData.message = "OK";
            authData.xmpp_password = xmppPassword;
            authData.access_token = token;
            authData.user_data = GetDataFromUserProfile(user);
            return authData;
        }

        private static bool IsAuthorized()
        {
            return true;
            UserTokenRepository tokenRepository = new UserTokenRepository();
            string token = WebOperationContext.Current.IncomingRequest.Headers["Authorization"];
            try
            {
                Guid guid = new Guid(token);
                return tokenRepository.Get(x => x.Token == guid).Any();
            }
            catch
            {
                return false;
            }
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
            UserTokenRepository tokenRepository = new UserTokenRepository();
            var exist = tokenRepository.Get(x => x.UserId == userId).FirstOrDefault();
            if (exist != null)
            {
                return exist.Token.ToString();
            }
            Guid guid = Guid.NewGuid();
            UserToken token = new UserToken();
            token.UserId = userId;
            token.Token = guid;
            token.CreatedOn = DateTime.Now;
            tokenRepository.Insert(token);
            tokenRepository.Save();
            return guid.ToString();
        }

        private static void SetDeviceToken(string xmppLogin, string devicetoken)
        {
            List<string> rosterUsers = new List<string>();
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["OpenfireConnection"];

            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();
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

        private static string UpdateOrAddXmppUser(string requestType, string login, string password, string name)
        {
            try
            {
                string requestStr =
                    string.Format("{0}:9090/plugins/userService/userservice?type={1}&secret={2}&username={3}",
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

        public AuthData Register(string login, string password, string name, string email, string phone, string recovertype, string question, string answer, string imageTitle, string imageContent)
        {
            return Service.RegisterUser(login, password, name, email, phone, recovertype, question, answer, imageTitle, imageContent);
        }

        public static AuthData RegisterUser(string login, string password, string name, string email, string phone, string recovertype, string question, string answer, string imageTitle, string imageContent)
        {
            AuthData result = new AuthData();
            string xmppLogin = GetXmppLogin(login);
            string xmppPassword = new string(PasswordUtility.Generate());

            try
            {
                ChatUserRepository userRepository = new ChatUserRepository();
                var uploadService = new UploadService();
                ChatUser user = userRepository.Get(u => u.Login.Equals(login)).FirstOrDefault();
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
                        user.Uuid = Guid.NewGuid().ToString().Replace("-", string.Empty) + rnd.Next(9999).ToString(CultureInfo.InvariantCulture);

                        user.PasswordRecoveryAnswer = answer;
                        user.PasswordRecoveryQuestion = question;
                        user.PasswordRecoveryType = int.Parse(recovertype);
                        userRepository.Insert(user);
                        userRepository.Save();

                        result.code = "200";
                        result.message = "OK";
                        result.xmpp_password = xmppPassword;
                        result.access_token = GetOrCreateAuthToken(user.Id);
                        result.user_data = GetDataFromUserProfile(user);

                        return result;
                    }
                    else
                    {
                        result.message = string.Format("Пользователя создать не удалось на сервере XMPP. {0}", xmppResult);
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
            catch (Exception ex)
            {
                result.message = string.Format("Пользователя создать не удалось. {0}", ex.Message);
                result.code = "403";
                return result;
            }
        }

        public CommonDataContract DeleteUser(string uid)
        {
            int userId = int.Parse(uid);
            var userRepository = new ChatUserRepository();

            var user = userRepository.Find(userId);

            string xmppResult = UpdateOrAddXmppUser("delete", user.XmppLogin, null, null);

            string message = string.Empty;
            string code = "200";

            if (xmppResult == null)
            {
                message += "remove xmpp user succesfull. ";
            }
            else
            {
                code = "500";
                message += xmppResult;
            }

            try
            {
                userRepository.Delete(user);
                userRepository.Save();
                message += "remove service user succesfull. ";
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
                RoomKeyRepository repository = new RoomKeyRepository();
                int existingRoomsCount = repository.All.Count(e => e.RoomId == roomJid);
                if (existingRoomsCount == 0)
                {
                    var roomKey = new RoomKey
                    {
                        RoomId = roomJid,
                        RoomPasswordKey = key,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now
                    };

                    repository.Insert(roomKey);
                    repository.Save();

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
                RoomKeyRepository repository = new RoomKeyRepository();
                int existingRoomsCount = repository.All.Count(e => e.RoomId == roomJid);
                if (existingRoomsCount > 0)
                {
                    var key = repository.All.FirstOrDefault(e => e.RoomId == roomJid).RoomPasswordKey;
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
            catch (Exception ex)
            {
                return new CommonDataContract
                {
                    code = "500",
                    message = ex.Message
                };
            }
        }

        public CommonDataContract RecoverPasswordByEmail(string email, string login)
        {
            var userRepository = new ChatUserRepository();
            var recoverRepository = new RecoverPasswordRequestRepository();

            var user = userRepository.Get(chatUser => chatUser.Login.Equals(login)).FirstOrDefault();
            if (user == null)
            {
                return new CommonDataContract() { code = "404", message = "Пользователь не найден" };
            }

            if (user.Email != email)
            {
                return new CommonDataContract() { code = "404", message = "Введённый email не соответствует данному пользователю" };
            }

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

            recoverRepository.Insert(request);
            recoverRepository.Save();

            try
            {
                using (var mail = new MailMessage(ConfigurationManager.AppSettings["PasswordSenderAddress"], user.Email))
                {
                    mail.Subject = "Восстановление пароля пользователя WeChat";

                    mail.Body = string.Format("<div>Здравствуйте, {0}</div>" +
                                "<div>Вы получили это письмо потому, что вы (либо кто-то, выдающий себя за вас) попросили выслать новый пароль к вашей учётной записи в приложении WeChat. Если вы не просили выслать пароль, то не обращайте внимания на это письмо, если же подобные письма будут продолжать приходить, обратитесь в службу технической поддержки.</div>" +
                                "<div>Вы сможете сменить пароль, перейдя по следующей ссылке</div>" +
                                //"<div><a href=\"https://{1}:{2}/Recover/Password?key={3}\">Ссылка для изменения пароля</a></div>",
                                "<div><a href=\"https://" + "localhost" + ":" + "83/Recover/Password?key=" + requestKey + "\">Ссылка для изменения пароля</a></div>",
                                user.Name,
                                ConfigurationManager.AppSettings["XmppServerName"],
                                ConfigurationManager.AppSettings["httpsPort"],
                                requestKey);
                                //"<div><a href=\"https://" + "localhost" + ":" + "83/Recover/Password?key=" + requestKey + "\">Ссылка для изменения пароля</a></div>,"
                    mail.IsBodyHtml = true;

                    using (var sc = new SmtpClient(ConfigurationManager.AppSettings["SmtpClient"], Convert.ToInt32(ConfigurationManager.AppSettings["SmtpClientPort"])))
                    {
                        sc.EnableSsl = true;
                        sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                        sc.UseDefaultCredentials = false;
                        sc.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["PasswordSenderAddress"], ConfigurationManager.AppSettings["PasswordSenderPassword"]);
                        sc.Send(mail);
                    }
                }

                return new CommonDataContract() { code = "200", message = "Пароль успешно заменён" };
            }
            catch (Exception ex)
            {
                return new CommonDataContract() { code = "500", message = ex.Message };
            }
        }

        public CommonDataContract RecoverPasswordByQuestion(string answer, string login)
        {
            ChatUserRepository userRepository = new ChatUserRepository();
            ChatUser user = userRepository.Get(chatUser => chatUser.Login.Equals(login)).FirstOrDefault();
            if (user == null)
            {
                return new CommonDataContract() { code = "404", message = "Пользователь не найден" };
            }

            if (user.PasswordRecoveryAnswer != answer)
            {
                return new CommonDataContract() { code = "404", message = "Введённый ответ не является правильным" };
            }

            Random rnd = new Random();
            var password = rnd.Next(99).ToString(CultureInfo.InvariantCulture) + Guid.NewGuid().ToString().Replace("-", string.Empty);
            user.Password = password;
            PasswordUtility pu = new PasswordUtility(password.ToCharArray());
            user.PasswordHash = pu.Hash;
            user.PasswordSalt = pu.Salt;
            userRepository.Update(user);
            userRepository.Save();

            return new CommonDataContract() { code = "200", message = "Пароль успешно заменён", response_date = password };
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
            userData.name = user.Name;
            userData.phone = user.Phone;
            userData.email = user.Email;
            return userData;
        }

        #endregion

        #region User
        public ChatUserData GetUserProfile(string uid)
        {
            ChatUserData result = new ChatUserData();
            if (!IsAuthorized(result))
            {
                return result;
            }

            int userId = int.Parse(uid);

            ChatUserRepository userRepository = new ChatUserRepository();
            ChatUser user = userRepository.Find(userId);
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

        public ChatUserData UpdateUser(string uid, string password, string name, string email, string phone, string recovertype, string question, string answer, string imageTitle, string imageContent)
        {
            ChatUserData result = new ChatUserData();
            if (!IsAuthorized(result))
            {
                return result;
            }

            int userId = int.Parse(uid);

            ChatUserRepository userRepository = new ChatUserRepository();
            var uploadService = new UploadService();
            ChatUser user = userRepository.Find(userId);
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
                    result.message = string.Format("Пользователя обновить не удалось на сервере XMPP. {0}", xmppResult);
                    result.code = "403";
                    return result;
                }

                if (!string.IsNullOrEmpty(name) && name != "0")
                    user.Name = name;
                if (!string.IsNullOrEmpty(email) && email != "0")
                    user.Email = email;
                if (!string.IsNullOrEmpty(phone) && phone != "0")
                    user.Phone = phone;
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
                if (!string.IsNullOrEmpty(answer) && answer != "0")
                    user.PasswordRecoveryAnswer = answer;
                if (!string.IsNullOrEmpty(question) && question != "0")
                    user.PasswordRecoveryQuestion = question;
                if (!string.IsNullOrEmpty(recovertype) && recovertype != "0")
                    user.PasswordRecoveryType = int.Parse(recovertype);
                userRepository.Save();
            }
            result = GetDataFromUserProfile(user);
            result.message = "OK";
            result.code = "200";
            return result;
        }

        public ChatUsersData GetRoster(string uid)
        {
            ChatUsersData result = new ChatUsersData();
            if (!IsAuthorized(result))
            {
                return result;
            }
            int userId = int.Parse(uid);

            List<string> rosterUsers = new List<string>();
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["OpenfireConnection"];

            ChatUserRepository userRepository = new ChatUserRepository();
            ChatUser user = userRepository.Find(userId);
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
            var chatUsers = rosterUsers.Join(userRepository.All, s => s, chatUser => chatUser.XmppLogin,
                                             (s, chatUser) => chatUser);
            result.users = chatUsers.Select(GetDataFromUserProfile).ToList();
            result.message = "OK";
            result.code = "200";
            return result;

        }

        public ChatUsersData GetProfilesByLogins(string logins)
        {
            ChatUsersData result = new ChatUsersData();
            if (!IsAuthorized(result))
            {
                return result;
            }

            var loginsList = logins.ToStringListFromDelimitedString(true);
            ChatUserRepository userRepository = new ChatUserRepository();
            var chatUsers = loginsList.Join(userRepository.All, s => s, chatUser => chatUser.Login,
                                             (s, chatUser) => chatUser);

            result.users = chatUsers.Select(GetDataFromUserProfile).ToList();
            result.message = "OK";
            result.code = "200";
            return result;

        }

        public ChatUsersData GetProfilesByXmppLogins(string xmpplogins)
        {
            ChatUsersData result = new ChatUsersData();
            if (!IsAuthorized(result))
            {
                return result;
            }

            var loginsList = xmpplogins.ToStringListFromDelimitedString(true);
            ChatUserRepository userRepository = new ChatUserRepository();
            var chatUsers = loginsList.Join(userRepository.All, s => s, chatUser => chatUser.XmppLogin,
                                             (s, chatUser) => chatUser);

            result.users = chatUsers.Select(GetDataFromUserProfile).ToList();
            result.message = "OK";
            result.code = "200";
            return result;

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

        #endregion

        #region Data

        #endregion
    }
}

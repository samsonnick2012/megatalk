using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Mvc;
using XChat.Models.DB;

namespace XChat.Service
{
    [ServiceContract]
    public interface IService
    {
        [WebInvoke(Method = "POST", UriTemplate = "auth")]
        [OperationContract]
        AuthData UserAuth(string login, string password, string devicetoken);

        [WebInvoke(Method = "POST", UriTemplate = "register")]
        [OperationContract]
        AuthData Register(string login, string password, string name, string email, string phone, string recovertype, string question, string answer, string imageTitle, string imageContent);

        [WebInvoke(Method = "GET", UriTemplate = "/userprofile/{uid}")]
        [OperationContract]
        ChatUserData GetUserProfile(string uid);

        [WebInvoke(Method = "POST", UriTemplate = "/updateuser/{uid}")]
        [OperationContract]
        ChatUserData UpdateUser(string uid, string password, string name, string email, string phone, string recovertype,
                                string question, string answer, string imageTitle, string imageContent);

        [WebInvoke(Method = "GET", UriTemplate = "/getroster/{uid}")]
        [OperationContract]
        ChatUsersData GetRoster(string uid);

        [WebInvoke(Method = "GET", UriTemplate = "/getprofilesbylogins/{logins}")]
        [OperationContract]
        ChatUsersData GetProfilesByLogins(string logins);

        [WebInvoke(Method = "GET", UriTemplate = "/getprofilesbyxmpplogins/{xmpplogins}")]
        [OperationContract]
        ChatUsersData GetProfilesByXmppLogins(string xmpplogins);

        [WebInvoke(Method = "POST", UriTemplate = "addroomkey")]
        [OperationContract]
        CommonDataContract AddRoomKey(string roomJid, string key);

        [WebInvoke(Method = "GET", UriTemplate = "getroomkey/{roomjid}")]
        [OperationContract]
        CommonDataContract GetRoomKey(string roomjid);

        [WebInvoke(Method = "GET", UriTemplate = "deleteuser/{uid}")]
        [OperationContract]
        CommonDataContract DeleteUser(string uid);

        [WebInvoke(Method = "POST", UriTemplate = "recoverpasswordbyemail")]
        [OperationContract]
        CommonDataContract RecoverPasswordByEmail(string email, string login);

        [WebInvoke(Method = "POST", UriTemplate = "recoverpasswordbyquestion")]
        [OperationContract]
        CommonDataContract RecoverPasswordByQuestion(string answer, string login);

        [WebInvoke(Method = "POST", UriTemplate = "setuserdevicetoken")]
        [OperationContract]
        CommonDataContract SetUserDeviceToken(string xmppLogin, string devicetoken);
    }
}

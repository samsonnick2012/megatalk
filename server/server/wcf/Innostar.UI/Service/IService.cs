using System.Collections;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Innostar.UI.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
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

        [WebInvoke(Method = "POST", UriTemplate = "/updateuser")]
        [OperationContract]
        ChatUserData UpdateUser(string uid, string password, string name, string email, string phone, string recovertype,
                                string question, string answer, string imageTitle, string messageStorageType, string isMessagesArchived, 
                                string isPasswordDeleteAllEnabled, string timeoutTypeClosedConference, string isConfirmedDelivery,
                                string imageContent, string passwordDeleteAll, string lastActivityTime);

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

        [WebInvoke(Method = "GET", UriTemplate = "recoverpassword/{info}")]
        [OperationContract]
        RecoverPasswordData RecoverPassword(string info);

        [WebInvoke(Method = "POST", UriTemplate = "recoverpasswordbyanswer")]
        [OperationContract]
        CommonDataContract RecoverPasswordByAnswer(string login, string answer);

        [WebInvoke(Method = "GET", UriTemplate = "IsUniqueEmail/{email}")]
        [OperationContract]
        BoolData IsUniqueEmail(string email);

        [WebInvoke(Method = "POST", UriTemplate = "changesafemode")]
        [OperationContract]
        CommonDataContract ChangeSafeMode(string login, string activate);

        [WebInvoke(Method = "GET", UriTemplate = "cleardevicetoken/{xmpplogin}")]
        [OperationContract]
        CommonDataContract ClearDeviceToken(string xmpplogin);

        [WebInvoke(Method = "POST", UriTemplate = "setdevicetoken")]
        [OperationContract]
        CommonDataContract SetUserDeviceToken(string xmppLogin, string devicetoken);

        [WebInvoke(Method = "GET", UriTemplate = "/usersettings/{id}")]
        [OperationContract]
        UserSettingsData GetUserSettings(string id);

        [WebInvoke(Method = "POST", UriTemplate = "setLastReadMessage")]
        [OperationContract]
        StringData SetLastReadMessageTime(string username, string jid, string utcTime);

        [WebInvoke(Method = "POST", UriTemplate = "setLastDeliveredMessage")]
        [OperationContract]
        StringData SetLastDeliveredMessageTime(string username, string jid, string utcTime);

        [WebInvoke(Method = "POST", UriTemplate = "saveconferenceconfiguration")]
        [OperationContract]
        CommonDataContract SaveConferenceConfiguration(string jid, string isMessagesArchivedAtServer, string isClearedByPasswordDeleteAll);

        [WebInvoke(Method = "GET", UriTemplate = "getconferenceconfiguration/{jid}")]
        [OperationContract]
        ConferenceConfigurationData GetConferenceConfiguration(string jid);

        [WebInvoke(Method = "POST", UriTemplate = "ispassworddeleteall")]
        [OperationContract]
        BoolData IsPasswordDeleteAll(string login, string password);

        [WebInvoke(Method = "GET", UriTemplate = "/searchprofiles/{profileInfo}")]
        [OperationContract]
        ChatUsersData SearchProfiles(string profileInfo);

        [WebInvoke(Method = "GET", UriTemplate = "/getinvitationmessagetemplate/")]
        [OperationContract]
        CommonDataContract GetInvitationMessageTemplate();
    }
}

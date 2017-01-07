using XChat.Filters;
using Microsoft.Web.WebPages.OAuth;

namespace XChat.App_Start
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            //InitializeSimpleMembershipAttribute.EnsureInitialized();

            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            OAuthWebSecurity.RegisterFacebookClient(
                appId: "1416288615256099",
                appSecret: "82de0acde7c8ec186160f5d98f1535a9");

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}

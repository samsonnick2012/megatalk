using System;
using Innostar.Models;

namespace Innostar.UI.Areas.Administration.ViewModels
{
    public class ChatUserViewModel
    {
        public int Id { get; set; }
        public bool Selected { get; set; }
        public string Username { get; set; }
        public string XmppLogin { get; set; }
        public string DisplayName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool SafeModeActivated { get; set; }
        public DateTime SafeModeStartDate { get; set; }
        public DateTime SafeModeEndDate { get; set; }
        public string Email { get; set; }

        public ChatUserViewModel(ChatUser chatUser, string XmppServerName)
        {
            Id = chatUser.Id;
            Username = chatUser.Login;
            XmppLogin = string.Format("{0}@{1}", chatUser.XmppLogin, XmppServerName);
            DisplayName = chatUser.Name;
            RegistrationDate = chatUser.RegistrationDate;
            SafeModeActivated = chatUser.SafeModeActivated;
            SafeModeStartDate = chatUser.SafeModeStartDate;
            SafeModeEndDate = chatUser.SafeModeEndDate;
            Email = chatUser.Email;
        }
    }
}
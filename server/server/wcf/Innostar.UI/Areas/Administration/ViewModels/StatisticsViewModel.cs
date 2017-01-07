using System.ComponentModel.DataAnnotations;

namespace Innostar.UI.Areas.Administration.ViewModels
{
    public class StatisticsViewModel
    {
        [Display(Name = "Количество всех пользователей")]
        public int UserNumber { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Количество платных пользователей")]
        public int SafeUserNumber { get; set; }

        [Display(Name = "Количество активных пользователей")]
        public int ActiveUserNumber { get; set; }

        public string XmppAdminLogin { get; set; }

        public string XmppAdminPassword { get; set; }

        public string HttpBindAddress { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Innostar.UI.ViewModels
{
    public class ConfirmEmailModel
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public string RequestKey { get; set; }

        [Display(Name = "Имя пользователя")]
        public string UserDisplayName { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Innostar.UI.ViewModels
{
    public class RecoverPasswordModel
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public string RequestKey { get; set; }

        [Display(Name = "Логин : ")]
        public string UserLogin { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен содержать как минимум {2} символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль :")]
        public string NewPassword { get; set; }
    }
}
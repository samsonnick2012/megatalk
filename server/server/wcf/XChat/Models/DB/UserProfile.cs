using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Security;
using XChat.Enums;
using XChat.Helpers;
using WebMatrix.WebData;

namespace XChat.Models.DB
{
    [Table("UserProfile")]
    [DisplayColumn("UserName")]
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Display(Name = "Имя (никнейм)")]
        public string NickName { get; set; }
        [Display(Name = "Почта")]
        public string Email { get; set; }
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        public int? ImageId { get; set; }
        
        [ForeignKey("ImageId")]
        [Display(Name = "Изображение")]
        public virtual DataFile Image { get; set; }


        [NotMapped]
        [ValidateImageFile]
        public HttpPostedFileBase ImageFile { get; set; }

        [Required]
        [NotMapped]
        [StringLength(100, ErrorMessage = "{0} должен содержать как минимум {2} символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NotMapped]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        

        [NotMapped]
        [Display(Name = "Роли")]
        public string[] AssignedRoles
        {
            get
            {
                if (WebSecurity.UserExists(UserName))
                {
                    return
                        Roles.GetRolesForUser(UserName);
                }
                return new string[]{};
            }
        }
    }
}
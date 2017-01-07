using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using XChat.Helpers;

namespace XChat.Models.DB
{
    public class ChatUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }
        [Display(Name = "Логин в XMPP")]
        public string XmppLogin { get; set; }

        [Display(Name = "Имя (никнейм)")]
        public string Name { get; set; }
        [Display(Name = "Почта")]
        public string Email { get; set; }
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Display(Name = "Тип восстановления пароля")]
        public int PasswordRecoveryType { get; set; }

        public string PasswordRecoveryQuestion { get; set; }

        public string PasswordRecoveryAnswer { get; set; }

        public bool Disabled { get; set; }

        public string Uuid { get; set; }

        public int? ImageId { get; set; }
        
        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        [ForeignKey("ImageId")]
        [Display(Name = "Изображение")]
        public virtual DataFile Image { get; set; }

        [NotMapped]
        //[StringLength(100, ErrorMessage = "{0} должен содержать как минимум {2} символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [NotMapped]
        [ValidateImageFile]
        public HttpPostedFileBase ImageFile { get; set; }

        [NotMapped]
        public string ImageTitle { get; set; }

        [NotMapped]
        public int ImageType { get; set; }

        [NotMapped]
        public string ImageContent { get; set; }

    }
}
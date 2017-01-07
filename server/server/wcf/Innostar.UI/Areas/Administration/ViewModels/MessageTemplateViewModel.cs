using System.ComponentModel.DataAnnotations;

namespace Innostar.UI.Areas.Administration.ViewModels
{
    public class MessageTemplateViewModel
    {
        [Display(Name = "Id ошибки")]
        public int Id { get; set; }

        [Display(Name = "Название шаблона")]
        public string Title { get; set; }

        [Display(Name = "Текст шаблона")]
        public string Text { get; set; }

        public bool Blocked { get; set; }
    }
}
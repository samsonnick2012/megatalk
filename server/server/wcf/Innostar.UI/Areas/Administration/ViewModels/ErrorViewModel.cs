using System;
using System.ComponentModel.DataAnnotations;

namespace Innostar.UI.Areas.Administration.ViewModels
{
    public class ErrorViewModel
    {
        [Display(Name = "Id ошибки")]
        public int Id { get; set; }

        [Display(Name = "Время произошедшей ошибки")]
        public DateTime Time { get; set; }

        [Display(Name = "GUID ошибки")]
        public string Title { get; set; }

        [Display(Name = "Подробное описание ошибки")]
        public string DetailedException { get; set; }

        [Display(Name = "Авторизованный пользователь")]
        public string AuthorizedUzer { get; set; }
    }
}
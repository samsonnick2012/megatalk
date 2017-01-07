using System.ComponentModel.DataAnnotations;
using Application.Core.Data;

namespace Innostar.Models
{
    public class MessageTemplate : ModelBase
    {
        [Required]
        public string Message { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public bool Blocked { get; set; }

        [Required]
        public int SpecialTemplate { get; set; }
    }
}

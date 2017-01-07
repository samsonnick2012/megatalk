using System.ComponentModel.DataAnnotations;
using Application.Core.Data;

namespace Innostar.Models
{
    public class ConferenceConfiguration : ModelBase
    {
        [Required]
        public string ConferenceJid { get; set; }

        [Required]
        public bool IsMessagesArchivedAtServer { get; set; }

        [Required]
        public bool IsClearedByPasswordDeleteAll { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

using Application.Core.Data;

namespace Innostar.Models
{
    public class AudioPiece : ModelBase
    {
        [Required]
        public string PhysicalFileName { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public bool ReadyForRemoving { get; set; }

        [Required]
        public DateTime UploadTime { get; set; }

        [Required]
        public DateTime ExpirationTime { get; set; }
    }
}

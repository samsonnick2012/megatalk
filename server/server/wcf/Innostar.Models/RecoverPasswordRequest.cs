using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Core.Data;

namespace Innostar.Models
{
    public class RecoverPasswordRequest: ModelBase
    {
        [Required]
        public string RequestKey { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime RequestAcceptedTime { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public ChatUser User { get; set; }
    }
}
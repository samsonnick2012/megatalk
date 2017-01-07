using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XChat.Models.DB
{
    public class RecoverPasswordRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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
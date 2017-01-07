using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XChat.Models.DB
{
    public class UserToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid Token { get; set; }
        public string DeviceToken { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }

        [ForeignKey("UserId")]
        public ChatUser User { get; set; }
    }
}
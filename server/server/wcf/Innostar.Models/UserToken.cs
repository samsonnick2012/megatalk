using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Core.Data;

namespace Innostar.Models
{
    public class UserToken : ModelBase 
    {
        public Guid Token { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }

        [ForeignKey("UserId")]
        public ChatUser User { get; set; }
    }
}
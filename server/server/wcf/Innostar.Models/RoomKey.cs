using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Core.Data;

namespace Innostar.Models
{
    public class RoomKey : ModelBase
    {
        [Required]
        public string RoomId { get; set; }

        [Required]
        public string RoomPasswordKey { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
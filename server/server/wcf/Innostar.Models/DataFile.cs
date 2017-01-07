using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Core.Data;

namespace Innostar.Models
{
    [DisplayColumn("FileName")]
    public class DataFile : ModelBase
    {
        [Required]
        public string OriginalFileName { get; set; }
        [Required]
        public string LocalFileName { get; set; }
        [Required]
        public int FileType { get; set; }

        public bool? isAvatar { get; set; }

        public DateTime? UploadTime { get; set; }

        public DateTime? ExpirationTime { get; set; }

        //[NotMapped]
        //public string Url
        //{
        //    get { return VirtualPathUtility.ToAbsolute(string.Format("~/Images/{0}", Id)); }
        //}
    }
}
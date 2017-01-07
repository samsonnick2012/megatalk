using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XChat.Helpers;

namespace XChat.Models.DB
{
    [DisplayColumn("FileName")]
    public class DataFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string OriginalFileName { get; set; }
        [Required]
        public string LocalFileName { get; set; }
        [Required]
        public int FileType { get; set; }

        [NotMapped]
        public string Url
        {
            get { return VirtualPathUtility.ToAbsolute(string.Format("~/Images/{0}", Id)); }
        }
    }
}
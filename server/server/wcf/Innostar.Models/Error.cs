using System;
using System.ComponentModel.DataAnnotations;
using Application.Core.Data;

namespace Innostar.Models
{
    [DisplayColumn("Error")]
    public class Error : ModelBase
    {
        public string Appdomain { get; set; }
        public string Aspnetcache { get; set; }
        public string Aspnetcontext { get; set; }
        public string Aspnetrequest { get; set; }
        public string Aspnetsession { get; set; }
        public DateTime Date { get; set; }
        public string Exception { get; set; }
        public string File { get; set; }
        public string Identity { get; set; }
        public string Location { get; set; }
        public string Level { get; set; }
        public int Line { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Method { get; set; }
        public string Ndc { get; set; }
        public string Property { get; set; }
        public string Stacktrace { get; set; }
        public string Stacktracedetail { get; set; }
        public long Timestamp { get; set; }
        public string Thread { get; set; }
        public string Type { get; set; }
        public string Username { get; set; }
        public DateTime Utcdate { get; set; }

    }
}

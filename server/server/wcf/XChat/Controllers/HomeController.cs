using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using XChat.Models;
using XChat.Models.DB;
using XChat.Service;

namespace XChat.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IDataFileRepository datafileRepository;

        public HomeController() : this(new DataFileRepository())
        {
        }

        public HomeController(IDataFileRepository imagefileRepository)
        {
            this.datafileRepository = imagefileRepository;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "X-Chat. Администрирование.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Как с нами связаться.";

            return View();
        }

        private string GetContentType(string filename)
        {
            FileInfo file = new FileInfo(filename); 
            switch (file.Extension.ToUpper())
            {
                case ".PNG": return "image/png";
                case ".JPG": return "image/jpeg";
                case ".JPEG": return "image/jpeg";
                case ".GIF": return "image/gif";
                case ".BMP": return "image/bmp";
                case ".TIFF": return "image/tiff";
                case ".MP3": return "audio/mpeg";
                case ".M4A": return "audio/m4a";
                default:
                    throw new NotSupportedException("The Specified File Type Is Not Supported");
            }
        }

        public FileResult GetFile(int id)
        {
            DataFile dataFile = datafileRepository.Find(id);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["FilesPath"], dataFile.LocalFileName);
            return base.File(path, GetContentType(path));
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase uploadFile)
        {
            try
            {
                var service = new UploadService();
                int fileType = 0;
                var extension = Path.GetExtension(uploadFile.FileName);
                if (extension != null)
                {
                    switch (extension.ToUpper())
                    {
                        case ".PNG":
                        case ".JPG":
                        case ".JPEG":
                        case ".GIF":
                        case ".BMP":
                        case ".TIFF":
                            fileType = 1;
                            break;
                        case ".MP3":
                        case ".M4A":
                            fileType = 2;
                            break;
                    }
                }

                UploadData result = service.UploadFile(uploadFile.FileName, fileType, uploadFile.InputStream);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new UploadData { code = "500", id = 0, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /*public FileResult GetUnitData(int id)
        {
            Unit unit = unitRepository.Find(id);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["FilesPath"], unit.FileName);
            return base.File(path, "application/zip", string.Format("{0}_{1}_{2}{3}", unit.Bundle.BundleName, unit.VersionName, unit.Version, Path.GetExtension(path)));
        }*/
    }
}

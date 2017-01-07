using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application.Core.UI.Attributes;
using Application.Core.UI.Controllers;
using Innostar.Dal.Infrastructure;
using Innostar.Dal.Repositories;
using Innostar.UI.Service;
using Innostar.UI.Services;
using Innostar.UI.ViewModels;

namespace Innostar.UI.Controllers
{
    [HandleErrorExtended]
    public class HomeController : BasicController
    {
        public ActionResult Index()
        {
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
                case ".DOC": return "application/msword";
                case ".DOCX": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".XLS": return "application/vnd.ms-excel";
                case ".XLSX": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".PDF": return "application/pdf";
                case ".ZIP": return "application/zip";
                default:
                    throw new NotSupportedException("The Specified File Type Is Not Supported");
            }
        }

        public ActionResult GetFile(int id)
        {
            using (var logFile = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["FilesPath"], @"FileLog.txt"), true))
            {
                var guid = Guid.NewGuid().ToString();

                logFile.WriteLine("Guid : {0} | File request accepted | Date : {1:H:mm:ss zzz dd/mm/yy } | Id: {2}", guid, DateTime.Now, id);

                try
                {
                    using (var context = new InnostarModelsContext())
                    {
                        var datafileRepository = new DataFileRepository(context);
                        var dataFiles = datafileRepository._All.Where(e => e.Id == id);
                        //GetBy(new Query<DataFile>(e => e.Id == id));
                        if (dataFiles.Any())
                        {
                            var dataFile = dataFiles.FirstOrDefault();
                            string path = Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                ConfigurationManager.AppSettings["FilesPath"],
                                dataFile.LocalFileName);
                            if (System.IO.File.Exists(path))
                            {
                                //logFile.WriteLine("Guid : {0} | File sent | Date : {1:H:mm:ss zzz dd/mm/yy } | Filename: {2}", guid, DateTime.Now, dataFile.OriginalFileName);
                                return base.File(path, GetContentType(path));
                            }
                            else
                            {
                                //logFile.WriteLine("Guid : {0} | No file at disc | Date : {1:H:mm:ss zzz dd/mm/yy }", guid, DateTime.Now);
                                return new HttpStatusCodeResult(404, "no such file");
                            }
                        }
                        else
                        {
                            //logFile.WriteLine(
                            //    "Guid : {0} | No file in DB | Date : {1:H:mm:ss zzz dd/mm/yy }",
                            //    guid,
                            //    DateTime.Now);
                            return new HttpStatusCodeResult(404, "no such file");
                        }
                    }
                }
                catch (Exception ex)
                {
                    //logFile.WriteLine("Guid : {0} | File sending exception| Date : {1:H:mm:ss zzz dd/mm/yy } | Exception: {2}", guid, DateTime.Now, ex.Message);
                    return new HttpStatusCodeResult(500, "server error");
                }
            }
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase uploadFile)
        {
            //using (var logFile = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["FilesPath"], @"FileLog.txt"), true))
            //{
                //var guid = Guid.NewGuid().ToString();

                //logFile.WriteLine("Guid : {0} | File received | Date : {1:H:mm:ss zzz dd/mm/yy } | Filename: {2}", guid, DateTime.Now, uploadFile.FileName);
                
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
                    //logFile.WriteLine("Guid : {0} | File saved | Date : {1:H:mm:ss zzz dd/mm/yy } | Filename: {2}", guid, DateTime.Now, uploadFile.FileName);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //logFile.WriteLine("Guid : {0} | File reception exception| Date : {1:H:mm:ss zzz dd/mm/yy } | Exception: {2}", guid, DateTime.Now, ex.Message);
                    return Json(new UploadData { code = "500", id = 0, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            //}
        }

        public JsonResult UploadAudioPiece(string key, string order, string piecesCount, HttpPostedFileBase piece)
        {
            int orderValue = 0;
            int piecesCountValue = 0;

            try
            {
                orderValue = Convert.ToInt32(order);
                piecesCountValue = Convert.ToInt32(piecesCount);

                var extension = Path.GetExtension(piece.FileName);
                if (extension != null && extension.ToLowerInvariant() != ".m4a")
                {
                    throw new Exception();
                }
            }
            catch
            {
                return Json(new
                {
                    code = 404,
                    messag = "Invalid arguments"
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var service = new AudioPieceService();
                service.UploadPiece(key, orderValue, piece.InputStream);
                if (piecesCountValue > 0)
                {
                    service.ConcatenatePieces(key, piecesCountValue);
                }
            }
            catch(ArgumentException exception)
            {
                return Json(new
                {
                    code = 400,
                    messag = exception.Message,
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    code = 500,
                    messag = "Internal server error"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                code = 200,
                message = "ok"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAudioFile(string key)
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        ConfigurationManager.AppSettings["AudioFilesPath"],
                        string.Format("{0}.m4a", key));

                if (System.IO.File.Exists(path))
                {
                    return File(path, GetContentType(path), string.Format("{0}.m4a", key));
                }

                return new HttpStatusCodeResult(400, "no file");
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(500, exception.Message);
            }
        }

        [HttpGet]
        public ActionResult ConfirmEmail(string key)
        {
            using (var dataContext = new InnostarModelsContext())
            {
                var repository = new ConfirmEmailRequestRepository(dataContext);

                var requests = repository._Get(u => u.RequestKey.Equals(key) && u.IsActive).ToList();

                if (requests.Any())
                {
                    var request = requests.FirstOrDefault();

                    if (request == null)
                    {
                        throw new Exception("Данного запроса не существует");
                    }

                    var userRepository = new ChatUserRepository(dataContext);
                    var user = userRepository._Get(e => e.Id == request.UserId).FirstOrDefault();

                    if (user == null)
                    {
                        throw new Exception("Произошла ошибка в обработке запроса. Обратитесь в службу поддержки");
                    }

                    user.Disabled = false;
                    userRepository._Update(user);
                    userRepository._Save();

                    request.IsActive = false;
                    repository._Update(request);
                    repository._Save();

                    return View(new ConfirmEmailModel
                    {
                        RequestId = request.Id,
                        RequestKey = key,
                        UserDisplayName = user.Name
                    });
                }
                else
                {
                    return RedirectToAction("NotFound", "Error");
                }
            }
        }
    }
}

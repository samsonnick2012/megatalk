using System;
using System.Configuration;
using System.IO;
using Innostar.Dal.Infrastructure;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Innostar.UI.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UploadService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select UploadService.svc or UploadService.svc.cs at the Solution Explorer and start debugging.
    public class UploadService : IUploadService
    {

        public UploadData UploadFile(string fileName, int fileType, Stream stream)
        {
            using (var context = new InnostarModelsContext())
            {
                UploadData result = new UploadData();
                if (!Service.IsAuthorized(result))
                {
                    return result;
                }
                DataFileRepository dataFileRepository = new DataFileRepository(context);
                DataFile dataFile = new DataFile();
                dataFile.OriginalFileName = fileName;
                dataFile.FileType = fileType;
                dataFile.LocalFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["FilesPath"], dataFile.LocalFileName);
                using (var fileStream = File.Create(filePath))
                {
                    stream.CopyTo(fileStream);
                }

                dataFileRepository._Insert(dataFile);
                dataFileRepository._Save();

                result.id = dataFile.Id;
                result.code = "200";
                result.message = "OK";
                return result;
            }
        }
    }
}


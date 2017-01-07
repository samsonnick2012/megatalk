using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using XChat.Models;
using XChat.Models.DB;

namespace XChat.Service
{
    [ServiceContract]
    public interface IUploader
    {
        [OperationContract]
        UploadData UploadFile(string fileName, int fileType, Stream stream);
    }

    public class UploadService : IUploader
    {
        public UploadData UploadFile(string fileName, int fileType, Stream stream)
        {
            UploadData result = new UploadData();
            if (!Service.IsAuthorized(result))
            {
                return result;
            }
            DataFileRepository dataFileRepository = new DataFileRepository();
            DataFile dataFile = new DataFile();
            dataFile.OriginalFileName = fileName;
            dataFile.FileType = fileType;
            dataFile.LocalFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["FilesPath"], dataFile.LocalFileName);
            using (var fileStream = File.Create(filePath))
            {
                stream.CopyTo(fileStream);
            }

            dataFileRepository.Insert(dataFile);
            dataFileRepository.Save();

            result.id = dataFile.Id;
            result.code = "200";
            result.message = "OK";
            return result;
        }
    }
}

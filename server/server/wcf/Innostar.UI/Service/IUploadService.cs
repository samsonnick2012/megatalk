using System.IO;
using System.ServiceModel;

namespace Innostar.UI.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUploadService" in both code and config file together.
    [ServiceContract]
    public interface IUploadService
    {
        [OperationContract]
        UploadData UploadFile(string fileName, int fileType, Stream stream);
    }
}

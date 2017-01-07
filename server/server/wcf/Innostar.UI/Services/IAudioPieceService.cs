using System.IO;

namespace Innostar.UI.Services
{
    public interface IAudioPieceService
    {
        void UploadPiece(string key, int order, Stream stream);

        void ConcatenatePieces(string key, int piecesCount);
    }
}

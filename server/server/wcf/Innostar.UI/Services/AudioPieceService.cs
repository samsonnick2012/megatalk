using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using Innostar.Dal.Infrastructure;
using Innostar.Dal.Repositories;
using Innostar.Models;
using Innostar.UI.Helpers;

using Microsoft.Practices.ObjectBuilder2;

namespace Innostar.UI.Services
{
    public class AudioPieceService : IAudioPieceService
    {
        public void UploadPiece(string key, int order, Stream stream)
        {
            var name = Guid.NewGuid() + ".m4a";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["PiecesPath"], name);

            if (!Directory.Exists(Directory.GetParent(filePath).FullName))
            {
                Directory.GetParent(filePath).Create();
            }

            using (var fileStream = File.Create(filePath))
            {
                stream.CopyTo(fileStream);
            }


            var time = DateTime.Now;

            var piece = new AudioPiece();
            piece.ExpirationTime = time.AddDays(3);
            piece.Key = key;
            piece.Order = order;
            piece.PhysicalFileName = filePath;
            piece.UploadTime = time;

            using (var context = new InnostarModelsContext())
            {
                var repository = new AudioPieceRepository(context);
                repository._Insert(piece);
                repository._Save();
            }
        }

        public void ConcatenatePieces(string key, int piecesCount)
        {
            using (var context = new InnostarModelsContext())
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    ConfigurationManager.AppSettings["AudioFilesPath"],
                        string.Format("{0}.m4a", key));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                var repository = new AudioPieceRepository(context);
                var paths = repository.GetPiecePaths(key);

                var sourceFiles = paths as IList<string> ?? paths.ToList();

                if (!sourceFiles.Any() || (piecesCount > 0 && sourceFiles.Count() != piecesCount))
                {
                    throw new ArgumentException("some pieces are lost");
                }

                if (!Directory.Exists(Directory.GetParent(filePath).FullName))
                {
                    Directory.GetParent(filePath).Create();
                }

                string fileListPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    ConfigurationManager.AppSettings["PiecesPath"],
                    string.Format("{0}.txt", key));

                if (File.Exists(fileListPath))
                {
                    File.Delete(fileListPath);
                }

                using (var writer = File.AppendText(fileListPath))
                {
                    sourceFiles.ForEach(e => writer.WriteLine(@"file '{0}'", e));
                }

                FfmpegConcatenator.Concatenate(ConfigurationManager.AppSettings["FfmpegUtilityPath"], fileListPath, filePath);

                File.Delete(fileListPath);

                if (!File.Exists(filePath))
                {
                    throw new ArgumentException("file isn't concatenated");
                }
            }
        }
    }
}
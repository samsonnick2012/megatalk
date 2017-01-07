using System;
using System.Collections.Generic;

using NAudio.Wave;

namespace Innostar.UI.Helpers
{
    public class WaveConcatenator
    {
        public static void Concatenate(string outputFile, IEnumerable<string> sourceFiles)
        {
            byte[] buffer = new byte[1024];
            WaveFileWriter waveFileWriter = null;

            try
            {
                foreach (string sourceFile in sourceFiles)
                {
                    using (var reader = new MediaFoundationReader(sourceFile))
                    {
                        using (var resampledReader = new ResamplerDmoStream(reader,new WaveFormat(
                            reader.WaveFormat.SampleRate,
                            reader.WaveFormat.BitsPerSample,
                            reader.WaveFormat.Channels)))
                        {
                            if (waveFileWriter == null)
                            {
                                waveFileWriter = new WaveFileWriter(outputFile, reader.WaveFormat);
                            }
                            else
                            {
                                if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))
                                {
                                    throw new InvalidOperationException(
                                        "Can't concatenate WAV Files that don't share the same format");
                                }
                            }

                            int read;
                            while ((read = resampledReader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                waveFileWriter.WriteData(buffer, 0, read);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (waveFileWriter != null)
                {
                    waveFileWriter.Dispose();
                }
            }
        }
    }
}
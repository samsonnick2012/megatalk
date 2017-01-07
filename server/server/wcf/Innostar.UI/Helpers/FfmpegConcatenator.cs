using System.Diagnostics;

namespace Innostar.UI.Helpers
{
    public class FfmpegConcatenator
    {
        public static void Concatenate(string ffmpegUtilityPath, string fileListPath, string outputFile)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = ffmpegUtilityPath;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.Arguments = string.Format("-f concat -i \"{0}\" -c copy \"{1}\"", fileListPath, outputFile);

            var process = Process.Start(processStartInfo);
            if (process != null)
            {
                process.WaitForExit();
            }
        }
    }
}
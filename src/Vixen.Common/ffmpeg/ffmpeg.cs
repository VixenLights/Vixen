using System.Diagnostics;
using System.Globalization;

namespace Vixen.Common.ffmpeg
{
	public class Ffmpeg
	{
		private static readonly string FfmpegPath;

		static Ffmpeg()
		{
			FfmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ffmpeg.exe");
		}

		//Nutcracker Video Effect
		public static void MakeThumbnails(string movieFile, int width, int height, string outputPath, int framesPerSecond = 20)
		{
			//make arguments string
			string args;
			args = " -i \"" + movieFile + "\"" +
			       " -s " + width + "x" + height +
			       " -vf " +
			       " fps=" + framesPerSecond + " \"" + outputPath + "\\%10d.png\"";
			//create a process
			Process myProcess = new Process();
			myProcess.StartInfo.UseShellExecute = false;
			myProcess.StartInfo.RedirectStandardOutput = true;
			//point ffmpeg location

			myProcess.StartInfo.FileName = FfmpegPath;
			//set arguments
			myProcess.StartInfo.Arguments = args;
			//Console.WriteLine(_ffmpegPath + " => " + args);
			myProcess.Start();

			myProcess.WaitForExit();

		}

		//Native Video Effect
		public static void MakeScaledThumbNails(string movieFile, string outputPath, double startPosition, double duration, int width, int height, bool maintainAspect, int rotateVideo, string cropVideo, double fps=20)
		{
			int maintainAspectValue = maintainAspect ? -1 : height;
			//make arguments string
			string args = $" -y -ss {startPosition} -i \"{movieFile}\" -an -t {duration.ToString(CultureInfo.InvariantCulture)} -vf \"scale={width}:{maintainAspectValue}{cropVideo}, rotate={rotateVideo}*(PI/180)\" -r {fps} \"{outputPath}\\%5d.bmp\"";
			
			ProcessStartInfo psi = new ProcessStartInfo(FfmpegPath, args);
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			Process process = new Process();
			process.StartInfo = psi;
			
			process.Start();
			
			process.WaitForExit();
		}

		//Get Video Info for native Video effect.
		public static string GetVideoInfo(string movieFile, string outputPath)
		{
			//Gets Video length and will continue if users start position is less then the video length.
			string args = " -i \"" + movieFile + "\"";
			
			ProcessStartInfo procStartInfo = new ProcessStartInfo(FfmpegPath, args);
			procStartInfo.RedirectStandardError = true;
			procStartInfo.UseShellExecute = false;
			procStartInfo.CreateNoWindow = true;
			Process proc = new Process();
			proc.StartInfo = procStartInfo;
			proc.Start();
			string result = proc.StandardError.ReadToEnd();
			return result;
		}

		//Get Native Video Size Effect
		public static void GetVideoSize(string movieFile, string outputPath)
		{
			//make arguments string
			string args = $" -i \"{movieFile}\"  -vframes 1 \"{outputPath}\"";
			
			//Console.Out.WriteLine(args);
			ProcessStartInfo psi = new ProcessStartInfo(FfmpegPath, args);
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			Process process = new Process();
			process.StartInfo = psi;
			process.Start();
			process.WaitForExit();
		}

	}
}
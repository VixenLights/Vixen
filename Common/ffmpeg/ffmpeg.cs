using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace ffmpeg
{
	public class ffmpeg
	{
		private string _movieFile = string.Empty;

		public ffmpeg(string movieFile)
		{
			_movieFile = movieFile;
		}

		//Nutcracker Video Effect
		public void MakeThumbnails(int width, int height, string outputPath, int framesPerSecond = 20)
		{
			//make arguements string
			string args;
			args = " -i \"" + _movieFile + "\"" +
			       " -s " + width.ToString() + "x" + height.ToString() +
			       " -vf " +
			       " fps=" + framesPerSecond.ToString() + " \"" + outputPath + "\\%10d.png\"";
			//create a process
			Process myProcess = new Process();
			myProcess.StartInfo.UseShellExecute = false;
			myProcess.StartInfo.RedirectStandardOutput = true;
			//point ffmpeg location
			string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory;
			ffmpegPath += "Common\\ffmpeg.exe";
			myProcess.StartInfo.FileName = ffmpegPath;
			//set arguements
			myProcess.StartInfo.Arguments = args;
			Console.WriteLine(ffmpegPath + " => " + args);
			myProcess.Start();
			//while (!myProcess.HasExited)
			//{
			//    Thread.Yield();
			//}
			myProcess.WaitForExit();
		}

		//Native Video Effect
		public void MakeScaledThumbNails(string outputPath, double startPosition, double duration, int width, int height, bool maintainAspect, int rotateVideo, string cropVideo, int fps=20)
		{
			int maintainAspectValue = maintainAspect ? -1 : height;
			//make arguements string
			string args = $" -y -ss {startPosition} -i \"{_movieFile}\" -an -t {duration.ToString(CultureInfo.InvariantCulture)} -vf \"scale={width}:{maintainAspectValue}{cropVideo}, rotate={rotateVideo}*(PI/180)\" -r {fps} \"{outputPath}\\%5d.bmp\"";
			string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory;
			ffmpegPath += "Common\\ffmpeg.exe";
			
			ProcessStartInfo psi = new ProcessStartInfo(ffmpegPath, args);
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			Process process = new Process();
			process.StartInfo = psi;
			
			process.Start();
			
			process.WaitForExit();
		}

		//Get Video Info for native Video effect.
		public string GetVideoInfo(string outputPath)
		{
			//Gets Video length and will continue if users start position is less then the video length.
			string args = " -i \"" + _movieFile + "\"";
			string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory;
			ffmpegPath += "Common\\ffmpeg.exe";

			ProcessStartInfo procStartInfo = new ProcessStartInfo(ffmpegPath, args);
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
		public void GetVideoSize(string outputPath)
		{
			//make arguements string
			string args = $" -i \"{_movieFile}\"  -vframes 1 \"{outputPath}\"";
			string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory;
			ffmpegPath += "Common\\ffmpeg.exe";
			Console.Out.WriteLine(args);
			ProcessStartInfo psi = new ProcessStartInfo(ffmpegPath, args);
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			Process process = new Process();
			process.StartInfo = psi;
			process.Start();
			process.WaitForExit();
		}

	}
}
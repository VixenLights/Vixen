using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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
		public void MakeThumbnails(string outputPath, double startPosition, double duration, int width, int height, bool maintainAspect, string frameRate, string colorType, int rotateVideo)
		{
			int maintainAspectValue = maintainAspect ? -1 : height;
			//make arguements string
			string args = " -ss " + startPosition + " -i \"" + _movieFile + "\"" + " -t " + duration + colorType + " -vf " + " \"scale=" + width + ":" + maintainAspectValue + ", rotate=" + rotateVideo + "*(PI/180)\" " + frameRate
				   + " \"" + outputPath + "\\%5d.bmp\"";
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

		//Get Video Info for native Video effect
		public string MakeThumbnails(string outputPath)
		{
			//Gets Video length and will continue if users start position is less then the video length.
			string args;
			args = " -i \"" + _movieFile + "\"";
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
	}
}
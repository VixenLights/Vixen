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
        private string _movieFile = "";

        public ffmpeg(string movieFile)
        {
            _movieFile = movieFile;
        }

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
    }
}

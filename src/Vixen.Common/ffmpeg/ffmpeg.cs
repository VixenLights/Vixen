using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Vixen.Common.ffmpeg
{
	public partial class Ffmpeg
	{
		private static readonly string FfmpegPath;

		//ffmpeg output line: "  Duration: 00:02:29.46, start: 0.00...."
		[GeneratedRegex(@"Duration: (\d+):(\d{2}):(\d{2})\.(\d{2})")]
		private static partial Regex parseDuration();

		// look for ", ####x####" where numbers can be 2 to 5 digits long
		[GeneratedRegex(@", (?<width>\d{2,5})x(?<height>\d{2,5})")]
		private static partial Regex parseResolution();

		static Ffmpeg()
		{
			FfmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ffmpeg.exe");
		}

		[Obsolete("Instead use MakeScaledThumbNails to limit the amount of video converted to thumbnails")]
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

		public static void MakeScaledThumbNails(string movieFile, string outputPath, double startPosition, double duration, int width, int height, bool maintainAspect, int rotateVideo, string cropVideo, double fps = 20, string cacheFileType = "bmp")
		{
			string args = $" -y -ss {startPosition} -i \"{movieFile}\" -an -t {duration.ToString(CultureInfo.InvariantCulture)} -vf \"scale={width}:{(maintainAspect ? -1 : height)}{cropVideo}, rotate={rotateVideo}*(PI/180)\" -r {fps} \"{outputPath}\\%5d.{cacheFileType}\"";

			ProcessStartInfo psi = new(FfmpegPath, args)
			{
				UseShellExecute = false,
				CreateNoWindow = true
			};
			using (Process process = new())
			{
				process.StartInfo = psi;
				process.Start();
				process.WaitForExit();
			};
		}

		public static void GetVideoDurationAndResolution(string videoFile, out TimeSpan duration, out int width, out int height)
		{
			duration = TimeSpan.Zero;
			width = 0;
			height = 0;
			
			string args = $"-hide_banner -an -sn -dn -i \"{videoFile}\"";

			ProcessStartInfo psi = new(FfmpegPath, args)
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardError = true
			};
			using (Process process = new())
			{
				process.StartInfo = psi;
				process.Start();

				// Keep all the ffmpeg output parsing logic in this class
				string line;
				Match match;
				while ((line = process.StandardError.ReadLine()) != null)
				{
					match = parseDuration().Match(line);
					if (match.Success)
					{
						duration = new TimeSpan(0,
							Int32.Parse(match.Groups[1].Value),
							Int32.Parse(match.Groups[2].Value),
							Int32.Parse(match.Groups[3].Value),
							Int32.Parse(match.Groups[4].Value) * 10);
					}
					// Find the " Video: " line, then look for the resolution
					else if (line.Contains(" Video: "))
					{
						match = parseResolution().Match(line);
						if (match.Success)
						{
							width = Int32.Parse(match.Groups["width"].Value);
							height = Int32.Parse(match.Groups["height"].Value);
							return; // Since Duration is always before the Video line, once width and height are found, we're done
						}
					}
				}
				// If we get here, something has failed to be found or parse
				process.WaitForExit();
				if (duration == TimeSpan.Zero)
				{
					throw new Exception($"Unable to parse Duration from ffmpeg output. Error code {process.ExitCode}");
				}
				else
				{
					throw new Exception($"Unable to parse Resolution from ffmpeg output. Error code {process.ExitCode}");
				}
			};
		}

		[Obsolete("Instead use GetVideoDurationAndResolution to return duration")]
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

		[Obsolete("Instead use GetVideoDurationAndResolution to return resolution")]
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
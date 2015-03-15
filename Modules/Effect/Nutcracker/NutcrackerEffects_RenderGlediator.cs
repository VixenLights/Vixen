using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using VixenModules.Effect.Nutcracker;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

namespace VixenModules.Effect.Nutcracker
{
	public partial class NutcrackerEffects
	{
		private void RenderGlediator(string gledFilename)
		{
			if (!File.Exists(gledFilename)) // if it doesnt exist, just return
			{
				return;
			}

			var f = new FileStream(Path.Combine(NutcrackerDescriptor.ModulePath, gledFilename), FileMode.Open, FileAccess.Read);

			long fileLength = f.Length;

			int seqNumChannels = (BufferWi * 3 * BufferHt);
			byte[] frameBuffer = new byte[seqNumChannels];
			int seqNumPeriods = (int)(fileLength / seqNumChannels);
			
			int period = ((int)State / 10) % seqNumPeriods;
			int offset = seqNumChannels * period;
			f.Seek(offset, SeekOrigin.Begin);
			long readcnt = f.Read(frameBuffer, 0, seqNumChannels);

			for (int j = 0; j < readcnt; j += 3)
			{
				// Loop thru all channels
				Color color = Color.FromArgb(255, frameBuffer[j], frameBuffer[j + 1], frameBuffer[j + 2]);
				int x = (j % (BufferWi * 3)) / 3;
				int y = (BufferHt - 1) - (j / (BufferWi * 3));
				if (x < BufferWi && y < BufferHt && y >= 0)
				{
					SetPixel(x, y, color);
				}

			}
			
			f.Close(); //Make sure this does not break things
		}
	}
}
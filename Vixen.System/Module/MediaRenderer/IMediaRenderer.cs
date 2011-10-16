using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Module.Media;

namespace Vixen.Module.MediaRenderer {
	public interface IMediaRenderer {
		IMediaModuleInstance Media { get; set; }
		void Render(Graphics g, Rectangle invalidRect, TimeSpan startTime, TimeSpan timeSpan);
		void Setup();
	}
}

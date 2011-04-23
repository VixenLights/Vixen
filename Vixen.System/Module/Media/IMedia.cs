using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Execution;
using Vixen.Module.Timing;

namespace Vixen.Module.Media {
	public interface IMedia : IExecutionControl {
		string MediaFilePath { get; set; }
		/// <summary>
		/// Load or prepare the media for execution starting at a given time offset.
		/// </summary>
		/// <param name="startTime">Time, in milliseconds, at which media is to start executing when Start is called.</param>
		void LoadMedia(long startTime);
		ITiming TimingSource { get; }
		void Setup();
	}
}

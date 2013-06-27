using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Execution
{
	public interface IExecutionControl
	{
		void Start();
		void Stop();
		void Pause();
		void Resume();
	}
}
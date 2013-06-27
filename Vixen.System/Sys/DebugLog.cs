using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys
{
	internal class DebugLog : Log
	{
		public DebugLog()
			: base("Debug")
		{
		}

		public override void Write(Exception ex)
		{
			base.Write(ex.ToString());
		}

		public override void Write(string qualifyingMessage, Exception ex)
		{
			base.Write(ex.ToString());
		}
	}
}
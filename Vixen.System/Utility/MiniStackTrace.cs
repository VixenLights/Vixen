using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Vixen.Utility
{
	// A little helper to aid tracing in routings called from multiple locations

	public class MiniStackTrace
	{
		static public string ToString(int levels = 4, int skip = 2)
		{
			if (levels < 1)
				return "bad levels value";
			if (skip < 0)
				return "bad skip value";

			var st = new System.Diagnostics.StackTrace(true);
			if (skip >= st.FrameCount)
				skip = st.FrameCount - 1;
			if (levels + skip > st.FrameCount)
				levels = st.FrameCount - skip;

			string ret = "";
			for (int i = 0; i < levels; i++)
			{
				var sf = st.GetFrame(skip + i);
				String sep = ret.Length > 0 ? ", " : "";
				String tmp = String.Format("{0}{1}:{2}", sep, Path.GetFileName(sf.GetFileName()), sf.GetFileLineNumber());
				ret += tmp;
			}

			return ret;
		}

		static public bool HasCaller(string fname, int line = -1)
		{
			if (fname == null)
				return false;

			var st = new System.Diagnostics.StackTrace(true);

			for (int i = 0; i < st.FrameCount; i++)
			{
				var sf = st.GetFrame(i);
				if (fname.Equals(Path.GetFileName(sf.GetFileName())))
				{
					if (line < 1)
						return true;
					if (line == sf.GetFileLineNumber())
						return true;
				}
			}
			return false;
		}
	}

}

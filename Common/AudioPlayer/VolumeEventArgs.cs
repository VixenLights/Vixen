using System;

namespace NAudioWrapper
{
	public class VolumeEventArgs:EventArgs
	{
		public VolumeEventArgs(float left, float right)
		{
			Left = left;
			Right = right;
		}

		public float Left { get; }

		public float Right { get;}
	}
}

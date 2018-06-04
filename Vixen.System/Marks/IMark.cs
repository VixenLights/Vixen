using System;
using System.ComponentModel;

namespace Vixen.Marks
{
	public interface IMark:INotifyPropertyChanged, IComparable<IMark>, ICloneable
	{
		TimeSpan StartTime { get; set; }
		TimeSpan EndTime { get; }
		TimeSpan Duration { get; set; }
		string Text { get; set; }
		IMarkCollection Parent { get; set; }
	}
}

using Vixen.Marks;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksBulkChangeInfo
	{
		public MarksBulkChangeInfo()
		{
			OriginalMarks = new Dictionary<IMark, IMark>();
		}

		public void Add(IMark markKey, IMark markValue)
		{
			OriginalMarks.Add(markKey, markValue == null ? null : markValue);
		}

		///<summary>All marks being modified and their original parameters.</summary>
		public Dictionary<IMark, IMark> OriginalMarks { get; private set; }

	}
	
}

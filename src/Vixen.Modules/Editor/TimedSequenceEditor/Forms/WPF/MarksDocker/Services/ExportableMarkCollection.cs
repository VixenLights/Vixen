using Vixen.Marks;

namespace TimedSequenceEditor.Forms.WPF.MarksDocker.Services
{
	public class ExportableMarkCollection(IMarkCollection markCollection, bool includeText)
	{
		public bool IsTextIncluded { get; init; } = includeText;

		public IMarkCollection MarkCollection { get; init; } = markCollection;
	}
}
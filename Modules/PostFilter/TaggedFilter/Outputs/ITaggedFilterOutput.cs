using Vixen.Data.Flow;

namespace VixenModules.OutputFilter.TaggedFilter.Outputs
{
	/// <summary>
	/// Maintains a tagged filter output.
	/// </summary>
	public interface ITaggedFilterOutput
	{
		/// <summary>
		/// Processes the input intent data.
		/// </summary>
		/// <param name="data">Intent data to process</param>
		void ProcessInputData(IntentsDataFlowData data);
	}
}

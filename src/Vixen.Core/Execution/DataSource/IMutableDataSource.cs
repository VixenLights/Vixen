using Vixen.Sys;

namespace Vixen.Execution.DataSource
{
	public interface IMutableDataSource : IDataSource
	{
		void Start();
		void Stop();
		void SetSequence(ISequence sequence);
		//void SetDataSource(IEnumerable<IEffectNode> data);
		//void SetFilters(IEnumerable<ISequenceFilterNode> filters); 
	}
}
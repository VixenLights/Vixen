using Vixen.Sys;

namespace Vixen.Execution.DataSource
{
	public interface IDataSource
	{
		IEnumerable<IEffectNode> GetDataAt(TimeSpan time);

		void GetDataAt(TimeSpan time, List<IEffectNode> destination)
		{
			destination.AddRange(GetDataAt(time));
		}
	}
}
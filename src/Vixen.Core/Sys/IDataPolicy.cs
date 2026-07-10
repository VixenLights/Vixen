using Vixen.Commands;
using Vixen.Data.Flow;

namespace Vixen.Sys
{
	public interface IDataPolicy
	{
		ICommand GenerateCommand(IDataFlowData dataFlowData);
	}
}
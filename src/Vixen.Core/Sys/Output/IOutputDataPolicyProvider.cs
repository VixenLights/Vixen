namespace Vixen.Sys.Output
{
	internal interface IOutputDataPolicyProvider
	{
		void UseFactory(IDataPolicyFactory dataPolicyFactory);
		IDataPolicy GetDataPolicyForOutput(CommandOutput output);
	}
}
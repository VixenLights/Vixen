namespace Vixen.Sys.Output {
	interface IOutputDataPolicyProvider {
		void UseFactory(IDataPolicyFactory dataPolicyFactory);
		IDataPolicy GetDataPolicyForOutput(CommandOutput output);
	}
}

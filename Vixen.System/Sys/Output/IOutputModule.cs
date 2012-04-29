namespace Vixen.Sys.Output {
	public interface IOutputModule {
		int UpdateInterval { get; }
		IDataPolicy DataPolicy { get; }
	}
}

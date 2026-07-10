using Vixen.Sys;

namespace Vixen.Module.App
{
	public interface IApp
	{
		void Loading();
		void Unloading();
		IApplication Application { set; }
	}
}
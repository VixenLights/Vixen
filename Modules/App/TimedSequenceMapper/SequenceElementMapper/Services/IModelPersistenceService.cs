using System.Threading.Tasks;
using Catel.Data;
using Vixen.Sys;

namespace VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Services
{
	public interface IModelPersistenceService<T> where T:ModelBase
	{
		Task<T> LoadModelAsync(string path);

		Task<bool> SaveModelAsync(T model, string path);

		Task<ElementNodeProxy> LoadElementNodeProxyAsync(string path);
	}
}

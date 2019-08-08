using System.Threading.Tasks;
using Catel.Data;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Services
{
	public interface IModelPersistenceService<T> where T:ModelBase
	{
		Task<T> LoadModelAsync(string path);

		Task<bool> SaveModelAsync(T model, string path);
	}
}

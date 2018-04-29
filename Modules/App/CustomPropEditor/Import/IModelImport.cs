using System.Threading.Tasks;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Import
{
	public interface IModelImport
	{
		Task<Prop> ImportAsync(string filePath);
	}
}

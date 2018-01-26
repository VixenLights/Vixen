using System.Threading.Tasks;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Model;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Import
{
    public interface IModelImport
    {
        Task<Prop> ImportAsync(string filePath);
    }
}

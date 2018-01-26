using System.IO;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Services
{
    public interface IOService
    {
        string OpenFileDialog(string defaultPath, string filters);

        //Other similar untestable IO operations
        Stream OpenFile(string path);
    }
}

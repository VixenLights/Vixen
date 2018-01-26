using System;
using System.IO;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Services
{
    public class FileService: IOService
    {
        public string OpenFileDialog(string defaultPath, string filters)
        {
            var dialog = new OpenFileDialog {InitialDirectory = defaultPath, Filter = filters};
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
            
        }

        public Stream OpenFile(string path)
        {
            throw new NotImplementedException();
        }
    }
}

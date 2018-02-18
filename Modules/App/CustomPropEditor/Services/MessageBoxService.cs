using System.Windows.Forms;
using Common.Controls;

namespace VixenModules.App.CustomPropEditor.Services
{
    public class MessageBoxService
    {
        public string GetUserInput(string question)
        {
            TextDialog dialog = new TextDialog(question);
            var input = string.Empty;

            var validInput = false;
            while (!validInput)
            {
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (dialog.Response == string.Empty)
                    {
                        continue;
                    }

                    input = dialog.Response;
                }

                validInput = true;
            }

            return input;
        }
    }
}

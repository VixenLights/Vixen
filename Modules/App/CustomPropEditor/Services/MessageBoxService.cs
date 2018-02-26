using System.Windows.Forms;
using Common.Controls;

namespace VixenModules.App.CustomPropEditor.Services
{
	public class MessageBoxService
	{
		public string GetUserInput(string question, string title)
		{
			TextDialog dialog = new TextDialog(question, title);
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

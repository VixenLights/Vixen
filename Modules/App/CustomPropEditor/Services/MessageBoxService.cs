using System.Drawing;
using System.Windows.Forms;
using Catel;
using Catel.Services;
using Common.Controls;

namespace VixenModules.App.CustomPropEditor.Services
{
	public class MessageBoxService
	{
		public MessageBoxResponse GetUserInput(string question, string title, string defaultText, Form parent=null)
		{
			TextDialog dialog = new TextDialog(question, title, defaultText, true);

			if (parent == null)
			{
				dialog.StartPosition = FormStartPosition.CenterScreen;
			}
			var input = string.Empty;

			var validInput = false;
			DialogResult result;
			do
			{
				result = dialog.ShowDialog(parent);
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
			while (!validInput && result != DialogResult.OK) ;

			return new MessageBoxResponse(Enum<MessageResult>.ConvertFromOtherEnumValue(result), input);
		}

		public MessageBoxResponse GetUserConfirmation(string question, string title)
		{
			MessageBoxForm mbf = new MessageBoxForm(question, title, MessageBoxButtons.YesNoCancel, SystemIcons.Question);
			mbf.ShowDialog();

			return new MessageBoxResponse(Enum<MessageResult>.ConvertFromOtherEnumValue(mbf.DialogResult), null);

		}

	}
}

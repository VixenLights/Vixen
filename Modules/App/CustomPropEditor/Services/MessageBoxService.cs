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

			CenterDialog(dialog);

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
			CenterDialog(mbf);
			mbf.ShowDialog();

			return new MessageBoxResponse(Enum<MessageResult>.ConvertFromOtherEnumValue(mbf.DialogResult), null);

		}

		private static void CenterDialog(Form dialog)
		{
			var parentWindow = System.Windows.Application.Current.MainWindow;
			if (parentWindow != null)
			{
				var relativeCenterParent = new System.Windows.Point(parentWindow.ActualWidth / 2, parentWindow.ActualHeight / 2);
				var centerParent = parentWindow.PointToScreen(relativeCenterParent);
				//This calculates the relative center of the child form.
				var hCenterChild = dialog.Width / 2;
				var vCenterChild = dialog.Height / 2;

				var childLocation = new Point(
					(int) centerParent.X - hCenterChild,
					(int) centerParent.Y - vCenterChild);

				dialog.Location = childLocation;

				//Set the start position to Manual, otherwise the location will be overwritten
				//by the start position calculation.
				dialog.StartPosition = FormStartPosition.Manual;
			}
			else
			{
				dialog.StartPosition = FormStartPosition.CenterScreen;
			}
		}

	}
}

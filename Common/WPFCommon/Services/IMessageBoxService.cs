using System.Windows.Forms;

namespace Common.WPFCommon.Services
{
	public interface IMessageBoxService
	{
		MessageBoxResponse GetUserInput(string question, string title, string defaultText, Form parent=null);
		MessageBoxResponse GetUserConfirmation(string question, string title);
		MessageBoxResponse ShowError(string message, string title);
	}
}
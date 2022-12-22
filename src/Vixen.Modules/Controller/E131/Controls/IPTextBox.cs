namespace VixenModules.Controller.E131.Controls
{
	using System.Windows.Forms;

	public class IpTextBox : TextBox
	{
		protected override CreateParams CreateParams
		{
			get
			{
				var createParams = base.CreateParams;
				createParams.ClassName = "SysIPAddress32";
				createParams.Height = 23;
                Font = (System.Drawing.Font)Font.Clone();
				return createParams;
			}
		}
	}
}
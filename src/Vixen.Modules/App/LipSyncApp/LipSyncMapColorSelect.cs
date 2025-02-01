using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.Theme;
using Vixen.Sys;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncMapColorSelect : BaseForm
	{

		public LipSyncMapColorSelect()
		{
			Location = ActiveForm != null ? new Point(ActiveForm.Location.X + 250, ActiveForm.Location.Y + 100) : new Point(500, 200);
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
		}


		public List<IElementNode> ChosenNodes
		{
			set
			{
				lipSyncMapColorCtrl1.ChosenNodes = value;
			}
		}

		public RGB RGBColor
		{
			get
			{
				return lipSyncMapColorCtrl1.RGBColor;
			}

			set
			{
				lipSyncMapColorCtrl1.RGBColor = value;
			}
		}

		public HSV HSVColor
		{
			get
			{
				return lipSyncMapColorCtrl1.HSVColor;
			}

			set
			{
				lipSyncMapColorCtrl1.HSVColor = value;
			}
		}

		public Color Color
		{
			get 
			{ 
				return lipSyncMapColorCtrl1.Color; 
			}

			set
			{
				lipSyncMapColorCtrl1.Color = value;
			}
		}   

		public double Intensity
		{
			get
			{
				return lipSyncMapColorCtrl1.Intensity;
			}

			set
			{
				lipSyncMapColorCtrl1.Intensity = value;
			}
		}
	}
}

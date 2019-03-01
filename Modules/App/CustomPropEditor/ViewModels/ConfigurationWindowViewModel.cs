using System.Windows.Forms;
using System.Windows.Media;
using Catel.Data;
using Catel.MVVM;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Brush = System.Windows.Media.Brush;
using Color = System.Drawing.Color;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class ConfigurationWindowViewModel:ViewModelBase
	{
		public ConfigurationWindowViewModel()
		{
			
			Config = ConfigurationService.Instance().Config;
			Title = "Edit Config";
		}



		#region Config model property

		/// <summary>
		/// Gets or sets the Config value.
		/// </summary>
		[Model]
		public Configuration Config
		{
			get { return GetValue<Configuration>(ConfigProperty); }
			private set { SetValue(ConfigProperty, value); }
		}

		/// <summary>
		/// Config property data.
		/// </summary>
		public static readonly PropertyData ConfigProperty = RegisterProperty("Config", typeof(Configuration));

		#endregion

		#region LightColor property

		/// <summary>
		/// Gets or sets the LightColor value.
		/// </summary>
		[ViewModelToModel("Config")]
		public Brush LightColor
		{
			get { return GetValue<Brush>(LightColorProperty); }
			set { SetValue(LightColorProperty, value); }
		}

		/// <summary>
		/// LightColor property data.
		/// </summary>
		public static readonly PropertyData LightColorProperty = RegisterProperty("LightColor", typeof(Brush));

		#endregion

		#region SelectedLightColor property

		/// <summary>
		/// Gets or sets the SelectedLightColor value.
		/// </summary>
		[ViewModelToModel("Config")]
		public Brush SelectedLightColor
		{
			get { return GetValue<Brush>(SelectedLightColorProperty); }
			set { SetValue(SelectedLightColorProperty, value); }
		}

		/// <summary>
		/// SelectedLightColor property data.
		/// </summary>
		public static readonly PropertyData SelectedLightColorProperty = RegisterProperty("SelectedLightColor", typeof(Brush), null);

		#endregion

		#region EditLightColor command

		private Command _editLightColorCommand;

		/// <summary>
		/// Gets the EditLightColor command.
		/// </summary>
		public Command EditLightColorCommand
		{
			get { return _editLightColorCommand ?? (_editLightColorCommand = new Command(EditLightColor)); }
		}

		/// <summary>
		/// Method to invoke when the EditLightColor command is executed.
		/// </summary>
		private void EditLightColor()
		{
			LightColor = EditColor((SolidColorBrush)LightColor);
		}

		#endregion

		#region EditSelectedLightColor command

		private Command _editSelectedLightColorCommand;

		/// <summary>
		/// Gets the EditSelectedLightColor command.
		/// </summary>
		public Command EditSelectedLightColorCommand
		{
			get { return _editSelectedLightColorCommand ?? (_editSelectedLightColorCommand = new Command(EditSelectedLightColor)); }
		}

		/// <summary>
		/// Method to invoke when the EditSelectedLightColor command is executed.
		/// </summary>
		private void EditSelectedLightColor()
		{
			SelectedLightColor = EditColor((SolidColorBrush) SelectedLightColor);
		}

		#endregion

		private Brush EditColor(SolidColorBrush brush)
		{
			var color = Convert(brush.Color);
			var newColor = color;
			using (ColorPicker cp = new ColorPicker())
			{
				cp.LockValue_V = false;
				cp.Color = XYZ.FromRGB(color);
				DialogResult result = cp.ShowDialog();
				if (result == DialogResult.OK)
				{
					newColor = cp.Color.ToRGB();
				}
			}
			
			return new SolidColorBrush(Convert(newColor)); ;
		}

		private System.Windows.Media.Color Convert(Color color)
		{
			return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		private Color Convert(System.Windows.Media.Color mediaColor)
		{
			return Color.FromArgb(
				mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
		}

	}
}

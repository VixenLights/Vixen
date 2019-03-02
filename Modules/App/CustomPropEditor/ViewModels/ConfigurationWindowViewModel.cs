using System.Threading.Tasks;
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
			UpdateColors();
			Title = "Preferences";
		}

		private void UpdateColors()
		{
			LightColor = Config.LightColor;
			SelectedLightColor = Config.SelectedLightColor;
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

		#region RestoreDefaults command

		private Command _restoreDefaultsCommand;

		/// <summary>
		/// Gets the RestoreDefaults command.
		/// </summary>
		public Command RestoreDefaultsCommand
		{
			get { return _restoreDefaultsCommand ?? (_restoreDefaultsCommand = new Command(RestoreDefaults)); }
		}

		/// <summary>
		/// Method to invoke when the RestoreDefaults command is executed.
		/// </summary>
		private void RestoreDefaults()
		{
			RestoreColorDefaults();
		}

		#endregion

		#region Cancel command

		private Command _cancelCommand;

		/// <summary>
		/// Gets the Cancel command.
		/// </summary>
		public Command CancelCommand
		{
			get { return _cancelCommand ?? (_cancelCommand = new Command(Cancel)); }
		}

		/// <summary>
		/// Method to invoke when the Cancel command is executed.
		/// </summary>
		public void Cancel()
		{
			this.CancelAndCloseViewModelAsync();
		}

		#endregion

		#region Ok command

		private Command _okCommand;

		/// <summary>
		/// Gets the Ok command.
		/// </summary>
		public Command OkCommand
		{
			get { return _okCommand ?? (_okCommand = new Command(Ok)); }
		}

		/// <summary>
		/// Method to invoke when the Ok command is executed.
		/// </summary>
		private void Ok()
		{
			this.SaveAndCloseViewModelAsync();

		}

		#endregion

		private void RestoreColorDefaults()
		{
			LightColor = Brushes.White;
			SelectedLightColor = Brushes.HotPink;
		}


		#region Overrides of ViewModelBase

		/// <inheritdoc />
		protected override Task<bool> SaveAsync()
		{
			Config.LightColor = LightColor;
			Config.SelectedLightColor = SelectedLightColor;
		
			return Task.FromResult(true);
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

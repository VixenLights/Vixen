﻿using System.Windows.Forms;
using Catel.Data;
using Catel.MVVM;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Color = System.Drawing.Color;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class ConfigurationWindowViewModel:ViewModelBase
	{
		public ConfigurationWindowViewModel()
		{
			
			Config = ConfigurationService.Instance().Config;
			UpdateConfigValues();
			Title = "Preferences";
		}

		private void UpdateConfigValues()
		{
			LightColor = Config.LightColor;
			SelectedLightColor = Config.SelectedLightColor;
			DefaultLightSize = Config.DefaultLightSize;
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
		public static readonly IPropertyData ConfigProperty = RegisterProperty<Configuration>(nameof(Config));

		#endregion

		#region DefaultLightSize property

		/// <summary>
		/// Gets or sets the DefaultLightSize value.
		/// </summary>
		public uint DefaultLightSize
		{
			get => GetValue<uint>(DefaultLightSizeProperty);
			set => SetValue(DefaultLightSizeProperty, value);
		}

		/// <summary>
		/// DefaultLightSize property data.
		/// </summary>
		public static readonly IPropertyData DefaultLightSizeProperty = RegisterProperty<uint>(nameof(DefaultLightSize));

		#endregion

		#region DefaultLightSize property

		/// <summary>
		/// Gets or sets the LightColor value.
		/// </summary>
		public Color LightColor
		{
			get { return GetValue<Color>(LightColorProperty); }
			set { SetValue(LightColorProperty, value); }
		}

		/// <summary>
		/// LightColor property data.
		/// </summary>
		public static readonly IPropertyData LightColorProperty = RegisterProperty<Color>(nameof(LightColor));

		#endregion

		#region SelectedLightColor property

		/// <summary>
		/// Gets or sets the SelectedLightColor value.
		/// </summary>
		public Color SelectedLightColor
		{
			get { return GetValue<Color>(SelectedLightColorProperty); }
			set { SetValue(SelectedLightColorProperty, value); }
		}

		/// <summary>
		/// SelectedLightColor property data.
		/// </summary>
		public static readonly IPropertyData SelectedLightColorProperty = RegisterProperty<Color>(nameof(SelectedLightColor));

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
			LightColor = EditColor(LightColor);
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
			SelectedLightColor = EditColor(SelectedLightColor);
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
			LightColor = Color.White;
			SelectedLightColor = Color.HotPink;
			DefaultLightSize = ElementModel.DefaultLightSize;
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

		#region Overrides of ViewModelBase

		/// <inheritdoc />
		protected override Task<bool> SaveAsync()
		{
			Config.LightColor = LightColor;
			Config.SelectedLightColor = SelectedLightColor;
			Config.DefaultLightSize = DefaultLightSize;
		
			return Task.FromResult(true);
		}


		#endregion


		private Color EditColor(Color color)
		{
			//var color = Convert(brush.Color);
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
			
			return newColor; 
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

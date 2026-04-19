using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Sys.Props.Model;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{

	/// <summary>
	/// Base class for a wizard page that also displays 3-D prop graphics.
	/// </summary>
	/// <typeparam name="TWizardPage">Type of wizard page</typeparam>
	/// <typeparam name="TPropModel">Type of prop model</typeparam>
	public abstract class LightWizardPageViewModel<TWizardPage, TPropModel> : PropBaseWizardPageViewModel<TWizardPage, TPropModel>
		where TWizardPage : class, IWizardPage
		where TPropModel : class, ILightPropModel, new()
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <typeparam name="TWizardPage">Type of wizard page</typeparam>
		public LightWizardPageViewModel(TWizardPage wizardPage) : base(wizardPage)
		{
		}

		#endregion

		#region LightSize property

		/// <summary>
		/// Gets or sets the size of each light.
		/// </summary>
		/// <remarks>
		/// The size of the lights is constrained by <see cref="LightSizeMinimum"/> and <see cref="LightSizeMaximum"/>, consequently
		/// set these values prior to setting LightSize.
		/// </remarks>
		[ViewModelToModel]
		public int LightSize
		{
			get { return GetValue<int>(LightSizeProperty); }
			set
			{
				SetValue(LightSizeProperty, Math.Clamp(value, LightSizeMinimum, LightSizeMaximum));
				PropModel.LightSize = Math.Clamp(value, LightSizeMinimum, LightSizeMaximum);				
			}
		}
		
		private static readonly IPropertyData LightSizeProperty = RegisterProperty<int>(nameof(LightSize));


		[ViewModelToModel]
		public int LightSizeMinimum
		{
			get { return GetValue<int>(LightSizeMinimumProperty); }
			set
			{
				if (LightSize < value)
				{
					LightSize = value;
				}
				SetValue(LightSizeMinimumProperty, value);
			}
		}
		private static readonly IPropertyData LightSizeMinimumProperty = RegisterProperty<int>(nameof(LightSizeMinimum));

		[ViewModelToModel]
		public int LightSizeMaximum
		{
			get { return GetValue<int>(LightSizeMaximumProperty); }
			set
			{
				if (LightSize > value)
				{
					LightSize = value;
				}
				SetValue(LightSizeMaximumProperty, value);
			}
		}
		private static readonly IPropertyData LightSizeMaximumProperty = RegisterProperty<int>(nameof(LightSizeMaximum));

		#endregion
	}
}

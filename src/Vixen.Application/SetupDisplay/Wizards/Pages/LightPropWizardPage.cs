using Catel.Data;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	/// <summary>
	/// Maintains base Light Prop Wizard page data.
	/// </summary>
	public abstract class LightPropWizardPage : BasePropWizardPage, ILightPropWizardPage
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public LightPropWizardPage()
		{
			LightSizeMinimum = 1;
			LightSizeMaximum = 50;
			LightSize = 2;
		}

		#endregion

		#region ILightPropWizardPage

		/// <summary>
		/// Gets or sets the size of each light.
		/// </summary>
		/// <remarks>
		/// The size of the lights is constrained by <see cref="LightSizeMinimum"/> and <see cref="LightSizeMaximum"/>, consequently
		/// set these values prior to setting LightSize.
		/// </remarks>
		public int LightSize
		{
			get { return GetValue<int>(LightSizeProperty); }
			set { SetValue(LightSizeProperty, Math.Clamp(value, LightSizeMinimum, LightSizeMaximum)); }
		}

		private static readonly IPropertyData LightSizeProperty = RegisterProperty<int>(nameof(LightSize));

		#endregion

		#region Protected Properties

		protected int LightSizeMinimum
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

		protected int LightSizeMaximum
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

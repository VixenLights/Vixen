using Catel.Data;
using Common.WPFCommon.Converters;
using Orc.Wizard;
using Vixen.Extensions;
using Vixen.Sys.Props;
using VixenModules.App.Props.Models.Arch;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	/// <summary>
	/// Maintains an Arch Prop Wizard page.
	/// </summary>
	public class ArchPropWizardPage : LightPropWizardPage
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ArchPropWizardPage()
		{
			Title = "Basic Attributes";
			Description = $"Enter attributes for {PropType.Arch.GetEnumDescription()}";
			NodeCountMinimum = 3;
			NodeCountMaximum = 10000;
			
			// Generic parameters
			Name = "Arch";
			NodeCount = 20;
			LightSize = 2;
			ArchWiringStart = ArchStartLocation.Left;			
		}

		#endregion

		#region NodeCount Property
		/// <summary>
		/// Gets or sets the number of lights.
		/// </summary>
		/// <remarks>
		/// The number of lights is constrained by <see cref="NodeCountMinimum"/> and <see cref="NodeCountMaximum"/>, consequently
		/// set these values prior to setting NodeCount.
		/// </remarks>
		public int NodeCount
		{
			get { return GetValue<int>(NodeCountProperty); }
			set { SetValue(NodeCountProperty, value); }
		}
		private static readonly IPropertyData NodeCountProperty = RegisterProperty<int>(nameof(NodeCount));

		protected int NodeCountMinimum
		{
			get { return GetValue<int>(NodeCountMinimumProperty); }
			set
			{
				if (NodeCount < value)
				{
					NodeCount = value;
				}
				SetValue(NodeCountMinimumProperty, value);
			}
		}
		private static readonly IPropertyData NodeCountMinimumProperty = RegisterProperty<int>(nameof(NodeCountMinimum));

		protected int NodeCountMaximum
		{
			get { return GetValue<int>(NodeCountMaximumProperty); }
			set
			{
				if (NodeCount > value)
				{
					NodeCount = value;
				}
				SetValue(NodeCountMaximumProperty, value);
			}
		}
		private static readonly IPropertyData NodeCountMaximumProperty = RegisterProperty<int>(nameof(NodeCountMaximum));
		
		#endregion
		
		#region ArchWiringStart Property

		/// <summary>
		/// Gets or sets the Wiring Start value.
		/// </summary>
		public ArchStartLocation ArchWiringStart
		{
			get { return GetValue<ArchStartLocation>(ArchWiringStartProperty); }
			set { SetValue(ArchWiringStartProperty, value); }
		}
		
		private static readonly IPropertyData ArchWiringStartProperty = RegisterProperty<ArchStartLocation>(nameof(ArchWiringStart));

		#endregion

		#region Public Methods

		/// <summary>
		/// Get a summary of the wizard page settings.
		/// </summary>
		/// <returns>Summary object.</returns>
		public override ISummaryItem GetSummary()
		{
			return new SummaryItem
			{
				Title = this.Title,
				Summary = $"Prop Type: {PropType.Arch.GetEnumDescription()}\n" +
						  $"Name: {Name}\n" +
						  $"Light Count: {NodeCount}\n" +
						  $"Light Size: {LightSize}\n" +
						  $"Starting Position: {EnumValueTypeConverter.GetDescription(ArchWiringStart)}\n" +
						  $"{Rotations[0].Axis} Rotation: {Rotations[0].RotationAngle}\u00B0\n" +
						  $"{Rotations[1].Axis} Rotation: {Rotations[1].RotationAngle}\u00B0\n" +
						  $"{Rotations[2].Axis} Rotation: {Rotations[2].RotationAngle}\u00B0"
			};
		}

		#endregion
	}
}

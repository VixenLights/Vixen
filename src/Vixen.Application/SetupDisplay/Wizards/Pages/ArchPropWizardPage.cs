﻿using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Vixen.Extensions;
using Vixen.Sys;
using Vixen.Sys.Props;
using VixenModules.App.Props.Models.Arch;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	public class ArchPropWizardPage : WizardPageBase
	{
		public ArchPropWizardPage()
		{
			Title = "Basic Attributes";
			Description = $"Enter attributes for {PropType.Arch.GetEnumDescription()}";
			Name = VixenSystem.Props.GenerateUniquePropTitle(PropType.Arch);
			NodeCountMinimum = 3;
			NodeCountMaximum = 100;
			NodeCount = 24;
			LightSizeMinimum = 1;
			LightSizeMaximum = 50;
			LightSize = 5;
			StringType = StringTypes.Pixel;
			RotationMinimum = 0;
			RotationMaximum = 359;
			Rotation = 0;
			ArchWiringStart = ArchStartLocation.Left;
			LeftRight = false;

			Number = 1;
		}

		#region Name property
		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		private static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));
		#endregion

		#region NodeCount property
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
			set { SetValue(NodeCountProperty, Math.Clamp(value, NodeCountMinimum, NodeCountMaximum)); }
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

		#region StringType property
		/// <summary>
		/// Gets or sets the prop type.
		/// </summary>
		public StringTypes StringType
		{
			get { return GetValue<StringTypes>(StringTypeProperty); }
			set { SetValue(StringTypeProperty, value); }
		}
		private static readonly IPropertyData StringTypeProperty = RegisterProperty<StringTypes>(nameof(StringType));
		#endregion

		#region LightSize property
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

		#region ArchWiringStart property
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

		#region Rotation property
		/// <summary>
		/// Gets or sets the degree of rotation.
		/// </summary>
		/// <remarks>
		/// The rotation is constrained by <see cref="RotationMinimum"/> and <see cref="RotationMaximum"/>, consequently
		/// set these values prior to setting Rotation.
		/// </remarks>
		public int Rotation
		{
			get { return GetValue<int>(RotationProperty); }
			set { SetValue(RotationProperty, Math.Clamp(value, RotationMinimum, RotationMaximum)); }
		}
		private static readonly IPropertyData RotationProperty = RegisterProperty<int>(nameof(Rotation));

		protected int RotationMinimum
		{
			get { return GetValue<int>(RotationMinimumProperty); }
			set
			{
				if (Rotation > value)
				{
					Rotation = value;
				}
				SetValue(RotationMinimumProperty, value);
			}
		}
		private static readonly IPropertyData RotationMinimumProperty = RegisterProperty<int>(nameof(RotationMinimum));

		protected int RotationMaximum
		{
			get { return GetValue<int>(RotationMaximumProperty); }
			set
			{
				if (Rotation > value)
				{
					Rotation = value;
				}
				SetValue(RotationMaximumProperty, value);
			}
		}
		private static readonly IPropertyData RotationMaximumProperty = RegisterProperty<int>(nameof(RotationMaximum));
		#endregion

		#region Optional Left / Right property
		/// <summary>
		/// Gets or sets if the wizard will additional generate a left and right group.
		/// </summary>
		public bool LeftRight
		{
			get { return GetValue<bool>(LeftRightProperty); }
			set { SetValue(LeftRightProperty, value); }
		}
		private static readonly IPropertyData LeftRightProperty = RegisterProperty<bool>(nameof(LeftRight));
		#endregion

		public override ISummaryItem GetSummary()
		{
			return new SummaryItem
			{
				Title = this.Title,
				Summary = $"Prop Type: {PropType.Arch.GetEnumDescription()}\nName: {Name}\nLight Count: {NodeCount}\nLight Size: {LightSize}\n" +
				          $"Light Type: {StringType}\nStarting Position: {ArchWiringStart}\nRotation: {Rotation}\u00B0"
			};
		}

		/// <summary>
		/// Gets the prop based on the settings in the wizard.
		/// </summary>
		/// <returns>A copy of the prop.</returns>
		public IProp GetProp()
		{
			var arch = VixenSystem.Props.CreateProp<Arch>(Name);
			arch.NodeCount = NodeCount;
			arch.StringType = StringType;
			arch.LightSize = LightSize;
			arch.Rotation = Rotation;
			return arch;
		}
	}
}

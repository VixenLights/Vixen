using Catel.Data;
using Catel.MVVM;
using System.Collections.ObjectModel;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Props.Models.Arch;
using VixenModules.App.Props.Models;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	public class ArchPropWizardPageViewModel : GraphicsWizardPageViewModelBase<ArchPropWizardPage, VixenModules.App.Props.Models.Arch.ArchModel>, IPropWizardPageViewModel, VixenModules.App.Props.Models.Arch.IAttributeData
	{
		public ArchPropWizardPageViewModel(ArchPropWizardPage wizardPage) : base(wizardPage)
		{
			LightPropModel.SetContext(this);
		}

		/// <summary>
		/// Refreshes the 3-D OpenGL graphics.
		/// </summary>
		private void RefreshGraphics()
		{
			// Update the prop nodes
			LightPropModel.UpdatePropNodes();
		}

		#region Name property
		/// <summary>
		/// Gets or sets the prop name.
		/// </summary>
		[ViewModelToModel]
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
		[ViewModelToModel]
		public int NodeCount
		{
			get { return GetValue<int>(NodeCountProperty); }
			set 
			{ 
				SetValue(NodeCountProperty, value);
				RefreshGraphics();
			}
		}
		private static readonly IPropertyData NodeCountProperty = RegisterProperty<int>(nameof(NodeCount));

		[ViewModelToModel]
		public int NodeCountMinimum
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

		[ViewModelToModel]
		public int NodeCountMaximum
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
		[ViewModelToModel]
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
		[ViewModelToModel]
		public int LightSize
		{
			get { return GetValue<int>(LightSizeProperty); }
			set 
			{
				SetValue(LightSizeProperty, Math.Clamp(value, LightSizeMinimum, LightSizeMaximum));
				RefreshGraphics();
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

		#region ArchWiringStart property
		/// <summary>
		/// Gets or sets the Wiring Start value.
		/// </summary>
		[ViewModelToModel]
		public ArchStartLocation ArchWiringStart
		{
			get { return GetValue<ArchStartLocation>(ArchWiringStartProperty); }
			set { SetValue(ArchWiringStartProperty, value); }
		}

		/// <summary>
		/// ArchWiringStart property data.
		/// </summary>
		private static readonly IPropertyData ArchWiringStartProperty = RegisterProperty<ArchStartLocation>(nameof(ArchWiringStart));
		#endregion

		#region Rotation Control
		/// <summary>
		/// Collection of rotations to support rotating the props around the x,y, and z axis.
		/// </summary>
		[ViewModelToModel]
		public ObservableCollection<AxisRotationViewModel> Rotations
		{
			get
			{
				return GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty);
			}
			set
			{
				SetValue(RotationsProperty, value);
			}
		}

		public static readonly IPropertyData RotationsProperty = RegisterProperty<ObservableCollection<AxisRotationViewModel>>(nameof(Rotations));
		#endregion

		#region Optional Left / Right property
		/// <summary>
		/// Gets or sets if the wizard will additional generate a left and right group.
		/// </summary>
		[ViewModelToModel]
		public bool LeftRight
		{
			get { return GetValue<bool>(leftRightProperty); }
			set { SetValue(leftRightProperty, value); }
		}
		private static readonly IPropertyData leftRightProperty = RegisterProperty<bool>(nameof(LeftRight));
		#endregion

		/// <summary>
		/// Performs validation on the properties.
		/// </summary>
		/// <param name="validationResults"></param>
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			base.ValidateFields(validationResults);

			if (string.IsNullOrWhiteSpace(Name))
			{
				validationResults.Add(FieldValidationResult.CreateError("Name", "Name is required"));
			}
		}
	}
}
using Catel.Data;
using Catel.MVVM;

using Vixen.Sys.Props;

using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Props;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	/// <summary>
	/// Maintains a tree prop wizard page view model.
	/// </summary>
	public class TreePropWizardPageViewModel : GraphicsWizardPageViewModelBase<TreePropWizardPage, VixenModules.App.Props.Models.Tree.TreeModel>, IPropWizardPageViewModel
	{
		public TreePropWizardPageViewModel(TreePropWizardPage wizardPage) : base(wizardPage)
		{			
		}
		
		/// <summary>
		/// Refreshes the 3-D OpenGL graphics.
		/// </summary>
		private void RefreshGraphics()
		{
			// Pass the properties needed to draw the graphics to the light prop model
			LightPropModel.Strings = Strings;
			LightPropModel.NodesPerString = NodesPerString;
			LightPropModel.TopWidth = TopWidth;
			LightPropModel.TopHeight = TopWidth;
			LightPropModel.BaseHeight = BaseHeight;
			LightPropModel.DegreesCoverage = DegreesCoverage;
			LightPropModel.Strings = Strings;
			LightPropModel.NodesPerString = NodesPerString;
			LightPropModel.LightSize = LightSize;
			LightPropModel.TopRadius = TopRadius;
			LightPropModel.BottomRadius = BottomRadius;			
		}

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		[ViewModelToModel]
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

		#endregion

		#region Strings property

		/// <summary>
		/// Gets or sets the Strings value.
		/// </summary>
		[ViewModelToModel]
		public int Strings
		{
			get { return GetValue<int>(NodeCountProperty); }
			set
			{
				SetValue(NodeCountProperty, value);
				RefreshGraphics();
			}
		}
		public static readonly IPropertyData NodeCountProperty = RegisterProperty<int>(nameof(Strings));

		#endregion

		#region NodesPerString property

		/// <summary>
		/// Gets or sets the NodesPerString value.
		/// </summary>
		[ViewModelToModel]
		public int NodesPerString
		{
			get { return GetValue<int>(NodesPerStringProperty); }
			set
			{
				SetValue(NodesPerStringProperty, value);
				RefreshGraphics();
			}
		}

		/// <summary>
		/// NodesPerString property data.
		/// </summary>
		public static readonly IPropertyData NodesPerStringProperty = RegisterProperty<int>(nameof(NodesPerString));

		#endregion


		[ViewModelToModel]
		public int LightSize
		{
			get { return GetValue<int>(LightSizeProperty); }
			set { SetValue(LightSizeProperty, Math.Clamp(value, LightSizeMinimum, LightSizeMaximum)); }
		}
		private static readonly IPropertyData LightSizeProperty = RegisterProperty<int>(nameof(LightSize));

		[ViewModelToModel]
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

		[ViewModelToModel]
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

		[ViewModelToModel]
		public int DegreesCoverage
		{
			get { return GetValue<int>(DegreesCoverageProperty); }
			set 
			{
				SetValue(DegreesCoverageProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData DegreesCoverageProperty = RegisterProperty<int>(nameof(DegreesCoverage));

		[ViewModelToModel]
		public int DegreeOffset
		{
			get { return GetValue<int>(DegreeOffsetProperty); }
			set 
			{
				SetValue(DegreeOffsetProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData DegreeOffsetProperty = RegisterProperty<int>(nameof(DegreeOffset));

		[ViewModelToModel]
		public int BaseHeight
		{
			get { return GetValue<int>(BaseHeightProperty); }
			set 
			{
				SetValue(BaseHeightProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData BaseHeightProperty = RegisterProperty<int>(nameof(BaseHeight));

		[ViewModelToModel]
		public int TopHeight
		{
			get { return GetValue<int>(TopHeightProperty); }
			set 
			{ 
				SetValue(TopHeightProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData TopHeightProperty = RegisterProperty<int>(nameof(TopHeight));

		[ViewModelToModel]
		public int TopWidth
		{
			get { return GetValue<int>(TopWidthProperty); }
			set 
			{ 
				SetValue(TopWidthProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData TopWidthProperty = RegisterProperty<int>(nameof(TopWidth));

		[ViewModelToModel]
		public StartLocation StartLocation
		{
			get { return GetValue<StartLocation>(StartLocationProperty); }
			set 
			{ 
				SetValue(StartLocationProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData StartLocationProperty = RegisterProperty<StartLocation>(nameof(StartLocation));

		[ViewModelToModel]
		public bool ZigZag
		{
			get { return GetValue<bool>(ZigZagProperty); }
			set 
			{ 
				SetValue(ZigZagProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData ZigZagProperty = RegisterProperty<bool>(nameof(ZigZag));

		[ViewModelToModel]
		public int ZigZagOffset
		{
			get { return GetValue<int>(ZigZagOffsetProperty); }
			set 
			{ 
				SetValue(ZigZagOffsetProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData ZigZagOffsetProperty = RegisterProperty<int>(nameof(ZigZagOffset));

		[ViewModelToModel]
		public float TopRadius
		{
			get { return GetValue<float>(TopRadiusProperty); }
			set 
			{ 
				SetValue(TopRadiusProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData TopRadiusProperty = RegisterProperty<float>(nameof(TopRadius));

		[ViewModelToModel]
		public float BottomRadius
		{
			get { return GetValue<float>(BottomRadiusProperty); }
			set 
			{ 
				SetValue(BottomRadiusProperty, value);
				RefreshGraphics();
			}
        }
		public static readonly IPropertyData BottomRadiusProperty = RegisterProperty<float>(nameof(BottomRadius));




		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			base.ValidateFields(validationResults);

			if (string.IsNullOrWhiteSpace(Name))
			{
				validationResults.Add(FieldValidationResult.CreateError(nameof(Name), "Name is required"));
			}

			if (Strings <= 0)
			{
				validationResults.Add(
					FieldValidationResult.CreateError(nameof(Strings), "String Count must be greater than 0"));
			}

			if (NodesPerString <= 0)
			{
				validationResults.Add(FieldValidationResult.CreateError(nameof(NodesPerString),
					"Nodes per string must be greater than 0"));

			}
		}

		protected override async Task InitializeAsync()
		{
			await base.InitializeAsync();

			RefreshGraphics();
		}
	}
}
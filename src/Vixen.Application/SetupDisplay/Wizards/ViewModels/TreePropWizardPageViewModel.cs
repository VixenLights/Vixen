using Catel.Data;
using Catel.MVVM;

using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Props;
using VixenModules.App.Props.Models.Tree;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	/// <summary>
	/// Maintains a tree prop wizard page view model.
	/// </summary>
	public class TreePropWizardPageViewModel : LightWizardPageViewModel<TreePropWizardPage, TreeModel>, IPropWizardPageViewModel
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="wizardPage">Type of wizard page</param>
		public TreePropWizardPageViewModel(TreePropWizardPage wizardPage) : base(wizardPage)
		{			
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Refreshes the 3-D OpenGL graphics.
		/// </summary>
		private void RefreshGraphics()
		{
			// Pass the properties needed to draw the graphics to the light prop model
			PropModel.Strings = Strings;
			PropModel.NodesPerString = NodesPerString;
			PropModel.TopWidth = TopWidth;
			PropModel.TopHeight = TopWidth;
			PropModel.BaseHeight = BaseHeight;
			PropModel.DegreesCoverage = DegreesCoverage;
			PropModel.Strings = Strings;
			PropModel.NodesPerString = NodesPerString;
			PropModel.LightSize = LightSize;
			PropModel.TopRadius = TopRadius;
			PropModel.BottomRadius = BottomRadius;			
		}

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

		#region Public Properties

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

		#endregion

		#region Protected Methods

		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			base.ValidateFields(validationResults);
			
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

		#endregion
	}
}
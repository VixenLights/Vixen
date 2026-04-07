using Catel.Data;
using Common.WPFCommon.Converters;
using Orc.Wizard;
using System.Collections.ObjectModel;
using Vixen.Extensions;
using Vixen.Sys;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Model;
using VixenApplication.SetupDisplay.ViewModels;
using VixenModules.App.Props;
using VixenModules.App.Props.Models.Tree;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
    public class TreePropWizardPage : WizardPageBase, IPropWizardFinalPage
    {
        public TreePropWizardPage()
        {
            Title = "Basic Attributes";
            Description = $"Enter attributes for {PropType.Tree.GetEnumDescription()}";

            // Generic parameters
            Name = "Tree 1";
            Strings = 16;
            NodesPerString = 50;
            DegreesCoverage = 360;
            LightSize = 2;
            TopRadius = 10;
            BottomRadius = 100;

			// Initialize the Rotation collection
			ObservableCollection<AxisRotationModel> rotations = new ObservableCollection<AxisRotationModel>();
			rotations.Add(new AxisRotationModel() { Axis = Axis.XAxis, RotationAngle = 0 });
			rotations.Add(new AxisRotationModel() { Axis = Axis.YAxis, RotationAngle = 0 });
			rotations.Add(new AxisRotationModel() { Axis = Axis.ZAxis, RotationAngle = 0 });
			Rotations = AxisRotationViewModel.ConvertToViewModel(rotations);
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

        /// <summary>
        /// Name property data.
        /// </summary>
        public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

        #endregion

        #region Strings property

        /// <summary>
        /// Gets or sets the Strings value.
        /// </summary>
        public int Strings
        {
            get { return GetValue<int>(NodeCountProperty); }
            set { SetValue(NodeCountProperty, value); }
        }
        public static readonly IPropertyData NodeCountProperty = RegisterProperty<int>(nameof(Strings));

        #endregion

        #region NodesPerString property

        /// <summary>
        /// Gets or sets the NodesPerString value.
        /// </summary>
        public int NodesPerString
        {
            get { return GetValue<int>(NodesPerStringProperty); }
            set { SetValue(NodesPerStringProperty, value); }
        }

        /// <summary>
        /// NodesPerString property data.
        /// </summary>
        public static readonly IPropertyData NodesPerStringProperty = RegisterProperty<int>(nameof(NodesPerString));

        #endregion

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

        public int DegreesCoverage
        {
            get { return GetValue<int>(DegreesCoverageProperty); }
            set { SetValue(DegreesCoverageProperty, value); }
        }
        public static readonly IPropertyData DegreesCoverageProperty = RegisterProperty<int>(nameof(DegreesCoverage));

        public int DegreeOffset
        {
            get { return GetValue<int>(DegreeOffsetProperty); }
            set { SetValue(DegreeOffsetProperty, value); }
        }
        public static readonly IPropertyData DegreeOffsetProperty = RegisterProperty<int>(nameof(DegreeOffset));

        public int BaseHeight
        {
            get { return GetValue<int>(BaseHeightProperty); }
            set { SetValue(BaseHeightProperty, value); }
        }
        public static readonly IPropertyData BaseHeightProperty = RegisterProperty<int>(nameof(BaseHeight));

        public int TopHeight
        {
            get { return GetValue<int>(TopHeightProperty); }
            set { SetValue(TopHeightProperty, value); }
        }
        public static readonly IPropertyData TopHeightProperty = RegisterProperty<int>(nameof(TopHeight));

        public int TopWidth
        {
            get { return GetValue<int>(TopWidthProperty); }
            set { SetValue(TopWidthProperty, value); }
        }
        public static readonly IPropertyData TopWidthProperty = RegisterProperty<int>(nameof(TopWidth));

        public StartLocation StartLocation
        {
            get { return GetValue<StartLocation>(StartLocationProperty); }
            set { SetValue(StartLocationProperty, value); }
        }
        public static readonly IPropertyData StartLocationProperty = RegisterProperty<StartLocation>(nameof(StartLocation));

        public bool ZigZag
        {
            get { return GetValue<bool>(ZigZagProperty); }
            set { SetValue(ZigZagProperty, value); }
        }
        public static readonly IPropertyData ZigZagProperty = RegisterProperty<bool>(nameof(ZigZag));

        public int ZigZagOffset
        {
            get { return GetValue<int>(ZigZagOffsetProperty); }
            set { SetValue(ZigZagOffsetProperty, value); }
        }
        public static readonly IPropertyData ZigZagOffsetProperty = RegisterProperty<int>(nameof(ZigZagOffset));

        public float TopRadius
        {
            get { return GetValue<float>(TopRadiusProperty); }
            set { SetValue(TopRadiusProperty, value); }
        }
        public static readonly IPropertyData TopRadiusProperty = RegisterProperty<float>(nameof(TopRadius));

        public float BottomRadius
        {
            get { return GetValue<float>(BottomRadiusProperty); }
            set { SetValue(BottomRadiusProperty, value); }
        }
        public static readonly IPropertyData BottomRadiusProperty = RegisterProperty<float>(nameof(BottomRadius));

        #region Rotation property
        
		
		public ObservableCollection<AxisRotationViewModel> Rotations
        {
            get { return GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty); }
            set { SetValue(RotationsProperty, value); }
        }
        private static readonly IPropertyData RotationsProperty = RegisterProperty<ObservableCollection<AxisRotationViewModel>>(nameof(Rotations));
		
        #endregion

        public TreeModel LightPropModel
        {
            get { return GetValue<TreeModel>(LightPropModelProperty); }
            set { SetValue(LightPropModelProperty, value); }
        }
        private static readonly IPropertyData LightPropModelProperty = RegisterProperty<TreeModel>(nameof(LightPropModel));

        public override ISummaryItem GetSummary()
        {
            return new SummaryItem
            {
                Title = this.Title,
                Summary = $"Prop Type: {PropType.Tree.GetEnumDescription()}\n" +
                          $"Name: {Name}\n" +
                          $"Strings: {Strings}\n" +
                          $"Light Size: {LightSize}\n" +
                          $"Start Location: {EnumValueTypeConverter.GetDescription(StartLocation)}\n" +
                          $"Nodes Per String: {NodesPerString}\n" +
                          $"Degree Offset: {DegreeOffset}\n" +
                          $"Degrees Coverage: {DegreesCoverage}\n" +
                          $"Base Height: {BaseHeight}\n" +
                          $"Top Height: {TopHeight}\n" +
                          $"Top Width: {TopWidth}\n" +
                          $"ZigZag: {ZigZag}\n" +
                          $"ZigZag Offset: {ZigZagOffset}\n" +
                          $"Top Radius: {TopRadius}\n" +
                          $"Bottom Radius: {BottomRadius}\n" +
                          $"{Rotations[0].Axis} Rotation: {Rotations[0].RotationAngle}\u00B0\n" +
                          $"{Rotations[1].Axis} Rotation: {Rotations[1].RotationAngle}\u00B0\n" +
                          $"{Rotations[2].Axis} Rotation: {Rotations[2].RotationAngle}\u00B0"
            };
        }

        public IProp GetProp()
        {
            var tree = VixenSystem.Props.CreateProp<TreeProp>(Name);
            tree.Strings = Strings;
            tree.NodesPerString = NodesPerString;
            //TODO add in other fields when wizard has full function
            return tree;
        }
    }
}
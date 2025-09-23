using Catel.Data;
using Orc.Wizard;
using Vixen.Sys;
using Vixen.Sys.Props;
using VixenModules.App.Props.Models.Arch;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	public class ArchPropWizardPage : WizardPageBase, IPropWizardFinalPage
	{
		public ArchPropWizardPage()
		{
			Title = "Arch";
			Description = $"Enter the details for your {Title}";
			Name = "Arch 1";
			NodeCount = 25;
			StringType = StringTypes.Pixel;
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

		#region NodeCount property

		/// <summary>
		/// Gets or sets the NodeCount value.
		/// </summary>
		public int NodeCount
		{
			get { return GetValue<int>(NodeCountProperty); }
			set { SetValue(NodeCountProperty, value); }
		}

		/// <summary>
		/// NodeCount property data.
		/// </summary>
		public static readonly IPropertyData NodeCountProperty = RegisterProperty<int>(nameof(NodeCount));

		#endregion

		#region StringType property

		/// <summary>
		/// Gets or sets the StringType value.
		/// </summary>
		public StringTypes StringType
		{
			get { return GetValue<StringTypes>(StringTypeProperty); }
			set { SetValue(StringTypeProperty, value); }
		}

		/// <summary>
		/// StringType property data.
		/// </summary>
		public static readonly IPropertyData StringTypeProperty = RegisterProperty<StringTypes>(nameof(StringType));

		#endregion

		public override ISummaryItem GetSummary()
		{
			return new SummaryItem
			{
				Title = "Arch",
				Summary = string.Format("A new {0} with name {1} and {2} {3} nodes", Title, Name, NodeCount,
					StringType.ToString())
			};
		}

		public IProp GetProp()
		{
			var arch = VixenSystem.Props.CreateProp<Arch>(Name);
			arch.NodeCount = NodeCount;
			arch.StringType = StringType;
			return arch;
		}
	}
}

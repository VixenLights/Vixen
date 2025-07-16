using Catel.Data;
using Orc.Wizard;
using Vixen.Sys;
using Vixen.Sys.Props;
using VixenModules.App.Props.Models.Tree;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	public class TreePropWizardPage : WizardPageBase, IPropWizardFinalPage
	{
		public TreePropWizardPage()
		{
			Title = "Tree";
			Description = $"Enter the details for your {Title}";
			Name = "Tree 1";
			Strings = 16;
			NodesPerString = 50;
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

		#region Strings property

		/// <summary>
		/// Gets or sets the Strings value.
		/// </summary>
		public int Strings
		{
			get { return GetValue<int>(NodeCountProperty); }
			set { SetValue(NodeCountProperty, value); }
		}

		/// <summary>
		/// Strings property data.
		/// </summary>
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
				Title = "Tree",
				Summary = string.Format("A new {0} with name {1} and {2} Strings with {3} nodes per string and {4} type nodes", Title, Name, Strings, NodesPerString,
					StringType.ToString())
			};
		}

		public IProp GetProp()
		{
			var tree = VixenSystem.Props.CreateProp<Tree>(Name);
			tree.Strings = Strings;
			tree.NodesPerString = NodesPerString;
			tree.StringType = StringType;
			//TODO add in other fields when wizard has full function
			return tree;
		}
	}
}
using Orc.Wizard;
using Vixen.Sys;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Model.Arch;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	public class ArchPropWizardPage: WizardPageBase, IPropWizardFinalPage
	{
        public ArchPropWizardPage()
        {
            Title = "Arch";
            Description = $"Enter the details for your {Title}";
            Name = "Arch 1";
        }

        public string Name { get; set; }

        public int LightCount { get; set; } = 25;

        public StringTypes StringType { get; set; } = StringTypes.Pixel;

        public override ISummaryItem GetSummary()
        {
            return new SummaryItem
            {
                Title = "Arch",
                Summary = string.Format("A new {0} with name {1} and {2} {3} lights", Title, Name, LightCount,
                    StringType.ToString())
            };
        }

        public IProp GetProp()
        {
            var arch = VixenSystem.Props.CreateProp<Arch>(Name);
            arch.NodeCount = LightCount;
            arch.StringType = StringType;
            return arch;
        }
    }
}

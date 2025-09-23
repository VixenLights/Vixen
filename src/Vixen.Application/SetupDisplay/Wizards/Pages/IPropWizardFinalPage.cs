using Vixen.Sys.Props;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
    /// <summary>
    /// Interface to identify the final page of the wizard that can create the prop with the gathered information.
    /// Only one page of any Prop creation wizard should implement this interface. 
    /// </summary>
    public interface IPropWizardFinalPage
    {
        IProp GetProp();
    }
}
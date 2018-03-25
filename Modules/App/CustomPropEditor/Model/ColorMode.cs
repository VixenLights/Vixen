using System.ComponentModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public enum ColorMode
    {
	    [Description("Full Color")]
		FullColor,
	    [Description("Multiple Color")]
		RGB,
	    [Description("Single Color")]
		Single
    }
}

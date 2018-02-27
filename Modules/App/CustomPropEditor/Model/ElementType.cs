using System.ComponentModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public enum ElementType
    {
	    [Description("Single Node")]
		Node,
	    [Description("Multiple Nodes")]
		String,
	    [Description("Group")]
	    Group

	}
}

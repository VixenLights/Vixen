using System;
using System.ComponentModel;

namespace VixenModules.App.CustomPropEditor.Model
{
	[Serializable]
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

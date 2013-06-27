using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Separated the user interface from the module so that the module could subclass
// a system base class and the UI could subclass something else (like Form).
//
// This interface is empty because everything has been moved to IEditorUserInterface
// while allowing the editor module type to follow the same pattern as the other
// module types.

namespace Vixen.Module.Editor
{
	public interface IEditor
	{
	}
}
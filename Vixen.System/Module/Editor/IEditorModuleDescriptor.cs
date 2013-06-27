using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Editor
{
	public interface IEditorModuleDescriptor : IModuleDescriptor
	{
		Type SequenceType { get; }
		Type EditorUserInterfaceClass { get; }
	}
}
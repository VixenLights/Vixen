using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Vixen.Module.FileTemplate {
	public interface IFileTemplateModuleDescriptor : IModuleDescriptor {
		/// <summary>
		/// Extension of the file type affected by the template.
		/// </summary>
		string FileType { get; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Sequence {
	public interface ISequenceModuleDescriptor : IModuleDescriptor {
		/// <summary>
		/// Includes the leading period.
		/// </summary>
		string FileExtension { get; }

		bool CanCreateNew { get; }
	}
}

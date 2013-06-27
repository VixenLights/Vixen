using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Media
{
	public interface IMediaModuleDescriptor : IModuleDescriptor
	{
		string[] FileExtensions { get; }

		/// <summary>
		/// True if the IMediaModuleInstance implementation has a non-null ITiming member.
		/// </summary>
		bool IsTimingSource { get; }
	}
}
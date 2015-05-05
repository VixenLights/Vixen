using System;

namespace Vixen.Module.SequenceType
{
	public interface ISequenceTypeModuleDescriptor : IModuleDescriptor
	{
		/// <summary>
		/// Includes the leading period.
		/// </summary>
		string FileExtension { get; }

		bool CanCreateNew { get; }

		int ObjectVersion { get; }
	}
}
﻿using Vixen.Sys;

namespace Vixen.Module.Effect
{
	public interface IEffectModuleDescriptor : IModuleDescriptor
	{
		string EffectName { get; }
		ParameterSignature Parameters { get; }
		EffectGroups EffectGroup { get; }
		bool SupportsMedia { get; }
		bool SupportsMarks { get; }
		bool SupportsFiles { get; }
		string[] SupportedFileExtensions { get; }
		Image GetRepresentativeImage();
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Commands;

namespace Vixen.Module.Effect {
	public interface IEffectModuleDescriptor : IModuleDescriptor {
		string EffectName { get; }
		CommandParameterSignature Parameters { get; }
		Image GetRepresentativeImage(int desiredWidth, int desiredHeight);
	}
}

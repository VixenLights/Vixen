using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Transform {
	public interface ITransformModuleDescriptor : IModuleDescriptor {
		Type[] TypesAffected { get; }
		/// <summary>
		/// Command Id : CommandParameterReference
		/// The commands and parameters affected by the transform, resulting from
		/// TypesAffected.
		/// </summary>
		CommandsAffected CommandsAffected { get; set; }
		/// <summary>
		/// This is useful for configuring multiple instances of the same kind of transform.
		/// </summary>
		/// <param name="transforms">Transform instances being configured.</param>
		void Setup(ITransform[] transforms);
	}
}

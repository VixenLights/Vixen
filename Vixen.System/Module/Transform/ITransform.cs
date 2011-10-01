using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;

namespace Vixen.Module.Transform {
	public interface ITransform {
		/// <summary>
		/// Called by the controller when a command needs to be transformed.
		/// The module needs to determine qualification on its own.
		/// </summary>
		/// <param name="command"></param>
		void Transform(Command command);
	}
}

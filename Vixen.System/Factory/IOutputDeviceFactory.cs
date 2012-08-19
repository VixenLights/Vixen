using System;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	interface IOutputDeviceFactory {
		IOutputDevice CreateDevice(Guid moduleId, string name);
		IOutputDevice CreateDevice(Guid id, Guid moduleId, string name);
	}
}

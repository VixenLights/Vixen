using System;
using System.Windows.Forms;
using Vixen.Sys;

namespace Vixen.Module.Preview {
	public abstract class AppContextPreviewModuleInstanceBase : PreviewModuleInstanceBase {
		protected override IThreadBehavior ThreadBehavior {
			get {
				if(VixenSystem.SystemConfig.IsPreviewThreaded) {
					// Needs to be cast to avoid ambiguity since there are no parameters
					// to differentiate the signature.
					Func<ApplicationContext> initMethod = Initialize;
					return new MultiThreadBehavior(initMethod);
				}
				throw new InvalidOperationException("Preview being started without a form in a single-threaded context.");
			}
		}
		abstract protected ApplicationContext Initialize();
	}
}

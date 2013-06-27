using System;
using System.Windows.Forms;
using Vixen.Sys;

namespace Vixen.Module.Preview
{
	public abstract class FormPreviewModuleInstanceBase : PreviewModuleInstanceBase
	{
		private IThreadBehavior _threadBehavior;

		protected override IThreadBehavior ThreadBehavior
		{
			get
			{
				if (_threadBehavior == null) {
					// Needs to be cast to avoid ambiguity since there are no parameters
					// to differentiate the signature.
					Func<Form> initMethod = Initialize;
					_threadBehavior = VixenSystem.SystemConfig.IsPreviewThreaded
					                  	? new MultiThreadBehavior(initMethod)
					                  	: (IThreadBehavior) new SingleThreadBehavior(initMethod);
				}
				return _threadBehavior;
			}
		}

		protected abstract Form Initialize();
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Vixen.Sys;

namespace Vixen.Module.Preview
{
	public abstract class WindowPreviewModuleInstanceBase : PreviewModuleInstanceBase
	{
		private IThreadBehavior _threadBehavior;

		protected override IThreadBehavior ThreadBehavior
		{
			get
			{
				if (_threadBehavior == null)
				{
					// Needs to be cast to avoid ambiguity since there are no parameters
					// to differentiate the signature.
					Func<Window> initMethod = Initialize;
					_threadBehavior = //VixenSystem.SystemConfig.IsPreviewThreaded
										  new WPFMultiThreadBehavior(initMethod);
										  //: (IThreadBehavior)new SingleThreadBehavior(initMethod);
				}
				return _threadBehavior;
			}
		}

		protected abstract Window Initialize();
	}
}

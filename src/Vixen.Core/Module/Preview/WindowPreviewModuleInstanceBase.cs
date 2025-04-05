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
		protected bool UseWPF { get; set; }

		private IThreadBehavior _threadBehavior;

		protected override IThreadBehavior ThreadBehavior
		{
			get
			{
				if (UseWPF)
				{
					if (_threadBehavior == null)
					{
#if OPENGL_PREVIEW_WIN_FORMS
						// Needs to be cast to avoid ambiguity since there are no parameters
						// to differentiate the signature.
						Func<Form> initMethod = InitializeForm;
						_threadBehavior = VixenSystem.SystemConfig.IsPreviewThreaded
											  ? new MultiThreadBehavior(initMethod)
											  : (IThreadBehavior)new SingleThreadBehavior(initMethod);						
#else
						// Needs to be cast to avoid ambiguity since there are no parameters
						// to differentiate the signature.
						Func<Window> initMethod = InitializeWindow;
						_threadBehavior = VixenSystem.SystemConfig.IsPreviewThreaded
											? new WPFMultiThreadBehavior(initMethod)
											: (IThreadBehavior)new WPFSingleThreadBehavior(initMethod);
#endif
					}
					return _threadBehavior;
				}
				else
				{
					if (_threadBehavior == null)
					{
						// Needs to be cast to avoid ambiguity since there are no parameters
						// to differentiate the signature.
						Func<Form> initMethod = InitializeForm;
						_threadBehavior = VixenSystem.SystemConfig.IsPreviewThreaded
											  ? new MultiThreadBehavior(initMethod)
											  : (IThreadBehavior)new SingleThreadBehavior(initMethod);
					}
				}

				return _threadBehavior;
			}
		}

		protected abstract Window InitializeWindow();

		protected abstract Form InitializeForm();				
	}
}

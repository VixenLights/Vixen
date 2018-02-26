using System.Collections.Generic;
using Catel.MVVM;
using Common.WPFCommon.Utils;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class LightViewModelCollection : TransformedCollection<Light, LightViewModel>
	{
		public LightViewModelCollection(IEnumerable<Light> lights, ViewModelBase parent) : base(
			lights,
			light => new LightViewModel(light, parent),
			lightViewModel => lightViewModel.Dispose())
		{
		}
	}
}

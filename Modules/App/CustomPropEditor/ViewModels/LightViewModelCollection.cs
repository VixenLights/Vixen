using System.Collections.Generic;
using Common.WPFCommon.Utils;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.ViewModel;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class LightViewModelCollection:TransformedCollection<Light, LightViewModel>
	{
		public LightViewModelCollection(IEnumerable<Light> lights) : base(
			lights,
			light => new LightViewModel(light), 
			lightViewModel => lightViewModel.Dispose())
		{
		}
	}
}

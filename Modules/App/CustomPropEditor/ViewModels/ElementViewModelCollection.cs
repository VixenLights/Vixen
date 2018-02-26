using System.Collections.Generic;
using Common.WPFCommon.Utils;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class ElementViewModelCollection: TransformedCollection<ElementModel, ElementModelViewModel>
	{
		public ElementViewModelCollection(IEnumerable<ElementModel> elementModels) : base(
			elementModels,
			elementModel => new ElementModelViewModel(elementModel), 
			elementModelViewModel => elementModelViewModel.Dispose())
		{
		}
		
	}
}

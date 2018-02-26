using System.Collections.Generic;
using Common.WPFCommon.Utils;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class ElementViewModelCollection : TransformedCollection<ElementModel, ElementModelViewModel>
	{
		public ElementViewModelCollection(IEnumerable<ElementModel> elementModels, ElementModelViewModel parent) : base(
			elementModels,
			elementModel => new ElementModelViewModel(elementModel, parent),
			elementModelViewModel => elementModelViewModel.Dispose())
		{
		}

	}
}

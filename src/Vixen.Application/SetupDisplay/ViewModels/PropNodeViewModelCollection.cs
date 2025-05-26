using Common.WPFCommon.Utils;
using Vixen.Sys.Props;

namespace VixenApplication.SetupDisplay.ViewModels
{
	public class PropNodeViewModelCollection : TransformedCollection<PropNode, PropNodeViewModel>
	{
		public PropNodeViewModelCollection(IEnumerable<PropNode> propNodeModels, PropNodeViewModel parent) : base(
            propNodeModels,
			elementModel => new PropNodeViewModel(elementModel, parent),
			propNodeViewModel => propNodeViewModel.Dispose())
		{
		}

	}
}

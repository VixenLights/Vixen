using Common.WPFCommon.Utils;
using Vixen.Sys.Props.Components;

namespace VixenApplication.SetupDisplay.ViewModels
{
	public class PropComponentNodeViewModelCollection : TransformedCollection<PropComponentNode, PropComponentNodeViewModel>
	{
		public PropComponentNodeViewModelCollection(IEnumerable<PropComponentNode> propComponentNodeModels, PropComponentNodeViewModel parent) : base(
			propComponentNodeModels,
			elementModel => new PropComponentNodeViewModel(elementModel, parent),
			propNodeViewModel => propNodeViewModel.Dispose())
		{
		}

	}
}

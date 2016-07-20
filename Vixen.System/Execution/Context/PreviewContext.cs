using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Execution.Context
{
	public class PreviewContext: LiveContext
	{
		public PreviewContext(string name) : base(name)
		{

		}

		protected override ILayer GetLayerForNode(IEffectNode node)
		{
			return Sequence.GetSequenceLayerManager().GetLayer(node);
		}

		public ISequence Sequence { get; set; }
		
	}
}

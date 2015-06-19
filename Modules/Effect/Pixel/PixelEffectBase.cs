using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Pixel
{
	public abstract class PixelEffectBase : EffectModuleInstanceBase
	{
		protected readonly List<int> StringPixelCounts = new List<int>();

		[ReadOnly(true)]
		[ProviderCategory(@"Config")]
		[ProviderDisplayName(@"StringCount")]
		[ProviderDescription(@"StringCount")]
		public int StringCount { get; set; }

		[ReadOnly(true)]
		[ProviderCategory(@"Config")]
		[ProviderDisplayName(@"PixelsPerString")]
		[Description(@"PixelsPerString")]
		public int MaxPixelsPerString { get; set; }

		private void CalculatePixelsPerString()
		{
			IEnumerable<ElementNode> nodes = FindLeafParents();
			StringPixelCounts.Clear();
			foreach (var node in nodes)
			{
				StringPixelCounts.Add(node.Count());
			}
		}


		private int CalculateMaxStringCount()
		{
			return FindLeafParents().Count();
		}

		protected IEnumerable<ElementNode> FindLeafParents()
		{
			var nodes = new List<ElementNode>();
			var nonLeafElements = new List<ElementNode>();

			if (TargetNodes.FirstOrDefault() != null)
			{
				nonLeafElements = TargetNodes.SelectMany(x => x.GetNonLeafEnumerator()).ToList();
				foreach (var elementNode in TargetNodes)
				{
					foreach (var leafNode in elementNode.GetLeafEnumerator())
					{
						nodes.AddRange(leafNode.Parents);
					}
				}

			}
			//Some nodes can have multiple node parents with odd groupings so this fancy linq query makes sure that the parent
			//node is part of the Target nodes lineage.
			return nodes.Distinct().Intersect(nonLeafElements);
		}

		protected override void TargetNodesChanged()
		{
			CalculatePixelsPerString();
			MaxPixelsPerString = StringPixelCounts.Concat(new[] { 0 }).Max();
			StringCount = CalculateMaxStringCount();
		}
	}
}

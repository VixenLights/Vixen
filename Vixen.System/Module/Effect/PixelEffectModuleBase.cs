
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Module.Effect
{
	public abstract class PixelEffectModuleBase:EffectModuleInstanceBase
	{

		protected readonly List<int> _stringPixelCounts = new List<int>(); 
		

		[ReadOnly(true)]
		[Category(@"Config")]
		[DisplayName(@"String Count")]
		[Description(@"The count of strings the effect will use.")]
		public int StringCount { get; set; }

		[ReadOnly(true)]
		[Category(@"Config")]
		[DisplayName(@"Pixels Per String")]
		[Description(@"The count of pixels on each string.")]
		public int MaxPixelsPerString { get; set; }

		private void CalculatePixelsPerString()
		{
			IEnumerable<ElementNode> nodes = FindLeafParents();
			_stringPixelCounts.Clear();
			foreach (var node in nodes)
			{
				_stringPixelCounts.Add(node.Count());
			}
		}


		private int CalculateMaxStringCount()
		{
			return FindLeafParents().Count();
		}

		private IEnumerable<ElementNode> FindLeafParents()
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
			MaxPixelsPerString = _stringPixelCounts.Concat(new[] { 0 }).Max();
			StringCount = CalculateMaxStringCount();
		}

	}
}

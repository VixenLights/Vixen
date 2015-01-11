//#define OLDWAY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace VixenModules.Effect.Nutcracker
{
	public class Nutcracker : EffectModuleInstanceBase
	{
		public static int frameMs = 50;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private NutcrackerModuleData _data;
		private EffectIntents _elementData = null;
		private List<int> _stringPixelCounts = new List<int>(); 

		public Nutcracker()
		{
			_data = new NutcrackerModuleData();
		}

		protected override void TargetNodesChanged()
		{
			CalculatePixelsPerString();
			MaxPixelsPerString = _stringPixelCounts.Concat(new[] {0}).Max();
			StringCount = CalculateMaxStringCount();
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			int scnt = StringCount;
			if (scnt < 2)
			{
				//_data.NutcrackerData.PreviewType = NutcrackerEffects.PreviewType.VerticalLine;
				switch (_data.NutcrackerData.PreviewType)
				{
					case NutcrackerEffects.PreviewType.Arch:
					case NutcrackerEffects.PreviewType.VerticalLine:
					case NutcrackerEffects.PreviewType.HorizontalLine:
						break;
					default:
						_data.NutcrackerData.PreviewType = NutcrackerEffects.PreviewType.VerticalLine;
						break;
				}
			}
				
			_elementData = new EffectIntents();

			foreach (ElementNode node in TargetNodes) {
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;
				
				if (node != null)
					RenderNode(node);
			}
		}

		//Nutcracker is special right now as we only ever generate one intent per element, we can skip a lot of logic
		//in the base class as if we are active, our intents are always in the relative time.
		public override ElementIntents GetElementIntents(TimeSpan effectRelativeTime)
		{
			_elementIntents.Clear();
			_AddLocalIntents();
			return _elementIntents;
		}

		private void _AddLocalIntents()
		{
			EffectIntents effectIntents = Render();
			foreach (KeyValuePair<Guid, IntentNodeCollection> keyValuePair in effectIntents)
			{
				_elementIntents.AddIntentNodeToElement(keyValuePair.Key, keyValuePair.Value.ToArray());
			}
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		public override IModuleDataModel ModuleData
		{
			get
			{
				_data.NutcrackerData.TargetNodes = TargetNodes;
				return _data;
			}
			set { _data = value as NutcrackerModuleData; }
		}

		public double IntensityLevel
		{
			get { return 100; }
			set { IsDirty = true; }
		}

		public Color Color
		{
			get { return SystemColors.ActiveBorder; }
			set { IsDirty = true; }
		}

		[Value]
		public NutcrackerData NutcrackerData
		{
			get
			{
				_data.NutcrackerData.TargetNodes = TargetNodes;
				return _data.NutcrackerData;
			}
			set
			{
				_data.NutcrackerData = value;
				IsDirty = true;
			}
		}

		public int StringCount { get; set; }

		public int MaxPixelsPerString { get; set; }

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

		private void CalculatePixelsPerString()
		{
			IEnumerable<ElementNode> nodes = FindLeafParents();
			_stringPixelCounts.Clear();
			foreach (var node in nodes)
			{
				_stringPixelCounts.Add(node.Count());
			}
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNode(ElementNode node)
		{
			int wid;
			int ht;
			if (_data.NutcrackerData.StringOrienation == NutcrackerEffects.StringOrientations.Horizontal)
			{
				wid = MaxPixelsPerString;
				ht = StringCount;
			}
			else
			{
				wid = StringCount;
				ht = MaxPixelsPerString;
			}
			int nFrames = (int)(TimeSpan.TotalMilliseconds / frameMs);
			var nccore = new NutcrackerEffects(_data.NutcrackerData) {Duration = TimeSpan};
			nccore.InitBuffer( wid, ht);
			int numElements = node.Count();
			
			TimeSpan startTime = TimeSpan.Zero;
			
			// OK, we're gonna create 1 intent per element
			// that intent will hold framesToRender Color values
			// that it will parcel out as intent states are called for...
			
			// set up arrays to hold the generated colors
			var pixels = new RGBValue[numElements][];
			for (int eidx = 0; eidx < numElements; eidx++)
				pixels[eidx] = new RGBValue[nFrames];
			List<ElementNode> nodes = FindLeafParents().ToList();
			// generate all the pixels
			for (int frameNum = 0; frameNum < nFrames; frameNum++)
			{
				nccore.RenderNextEffect(_data.NutcrackerData.CurrentEffect);
				// peel off this frames pixels...
				if (_data.NutcrackerData.StringOrienation == NutcrackerEffects.StringOrientations.Horizontal)
				{
					int i = 0;
					for (int y = 0; y < ht; y++)
					{
						for (int x = 0; x < _stringPixelCounts[y]; x++)
						{
							pixels[i][frameNum] = new RGBValue(nccore.GetPixel(x, y));
							i++;
						}
					}
				}
				else
				{
					int i = 0;
					for (int x = 0; x < wid; x++)
					{
						for (int y = 0; y < _stringPixelCounts[x]; y++)
						{
							pixels[i][frameNum] = new RGBValue(nccore.GetPixel(x, y));
							i++;
						}
					}
				}
			};

			// create the intents
			var frameTs = new TimeSpan(0, 0, 0, 0, frameMs);
			List<Element> elements = node.ToList();
			for (int eidx = 0; eidx < numElements; eidx++)
			{
				IIntent intent = new StaticArrayIntent<RGBValue>(frameTs, pixels[eidx], TimeSpan);
				_elementData.AddIntentForElement(elements[eidx].Id, intent, startTime);
			}

			nccore.Dispose();
		}

	}

}
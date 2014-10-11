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

		public Nutcracker()
		{
			_data = new NutcrackerModuleData();
		}

		protected override void TargetNodesChanged()
		{
			//Nothing to do
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

		private int StringCount
		{
			get
			{
				List<ElementNode> nodes = new List<ElementNode>();
				int childCount = 0;
				if (TargetNodes.FirstOrDefault() != null)
				{
					List<ElementNode> nonLeafElements = TargetNodes.SelectMany(x => x.GetNonLeafEnumerator()).ToList();
					foreach (var elementNode in TargetNodes)
					{
						foreach (var leafNode in elementNode.GetLeafEnumerator())
						{
							nodes.AddRange(leafNode.Parents);
						}
					}
					//Some nodes can have multiple node parents with odd groupings so this fancy linq query makes sure that the parent
					//node is part of the Target nodes lineage.
					childCount = nodes.Distinct().Intersect(nonLeafElements).Count();
				}

				if (childCount == 0)
					childCount = 1;

				return childCount;
			}
		}

		private int PixelsPerString()
		{
			int pps = PixelsPerString(TargetNodes.FirstOrDefault());
			return pps;
		}

		private int PixelsPerString(ElementNode parentNode)
		{
			//TODO: what would we do if parentNode is null?
			int pps = 0;
			int leafCount = 0;
			int groupCount = 0;
			// if no groups are children, then return nChildren
			// otherwise return the size of the first group
			ElementNode firstGroup = null;
			foreach (ElementNode node in parentNode.Children)
			{
				if (node.IsLeaf) {
					leafCount++;
				}
				else {
					groupCount++;
					if (firstGroup == null)
						firstGroup = node;
				}
			}
			if (groupCount == 0) {
				pps = leafCount;
			}
			else {
				// this needs to be called on a group, first might be an element
				//pps = PixelsPerStringx(parentNode.Children.FirstOrDefault());
				// this is marginally better but its not clear what to do about further nesting
				pps = PixelsPerString(firstGroup);
			}

            if (pps == 0)
                pps = 1;

			return pps;
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNode(ElementNode node)
		{
			int wid = 0;
			int ht = 0;
			if (_data.NutcrackerData.StringOrienation == NutcrackerEffects.StringOrientations.Horizontal)
			{
				wid = PixelsPerString();
				ht = StringCount;
			}
			else
			{
				wid = StringCount;
				ht = PixelsPerString();
			}
			int nFrames = (int)(TimeSpan.TotalMilliseconds / frameMs);
			NutcrackerEffects nccore = new NutcrackerEffects(_data.NutcrackerData);
			nccore.Duration = TimeSpan;
			nccore.InitBuffer( wid, ht);
			int totalPixels = nccore.PixelCount();
			if( totalPixels != wid * ht)
				throw new Exception("bad pixel count");
			int numElements = node.Count();
			if (numElements != totalPixels)
				Logging.Warn( "numEle " + numElements + " != totalPixels " + totalPixels);
			
			TimeSpan startTime = TimeSpan.Zero;
			//TimeSpan ms50 = new TimeSpan(0, 0, 0, 0, frameMs);
			Stopwatch timer = new Stopwatch();
			timer.Start();

			// OK, we're gonna create 1 intent per element
			// that intent will hold framesToRender Color values
			// that it will parcel out as intent states are called for...
			
			// set up arrays to hold the generated colors
			var pixels = new RGBValue[numElements][];
			for (int eidx = 0; eidx < numElements; eidx++)
				pixels[eidx] = new RGBValue[nFrames];

			// generate all the pixels
			int pps = PixelsPerString();
			int sc = StringCount;
			for (int frameNum = 0; frameNum < nFrames; frameNum++)
			{
				nccore.RenderNextEffect(_data.NutcrackerData.CurrentEffect);
				// peel off this frames pixels...
				if (_data.NutcrackerData.StringOrienation == NutcrackerEffects.StringOrientations.Horizontal)
				{
					int i = 0;
					for (int y = 0; y < sc; y++)
					{
						for (int x = 0; x < pps; x++)
						{
							pixels[i][frameNum] = new RGBValue(nccore.GetPixel(x, y));
							i++;
						}
					}
				}
				else
				{
					for (int i = 0; i < numElements; i++)
					{
						pixels[i][frameNum] = new RGBValue(nccore.GetPixel(i));
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
			timer.Stop();
			Logging.Debug(" {0}ms, Frames: {1}, wid: {2}, ht: {3},  pix: {4}, intents: {5}",
							timer.ElapsedMilliseconds, nFrames, wid, ht, totalPixels, _elementData.Count());
		}

	}

}
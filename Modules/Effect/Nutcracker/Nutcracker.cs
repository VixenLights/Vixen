//#define OLDWAY
using System;
using System.Collections.Generic;
using System.Linq;
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

		protected override void _PreRender()
		{
			int scnt = StringCount;
			if (scnt < 2)
				_data.NutcrackerData.PreviewType = NutcrackerEffects.PreviewType.VerticalLine;
			_elementData = new EffectIntents();

			foreach (ElementNode node in TargetNodes) {
				if (node != null)
					RenderNode(node);
			}
			GC.Collect();
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
				int childCount = 0;

				if (TargetNodes.FirstOrDefault() != null)
				{
					foreach (ElementNode node in TargetNodes.FirstOrDefault().Children)
					{
						if (!node.IsLeaf)
						{
							childCount++;
						}
					}
					if (childCount == 0 && TargetNodes.FirstOrDefault().Children.Count() > 0)
					{
						childCount = 1;
					}
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
			int pps = 0;
			// Is this a single string?
			int leafCount = 0;
			int groupCount = 0;
			foreach (ElementNode node in parentNode.Children) {
				if (node.IsLeaf) {
					leafCount++;
				}
				else {
					groupCount++;
				}
			}
			if (groupCount == 0) {
				pps = leafCount;
			}
			else {
				pps = PixelsPerString(parentNode.Children.FirstOrDefault());
			}

            if (pps == 0)
                pps = 1;

			return pps;
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNode(ElementNode node)
		{
			int wid = StringCount;
			int ht = PixelsPerString();
			int nFrames = (int)(TimeSpan.TotalMilliseconds / frameMs);
			NutcrackerEffects nccore = new NutcrackerEffects(_data.NutcrackerData);
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
			for (int frameNum = 0; frameNum < nFrames; frameNum++)
			{
				nccore.RenderNextEffect(_data.NutcrackerData.CurrentEffect);
				// peel off this frames pixels...
				for (int i = 0; i < numElements; i++)
				{
					pixels[i][frameNum] = new RGBValue(nccore.GetPixel(i));
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

			timer.Stop();
			Logging.Debug(" {0}ms, Frames: {1}, wid: {2}, ht: {3},  pix: {4}, intents: {5}",
							timer.ElapsedMilliseconds, nFrames, wid, ht, totalPixels, _elementData.Count());
		}

	}

}
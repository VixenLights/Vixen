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
		private void RenderNodeX(ElementNode node)
		{
			int stringCount = StringCount;
			int framesToRender = (int) TimeSpan.TotalMilliseconds/50;
			NutcrackerEffects effect = new NutcrackerEffects(_data.NutcrackerData);
			int pixelsPerString = PixelsPerString();
			effect.InitBuffer(stringCount, pixelsPerString);
			int totalPixels = effect.PixelCount();
			TimeSpan startTime = TimeSpan.Zero;
			TimeSpan ms50 = new TimeSpan(0, 0, 0, 0, 50);
			Stopwatch timer = new Stopwatch();
			timer.Start();

			for (int frameNum = 0; frameNum < framesToRender; frameNum++) {
				// Parallel will not work here. Nutcracker effects must be run in order
				effect.RenderNextEffect(_data.NutcrackerData.CurrentEffect);

				// Parallel works well here
				// ElementAt is slow so convert it to a list first!
				List<Element> elements = node.ToList();
				int elementCount = node.Count();
				Parallel.For(0, elementCount, elementNum =>
				{
					Color color = effect.GetPixel(elementNum);

					if (color.A > 0 && (color.R > 0 || color.G > 0 || color.B > 0)) {
						LightingValue lightingValue = new LightingValue(color, (float) ((float) color.A/(float) byte.MaxValue));
						IIntent intent = new LightingIntent(lightingValue, lightingValue, ms50);
						_elementData.AddIntentForElement(elements[elementNum].Id, intent, startTime);
					}
				});

				startTime = startTime.Add(ms50);
			};
			timer.Stop();
			Console.WriteLine("Nutcracker Render:" + timer.ElapsedMilliseconds + "ms, Frames:" + framesToRender
							+ "    wid:" + stringCount + ", ht:" + pixelsPerString
							+ "    pix:" + totalPixels + ", intents:" + _elementData.Count());
		}


		internal class NutcrackerIntentState : Dispatchable<NutcrackerIntentState>, IIntentState
		{
			public NutcrackerIntentState( NutcrackerIntent intent, TimeSpan intentRelativeTime)
			{
				if (intent == null) throw new ArgumentNullException("intent");

				Intent = intent;
				RelativeTime = intentRelativeTime;
			}

			public NutcrackerIntent Intent { get; private set; }

			IIntent IIntentState.Intent
			{
				get { return Intent; }
			}

			public TimeSpan RelativeTime { get; private set; }

			public LightingValue GetValue()
			{
				return Intent.GetStateAt(RelativeTime);
			}

			object IIntentState.GetValue()
			{
				return GetValue();
			}

			public IIntentState Clone()
			{
				NutcrackerIntentState newIntentState = new NutcrackerIntentState(Intent, RelativeTime);

				return newIntentState;
			}
		}

		internal class NutcrackerIntent : Dispatchable<NutcrackerIntent>, IIntent<LightingValue>
		{
			TimeSpan _timespan;
			Color[] _pixels;

			public NutcrackerIntent( Color[] pixels, TimeSpan timeSpan)
			{
				_timespan = timeSpan;
				_pixels = pixels;
			}

			public TimeSpan TimeSpan { get { return _timespan; } private set { } }

			public IIntentState CreateIntentState(TimeSpan intentRelativeTime)
			{
				return new NutcrackerIntentState( this, intentRelativeTime);
			}

			public void FractureAt(TimeSpan intentRelativeTime)
			{
			}

			public void FractureAt(IEnumerable<TimeSpan> intentRelativeTimes)
			{
			}

			public void FractureAt(ITimeNode intentRelativeTime)
			{
			}

			public IIntent[] DivideAt(TimeSpan intentRelativeTime)
			{
				if (intentRelativeTime >= _timespan || intentRelativeTime <= TimeSpan.Zero)
					return null;
				throw new NotImplementedException();
			}

			public void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteIntentStartTime)
			{
				throw new NotImplementedException();
			}

			object IIntent.GetStateAt(TimeSpan intentRelativeTime)
			{
				return GetStateAt(intentRelativeTime);
			}

			public LightingValue GetStateAt(TimeSpan intentRelativeTime)
			{
				int idx = Math.Min( _pixels.Length-1,(int)intentRelativeTime.TotalMilliseconds / Nutcracker.frameMs);
				Color color = _pixels[idx];
				return new LightingValue(color, (float)((float)color.A / (float)byte.MaxValue));
			}

		}


		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNode(ElementNode node)
		{
			int wid = StringCount;
			int ht = PixelsPerString();
			int nFrames = (int)TimeSpan.TotalMilliseconds / frameMs;
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
			Color[][] pixels = new Color[numElements][];
			for( int eidx = 0; eidx < numElements; eidx++)
				pixels[eidx] = new Color[nFrames];

			// generate all the pixels
			for (int frameNum = 0; frameNum < nFrames; frameNum++)
			{
				nccore.RenderNextEffect(_data.NutcrackerData.CurrentEffect);
				// peel off this frames pixels...
				for (int i = 0; i < numElements; i++)
				{
					pixels[i][frameNum] = nccore.GetPixel(i);
				}

			};

			// create the intents
			List<Element> elements = node.ToList();
			for (int eidx = 0; eidx < numElements; eidx++)
			{
				IIntent intent = new NutcrackerIntent(pixels[eidx], TimeSpan);
				_elementData.AddIntentForElement(elements[eidx].Id, intent, startTime);
			}

			timer.Stop();
			Console.WriteLine("Nutcracker Render2:" + timer.ElapsedMilliseconds + "ms, Frames:" + nFrames
							+ "    wid:" + wid + ", ht:" + ht
							+ "    pix:" + totalPixels + ", intents:" + _elementData.Count());
		}

	}

}
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
		private NutcrackerModuleData _data;
		private EffectIntents _elementData = null;

		public Nutcracker()
		{
			_data = new NutcrackerModuleData();
		}

		protected override void _PreRender()
		{
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
				foreach (ElementNode node in TargetNodes.FirstOrDefault().Children) {
					if (!node.IsLeaf) {
						childCount++;
					}
				}
				if (childCount == 0 && TargetNodes.FirstOrDefault().Children.Count() > 0) {
					childCount = 1;
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
			//Console.WriteLine("Nutcracker Node:" + node.Name);
			//bool CW = true;
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
					int stringNum = 0;
					if (NutcrackerData.PreviewType == NutcrackerEffects.PreviewType.Tree90 ||
						NutcrackerData.PreviewType == NutcrackerEffects.PreviewType.Tree180 ||
						NutcrackerData.PreviewType == NutcrackerEffects.PreviewType.Tree270 ||
						NutcrackerData.PreviewType == NutcrackerEffects.PreviewType.Tree360)
					{
						stringNum = stringCount - (elementNum / pixelsPerString);
					}
					else
					{
						stringNum = (elementNum / pixelsPerString);
					}
					int pixelNum = (stringNum * pixelsPerString) -
									(pixelsPerString - (elementNum % pixelsPerString));
				    Color color = effect.GetPixel(pixelNum);
					
				    LightingValue lightingValue = new LightingValue(color,(float)((float) color.A / (float) byte.MaxValue));
				    IIntent intent = new LightingIntent(lightingValue, lightingValue, ms50);
					
				    _elementData.AddIntentForElement(elements[elementNum].Id, intent, startTime);
				});

				startTime = startTime.Add(ms50);
			};

			timer.Stop();
			//Console.WriteLine("Nutcracker Render:" + timer.ElapsedMilliseconds + "ms Frames:" + framesToRender);
		}
	}
}
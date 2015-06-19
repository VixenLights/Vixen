using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Pixel
{
	public abstract class PixelEffectBase : EffectModuleInstanceBase
	{

		protected PixelEffectBase()
		{
			StringOrientation = StringOrientation.Vertical;
		}

		protected readonly List<int> StringPixelCounts = new List<int>();

		protected EffectIntents _elementData;

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			foreach (ElementNode node in TargetNodes)
			{
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;

				if (node != null)
					RenderNode(node);
			}
		}

		[ReadOnly(true)]
		[ProviderCategory(@"Setup")]
		[ProviderDisplayName(@"StringCount")]
		[ProviderDescription(@"StringCount")]
		[PropertyOrder(0)]
		public int StringCount { get; set; }

		[ReadOnly(true)]
		[ProviderCategory(@"Setup")]
		[ProviderDisplayName(@"PixelsPerString")]
		[Description(@"PixelsPerString")]
		[PropertyEditor("Label")]
		[PropertyOrder(1)]
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

		protected abstract void RenderEffect(int frameNum);

		protected int BufferHt
		{
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? StringCount : MaxPixelsPerString;
			}

		}

		protected int BufferWi
		{
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? MaxPixelsPerString : StringCount;
			}
		}

		
		protected StringOrientation StringOrientation { get; set; }

		private RGBValue[][] _pixels;
		protected void InitBuffer()
		{

			if (StringOrientation == StringOrientation.Horizontal)
			{
				_pixels = new RGBValue[MaxPixelsPerString][];
				for (int i = 0; i < MaxPixelsPerString; i++)
				{
					_pixels[i] = new RGBValue[StringCount];
				}
			}
			else
			{
				_pixels = new RGBValue[StringCount][];
				for (int i = 0; i < StringCount; i++)
				{
					_pixels[i] = new RGBValue[MaxPixelsPerString];
				}
			}
		}

		// 0,0 is lower left
		protected void SetPixel(int x, int y, Color color)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
			{
				_pixels[x][y] = new RGBValue(color);
			}
		}

		// 0,0 is lower left
		protected void SetPixel(int x, int y, HSV hsv)
		{
			Color color = hsv.ToRGB().ToArgb();
			SetPixel(x, y, color);
		}

		protected void RenderNode(ElementNode node)
		{
			var frameMs = 50;
			
			int nFrames = (int)(TimeSpan.TotalMilliseconds / 50);

			InitBuffer();
			int numElements = node.Count();

			TimeSpan startTime = TimeSpan.Zero;

			// set up arrays to hold the generated colors
			var pixels = new RGBValue[numElements][];
			for (int eidx = 0; eidx < numElements; eidx++)
				pixels[eidx] = new RGBValue[nFrames];

			// generate all the pixels
			for (int frameNum = 0; frameNum < nFrames; frameNum++)
			{
				RenderEffect(frameNum);
				// peel off this frames pixels...
				if (StringOrientation == StringOrientation.Horizontal)
				{
					int i = 0;
					for (int y = 0; y < BufferHt; y++)
					{
						for (int x = 0; x < StringPixelCounts[y]; x++)
						{
							pixels[i][frameNum] = _pixels[x][y];
							i++;
						}
					}
				}
				else
				{
					int i = 0;
					for (int x = 0; x < BufferWi; x++)
					{
						for (int y = 0; y < StringPixelCounts[x]; y++)
						{
							pixels[i][frameNum] = _pixels[x][y];
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
		}

		protected double GetEffectTimeIntervalPosition(int frame)
		{
			double retval;
			if (TimeSpan == TimeSpan.Zero)
			{
				retval = 1;
			}
			else
			{
				retval = frame / (TimeSpan.TotalMilliseconds / 50);
			}
			return retval;
		}
	}
}

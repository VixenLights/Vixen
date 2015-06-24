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

		private const short FrameTime = 50;

		protected readonly List<int> StringPixelCounts = new List<int>();

		protected EffectIntents _elementData;

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			EffectIntents data = new EffectIntents();
			foreach (ElementNode node in TargetNodes)
			{
				if (node != null)
					data.Add(RenderNode(node));
			}
			_elementData = data;
		}

		[ReadOnly(true)]
		[ProviderCategory(@"Setup",0)]
		[ProviderDisplayName(@"StringCount")]
		[ProviderDescription(@"StringCount")]
		[PropertyOrder(0)]
		public int StringCount { get; set; }

		[ReadOnly(true)]
		[ProviderCategory(@"Setup",0)]
		[ProviderDisplayName(@"PixelsPerString")]
		[Description(@"PixelsPerString")]
		[PropertyEditor("Label")]
		[PropertyOrder(1)]
		public int MaxPixelsPerString { get; set; }

		[ProviderCategory(@"Setup", 0)]
		[ProviderDisplayName(@"Orientation")]
		[ProviderDescription(@"Orientation")]
		[PropertyOrder(2)]
		public abstract StringOrientation StringOrientation { get; set; }

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

		private Color[][] _pixels;
		protected void InitBuffer()
		{
			_pixels = new Color[BufferWi][];
			for (int i = 0; i < BufferWi; i++)
			{
				_pixels[i] = new Color[BufferHt];
			}	
		}

		public void ClearBuffer()
		{
			for (int i=0; i < BufferWi; i++)
			{
				for (int z = 0; z < BufferHt; z++)
				{
					_pixels[i][z] = Color.Transparent;
				}
			}
		}

		// 0,0 is lower left
		protected void SetPixel(int x, int y, Color color)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
			{
				_pixels[x][y] = color;
			}
		}

		// 0,0 is lower left
		protected void SetPixel(int x, int y, HSV hsv)
		{
			Color color = hsv.ToRGB().ToArgb();
			SetPixel(x, y, color);
		}

		protected EffectIntents RenderNode(ElementNode node)
		{
			EffectIntents effectIntents = new EffectIntents();
			int nFrames = (int)(TimeSpan.TotalMilliseconds / FrameTime);

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
				ClearBuffer();
				RenderEffect(frameNum);
				// peel off this frames pixels...
				if (StringOrientation == StringOrientation.Horizontal)
				{
					int i = 0;
					for (int y = 0; y < BufferHt; y++)
					{
						for (int x = 0; x < StringPixelCounts[y]; x++)
						{
							pixels[i][frameNum] = new RGBValue(_pixels[x][y]);
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
							pixels[i][frameNum] = new RGBValue(_pixels[x][y]);
							i++;
						}
					}
				}
			};

			// create the intents
			var frameTs = new TimeSpan(0, 0, 0, 0, FrameTime);
			List<Element> elements = node.ToList();
			for (int eidx = 0; eidx < numElements; eidx++)
			{
				IIntent intent = new StaticArrayIntent<RGBValue>(frameTs, pixels[eidx], TimeSpan);
				effectIntents.AddIntentForElement(elements[eidx].Id, intent, startTime);
			}

			return effectIntents;
		}

		protected double GetEffectTimeIntervalPosition(int frame)
		{
			double position;
			if (TimeSpan == TimeSpan.Zero)
			{
				position = 0;
			}
			else
			{
				position = (frame * FrameTime) / TimeSpan.TotalMilliseconds;
			}
			return position;
		}
	}
}

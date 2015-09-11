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
using VixenModules.App.Curves;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Pixel
{
	public abstract class PixelEffectBase : EffectModuleInstanceBase
	{

		protected const short FrameTime = 50;

		protected readonly List<int> StringPixelCounts = new List<int>();

		private EffectIntents _elementData;
		private int _stringCount;
		private int _maxPixelsPerString;
		private static Curve _baseLevelCurve = new Curve(CurveType.Flat100);

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			SetupRender();
			EffectIntents data = new EffectIntents();
			foreach (ElementNode node in TargetNodes)
			{
				if (node != null)
					data.Add(RenderNode(node));
			}
			_elementData = data;
			CleanUpRender();
		}

		[ReadOnly(true)]
		[ProviderCategory(@"Setup", 0)]
		[ProviderDisplayName(@"StringCount")]
		[ProviderDescription(@"StringCount")]
		[PropertyOrder(0)]
		public int StringCount
		{
			get { return _stringCount; }
			private set
			{
				_stringCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[ReadOnly(true)]
		[ProviderCategory(@"Setup", 0)]
		[ProviderDisplayName(@"PixelsPerString")]
		[ProviderDescription(@"PixelsPerString")]
		[PropertyEditor("Label")]
		[PropertyOrder(1)]
		public int MaxPixelsPerString
		{
			get { return _maxPixelsPerString; }
			private set
			{
				_maxPixelsPerString = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[ProviderCategory(@"Setup", 0)]
		[ProviderDisplayName(@"Orientation")]
		[ProviderDescription(@"Orientation")]
		[PropertyOrder(2)]
		public abstract StringOrientation StringOrientation { get; set; }

		[Browsable(false)]
		public virtual Color BaseColor { 
			get { return Color.Transparent; }
			set { }
		}

		[Browsable(false)]
		public virtual Curve BaseLevelCurve {
			get
			{
				return _baseLevelCurve;
			} 
			set{} 
		}

		[Browsable(false)]
		public virtual bool UseBaseColor { get; set; }

		
		
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
			var nonLeafElements = Enumerable.Empty<ElementNode>();

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

		protected abstract void SetupRender();
		protected abstract void RenderEffect(int frameNum, ref PixelFrameBuffer frameBuffer);
		protected abstract void CleanUpRender();

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

		protected EffectIntents RenderNode(ElementNode node)
		{
			EffectIntents effectIntents = new EffectIntents();
			int nFrames = (int)(TimeSpan.TotalMilliseconds / FrameTime);

			var buffer = new PixelFrameBuffer(BufferWi, BufferHt, UseBaseColor?BaseColor:Color.Transparent);

			int bufferSize = StringPixelCounts.Sum();

			TimeSpan startTime = TimeSpan.Zero;

			// set up arrays to hold the generated colors
			var pixels = new RGBValue[bufferSize][];
			for (int eidx = 0; eidx < bufferSize; eidx++)
				pixels[eidx] = new RGBValue[nFrames];

			// generate all the pixels
			for (int frameNum = 0; frameNum < nFrames; frameNum++)
			{
				if (UseBaseColor)
				{
					var level = BaseLevelCurve.GetValue(GetEffectTimeIntervalPosition(frameNum)*100)/100;
					buffer.ClearBuffer(level);
				}
				else
				{
					buffer.ClearBuffer();
				}
				
				RenderEffect(frameNum, ref buffer);
				// peel off this frames pixels...
				if (StringOrientation == StringOrientation.Horizontal)
				{
					int i = 0;
					for (int y = 0; y < BufferHt; y++)
					{
						for (int x = 0; x < StringPixelCounts[y]; x++)
						{
							pixels[i][frameNum] = new RGBValue(buffer.GetColorAt(x,y));
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
							pixels[i][frameNum] = new RGBValue(buffer.GetColorAt(x, y));
							i++;
						}
					}
				}
			};

			// create the intents
			var frameTs = new TimeSpan(0, 0, 0, 0, FrameTime);
			List<Element> elements = node.ToList();
			int numElements = node.Count();
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Location;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// This class provides some additional utility functions for all the grid style efects
	/// commonly thought of as pixel effects.  
	/// </summary>
	public abstract class PixelEffectBase : BaseEffect
	{

		protected const short FrameTime = 50;

		protected readonly List<int> StringPixelCounts = new List<int>();
		protected List<ElementLocation> ElementLocations; 

		private EffectIntents _elementData;
		private int _stringCount;
		private int _maxPixelsPerString;
		private Curve _baseLevelCurve = new Curve(CurveType.Flat100);

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			if (TargetPositioning == TargetPositioningType.Strings)
			{
				ConfigureStringBuffer();
			}
			else
			{
				ConfigureVirtualBuffer();
			}
			
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

		[Value]
		[Browsable(false)]
		[ProviderCategory(@"Setup", 0)]
		[ProviderDisplayName(@"TargetPositioning")]
		[ProviderDescription(@"TargetPositioning")]
		[PropertyOrder(3)]
		public TargetPositioningType TargetPositioning
		{
			get { return EffectModuleData.TargetPositioning; }
			set
			{
				EffectModuleData.TargetPositioning = value;
				if (TargetPositioning == TargetPositioningType.Locations)
				{
					StringOrientation = StringOrientation.Vertical;
				}
				UpdateStringOrientationAttributes(true);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Browsable(false)]
		public virtual Color BaseColor { 
			get { return Color.Transparent; }
			set { }
		}

		[Browsable(false)]
		public virtual Curve BaseLevelCurve {
			get { return _baseLevelCurve; }
			set { }
		}

		[Browsable(false)]
		public virtual bool UseBaseColor { get; set; }

		protected void UpdateStringOrientationAttributes(bool refresh = false)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"StringOrientation", TargetPositioning.Equals(TargetPositioningType.Strings)}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected void EnableTargetPositioning(bool enable, bool refresh = false)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"TargetPositioning", enable}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

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
			CalculateStringCounts();
		}

		private void ConfigureStringBuffer()
		{
			_bufferHt = StringCount;
			_bufferWi = MaxPixelsPerString;
			_xOffset = 0;
			_yOffset = 0;
		}

		private void CalculateStringCounts()
		{
			CalculatePixelsPerString();
			MaxPixelsPerString = StringPixelCounts.Concat(new[] {0}).Max();
			StringCount = CalculateMaxStringCount();
		}

		private void ConfigureVirtualBuffer()
		{
			ElementLocations = TargetNodes.SelectMany(x => x.GetLeafEnumerator()).Select(x => new ElementLocation(x)).ToList();
			var xMax = ElementLocations.Max(p => p.X);
			var xMin = ElementLocations.Min(p => p.X);
			var yMax = ElementLocations.Max(p => p.Y);
			var yMin = ElementLocations.Min(p => p.Y);

			_bufferWi = (yMax - yMin) + 1;
			_yOffset = yMin;
			_bufferHt = (xMax - xMin) + 1;
			_xOffset = xMin;

		}

		private int _xOffset;
		private int _yOffset;
		protected int BufferHtOffset
		{
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? _yOffset : _xOffset;
			}
		}

		protected int BufferWiOffset
		{
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? _xOffset : _yOffset;
			}
		}

		protected int StringCountOffset { get; set; }
		protected int MaxPixelsPerStringOffset { get; set; }

		protected abstract void SetupRender();
		protected abstract void RenderEffect(int frameNum, IPixelFrameBuffer frameBuffer);

		protected virtual void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			
		}
		protected abstract void CleanUpRender();

		private int _bufferHt = 0;
		protected int BufferHt
		{
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? _bufferHt : _bufferWi;
			}

		}

		private int _bufferWi = 0;
		protected int BufferWi
		{
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? _bufferWi : _bufferHt;
			}
		}

		protected EffectIntents RenderNode(ElementNode node)
		{
			if (TargetPositioning == TargetPositioningType.Strings)
			{
				return RenderNodeByStrings(node);
			}
			return RenderNodeByLocation(node);
		}

		protected EffectIntents RenderNodeByLocation(ElementNode node)
		{
			Console.Out.WriteLine("Render by location");
			EffectIntents effectIntents = new EffectIntents();
			int nFrames = GetNumberFrames();
			if (nFrames <= 0 | BufferWi == 0 || BufferHt == 0) return effectIntents;
			PixelLocationFrameBuffer buffer = new PixelLocationFrameBuffer(ElementLocations, nFrames, _xOffset, _yOffset, BufferHt);
			
			TimeSpan startTime = TimeSpan.Zero;

			// generate all the pixels
			RenderEffectByLocation(nFrames, buffer);

			// create the intents
			var frameTs = new TimeSpan(0, 0, 0, 0, FrameTime);

			foreach (var tuple in buffer.GetElementData())
			{
				if (tuple.Item2.Count == 0)
				{
					Debugger.Break();
				}
				IIntent intent = new StaticArrayIntent<RGBValue>(frameTs, tuple.Item2.ToArray(), TimeSpan);
				effectIntents.AddIntentForElement(tuple.Item1.ElementNode.Element.Id, intent, startTime);
			}
			//int numElements = ElementLocations.Count;
			//List<ElementNode> elements = ElementLocations.Keys.ToList();
			//for (int eidx = 0; eidx < numElements; eidx++)
			//{
			//	IIntent intent = new StaticArrayIntent<RGBValue>(frameTs, pixels[eidx], TimeSpan);
			//	effectIntents.AddIntentForElement(elements[eidx].Element.Id, intent, startTime);
			//}

			return effectIntents;
		}

		protected EffectIntents RenderNodeByStrings(ElementNode node)
		{
			EffectIntents effectIntents = new EffectIntents();
			int nFrames = GetNumberFrames();
			if (nFrames <= 0 | BufferWi==0 || BufferHt==0) return effectIntents;
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
				
				RenderEffect(frameNum, buffer);
				// peel off this frames pixels...
				if (StringOrientation == StringOrientation.Horizontal)
				{
					int i = 0;
					for (int y = 0; y < BufferHt; y++)
					{
						for (int x = 0; x < StringPixelCounts[y]; x++)
						{
							if (i >= pixels.Length) break;
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
							if (i >= pixels.Length) break;
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

		protected int GetNumberFrames()
		{
			if (TimeSpan == TimeSpan.Zero)
			{
				return 0;
			}
			return (int)(TimeSpan.TotalMilliseconds / FrameTime);
		}
	}
}

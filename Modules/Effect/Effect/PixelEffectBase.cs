using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using NLog;
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
		private static Logger Logging = LogManager.GetCurrentClassLogger();
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
			ElementLocations = null;
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
				TargetPositioningChanged();
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

		/// <summary>
		/// Override to do things when the target locations changes 
		/// </summary>
		protected virtual void TargetPositioningChanged()
		{
			
		}

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
			_bufferHtOffset = 0;
			_bufferWiOffset = 0;
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
			_bufferHt = (xMax - xMin) + 1;
			_bufferWiOffset = yMin;
			_bufferHtOffset = xMin;
		}

		protected int StringCountOffset { get; set; }
		protected int MaxPixelsPerStringOffset { get; set; }

		protected abstract void SetupRender();
		protected abstract void RenderEffect(int frameNum, IPixelFrameBuffer frameBuffer);

		/// <summary>
		/// Called by for effects that support location based rendering when it is enabled
		/// The normal array based logic inverts the effect data. This si the formula to convert the x, y coordinates 
		/// in order to do similar math
		/// This inverts the coordinate
		/// y = Math.Abs((BufferHtOffset - y) + (BufferHt - 1 + BufferHtOffset));
		/// 
		/// This offsets it to be zero based like the others
		///	y = y - BufferHtOffset;
		///	x = x - BufferWiOffset;
		/// See full example in Butteryfly or partial in Colorwash. 
		/// </summary>
		/// <param name="numFrames"></param>
		/// <param name="frameBuffer"></param>
		protected virtual void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			throw new NotImplementedException();
		}

		protected abstract void CleanUpRender();

		private int _bufferHt;
		protected int BufferHt
		{
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? _bufferHt : _bufferWi;
			}

		}

		private int _bufferWi;
		protected int BufferWi
		{
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? _bufferWi : _bufferHt;
			}
		}

		private int _bufferHtOffset;

		protected int BufferHtOffset
		{
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? _bufferHtOffset : _bufferWiOffset;
			}
		}

		private int _bufferWiOffset;
		protected int BufferWiOffset {
			get
			{
				return StringOrientation == StringOrientation.Horizontal ? _bufferWiOffset : _bufferHtOffset;
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
			EffectIntents effectIntents = new EffectIntents();
			int nFrames = GetNumberFrames();
			if (nFrames <= 0 | BufferWi == 0 || BufferHt == 0) return effectIntents;
			PixelLocationFrameBuffer buffer = new PixelLocationFrameBuffer(ElementLocations.Distinct().ToList(), nFrames);
			
			TimeSpan startTime = TimeSpan.Zero;

			// generate all the pixels
			RenderEffectByLocation(nFrames, buffer);

			// create the intents
			var frameTs = new TimeSpan(0, 0, 0, 0, FrameTime);

			foreach (var tuple in buffer.GetElementData())
			{
				if (tuple.Item2.Count != nFrames)
				{
					Logging.Error("{0} count has {1} instead of {2}", tuple.Item1.ElementNode.Name, tuple.Item2.Count, nFrames );
				}
				IIntent intent = new StaticArrayIntent<RGBValue>(frameTs, tuple.Item2.ToArray(), TimeSpan);
				effectIntents.AddIntentForElement(tuple.Item1.ElementNode.Element.Id, intent, startTime);
			}
			
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
			}

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

		protected double CalculateAcceleration(double ratio, double accel)
		{
			if (accel == 0) return ratio;

			double pctAccel = (Math.Abs(accel) - 1.0) / 9.0;
			double newAccel1 = pctAccel * 5 + (1.0 - pctAccel) * 1.5;
			double newAccel2 = 1.5 + (ratio * newAccel1);
			double finalAccel = pctAccel * newAccel2 + (1.0 - pctAccel) * newAccel1;

			if (accel > 0)
			{
				return Math.Pow(ratio, finalAccel);
			}
			else
			{
				return (1.0 - Math.Pow(1.0 - ratio, newAccel1));
			}
		}

		protected double GetStepAngle(int width, int height)
		{
			double step = 0.5;
			int biggest = Math.Max(width, height);
			if (biggest > 50)
			{
				step = 0.4;
			}
			if (biggest > 150)
			{
				step = 0.3;
			}
			if (biggest > 250)
			{
				step = 0.2;
			}
			if (biggest > 350)
			{
				step = 0.1;
			}
			return step;
		}
	}
}

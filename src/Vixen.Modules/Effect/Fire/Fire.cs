using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using System.ComponentModel;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Fire
{
	/// <summary>
	/// Renders a heat-based fire effect for string and preview-location targets.
	/// </summary>
	public class Fire:PixelEffectBase
	{
		private FireData _data;
		private int[] _fireBuffer = new int[1];
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Fire"/> class.
		/// </summary>
		public Fire()
		{
			_data = new FireData();
			EnableTargetPositioning(true, true);
			InitAllAttributes();
		}

		/// <summary>
		/// Updates target-specific property visibility after the selected targets change.
		/// </summary>
		protected override void TargetNodesChanged()
		{
			base.TargetNodesChanged();
			UpdateTargetingAttributes();
			TypeDescriptor.Refresh(this);
		}

		/// <summary>
		/// Gets the target-root groups that receive independent Fire simulations.
		/// </summary>
		/// <returns>The selected targets as one group, or the groups resolved by individual target handling.</returns>
		protected override IEnumerable<IReadOnlyCollection<IElementNode>> GetRenderGroups()
		{
			if (TargetNodeHandling == TargetNodeSelection.Group)
			{
				return base.GetRenderGroups();
			}

			if (TargetNodes.Length > 1)
			{
				return TargetNodes
					.Where(node => node != null)
					.Distinct()
					.Select(node => (IReadOnlyCollection<IElementNode>)new[] {node});
			}

			var targetNode = TargetNodes.FirstOrDefault();
			if (targetNode == null)
			{
				return Enumerable.Empty<IReadOnlyCollection<IElementNode>>();
			}

			return GetNodesAtEffectDepth(targetNode, DepthOfEffect)
				.Where(node => node != null)
				.Distinct()
				.Select(node => (IReadOnlyCollection<IElementNode>)new[] {node});
		}

		#region Setup

		[Value]
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Config

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Location")]
		[ProviderDescription(@"Location")]
		[PropertyOrder(0)]
		public FireDirection Location
		{
			get { return _data.Location; }
			set
			{
				_data.Location = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Height")]
		[ProviderDescription(@"Height")]
		[PropertyOrder(1)]
		public Curve Height
		{
			get { return _data.Height; }
			set
			{
				_data.Height = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"HueShift")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public Curve HueShiftCurve
		{
			get { return _data.HueShiftCurve; }
			set
			{
				_data.HueShiftCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Target handling

		/// <summary>
		/// Gets or sets how Fire applies to the selected target nodes.
		/// </summary>
		/// <value>The target-node handling mode. The default is <see cref="TargetNodeSelection.Group" />.</value>
		[Value]
		[ProviderCategory(@"Behavior", 0)]
		[ProviderDisplayName(@"FireTargetNodeSelection")]
		[ProviderDescription(@"FireTargetNodeSelection")]
		public TargetNodeSelection TargetNodeHandling
		{
			get { return _data.TargetNodeSelection; }
			set
			{
				_data.TargetNodeSelection = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateTargetingAttributes();
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Gets or sets the target hierarchy depth used when target-node handling is individual.
		/// </summary>
		/// <value>The selected target hierarchy depth. The default is <c>0</c>.</value>
		[Value]
		[ProviderCategory(@"Depth", 20)]
		[ProviderDisplayName(@"Depth")]
		[ProviderDescription(@"Depth")]
		[TypeConverter(typeof(FireTargetElementDepthConverter))]
		[PropertyEditor("SelectionEditor")]
		public int DepthOfEffect
		{
			get { return _data.DepthOfEffect; }
			set
			{
				var previousDepth = _data.DepthOfEffect;
				_data.DepthOfEffect = value;
				UpdateTargetingAttributes();
				if (_data.DepthOfEffect != previousDepth)
				{
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/fire/"; }
		}

		#endregion

		/// <summary>
		/// Gets or sets the serialized Fire settings and refreshes setup-property visibility.
		/// </summary>
		/// <value>The Fire module data that supplies effect settings.</value>
		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as FireData;
				InitAllAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes(true);
			UpdateTargetingAttributes();
		}

		private void UpdateTargetingAttributes()
		{
			var depth = DetermineDepth();
			var hasUsefulIntermediateDepth = HasUsefulIntermediateDepth(depth);
			var targetNodeHandlingVisible = TargetNodes.Any() && (TargetNodes.Length > 1 || depth > 2);

			if (!targetNodeHandlingVisible && TargetNodeHandling == TargetNodeSelection.Individual)
			{
				_data.TargetNodeSelection = TargetNodeSelection.Group;
			}

			if (TargetNodes.Length > 1 || TargetNodeHandling == TargetNodeSelection.Group)
			{
				_data.DepthOfEffect = 0;
			}
			else if (TargetNodeHandling == TargetNodeSelection.Individual && !IsUsefulIntermediateDepth(DepthOfEffect, depth))
			{
				_data.DepthOfEffect = GetFirstUsefulIntermediateDepth(depth);
			}

			SetBrowsable(new Dictionary<string, bool>(2)
			{
				{nameof(TargetNodeHandling), targetNodeHandlingVisible},
				{nameof(DepthOfEffect), TargetNodeHandling == TargetNodeSelection.Individual && TargetNodes.Length == 1 && hasUsefulIntermediateDepth}
			});
		}

		private static bool HasUsefulIntermediateDepth(int depth)
		{
			return depth > 2;
		}

		private static bool IsUsefulIntermediateDepth(int selectedDepth, int availableDepth)
		{
			return selectedDepth > 0 && selectedDepth < availableDepth - 1;
		}

		private static int GetFirstUsefulIntermediateDepth(int depth)
		{
			return HasUsefulIntermediateDepth(depth) ? 1 : 0;
		}

		// 0 <= x < BufferWi
		// 0 <= y < BufferHt
		private int GetFireBuffer(int x, int y, int maxWi, int maxHt)
		{
			if (x >= 0 && x < maxWi && y >= 0 && y < maxHt)
			{
				return _fireBuffer[y * maxWi + x];
			}
			return -1;
		}

		protected override void SetupRender()
		{
			_fireBuffer = new int[BufferWi*BufferHt];
		}

		protected override void CleanUpRender()
		{
			_fireBuffer = null;
		}

		/// <summary>
		/// Renders a Fire frame using the current string-target projection.
		/// </summary>
		/// <param name="frame">The zero-based frame index to render.</param>
		/// <param name="frameBuffer">The string-target frame buffer that receives rendered colors.</param>
		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var (maxWi, maxHt) = GetSimulationDimensions();
			var frameState = CreateFrameState(frame, maxHt);
			GenerateFireBuffer(maxWi, maxHt, frameState.Step);

			for (var y = 0; y < maxHt; y++)
			{
				for (var x = 0; x < maxWi; x++)
				{
					if (!TryGetFireColor(x, y, maxWi, maxHt, frameState, out var hsv)) continue;

					int xp = x;
					int yp = y;
					if (Location == FireDirection.Top || Location == FireDirection.Right)
					{
						yp = maxHt - y - 1;
					}
					if (Location == FireDirection.Left || Location == FireDirection.Right)
					{
						int t = xp;
						xp = yp;
						yp = t;
					}
					frameBuffer.SetPixel(xp, yp, hsv);
				}
			}
		}

		/// <summary>
		/// Renders Fire frames for the configured sparse preview locations.
		/// </summary>
		/// <param name="numFrames">The number of frames to render.</param>
		/// <param name="frameBuffer">The sparse preview-location frame buffer that receives rendered colors.</param>
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			if (numFrames <= 0 || BufferWi <= 0 || BufferHt <= 0)
			{
				return;
			}

			var (maxWi, maxHt) = GetSimulationDimensions();
			for (var frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				var frameState = CreateFrameState(frame, maxHt);
				GenerateFireBuffer(maxWi, maxHt, frameState.Step);

				foreach (var elementLocation in frameBuffer.ElementLocations)
				{
					var outputX = elementLocation.X - BufferWiOffset;
					var outputY = Math.Abs((BufferHtOffset - elementLocation.Y) + (BufferHt - 1 + BufferHtOffset));
					outputY -= BufferHtOffset;
					var (simulationX, simulationY) = GetSimulationCoordinate(Location, outputX, outputY);
					if (TryGetFireColor(simulationX, simulationY, maxWi, maxHt, frameState, out var hsv))
					{
						frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, hsv);
					}
				}
			}
		}

		private (int Width, int Height) GetSimulationDimensions()
		{
			var maxHt = BufferHt;
			var maxWi = BufferWi;
			if (Location == FireDirection.Left || Location == FireDirection.Right)
			{
				maxHt = BufferWi;
				maxWi = BufferHt;
			}

			return (maxWi, maxHt);
		}

		private (int X, int Y) GetSimulationCoordinate(FireDirection direction, int outputX, int outputY)
		{
			return direction switch
			{
				FireDirection.Bottom => (outputX, outputY),
				FireDirection.Top => (outputX, BufferHt - outputY - 1),
				FireDirection.Left => (outputY, outputX),
				FireDirection.Right => (outputY, BufferWi - outputX - 1),
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}

		private FireFrameState CreateFrameState(int frame, int maxHt)
		{
			var intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
			var effectiveHeight = (int)Height.GetValue(intervalPosFactor);
			if (effectiveHeight <= 0)
			{
				effectiveHeight = 1;
			}

			return new FireFrameState(
				LevelCurve.GetValue(intervalPosFactor) / 100,
				CalculateHueShift(intervalPosFactor),
				255 * 100 / maxHt / effectiveHeight);
		}

		private void GenerateFireBuffer(int maxWi, int maxHt, int step)
		{
			for (var x = 0; x < maxWi; x++)
			{
				var r = x % 2 == 0 ? 190 + (Rand() % 10) : 100 + (Rand() % 50);
				_fireBuffer[x] = r;
			}

			for (var y = 1; y < maxHt; y++)
			{
				for (var x = 0; x < maxWi; x++)
				{
					var v1 = GetFireBuffer(x - 1, y - 1, maxWi, maxHt);
					var v2 = GetFireBuffer(x + 1, y - 1, maxWi, maxHt);
					var v3 = GetFireBuffer(x, y - 1, maxWi, maxHt);
					var v4 = GetFireBuffer(x, y - 1, maxWi, maxHt);
					var n = 0;
					var sum = 0;
					if (v1 >= 0)
					{
						sum += v1;
						n++;
					}
					if (v2 >= 0)
					{
						sum += v2;
						n++;
					}
					if (v3 >= 0)
					{
						sum += v3;
						n++;
					}
					if (v4 >= 0)
					{
						sum += v4;
						n++;
					}
					var newIndex = n > 0 ? sum / n : 0;
					if (newIndex > 0)
					{
						newIndex += (Rand() % 100 < 20) ? step : -step;
						if (newIndex < 0) newIndex = 0;
						if (newIndex >= FirePalette.Count()) newIndex = FirePalette.Count() - 1;
					}
					_fireBuffer[y * maxWi + x] = newIndex;
				}
			}
		}

		private bool TryGetFireColor(int x, int y, int maxWi, int maxHt, FireFrameState frameState, out HSV hsv)
		{
			var colorIndex = GetFireBuffer(x, y, maxWi, maxHt);
			if (colorIndex == 0)
			{
				hsv = default;
				return false;
			}

			hsv = FirePalette.GetColor(colorIndex);
			if (frameState.HueShift > 0) hsv.H = hsv.H + frameState.HueShift / 100.0f;
			hsv.V *= frameState.Level;
			return true;
		}

		private double CalculateHueShift(double intervalPos)
		{
			return ScaleCurveToValue(HueShiftCurve.GetValue(intervalPos), 100, 0);
		}

		private readonly record struct FireFrameState(double Level, double HueShift, int Step);
		
	}
}

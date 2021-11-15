using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Vixen.Attributes;
using Vixen.Extensions;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Polygon;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using Line = VixenModules.App.Polygon.Line;

namespace VixenModules.Effect.Morph
{
	/// <summary>
	/// Morph pixel effect.
	/// </summary>
	public class Morph : PixelEffectBase
	{
		#region Private Fields

		/// <summary>
		/// Data associated with the effect.
		/// </summary>
		private MorphData _data;

		/// <summary>
		/// Logical buffer height.
		/// Note this height might not match the actual effect height when the effect is operating in Location mode.
		/// </summary>
		private int _bufferHt;

		/// <summary>
		/// Logical buffer height.
		/// Note this width might not match the actual effect width when the effect is operating in Location mode.
		/// </summary>
		private int _bufferWi;
		
		/// <summary>
		/// Defines the Polygon Editor capabilities applicable to free form mode.
		/// </summary>
		private readonly PolygonEditorCapabilities _freeFormEditorCapabilities;
		
		/// <summary>
		/// Defines the Polygon Editor capabilities applicable to the time based mode.
		/// </summary>
		private readonly PolygonEditorCapabilities _timeBasedEditorCapabilities;
		
		/// <summary>
		/// Defines the Polygon Editor capabilities applicable to the pattern mode.
		/// </summary>
		private readonly PolygonEditorCapabilities _patternEditorCapabilities;

		/// <summary>
		/// Collection of data needed to render the wipe polygons.
		/// </summary>
		private readonly List<MorphWipePolygonRenderData> _wipePolygonRenderData;

		/// <summary>
		/// Collection of morph polygons that were created by applying the repeating pattern.
		/// </summary>
		private List<IMorphPolygon> _patternExpandedMorphPolygons;

		/// <summary>
		/// Used in <c>CalculatePixel</c> to transfer pixels from bitmap to frame buffer.
		/// </summary>
		private static Color _emptyColor = Color.FromArgb(0, 0, 0, 0);

		/// <summary>
		/// X axis offset from the virtual frame buffer to the actual/true frame buffer.
		/// </summary>
		private int _marginOffsetX;

		/// <summary>
		/// Y axis offset from the virtual frame buffer to the actual/true frame buffer.
		/// </summary>
		private int _marginOffsetY;

		#endregion

		#region Private Properties

		/// <summary>
		/// Flag to track that the target nodes are changing.
		/// </summary>
		private bool TargetNodesChanging { get; set; }

		#endregion

		#region Private Constants

		/// <summary>
		/// Maximum acceleration of the wipe.
		/// </summary>
		private const double MaxAcceleration = 10.0;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Morph()
		{
			// Enable both string and location positioning
			EnableTargetPositioning(true, true);

			// Initialize the collection of morph polygons
			_morphPolygons = new MorphPolygonsObservableCollection();

			// Initialize the render data for patterned polygons			
			_wipePolygonRenderData = new List<MorphWipePolygonRenderData>();

			// Initialize the capabilities of the pattern mode polygon editor
			_patternEditorCapabilities = new PolygonEditorCapabilities()
			{
				DeletePolygons = false,
				DeletePoints = true,
				AddPolygons = false,				
				PastePolygons = false,
				CutPolygons = false,
				DefaultToSelect = true,
				ToggleStartSide = true,
				ToggleStartPoint = true,
				CopyPolygons = false,
				ShowStartSide = true,
				ShowTimeBar = false,
				AddPoint = true,
				AllowMultipleShapes = false,
			};

			// Initialize the capabilities of the free form mode polygon editor
			_freeFormEditorCapabilities = new PolygonEditorCapabilities
			{
				DeletePolygons = true,
				DeletePoints = true,
				AddPolygons = true,				
				PastePolygons = true,
				CutPolygons = true,
				DefaultToSelect = true,
				ToggleStartSide = true,
				ToggleStartPoint = true,
				CopyPolygons = true,
				ShowStartSide = true,
				ShowTimeBar = false,
				AddPoint = true,
				AllowMultipleShapes = true,
			};

			// Initialize the capabilities of the time based mode polygon editor
			_timeBasedEditorCapabilities = new PolygonEditorCapabilities()
			{
				DeletePolygons = true,
				DeletePoints = true,
				AddPolygons = true,				
				PastePolygons = true,
				CutPolygons = true,
				DefaultToSelect = false,
				ToggleStartSide = false,
				ToggleStartPoint = false,
				CopyPolygons = true,
				ShowStartSide = false,
				ShowTimeBar = true,
				AddPoint = true,
				AllowMultipleShapes = false,
			};
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// String orientation of the effect.
		/// </summary>
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				// Save off the previous display element dimensions
				int previousBufferWidth = _bufferWi; 
				int previousBufferHeight = _bufferHt; 

				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();

				// When the TargetPositioning or TargetNodes change the orientation may change.
				// Only want to scale the shapes when the user explicitly changes the orientation.
				if (!TargetPositioningChanging && !TargetNodesChanging)
				{
					// Update the virtual buffer size for the orientation change
					UpdateVirtualFrameBufferSize();

					// Scale the shapes associated with the morph polygons
					ScaleShapesToFitDisplayElement(previousBufferWidth, previousBufferHeight);
				}
			}
		}

		/// <summary>
		/// Module data associated with the effect.
		/// </summary>
		public override IModuleDataModel ModuleData
		{
			get
			{
				// Update the serialized data 
				UpdateMorphSerializedData();

				// Return the effect data
				return _data;
			}
			set
			{
				// Save off the data for the effect
				_data = value as MorphData;

				// Update the morph polygon model data
				UpdatePolygonModel(_data);
				
				// Update the visibility of controls
				UpdateAttributes();

				// Mark the effect as dirty
				MarkDirty();
			}
		}

		#endregion

		#region Information Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/Morph/"; }
		}

		#endregion

		#region Polygon Type Configuration Properties

		/// <summary>
		/// Determines the type of polygon or line.  This setting controls the three modes of operation for the effect.
		/// </summary>
		[ProviderCategory(@"PolygonConfiguration", 2)]
		[ProviderDisplayName(@"PolygonType")]
		[ProviderDescription(@"PolygonType")]
		[PropertyOrder(1)]
		public PolygonType PolygonType
		{
			get { return _data.PolygonType; }
			set
			{
				// If the user is switching between Pattern type and Free Form type
				// and a repeating pattern was established then...
				if (_data.PolygonType == PolygonType.Pattern &&
				    value == PolygonType.FreeForm &&
				    RepeatCount != 0)
				{
					// Create the message box prompt to see if the user wants the Pattern polygons converted into Free Form polygons
					MessageBoxForm messageBox = new MessageBoxForm(
						"Would you like to convert the Pattern polygons to Free Form Polygons?", 
						"Convert Polygons", 
						MessageBoxButtons.YesNo, 
						SystemIcons.Question);
					
					// Display the prompt
					DialogResult result = messageBox.ShowDialog();

					// If the user selected to convert the 
					if (result == DialogResult.OK)
					{
						// Convert the pattern polygons into Morph polygons
						ExpandPatternMorphPolygons();
						
						// Clear the Morph Polygon collection
						MorphPolygons.Clear();

						// Add the pattern polygons that fit on the display element to the collection
						MorphPolygons.AddRange(_patternExpandedMorphPolygons.Where(shape => 
							!shape.GetPointBasedShape().IsShapeOffDisplayElement(_bufferWi, _bufferHt)).ToList());
					}
				}

				_data.PolygonType = value;

				// If the polygon type is Free Form then...
				if (_data.PolygonType == PolygonType.FreeForm)
				{
					// Reset the repeat count
					RepeatCount = 0;
				}

				// Pattern mode only allows one polygon to be drawn by the user
				// Time Based mode starts out with one polygon
				if (_data.PolygonType == PolygonType.Pattern ||
				    _data.PolygonType == PolygonType.TimeBased)
				{					
					while (MorphPolygons.Count > 1)
					{
						MorphPolygons.Remove(MorphPolygons[MorphPolygons.Count - 1]);
					}					
				}

				IsDirty = true;
				OnPropertyChanged();

				// Update the visible attributes based on the polygon type
				UpdatePolygonTypeAttributes(true);
			}
		}

		/// <summary>
		/// Determines how the polygon/line is filled (Wipe, Solid, or Outline).
		/// </summary>
		[Value]
		[ProviderCategory(@"PolygonConfiguration", 3)]
		[ProviderDisplayName(@"FillType")]
		[ProviderDescription(@"FillType")]
		[PropertyOrder(2)]
		public PolygonFillType FillType
		{
			get
			{
				return _data.PolygonFillType;
			}
			set
			{
				_data.PolygonFillType = value;
				IsDirty = true;

				// Update the fill type on each of the morph polygons
				foreach(IMorphPolygon morphPolygon in MorphPolygons)
				{
					morphPolygon.FillType = value;
				}

				OnPropertyChanged();
				UpdatePolygonFillTypeAttributes();
			}
		}

		/// <summary>
		/// Margin of the display element drawing surface.
		/// </summary>
		[Value]
		[ProviderCategory(@"PolygonConfiguration", 2)]
		[ProviderDisplayName(@"Margin")]
		[ProviderDescription(@"Margin")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(4)]
		public int Margin
		{
			get
			{
				return _data.Margin;
			}
			set
			{
				// Move all the morph polygons as if there is no margin
				int oldMarginX = -1 * (int)Math.Round(BufferWi * _data.Margin / 100.0);
				int oldMarginY = -1 * (int)Math.Round(BufferHt * _data.Margin / 100.0);

				MoveMorphPolygons(oldMarginX, oldMarginY);

				_data.Margin = value;
				IsDirty = true;
				OnPropertyChanged();

				// Move the morph polygons for the new margin
				int marginChangeX = (int)Math.Round(BufferWi * _data.Margin / 100.0);
				int marginChangeY = (int)Math.Round(BufferHt * _data.Margin / 100.0);

				MoveMorphPolygons(marginChangeX, marginChangeY);
			}
		}

		/// <summary>
		/// This property is used as the input to the Polygon Editor.  This property type is associated to the polygon editor.		
		/// </summary>
		[Value]
		[ProviderCategory(@"PolygonConfiguration", 2)]
		[ProviderDisplayName(@" ")]
		[ProviderDescription(@"EditPolygons")]
		[PropertyOrder(3)]
		public PolygonContainer PolygonContainer
		{
			get
			{
				PolygonContainer container = new PolygonContainer();

				switch (PolygonType)
				{
					case PolygonType.Pattern:
						container.EditorCapabilities = _patternEditorCapabilities;
						break;
					case PolygonType.FreeForm:
						container.EditorCapabilities = _freeFormEditorCapabilities;
						break;
					case PolygonType.TimeBased:
						container.EditorCapabilities = _timeBasedEditorCapabilities;
						break;
					default:
						Debug.Assert(false, "Unsupported Polygon Type");
						break;
				}
				
				foreach (IMorphPolygon morphPolygon in MorphPolygons)
				{
					if (morphPolygon.Polygon != null)
					{
						Polygon clone = morphPolygon.Polygon.Clone();

						// Reset the GUID as we want to maintain identity of the cloned polygon
						clone.ID = morphPolygon.Polygon.ID;
						container.Polygons.Add(clone);
						container.PolygonTimes.Add(morphPolygon.Time);
					}
					else if (morphPolygon.Line != null)
					{
						Line clone = morphPolygon.Line.Clone();

						// Reset the GUID as we want to maintain identity of the cloned line
						clone.ID = morphPolygon.Line.ID;
						container.Lines.Add(clone);
						container.LineTimes.Add(morphPolygon.Time);						
					}
					else if (morphPolygon.Ellipse != null)
					{
						Ellipse clone = morphPolygon.Ellipse.Clone();

						// Reset the GUID as we want to maintain identity of the cloned ellipse
						clone.ID = morphPolygon.Ellipse.ID;
						container.Ellipses.Add(clone);
						container.EllipseTimes.Add(morphPolygon.Time);
					}
				}

				// Give the container the dimensions of the (virtual) display element
				container.Width = _bufferWi; 
				container.Height = _bufferHt;

				// Give the polygon container
				container.DisplayElementWidth = BufferWi;
				container.DisplayElementHeight = BufferHt;

				// Determine if the display element outline should be displayed
				container.ShowDisplayElement = (Margin != 0);

				return container;
			}
			set
			{
				// Default all the polygons and lines to have been removed in the editor
				foreach (IMorphPolygon morphPolygon in MorphPolygons)
				{
					morphPolygon.Removed = true;
				}

				UpdateMorphPolygonsFromContainerPolygons(value);
				UpdateMorphPolygonsFromContainerLines(value);
				UpdateMorphPolygonsFromContainerEllipses(value);

				// Remove any morph polygons were the corresponding shape was removed in the polygon editor
				foreach (IMorphPolygon morphPolygon in MorphPolygons.ToList())
				{
					if (morphPolygon.Removed)
					{
						MorphPolygons.Remove(morphPolygon);
					}
				}

				// If we are in time based mode then...
				if (PolygonType == PolygonType.TimeBased)
				{
					// Sort the morph polygons by the associated time
					List<IMorphPolygon> morphPolygons = MorphPolygons.OrderBy(mp => mp.Time).ToList();
					
					// Add the sorted morph polygons back to the collection
					MorphPolygons.Clear();
					MorphPolygons.AddRange(morphPolygons);
				}
				// Otherwise if the polygon type is pattern then...
				else if (PolygonType == PolygonType.Pattern)
				{
					// If there is a morph polygon then...
					if (MorphPolygons.Count > 0)
					{
						// Update the effect fill type from the morph polygon fill type
						// The polygon might be more than four points at which a wipe is not applicable
						FillType = MorphPolygons[0].FillType;
					}
				}

				// Force the view to refresh
				OnPropertyChanged(nameof(MorphPolygons));

				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Pattern Configuration Properties

		/// <summary>
		/// Determines  how many times to repeat the polygon/line.
		/// </summary>
		[Value]
		[ProviderCategory(@"PatternConfiguration", 3)]
		[ProviderDisplayName(@"RepeatCount")]
		[ProviderDescription(@"RepeatCount")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 50, 1)]
		[PropertyOrder(2)]		
		public int RepeatCount
		{
			get
			{
				return _data.RepeatCount;
			}
			set
			{
				_data.RepeatCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the direction of the repeating pattern of polygons or lines.
		/// </summary>
		[Value]
		[ProviderCategory(@"PatternConfiguration", 3)]
		[ProviderDisplayName(@"RepeatDirection")]
		[ProviderDescription(@"RepeatDirection")]
		[PropertyOrder(3)]		
		public WipeRepeatDirection RepeatDirection
		{
			get
			{
				return _data.RepeatDirection;
			}
			set
			{
				_data.RepeatDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the spacing in-between the repeating polygons/lines.
		/// </summary>
		[Value]
		[ProviderCategory(@"PatternConfiguration", 3)]
		[ProviderDisplayName(@"RepeatSkip")]
		[ProviderDescription(@"RepeatSkip")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(4)]
		public int RepeatSkip
		{
			get
			{
				return _data.RepeatSkip;
			}
			set
			{
				_data.RepeatSkip = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines if the start of the wipe is staggered for the repeating polygons/lines.
		/// </summary>
		[Value]
		[ProviderCategory(@"PatternConfiguration", 3)]
		[ProviderDisplayName(@"Stagger")]
		[ProviderDescription(@"Stagger")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(5)]		
		public int Stagger
		{
			get
			{
				return _data.Stagger;
			}
			set
			{
				_data.Stagger = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Wipe Configuration Properties

		/// <summary>
		/// Determines the length of the wipe head.
		/// </summary>
		[Value]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"HeadLength")]
		[ProviderDescription(@"HeadLength")]
		[PropertyOrder(1)]		
		public int HeadLength
		{
			get
			{
				return _data.HeadLength;
			}
			set
			{
				_data.HeadLength = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the percentage of the effect it takes the wipe head to travel across the polygon/line.
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"HeadDuration")]
		[ProviderDescription(@"HeadDuration")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(2)]		
		public int HeadDuration
		{
			get
			{
				return _data.HeadDuration;
			}
			set
			{
				_data.HeadDuration = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the wipe head color over the duration of the effect.
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"HeadColor")]
		[ProviderDescription(@"HeadColor")]
		[PropertyOrder(5)]
		public ColorGradient HeadColor
		{
			get
			{
				return _data.HeadColor;
			}
			set
			{
				_data.HeadColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines brightness of the wipe head.
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"HeadBrightness")]
		[ProviderDescription(@"HeadBrightness")]
		[PropertyOrder(6)]
		public Curve HeadBrightness
		{
			get
			{
				return _data.HeadBrightness;
			}
			set
			{
				_data.HeadBrightness = value;
				IsDirty = true;

				// Update the wipe head brightness on each of the morph polygons
				foreach (IMorphPolygon morphPolygon in MorphPolygons)
				{
					morphPolygon.HeadBrightness = value;
				}

				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the wipe tail color over the duration of the effect.
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"TailColor")]
		[ProviderDescription(@"TailColor")]
		[PropertyOrder(7)]
		public ColorGradient TailColor
		{
			get
			{
				return _data.TailColor;
			}
			set
			{
				_data.TailColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines brightness of the wipe tail.
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"TailBrightness")]
		[ProviderDescription(@"TailBrightness")]
		[PropertyOrder(8)]
		public Curve TailBrightness
		{
			get
			{
				return _data.TailBrightness;
			}
			set
			{
				_data.TailBrightness = value;
				IsDirty = true;

				// Update the wipe tail brightness on each of the morph polygons
				foreach (IMorphPolygon morphPolygon in MorphPolygons)
				{
					morphPolygon.TailBrightness = value;
				}

				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the acceleration of the wipe.  The acceleration can be either increasing or decreasing (deceleration).
		/// </summary>
		[Value]
		[ProviderCategory(@"WipeConfiguration", 4)]
		[ProviderDisplayName(@"Acceleration")]
		[ProviderDescription(@"Acceleration")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-10, 10, 1)]
		[PropertyOrder(9)]		
		public int Acceleration
		{
			get
			{
				return _data.Acceleration;
			}
			set
			{
				_data.Acceleration = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines the color of the polygon or line.  This setting is only applicable to Time Based mode.
		/// </summary>
		[Value]
		[ProviderCategory(@"PolygonConfiguration", 4)]
		[ProviderDisplayName(@"PolygonColor")]
		[ProviderDescription(@"PolygonColor")]
		[PropertyOrder(6)]
		public ColorGradient FillColor
		{
			get
			{
				return _data.FillColor;
			}
			set
			{
				_data.FillColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines brightness of the wipe tail.
		/// </summary>
		[Value]
		[ProviderCategory(@"PolygonConfiguration", 4)]
		[ProviderDisplayName(@"FillBrightness")]
		[ProviderDescription(@"FillBrightness")]
		[PropertyOrder(7)]
		public Curve FillBrightness
		{
			get
			{
				return _data.FillBrightness;
			}
			set
			{
				_data.FillBrightness = value;
				IsDirty = true;

				// Update the fill brightness on each of the morph polygons
				foreach (IMorphPolygon morphPolygon in MorphPolygons)
				{
					morphPolygon.FillBrightness = value;
				}

				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Determines if the Fill Type of the polygon (Solid or Outline).
		/// </summary>
		[Value]
		[ProviderCategory(@"PolygonConfiguration", 4)]
		[ProviderDisplayName(@"FillPolygon")]
		[ProviderDescription(@"FillPolygon")]
		[PropertyOrder(8)]
		public bool FillPolygon
		{
			get
			{
				return _data.FillPolygon;
			}
			set
			{
				_data.FillPolygon = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Configuration Properties

		private MorphPolygonsObservableCollection _morphPolygons;

		/// <summary>
		/// Gets and sets the Morph polygons.  This property is visible in the
		/// effect editor when the mode is set to free form but is used in the background for the other modes.
		/// </summary>
		[ProviderCategory(@"Config", 6)]
		[ProviderDisplayName(@"Polygons")]
		[ProviderDescription(@"Polygons")]
		[PropertyOrder(2)]
		public MorphPolygonsObservableCollection MorphPolygons
		{
			get
			{
				return _morphPolygons;
			}
			set
			{
				_morphPolygons = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Allows derived effects to react to when the target positioning (strings vs locations) has changed.
		/// </summary>
		protected override void TargetPositioningChanged()
		{
			// Save off the previous width and height
			int previousBufferWidth = _bufferWi; 
			int previousBufferHeight = _bufferHt; 

			// Call the base class implementation
			base.TargetPositioningChanged();

			// Configure the new display element size for the target positioning changing
			ConfigureDisplayElementSize();

			// Update the virtual buffer size for the change in target positioning
			UpdateVirtualFrameBufferSize();

			// Scale the shapes associated with the morph polygons
			ScaleShapesToFitDisplayElement(previousBufferWidth, previousBufferHeight);
		}

		/// <summary>
		/// Handles resizing polygons/lines when the display element changes.
		/// </summary>
		protected override void TargetNodesChanged()
		{
			// Set flag that the target nodes are changing
			TargetNodesChanging = true;

			// Save off the previous width and height
			int previousBufferWidth = _bufferWi; 
			int previousBufferHeight = _bufferHt;

			// Call the base class implementation
			base.TargetNodesChanged();

			// Configure the display element size (strings vs position)
			ConfigureDisplayElementSize();

			// Update the virtual buffer size for the display element changing
			UpdateVirtualFrameBufferSize();

			// If there are not any polygons or lines then...
			if (MorphPolygons.Count == 0)
			{
				// Default to a polygon that fills the display element
				IMorphPolygon morphPolygon = new MorphPolygon();
				MorphPolygons.Add(morphPolygon);

				// Check to see if we are dealing with a really skinny display element in
				// which case a line is going to be more appropriate
				if (_bufferWi == 1 || _bufferHt == 1)
				{
					// Determine which way to orient the line
					if (_bufferWi > _bufferHt)
					{
						// Create the line along the x-axis
						Line line = new Line();
						morphPolygon.Line = line;
						PolygonPoint pt1 = new PolygonPoint();
						pt1.X = 0;
						pt1.Y = 0;
						line.Points.Add(pt1);

						PolygonPoint pt2 = new PolygonPoint();
						pt2.X = BufferWi - 1;
						pt2.Y = 0;
						line.Points.Add(pt2);
					}
					else
					{
						// Create the line along the Y-axis
						Line line = new Line();
						PolygonPoint pt1 = new PolygonPoint();
						pt1.X = 0;
						pt1.Y = 0;
						line.Points.Add(pt1);

						PolygonPoint pt2 = new PolygonPoint();
						pt2.X = 0;
						pt2.Y = BufferHt - 1;
						line.Points.Add(pt2);
					}
				}
				else
				{
					// Otherwise create a polygon to fill the display element
					morphPolygon.Polygon = new Polygon();

					PolygonPoint ptTopLeft = new PolygonPoint();
					ptTopLeft.X = 0;
					ptTopLeft.Y = 0;
					PolygonPoint ptTopRight = new PolygonPoint();
					ptTopRight.X = BufferWi - 1;
					ptTopRight.Y = 0;
					PolygonPoint ptBottomRight = new PolygonPoint();
					ptBottomRight.X = BufferWi - 1;
					ptBottomRight.Y = BufferHt - 1;
					PolygonPoint ptBottomLeft = new PolygonPoint();
					ptBottomLeft.X = 0;
					ptBottomLeft.Y = BufferHt - 1;

					morphPolygon.Polygon.Points.Add(ptTopLeft);
					morphPolygon.Polygon.Points.Add(ptTopRight);
					morphPolygon.Polygon.Points.Add(ptBottomRight);
					morphPolygon.Polygon.Points.Add(ptBottomLeft);
				}
			}

			// If effect has been associated with a display element then...
			if (previousBufferHeight != 0 && previousBufferWidth != 0)
			{
				// Scale the shapes associated with the morph polygons
				ScaleShapesToFitDisplayElement(previousBufferWidth, previousBufferHeight);
			}

			// Clear flag that target nodes are no longer changing
			TargetNodesChanging = false;
		}

		/// <summary>
		/// Gets the data associated with the effect.
		/// </summary>
		protected override EffectTypeModuleData EffectModuleData
		{
			get
			{
				UpdateMorphSerializedData();
				return _data;
			}
		}

		/// <summary>
		/// Releases resources from the rendering process.
		/// </summary>
		protected override void CleanUpRender()
		{
		}

		/// <summary>
		/// Renders the effect by location.
		/// </summary>		
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			// If there is no margin then...
			if (Margin == 0)
			{
				// Render the effect normally
				RenderEffectByLocationInternal(numFrames, frameBuffer);
			}
			// Otherwise render the effect on the virtual display element
			else
			{
				// Render the effect on the virtual display element
				RenderEffectByLocationVirtual(numFrames, frameBuffer);
			}
		}

		/// <summary>
		/// Renders the effect by location.
		/// </summary>		
		protected void RenderEffectByLocationInternal(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			// Create a virtual matrix based on the rendering scale factor
			PixelFrameBuffer virtualFrameBuffer = new PixelFrameBuffer(_bufferWi, _bufferHt);

			// Loop over the frames
			for (int frameNum = 0; frameNum < numFrames; frameNum++)
			{
				//Assign the current frame
				frameBuffer.CurrentFrame = frameNum;

				// Render the effet to the virtual frame buffer
				RenderEffect(frameNum, virtualFrameBuffer);

				// Loop through the sparse matrix
				foreach (ElementLocation elementLocation in frameBuffer.ElementLocations)
				{
					// Lookup the pixel from the virtual frame buffer
					UpdateFrameBufferForLocationPixel(
						elementLocation.X,
						elementLocation.Y,
						_bufferHt,
						virtualFrameBuffer,
						frameBuffer);
				}

				// Clear the buffer for the next frame,
				virtualFrameBuffer.ClearBuffer();
			}
		}

		/// <summary>
		/// Renders the effect for string based display elements.
		/// </summary>		
		protected override void RenderEffect(int frameNum, IPixelFrameBuffer frameBuffer)
		{
			// If a Margin has been defined then...
			if (Margin != 0)
			{
				// Create a virtual matrix based on the display element and the margin
				PixelFrameBuffer virtualFrameBuffer = new PixelFrameBuffer(_bufferWi, _bufferHt);

				// Render the effect on the virtual frame buffer
				RenderEffectInternal(frameNum, virtualFrameBuffer);

				// Extract the pixels that correspond to actual frame buffer
				for (int x = 0; x < BufferWi; x++)
				{
					for (int y = 0; y < BufferHt; y++)
					{
						frameBuffer.SetPixel(x, y, virtualFrameBuffer.GetColorAt(x + _marginOffsetX, y + _marginOffsetY));
					}
				}
			}
			// Otherwise render the effect normally into the actual frame buffer
			else
			{
				RenderEffectInternal(frameNum, frameBuffer);
			}
		}

		/// <summary>
		/// Render the effect in String mode to the specified frame buffer.
		/// </summary>
		/// <param name="frameNum">Current frame number being processed</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		private void RenderEffectInternal(int frameNum, IPixelFrameBuffer frameBuffer)
		{
			// Get the position within the effect
			double intervalPos = GetEffectTimeIntervalPosition(frameNum);

			// Render based on the polygon type
			switch (PolygonType)
			{
				case PolygonType.TimeBased:
					// If there is only one morph polygon revert to free form mode
					if (MorphPolygons.Count > 1)
					{
						RenderEffectTimeBased(frameNum, frameBuffer, intervalPos);
					}
					else
					{
						// Time Based mode really requires at least two
						// morph polygons but this code allows the effect to
						// produce an output rather than being blank

						// Transfer the fill color to the morph polygon
						MorphPolygons[0].FillColor = FillColor;

						// If the polygon is to be filled then...
						if (FillPolygon)
						{
							// Set the fill type to solid on the morph polygon
							MorphPolygons[0].FillType = PolygonFillType.Solid;
						}
						else
						{
							// Otherwise set the fill type to outline
							MorphPolygons[0].FillType = PolygonFillType.Outline;
						}

						RenderEffectFreeForm(frameNum, frameBuffer, intervalPos);
					}
					break;
				case PolygonType.Pattern:
					RenderEffectPattern(frameNum, frameBuffer, intervalPos);
					break;
				case PolygonType.FreeForm:
					RenderEffectFreeForm(frameNum, frameBuffer, intervalPos);
					break;
				default:
					Debug.Assert(false, "Unsupported Polygon Type");
					break;
			}
		}

		/// <summary>
		/// Updates the virtual frame buffer size.
		/// </summary>
		private void UpdateVirtualFrameBufferSize()
		{
			// Calculate the margin for the display element
			_marginOffsetX = (int)Math.Round(BufferWi * Margin / 100.0);
			_marginOffsetY = (int)Math.Round(BufferHt * Margin / 100.0);

			// Store off the virtual frame buffer dimensions by putting a margin on all sides of the
			// original frame buffer
			_bufferWi = BufferWi + 2 * _marginOffsetX;
			_bufferHt = BufferHt + 2 * _marginOffsetY;
		}

		/// <summary>
		/// Setup for rendering.
		/// </summary>
		protected override void SetupRender()
		{
			// Calculate the previous margin offsets for the display element
			int marginOffsetX = (int)Math.Round(_data.DisplayElementWidth * Margin / 100.0);
			int marginOffsetY = (int)Math.Round(_data.DisplayElementHeight * Margin / 100.0);

			// Calculate the previous margin for the display element
			int previousBufferWi = _data.DisplayElementWidth + 2 * marginOffsetX;
			int previousBufferHt = _data.DisplayElementHeight + 2 * marginOffsetY;

			// Calculate the virtual frame buffer size
			UpdateVirtualFrameBufferSize();

			// Check to see if the display element changed size and if the morph polygons needs to scaled
			ScaleShapesToFitDisplayElement(previousBufferWi, previousBufferHt);

			// Save off the display element width and height
			// Note we are saving off the size of the actual display element not including any margin!
			_data.DisplayElementWidth = BufferWi;
			_data.DisplayElementHeight = BufferHt;

			// If the polygon type is Pattern then...
			if (PolygonType == PolygonType.Pattern)
			{
				// Transfer the top level settings to the morph polygon
				MorphPolygons[0].HeadLength = HeadLength;
				MorphPolygons[0].HeadDuration = HeadDuration;
				MorphPolygons[0].Acceleration = Acceleration;
				MorphPolygons[0].HeadColor = new ColorGradient(HeadColor);
				MorphPolygons[0].TailColor = new ColorGradient(TailColor);
			}

			// Give the morph polygon access to the display element dimensions
			MorphPolygon.BufferWidth = _bufferWi;
			MorphPolygon.BufferHeight = _bufferHt;

			// Clear the wipe polygon render data
			_wipePolygonRenderData.Clear();

			// Setup the pattern wipe polygons
			SetupRenderPattern();

			// Discard the expanded morph polygons
			_patternExpandedMorphPolygons = null;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Renders the effect by location on a virtual display element.
		/// </summary>		
		private void RenderEffectByLocationVirtual(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			// Create a virtual matrix based on the display element and the margin
			PixelFrameBuffer virtualFrameBuffer = new PixelFrameBuffer(_bufferWi, _bufferHt);

			// Loop over the frames
			for (int frameNum = 0; frameNum < numFrames; frameNum++)
			{
				// Assign the current frame
				frameBuffer.CurrentFrame = frameNum;

				// Render the effet to the virtual frame buffer
				RenderEffectInternal(frameNum, virtualFrameBuffer);

				// Loop through the sparse matrix
				foreach (ElementLocation elementLocation in frameBuffer.ElementLocations)
				{
					// Lookup the pixel from the virtual frame buffer
					UpdateFrameBufferForLocationPixel(
						elementLocation.X,
						elementLocation.Y,
						_bufferHt,
						virtualFrameBuffer,
						frameBuffer);
				}

				// Clear the buffer for the next frame,
				virtualFrameBuffer.ClearBuffer();
			}
		}

		/// <summary>
		/// Moves all the morph polygons by specified X and Y offsets.
		/// </summary>
		/// <param name="x">Amount to move the morph polygons in the X axis</param>
		/// <param name="y">Amount to move the morph polygons in the Y axis</param>
		private void MoveMorphPolygons(int x, int y)
		{
			// Loop over the morph polygons
			foreach (IMorphPolygon morphPolygon in MorphPolygons)
			{
				// Move the points associated the specified morph polygon
				morphPolygon.GetPointBasedShape().MovePoints(x, y);
			}
		} 
		
		/// <summary>
		/// Scales the shapes to fit the current display element.
		/// </summary>
		/// <param name="previousBufferWidth">Previous display element width</param>
		/// <param name="previousBufferHeight">Previous display element height</param>
		private void ScaleShapesToFitDisplayElement(int previousBufferWidth, int previousBufferHeight)
		{
			// If the current display element is not the same as the previous and
			// the previous display element size was saved off then...
			if ((previousBufferWidth != _bufferWi || previousBufferHeight != _bufferHt) &&
			     previousBufferHeight != 0 && 
			     previousBufferWidth != 0 &&
			     _bufferWi != 0 &&
			     _bufferHt != 0)
			{
				// Loop over the morph polygons
				foreach (IMorphPolygon morphPolygon in MorphPolygons)
				{
					double widthDenominator = previousBufferWidth - 1.0;
				
					// Make sure we don't attempt to scale with a zero denominator
					if (widthDenominator == 0.0)
					{
						widthDenominator = 1.0;
					}

					double heightDenominator = previousBufferHeight - 1.0;

					// Make sure we don't attempt to scale with a zero denominator
					if (heightDenominator == 0.0)
					{
						heightDenominator = 1.0;
					}

					// Scale the shapes associated with the morph polygon
					morphPolygon.Scale(
						(_bufferWi - 1.0) / widthDenominator,
						(_bufferHt - 1.0) / heightDenominator,
						_bufferWi,
						_bufferHt);
				}

				// Save off the display element width and height
				_data.DisplayElementWidth = BufferWi;
				_data.DisplayElementHeight = BufferHt;

				// Mark the sequence dirty so that the scaled polygons and the new width and height of the display element
				// are saved off
				IsDirty = true;
			}
		}
		
		/// <summary>
		/// Setup for render pattern polygons.
		/// </summary>
		private void SetupRenderPattern()
		{
			// Get the polygon configured to be wipe polygons
			List<IMorphPolygon> wipePolygons = GetWipePolygons();

			// Loop over the wipe polygons
			for (int polygonIndex = 0; polygonIndex < wipePolygons.Count; polygonIndex++)
			{
				// Get the specified morph polygon
				IMorphPolygon morphPolygon = wipePolygons[polygonIndex];

				// Create the wipe render data
				MorphWipePolygonRenderData wipeRenderData = new MorphWipePolygonRenderData();

				// If the polygon is pattern then...
				if (PolygonType == PolygonType.Pattern)
				{
					// Add a head is done flag for each repeating polygon
					for (int index = 0; index < RepeatCount + 1; index++)
					{
						wipeRenderData.HeadIsDone.Add(false);
					}

					// Transfer the stagger spacing to the render data 
					wipeRenderData.Stagger = Stagger;

					// Adjust the stagger so that it does not exceed the total duration of the effect
					if (TimeSpan.TotalMilliseconds * (wipeRenderData.Stagger / 100.0) * RepeatCount > TimeSpan.TotalMilliseconds)
					{
						wipeRenderData.Stagger = (int)(((TimeSpan.TotalMilliseconds / RepeatCount) / TimeSpan.TotalMilliseconds) * 100);
					}
				}
				else
				{
					// Otherwise just add one flag 
					wipeRenderData.HeadIsDone.Add(false);

					// Otherwise set the stagger to the start offset
					wipeRenderData.Stagger = morphPolygon.StartOffset;
				}

				// Create a new instance of wipe render data
				_wipePolygonRenderData.Add(wipeRenderData);

				// Get the model polygon from the morph polygon
				Polygon polygonModel = morphPolygon.Polygon;

				// If the morph polyon is a triangle then...
				if (morphPolygon.Polygon != null &&
					morphPolygon.Polygon.Points.Count == 3)
				{
					// Create a new polygon model
					polygonModel = new Polygon();

					// Transfer the triangle to polygon points
					// duplicating the last point
					PolygonPoint pt1 = morphPolygon.Polygon.Points[0];
					PolygonPoint pt2 = morphPolygon.Polygon.Points[1];
					PolygonPoint pt3 = morphPolygon.Polygon.Points[2];
					PolygonPoint pt4 = morphPolygon.Polygon.Points[2];

					// Add the points to the polygon model
					polygonModel.Points.Add(pt1);
					polygonModel.Points.Add(pt2);
					polygonModel.Points.Add(pt3);
					polygonModel.Points.Add(pt4);
				}
				// If the MorphPolygon is an ellipse then...
				else if (morphPolygon.Polygon == null && 
				         morphPolygon.Ellipse != null)
				{
					// Create a new polygon model
					polygonModel = new Polygon();

					// Transfer the ellipse rectangle to polygon points
					PolygonPoint pt1 = morphPolygon.Ellipse.Points[0];
					PolygonPoint pt2 = morphPolygon.Ellipse.Points[1];
					PolygonPoint pt3 = morphPolygon.Ellipse.Points[2];
					PolygonPoint pt4 = morphPolygon.Ellipse.Points[3];

					// Add the points to the polygon model
					polygonModel.Points.Add(pt1);
					polygonModel.Points.Add(pt2);
					polygonModel.Points.Add(pt3);
					polygonModel.Points.Add(pt4);
				}
				// Otherwise if the morph polygon is a line then...
				else if (morphPolygon.Line != null)
				{
					// Create a new polygon model
					polygonModel = new Polygon();

					// Convert the line into a skinny (line) polygon
					PolygonPoint pt1 = morphPolygon.Line.Points[0];
					PolygonPoint pt2 = morphPolygon.Line.Points[0];
					PolygonPoint pt3 = morphPolygon.Line.Points[1];
					PolygonPoint pt4 = morphPolygon.Line.Points[1];

					// Add the points to the polygons model
					polygonModel.Points.Add(pt1);
					polygonModel.Points.Add(pt2);
					polygonModel.Points.Add(pt3);
					polygonModel.Points.Add(pt4);
				}
				
				// Calculate the points along the side of the polygon
				// Note this is neither the start or end side
				StoreLine(
					(int)(polygonModel.Points[1].X),
					_bufferHt - (int)(polygonModel.Points[1].Y) - 1,
					(int)(polygonModel.Points[2].X),
					_bufferHt - (int)(polygonModel.Points[2].Y) - 1, 
					_wipePolygonRenderData[polygonIndex].X1Points, 
					_wipePolygonRenderData[polygonIndex].Y1Points);

				// Calculate the points along the other side of the polygon
				StoreLine(
					(int)(polygonModel.Points[0].X),
					_bufferHt - (int)(polygonModel.Points[0].Y) - 1,
					(int)(polygonModel.Points[3].X),
					_bufferHt - (int)(polygonModel.Points[3].Y) - 1, 
					_wipePolygonRenderData[polygonIndex].X2Points, 
					_wipePolygonRenderData[polygonIndex].Y2Points);

				// Calculate the direction of the polygon
				_wipePolygonRenderData[polygonIndex].Direction = CalculateDirection(polygonModel);

				// Determine the length of the long side of the polygon				
				int length = GetLengthOfWipePolygon(_wipePolygonRenderData[polygonIndex]);

				double time = 0.0;

				// If this is Free Form polygon then...
				if (RepeatCount == 0 && morphPolygon.StartOffset != 0)
				{
					// Calculate the time each polygon has to perform the wipe
					// Subtracting off all the staggers
					time = TimeSpan.TotalMilliseconds -
					       TimeSpan.TotalMilliseconds * (_wipePolygonRenderData[polygonIndex].Stagger / 100.0);
				}
				else
				{
					// Calculate the time each polygon has to perform the wipe
					// Subtracting off all the staggers
					time = TimeSpan.TotalMilliseconds -
					       TimeSpan.TotalMilliseconds * (_wipePolygonRenderData[polygonIndex].Stagger / 100.0) * RepeatCount;
				}

				// Need the head to travel past the end of the polygon/line for the length of the head
				int polygonLengthForHead = length + morphPolygon.HeadLength;

				// If the user selected an acceleration or de-acceleration then...
				if (morphPolygon.Acceleration != 0)
				{
					// If the acceleration is negative then...
					if (morphPolygon.Acceleration < 0)
					{
						// Calculate the head and tail accelerations using special iterative methods
						_wipePolygonRenderData[polygonIndex].HeadAcceleration = GetNegativeAcceleration(polygonLengthForHead, time * (morphPolygon.HeadDuration / 100.0), morphPolygon) * Math.Abs(morphPolygon.Acceleration / MaxAcceleration);
						_wipePolygonRenderData[polygonIndex].TailAcceleration = GetNegativeAcceleration(length, time, morphPolygon) * Math.Abs(morphPolygon.Acceleration / MaxAcceleration);
					}
					else				
					{
						// Otherwise calculate the positive accelerations
						_wipePolygonRenderData[polygonIndex].HeadAcceleration = GetPositiveAcceleration(length, time * (morphPolygon.HeadDuration / 100.0)) * morphPolygon.Acceleration / MaxAcceleration;
						_wipePolygonRenderData[polygonIndex].TailAcceleration = GetPositiveAcceleration(length, time) * morphPolygon.Acceleration / MaxAcceleration;
					}
				}
				else
				{
					// Otherwise set the accelerations to zero
					_wipePolygonRenderData[polygonIndex].HeadAcceleration = 0;
					_wipePolygonRenderData[polygonIndex].TailAcceleration = 0;
				}

				// Calculate the head and tail initial velocities of the wipe
				_wipePolygonRenderData[polygonIndex].HeadVelocityZero = GetHeadVelocityZero(polygonLengthForHead, _wipePolygonRenderData[polygonIndex].HeadAcceleration, time, morphPolygon);
				_wipePolygonRenderData[polygonIndex].TailVelocityZero = GetIncreasingVelocityZero(_wipePolygonRenderData[polygonIndex].TailAcceleration, length, time);
			}
		}

		/// <summary>
		/// Calculates the direction of the specified polygon model.  Direction is used to assist in drawing the polygon lines.
		/// </summary>		
		private bool CalculateDirection(Polygon polygonModel)
		{
			int x1A = (int)(polygonModel.Points[1].X);
			int y1A = _bufferHt - (int)(polygonModel.Points[1].Y);

			int x1B = (int)(polygonModel.Points[0].X);
			int y1B = _bufferHt - (int)(polygonModel.Points[0].Y);

			int x2A = (int)(polygonModel.Points[2].X);
			int y2A = _bufferHt - (int)(polygonModel.Points[2].Y);

			int x2B = (int)(polygonModel.Points[3].X);
			int y2B = _bufferHt - (int)(polygonModel.Points[3].Y);

			int deltaXa = x2A - x1A;
			int deltaXb = x2B - x1B;
			int deltaYa = y2A - y1A;
			int deltaYb = y2B - y1B;
			int direction = deltaXa + deltaXb + deltaYa + deltaYb;

			return (direction >= 0);
		}

		/// <summary>
		/// Updates the morph polygons from the specified polygon container polygons.
		/// </summary>		
		private void UpdateMorphPolygonsFromContainerLines(PolygonContainer container)
		{
			// Loop over the lines coming out of the polygon editor							
			for (int lineIndex = 0; lineIndex < container.Lines.Count; lineIndex++)
			{
				// Get the specified line
				Line line = container.Lines[lineIndex];

				// If the line already existed in the morph collection then...
				if (MorphPolygons.Any(poly => poly.Line != null && poly.Line.ID == line.ID))
				{
					// Find the line in the morph collection by GUID
					IMorphPolygon morphPolygon = MorphPolygons.Single(poly => poly.Line != null && poly.Line.ID == line.ID);

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// Update the line model associated with morph polygon
					morphPolygon.Line = line;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the polygon's normalized time
						morphPolygon.Time = container.LineTimes[lineIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}
				}
				// Else the line was not found...
				else
				{
					IMorphPolygon morphPolygon = new MorphPolygon();
					morphPolygon.Line = line;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the polygon's normalized time
						morphPolygon.Time = container.LineTimes[lineIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// Add the polygon to collection
					MorphPolygons.Add(morphPolygon);
				}
			}
		}

		/// <summary>
		/// Updates the morph polygon ellipse from the specified polygon container ellipses.
		/// </summary>		
		private void UpdateMorphPolygonsFromContainerEllipses(PolygonContainer container)
		{
			// Loop over the ellipses coming out of the polygon editor							
			for (int ellipseIndex = 0; ellipseIndex < container.Ellipses.Count; ellipseIndex++)
			{
				// Get the specified ellipse
				Ellipse ellipse = container.Ellipses[ellipseIndex];

				// If the line already existed in the morph collection then...
				if (MorphPolygons.Any(poly => poly.Ellipse != null && poly.Ellipse.ID == ellipse.ID))
				{
					// Find the ellipse in the morph collection by GUID
					IMorphPolygon morphPolygon = MorphPolygons.Single(poly => poly.Ellipse != null && poly.Ellipse.ID == ellipse.ID);

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// Update the line model associated with morph ellipse
					morphPolygon.Ellipse = ellipse;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the polygon's normalized time
						morphPolygon.Time = container.EllipseTimes[ellipseIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}
				}
				// Else the ellipse was not found...
				else
				{
					IMorphPolygon morphPolygon = new MorphPolygon();
					morphPolygon.Ellipse = ellipse;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the ellipses' normalized time
						morphPolygon.Time = container.EllipseTimes[ellipseIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// Add the polygon to collection
					MorphPolygons.Add(morphPolygon);
				}
			}
		}

		/// <summary>
		/// Updates the morph polygons from the specified polygon container lines.
		/// </summary>						
		private void UpdateMorphPolygonsFromContainerPolygons(PolygonContainer container)
		{
			// Loop over the polygons coming out of the polygon editor
			for (int polygonIndex = 0; polygonIndex < container.Polygons.Count; polygonIndex++)
			{
				// Get the specified polygon
				Polygon polygon = container.Polygons[polygonIndex];

				// If the polygon already existed in the morph collection then...
				if (MorphPolygons.Any(poly => poly.Polygon != null && poly.Polygon.ID == polygon.ID))
				{
					// Find the polygon in the morph collection by GUID
					IMorphPolygon morphPolygon = MorphPolygons.Single(poly => poly.Polygon != null && poly.Polygon.ID == polygon.ID);

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// Update the polygon model associated with morph polygon
					morphPolygon.Polygon = polygon;

					// If the polygon's fill type is set to wipe and
					// the polygon is not a rectangle or triangle then...
					if (polygon.FillType == PolygonFillType.Wipe &&
					    !(polygon.Points.Count == 4 ||
					      polygon.Points.Count == 3))
					{
						// Set the polygon fill type to solid
						polygon.FillType = PolygonFillType.Solid;
					}

					// Update the morph polygon fill type
					morphPolygon.FillType = polygon.FillType;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the polygon's normalized time
						morphPolygon.Time = container.PolygonTimes[polygonIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}
				}
				// Else the polygon was not found...
				else
				{
					// Create a new morph polygon
					IMorphPolygon morphPolygon = new MorphPolygon();
					morphPolygon.Polygon = polygon;

					// If we are in time based mode then...
					if (PolygonType == PolygonType.TimeBased)
					{
						// Set the polygon's normalized time
						morphPolygon.Time = container.PolygonTimes[polygonIndex];
					}
					else
					{
						// Otherwise just set the time to zero since it is a don't care
						morphPolygon.Time = 0.0;
					}

					// Indicate that we don't need to remove this morph polygon
					morphPolygon.Removed = false;

					// If the polygon's fill type is set to wipe and
					// the polygon is not a rectangle or triangle then...
					if (polygon.FillType == PolygonFillType.Wipe &&
					    !(polygon.Points.Count == 4 ||
					      polygon.Points.Count == 3))
					{
						// Set the polygon fill type to solid
						polygon.FillType = PolygonFillType.Solid;
					}
					
					// Update the morph polygon fill ;type
					morphPolygon.FillType = polygon.FillType;

					// Add the polygon to collection
					MorphPolygons.Add(morphPolygon);
				}
			}
		}

		/// <summary>
		/// Updates the frame buffer for a location based pixel.
		/// </summary>
		private void UpdateFrameBufferForLocationPixel(int x, int y, int bufferHt, IPixelFrameBuffer tempFrameBuffer, IPixelFrameBuffer frameBuffer)
		{
			// Save off the original location node
			int yCoord = y;
			int xCoord = x;

			// Flip me over so and offset my coordinates I can act like the string version
			y = Math.Abs((BufferHtOffset - _marginOffsetY) + bufferHt - y - 1);
			y = y;
			x = x - (BufferWiOffset - _marginOffsetX);

			// Retrieve the color from the bitmap
			Color color = tempFrameBuffer.GetColorAt(x, y);

			// Set the pixel on the frame buffer
			frameBuffer.SetPixel(xCoord, yCoord, color);
		}

		/// <summary>
		/// Calculates velocity at time zero for the specified length and effect duration (time).
		/// </summary>
		/// <remarks>
		/// This method was based on the math at the following URL:
		/// https://math.stackexchange.com/questions/1777751/calculate-position-with-increasing-acceleration
		/// </remarks>
		private double GetIncreasingVelocityZero(double acceleration, double length, double time)
		{
			double t = time;
			double i = acceleration;

			double velocityZero = (length - 1.0 / 6.0 * i * t * t * t) / t;

			return velocityZero;
		}

		/// <summary>
		/// Calculates the position of the wipe head the specified time and startig velocity.
		/// </summary>
		/// <remarks>
		/// This method was based on the math at the following URL:
		/// https://math.stackexchange.com/questions/1777751/calculate-position-with-increasing-acceleration
		/// </remarks>
		double GetIncreasingHeadPosition(double acceleration, double time, double velocityZero)
		{
			double t = time;
			double i = acceleration;
			double v = velocityZero;

			double x = 1.0 / 6.0 * (i * t * t * t) + (v * t); 

			return x;
		}

		/// <summary>
		/// This method is needed to compare two colors for masking ellipse wipes.
		/// The built in Equals method was not working because of the empty property of one of the two colors.
		/// </summary>
		/// <param name="left">Left color to compare</param>
		/// <param name="right">Right color to compare</param>
		/// <returns>True if the two colors are identical for base color attributes</returns>
		private bool ColorEquals(Color left, Color right)
		{
			return (left.A == right.A &&
			       left.R == right.R &&
			       left.G == right.G &&
			       left.B == right.B);
		}
		
		/// <summary>
		/// Draws a line between the specified two points.
		/// </summary>		
		private void DrawThickLine(
			// ReSharper disable once InconsistentNaming
			int x0_,
			// ReSharper disable once InconsistentNaming
			int y0_,
			// ReSharper disable once InconsistentNaming
			int x1_,
			// ReSharper disable once InconsistentNaming
			int y1_, 
			Color color, 
			bool direction, 
			IPixelFrameBuffer frameBuffer,
			IPixelFrameBuffer maskFrameBuffer)
		{
			int x0 = x0_;
			int x1 = x1_;
			int y0 = y0_;
			int y1 = y1_;
			int lastx = x0;
			int lasty = y0;

			int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
			int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
			int err = (dx > dy ? dx : -dy) / 2, e2;

			for(;;)
			{
				// Ellipses are being drawn using a mask buffer
				// This if check is checking to see if the corresponding pixel is set in the mask buffer OR
				// if the mask buffer is not applicable (Polygons and lines).
				if (maskFrameBuffer == null || 
					y0 >= 0 &&
					x0 >= 0 &&
					!ColorEquals(maskFrameBuffer.GetColorAt(x0, y0), _emptyColor))
				{
					frameBuffer.SetPixel(x0, y0, color);
				}

				if((x0 != lastx) && (y0 != lasty) && (x0_ != x1_) && (y0_ != y1_) )
				{
					int fix = 0;
					if(x0 > lastx ) fix += 1;
					if(y0 > lasty ) fix += 2;
					if(direction  ) fix += 4;
					switch (fix)
					{
					case 2:
					case 4:
							if (x0 < _bufferWi - 2)
							{
								// If the mask is NOT applicable or
								// if the pixel is set in the mask then...
								if (maskFrameBuffer == null ||
								    !ColorEquals(maskFrameBuffer.GetColorAt(x0 + 1, y0), _emptyColor))
								{
									frameBuffer.SetPixel(x0 + 1, y0, color);
								}
							}
							break;
					case 3:
					case 5:
							if (x0 > 0)
							{
								// If the mask is NOT applicable or
								// if the pixel is set in the mask then...
								if (maskFrameBuffer == null ||
								    !ColorEquals(maskFrameBuffer.GetColorAt(x0 - 1, y0), _emptyColor))
								{
									frameBuffer.SetPixel(x0 - 1, y0, color);
								}
							}
							break;
					case 0:
					case 1:
							if (y0 < _bufferHt - 2)
							{
								// If the mask is NOT applicable or
								// if the pixel is set in the mask then...
								if (maskFrameBuffer == null ||
								    !ColorEquals(maskFrameBuffer.GetColorAt(x0, y0 + 1), _emptyColor))
								{
									frameBuffer.SetPixel(x0, y0 + 1, color);
								}
							}
							break;
					case 6:
					case 7:
							if (y0 > 0)
							{
								// If the mask is NOT applicable or
								// if the pixel is set in the mask then...
								if (maskFrameBuffer == null || 
								    !ColorEquals(maskFrameBuffer.GetColorAt(x0, y0 - 1), _emptyColor))
								{
									frameBuffer.SetPixel(x0, y0 - 1, color);
								}
							}
							break;
					}
				}
				lastx = x0;
				lasty = y0;
				if (x0==x1 && y0==y1) break;
				e2 = err;
				if (e2 >-dx) { err -= dy; x0 += sx; }
				if (e2<dy) { err += dx; y0 += sy; }
			}
		}
		
		/// <summary>
		/// Calculates the points along the specified line and stores them in the specified collections.
		/// </summary>		
		private void StoreLine(
			// ReSharper disable once InconsistentNaming
			int x0_,
			// ReSharper disable once InconsistentNaming
			int y0_,
			// ReSharper disable once InconsistentNaming
			int x1_,
			// ReSharper disable once InconsistentNaming
			int y1_, 
			List<int> vx, List<int> vy)
		{
			int x0 = x0_;
			int x1 = x1_;
			int y0 = y0_;
			int y1 = y1_;

			int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
			int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
			int err = (dx > dy ? dx : -dy) / 2, e2;

			for(;;)
			{
				vx.Add(x0);
				vy.Add(y0);
				if (x0==x1 && y0==y1) break;
				e2 = err;
				if (e2 >-dx) { err -= dy; x0 += sx; }
				if (e2<dy) { err += dx; y0 += sy; }
			}
		}

		/// <summary>
		/// Calculates the smallest positive acceleration for the specified morph polygon
		/// that allows the wipe to make it to the end of the polygon/line with a zero starting velocity.
		/// </summary>		
		/// <remarks>
		/// If the acceleration gets any larger the velocity zero starts negative.
		/// This method finds the largest acceleration where the velocity zero is positive.
		/// This value is then scaled based on the user's selection for acceleration.
		/// </remarks>
		private double GetPositiveAcceleration(int length, double time)
		{
			return length / (time * time * time) * 6.0;						
		}

		/// <summary>
		/// Gets the head duration as a percentage.
		/// </summary>		
		private double GetHeadDurationPercentage(IMorphPolygon morphPolygon)
		{
			return morphPolygon.HeadDuration / 100.0; 
		}

		/// <summary>
		/// Refines the acceleration using the specified good and bad accelerations.  Using a binary search approach.
		/// </summary>
		/// <param name="validAcceleration">Valid acceleration</param>
		/// <param name="invalidAcceleration">Invalid acceleration</param>
		/// <param name="length">Length of the polygon/line</param>
		/// <param name="time">Time of the wipe</param>
		/// <returns>Refined acceleration</returns>
		private double RefineAcceleration(double invalidAcceleration, double validAcceleration, int length, double time)
		{
			const int MaxIterations = 10000;
			int iterations = 0;

			// Loop until the acceleration converge or we have iterated for the maximum number of iterations
			while (invalidAcceleration != validAcceleration && iterations < MaxIterations)
			{
				// Try an acceleration between the two accelerations
				double testAcceleration = (invalidAcceleration + validAcceleration) / 2.0;

				// If the middle acceleration is valid then...
				if (IsIncreasingAccelerationValid(testAcceleration, length, time))
				{
					// Swap out the valid acceleration
					validAcceleration = testAcceleration;
				}
				else
				{
					// Swap out the invalid acceleration
					invalidAcceleration = testAcceleration;
				}

				iterations++;
			}

			// Return the refined acceleration
			return validAcceleration;
		}
		

		/// <summary>
		/// Calculates the tail acceleration when the acceleration is negative.
		/// </summary>
		/// <param name="length">Length of the polygon/line</param>
		/// <param name="time">Time for the wipe</param>
		/// <param name="morphPolygon">Morph polygon associated with the wipe</param>
		/// <returns>Acceleration of the wipe</returns>
		private double GetNegativeAcceleration(int length, double time, IMorphPolygon morphPolygon)
		{
			// Default the acceleration to the morph polygon setting to something large
			double tailAcceleration = morphPolygon.Acceleration;
			
			// Keep track of the previous acceleration
			double previousAcceleration = 0.0;

			// Loop until we find a valid acceleration
			while (!IsIncreasingAccelerationValid(tailAcceleration, length, time))
			{
				// Keep reducing the acceleration by half until we find a valid acceleration
				previousAcceleration = tailAcceleration;
				tailAcceleration = tailAcceleration / 2.0;
			}

			// Now we have a valid acceleration and an invalid acceleration
			// Use these two values to refine the acceleration
			return RefineAcceleration(previousAcceleration, tailAcceleration, length, time);
		}

		/// <summary>
		/// Returns true if the specified acceleration is valid for the duration of the effect.
		/// </summary>
		/// <param name="acceleration">Acceleration to test</param>
		/// <param name="length">Length of the wipe</param>
		/// <param name="time">Duration of the wipe in milliseconds</param>
		/// <returns>Whether the acceleration is valid</returns>
		/// <remarks>
		/// Some of the math used in this method was derived from:
		/// https://math.stackexchange.com/questions/1777751/calculate-position-with-increasing-acceleration
		///</remarks>
		private bool IsIncreasingAccelerationValid(double acceleration, double length, double time)
		{
			// Store off the time of the wipe
			double t = time;

			// Initialize the acceleration
			double i = acceleration;

			// Calculate initial velocity of the wipe
			double v = (length - 1.0 / 6.0 * i * t * t * t) / t;

			// Default the acceleration to valid
			bool valid = true;

			// Keep track of the previous X position
			double xPrevious = 0;

			// Iterate over all the frames
			for (double ti = 0; ti < t; ti += FrameTime)
			{
				// Calculate the position of X
				double x = GetIncreasingHeadPosition(i, ti, v);
				
				// If X starts backing up at any point then...
				if (x < xPrevious)
				{
					// Indicate the acceleration is invalid
					valid = false;
					break;
				}

				// Update the previous X value
				xPrevious = x;
			}

			// Return whether the starting velocity was positive and
			// that the wipe never starts going backwards
			return (v > 0.0) && valid;
		}
		
		/// <summary>
		/// Gets the head velocity zero for the specified morph polygon.
		/// </summary>		
		private double GetHeadVelocityZero(double length, double headAcceleration, double time, IMorphPolygon morphPolygon)
		{
			// Get the percentage of the effect that it should take the head to travel across the polygon
			double headDuration = GetHeadDurationPercentage(morphPolygon);			

			// Calculate the velocity zero of the head
			return GetIncreasingVelocityZero(headAcceleration, length, time * headDuration);
		}
		
		/// <summary>
		/// Renders time based polygons.
		/// </summary>		
		private void RenderEffectTimeBased(
			int frameNum, 
			IPixelFrameBuffer frameBuffer, 
			double intervalPos)
		{			
			// If there is at least one morph polygon then...
			if (MorphPolygons.Count > 0)
			{
				// If the morph polygon contains a polygon then...
				if (MorphPolygons[0].Polygon != null)
				{
					// The first polygon determine the number of polygon points for all polygon in the effect
					int numberOfPoints = MorphPolygons[0].Polygon.Points.Count;
					RenderEffectTimeBasedPolygon(frameNum, frameBuffer, intervalPos, numberOfPoints);
				}
				// Otherwise if the morph polygon contains a line then...
				else if (MorphPolygons[0].Line != null)
				{
					RenderEffectTimeBasedLine(frameNum, frameBuffer, intervalPos);
				}
				// Otherwise if the morph polygon contains an ellipse then...
				else if (MorphPolygons[0].Ellipse != null)
				{
					RenderEffectTimeBasedEllipse(frameNum, frameBuffer, intervalPos);
				}
			}
		}

		/// <summary>
		/// Find the two applicable morph polygons frame snapshots based on the current position in the effect timeline.
		/// </summary>		
		private int FindTwoPolygonsOnTimeline(
			double time,
			List<IMorphPolygon> morpPolygons)			
		{
			int polygonIndex = -1;

			// Calculate how are we are into the effect as a percentage
			double timeFraction = time / TimeSpan.TotalMilliseconds;

			// Loop over the polygons and find the two polygons we are between based on the timeline
			for (int index = morpPolygons.Count - 1; index >= 1; index--)
			{
				IMorphPolygon firstPolygon = MorphPolygons[index - 1];
				IMorphPolygon secondPolygon = MorphPolygons[index];

				if (firstPolygon.Time < timeFraction && timeFraction <= secondPolygon.Time)
				{
					polygonIndex = index - 1;
					break;
				}
			}

			return polygonIndex;
		}

		/// <summary>
		///  Renders the time based polygon.
		/// </summary>		
		private void RenderEffectTimeBasedPolygon(
			int frameNum, 
			IPixelFrameBuffer frameBuffer, 
			double intervalPos, 
			int numberOfPoints)
		{
			// Get the morph polygons that have the same number of polygon points as the first polygon
			// All other polygons are going to be ignored
			List<IMorphPolygon> morpPolygons = MorphPolygons.Where(morphPolygon => morphPolygon.Polygon.Points.Count == numberOfPoints).ToList();

			// Render the time based morph polygons
			RenderEffectTimeBasedMorpPolygon(frameNum, morpPolygons, frameBuffer, intervalPos);			
		}

		/// <summary>
		///  Renders the time based ellipse.
		/// </summary>		
		private void RenderEffectTimeBasedEllipse(
			int frameNum,
			IPixelFrameBuffer frameBuffer,
			double intervalPos)
		{
			// Get the morph polygons that contain an ellipse
			List<IMorphPolygon> morpPolygons = MorphPolygons.Where(morphPolygon => morphPolygon.Ellipse != null).ToList();

			// Render the time based morph ellipses
			RenderEffectTimeBasedMorphEllipse(frameNum, morpPolygons, frameBuffer, intervalPos);
		}

		/// <summary>
		/// Renders the time based line.
		/// </summary>		
		private void RenderEffectTimeBasedLine(
			int frameNum, 
			IPixelFrameBuffer frameBuffer, 
			double intervalPos)
		{
			// Get the morph polygons that contain lines
			// All other polygons are ignored
			List<IMorphPolygon> morphLines = MorphPolygons.Where(morphPolygon => morphPolygon.Line != null).ToList();

			// Render the time based morph polygons (lines)
			RenderEffectTimeBasedMorpPolygon(frameNum, morphLines, frameBuffer, intervalPos);
		}

		/// <summary>
		/// Renders the collection of morph polygons.
		/// </summary>		
		private void RenderEffectTimeBasedMorpPolygon(
			int frameNum, 
			List<IMorphPolygon> morpPolygons, 
			IPixelFrameBuffer frameBuffer, 
			double intervalPos)
		{
			// Calculate how far into effect we are
			double time = frameNum * FrameTime;

			// Find the index of the two polygons based on where we are on the timeline of the effect
			// The polygonIndex points to the first polygon			
			int polygonIndex = FindTwoPolygonsOnTimeline(time, morpPolygons);

			// If two polygons were found then...
			if (polygonIndex < MorphPolygons.Count - 1 && polygonIndex != -1)
			{
				// Collection of points for the polygon that is moving between the two frame snapshots
				List<Point> points = new List<Point>();

				// Loop over the polygon points
				for (int ptIndex = 0; ptIndex < MorphPolygons[polygonIndex].GetPolygonPoints().Count; ptIndex++)
				{
					// Get the start polygon and the end polygon
					IMorphPolygon startPolygon = MorphPolygons[polygonIndex];
					IMorphPolygon endPolygon = MorphPolygons[polygonIndex + 1];

					// Add the point to the intermediate snapshot
					points.Add(CalculateItermediatePoint(time, startPolygon.GetPolygonPoints()[ptIndex], endPolygon.GetPolygonPoints()[ptIndex], startPolygon.Time, endPolygon.Time));
				}

				// Render the intermediate polygon
				RenderStaticPolygon(frameBuffer, intervalPos, points, FillBrightness);
			}
		}

		/// <summary>
		/// Renders the collection of time based morph polygon ellipses.
		/// </summary>		
		private void RenderEffectTimeBasedMorphEllipse(
			int frameNum,
			List<IMorphPolygon> morpPolygons,
			IPixelFrameBuffer frameBuffer,
			double intervalPos)
		{
			// Calculate how far into effect we are
			double time = frameNum * FrameTime;

			// Find the index of the two polygons based on where we are on the timeline of the effect
			// The polygonIndex points to the first polygon			
			int polygonIndex = FindTwoPolygonsOnTimeline(time, morpPolygons);

			// If two polygons were found then...
			if (polygonIndex < MorphPolygons.Count - 1 && polygonIndex != -1)
			{
				// Collection of points for the polygon that is moving between the two frame snapshots
				List<Point> points = new List<Point>();

				// Get the start polygon and the end polygon
				IMorphPolygon startPolygon = MorphPolygons[polygonIndex];
				IMorphPolygon endPolygon = MorphPolygons[polygonIndex + 1];

				// Loop over the polygon points
				for (int ptIndex = 0; ptIndex < MorphPolygons[polygonIndex].GetPolygonPoints().Count; ptIndex++)
				{
					// Add the point to the intermediate snapshot
					points.Add(CalculateItermediatePoint(time, startPolygon.GetPolygonPoints()[ptIndex], endPolygon.GetPolygonPoints()[ptIndex], startPolygon.Time, endPolygon.Time));
				}

				// Calculate a new center for the ellipse based on the time line
				Point newCenter = CalculateItermediatePoint(time, startPolygon.Ellipse.Center, endPolygon.Ellipse.Center, startPolygon.Time, endPolygon.Time);
				
				// Clone the starting polygon
				Ellipse intermediateEllipse = startPolygon.Ellipse.Clone();
				
				// Update the center of the polygon
				intermediateEllipse.Center.X = newCenter.X;
				intermediateEllipse.Center.Y = newCenter.Y;

				// Calculates the intermediate angle of the ellipse
				intermediateEllipse.Angle = CalculateIntermediateValue(time, startPolygon.Ellipse.Angle,
					endPolygon.Ellipse.Angle, startPolygon.Time, endPolygon.Time);

				// Calculate the intermediate width of the ellipse
				intermediateEllipse.Width = CalculateIntermediateValue(time, startPolygon.Ellipse.Width,
					endPolygon.Ellipse.Width, startPolygon.Time, endPolygon.Time);

				// Calculate the intermediate height of the ellipse
				intermediateEllipse.Height = CalculateIntermediateValue(time, startPolygon.Ellipse.Height,
					endPolygon.Ellipse.Height, startPolygon.Time, endPolygon.Time);

				// Render the intermediate ellipse
				RenderStaticEllipse(frameBuffer, points, intermediateEllipse.Angle, intermediateEllipse, FillPolygon, GetFillColor(intervalPos, FillBrightness));
			}
		}

		/// <summary> 
		/// Calculate a point on the intermediate snapshot polygon/line.
		/// </summary>		
		private Point CalculateItermediatePoint(double time, PolygonPoint startPoint, PolygonPoint endPoint, double startTime, double endTime)
		{
			// Calculate the total time between the two polygons
			double totalTimeOnLine = (endTime - startTime) * TimeSpan.TotalMilliseconds;

			// Calculate the distance in the x-axis between the two points
			double distanceX = endPoint.X - startPoint.X;

			// Calculate the velocity in the x-axis required to move between the two points
			double velocityX = distanceX / totalTimeOnLine;

			// Calculate the distance in the y-axis between the two points
			double distanceY = endPoint.Y - startPoint.Y;

			// Calculate the velocity in the y-axis required to move between the two points
			double velocityY = distanceY / totalTimeOnLine;

			// Calculate the time on the line
			double currentTimeOnLine = time - (startTime * TimeSpan.TotalMilliseconds);

			// Calculate the x and Y positions on the line
			double x = startPoint.X + velocityX * currentTimeOnLine;
			double y = startPoint.Y + velocityY * currentTimeOnLine;

			// Return the intermediate point
			return new Point((int)Math.Round(x), (int)Math.Round(y));
		}

		/// <summary>
		/// Calculates an intermediate value based on a start and and end value and a time line.
		/// </summary>
		/// <param name="time">Current time in the effect</param>
		/// <param name="startValue">Start value</param>
		/// <param name="endValue">End value</param>
		/// <param name="startTime">Start time associated with the start value</param>
		/// <param name="endTime">End time associated with the end value</param>
		/// <returns>Intermediate value based on time line</returns>
		private double CalculateIntermediateValue(double time, double startValue, double endValue, double startTime, double endTime)
		{
			// Calculate the total time between the two ellipses
			double totalTimeOnLine = (endTime - startTime) * TimeSpan.TotalMilliseconds;

			// Calculate the difference in the two values
			double difference = endValue - startValue;

			// Calculate the velocity of the value 
			double velocity = difference / totalTimeOnLine;

			// Calculate the time on the time line
			double currentTimeOnLine = time - (startTime * TimeSpan.TotalMilliseconds);

			// Calculate the intermediate value
			double intermediateValue= startValue + velocity * currentTimeOnLine;

			return intermediateValue;
		}

		/// <summary>
		/// Renders a static polygon.
		/// </summary>		
		private void RenderStaticPolygon(IPixelFrameBuffer frameBuffer, 
			double intervalPos, 
			List<Point> points,
			Curve intensity)
		{
			// Create a bitmap the size of the display element			
			using (var bitmap = new Bitmap(_bufferWi, _bufferHt))
			{
				if (points.Count > 2)
				{
					// Render the polygon on the bitmap
					InitialRenderPolygon(bitmap, points.ToArray(), FillPolygon, GetFillColor(intervalPos, intensity));
				}
				else
				{
					// Render the line on the bitmap
					InitialRenderLine(bitmap, points.ToArray(), GetFillColor(intervalPos, intensity));
				}

				// Copy from the bitmap into the frame buffer
				CopyBitmapToPixelFrameBuffer(bitmap, frameBuffer);
			}
		}

		/// <summary>
		/// Renders a static polygon.
		/// </summary>		
		private void RenderStaticEllipse(
			IPixelFrameBuffer frameBuffer,
			List<Point> points,
			double angle,
			Ellipse ellipse,
			bool fillEllipse,
			Color color)
		{
			// Create a bitmap the size of the display element			
			using (var bitmap = new Bitmap(_bufferWi, _bufferHt))
			{
				// Render the ellipse on the bitmap
				InitialRenderEllipse(bitmap, points.ToArray(), fillEllipse, color, angle, ellipse);
				
				// Copy from the bitmap to the frame buffer
				CopyBitmapToPixelFrameBuffer(bitmap, frameBuffer);
			}
		}

		/// <summary>
		/// Renders the collection of morph polygons.
		/// </summary>		
		private void RenderStaticPolygons(IPixelFrameBuffer frameBuffer, double intervalPos, List<IMorphPolygon> expandedMorphPolygon)
		{
			// If there are any static polygon/lines then...
			if (expandedMorphPolygon.Any())
			{
				// Create a bitmap the size of the display element			
				using (Bitmap bitmap = new Bitmap(_bufferWi, _bufferHt))
				{
					// Loop over the morph polygons
					foreach (IMorphPolygon morphPolygon in expandedMorphPolygon)
					{
					
						// Convert the polygon/line points into Microsoft Drawing Points
						List<Point> points = morphPolygon.GetPolygonPoints()
							.Select(pt => new Point((int) Math.Round(pt.X, MidpointRounding.AwayFromZero), (int) Math.Round(pt.Y, MidpointRounding.AwayFromZero)))
							.ToList();

						// If the points make a polygon then...
						if (points.Count > 2)
						{
							// Render the polygon
							InitialRenderPolygon(bitmap, points.ToArray(),
								morphPolygon.FillType == PolygonFillType.Solid,
								GetFillColor(intervalPos, morphPolygon));
						}
						else
						{
							// Otherwise render the line
							InitialRenderLine(bitmap, points.ToArray(), GetFillColor(intervalPos, morphPolygon));
						}
					}

					// Copy the polygons/lines to the frame buffer
					CopyBitmapToPixelFrameBuffer(bitmap, frameBuffer);
				}
			}
		}

		/// <summary>
		/// Renders the collection of morph ellipses.
		/// </summary>		
		private void RenderStaticEllipse(
			IPixelFrameBuffer frameBuffer, 
			double intervalPos, 
			List<IMorphPolygon> expandedMorphPolygon)
		{
			// If there are any static ellipses then...
			if (expandedMorphPolygon.Any())
			{
				// Create a bitmap the size of the display element			
				using (Bitmap bitmap = new Bitmap(_bufferWi, _bufferHt))
				{
					// Loop over the morph polygons
					foreach (IMorphPolygon morphPolygon in expandedMorphPolygon)
					{
						// Convert the ellipse points into Microsoft Drawing Points
						List<Point> points = morphPolygon.GetPolygonPoints()
							.Select(pt => new Point((int)Math.Round(pt.X), (int)Math.Round(pt.Y))).ToList();

						// Render the ellipse
						InitialRenderEllipse(
							bitmap,
							points.ToArray(),
							morphPolygon.FillType == PolygonFillType.Solid ||
							morphPolygon.FillType == PolygonFillType.Wipe,
							GetFillColor(intervalPos, morphPolygon),
							morphPolygon.Ellipse.Angle,
							morphPolygon.Ellipse);
					}

					// Copy the ellipse to the frame buffer
					CopyBitmapToPixelFrameBuffer(bitmap, frameBuffer);
				}
			}
		}

		/// <summary>
		/// Copies the bitmap to the pixel frame buffer.
		/// </summary>		
		private void CopyBitmapToPixelFrameBuffer(Bitmap bitmap, IPixelFrameBuffer frameBuffer) 
		{
			// Create a FastPixel instance based on the bitmap				
			using (FastPixel.FastPixel fp = new FastPixel.FastPixel(bitmap))
			{
				fp.Lock();

				// Copy from the fastpixel bitmap to the frame buffer
				for (int x = 0; x < _bufferWi; x++)
				{
					for (int y = 0; y < _bufferHt; y++)
					{
						// Transfer the pixel from the bitmap to the frame buffer
						CalculatePixelFlip(x, y, _bufferHt, fp, frameBuffer);
					}
				}
				fp.Unlock(false);
			}
		}
		
		/// <summary>
		/// Renders the specified polygon points.
		/// </summary>		
		private void InitialRenderPolygon(Bitmap bitmap, Point[] points, bool fillPolygon, Color fillColor)
		{			
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				if (fillPolygon)
				{
					// Create a brush using the fill color
					using (SolidBrush brush = new SolidBrush(fillColor))
					{
						// Fill the polygon
						graphics.FillPolygon(brush, points);
					}
				}

				// Create a pen using the fill color
				// Doesn't seem like DrawPolyon should be necessary when
				// the polygon is filled but testing has proven otherwise.
				// The documentation for FillPolygon does mention that FillPolygon
				// fills the interior of the polygon.
				using (Pen pen = new Pen(fillColor))
				{
					// Outline the polygon
					graphics.DrawPolygon(pen, points);
				}
			}
		}

		/// <summary>
		/// Renders the specified ellipse.
		/// </summary>		
		private void InitialRenderEllipse(Bitmap bitmap, Point[] points, bool fillEllipse, Color fillColor, double angle, Ellipse ellipse)
		{
			// Create a Graphics instance from the bitmap
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				// Translate the Graphics origin to the (rotation) center of the ellipse
				graphics.TranslateTransform((int)ellipse.Center.X, (int)ellipse.Center.Y);
				graphics.RotateTransform((float)angle);
				
				// If the ellipse is filled then...
				if (fillEllipse)
				{
					// Create a solid brush with the fill color
					using (SolidBrush brush = new SolidBrush(fillColor))
					{
						// Draw the ellipse using the brush
						graphics.FillEllipse(
							brush,
							new Rectangle(
								(int)(- ellipse.Width / 2.0),(int)(-ellipse.Height / 2.0),
								(int)ellipse.Width,
								(int)ellipse.Height));
					}
				}
				// Otherwise the ellipse is just an outline
				else
				{
					// Create a pen using the fill color
					using (Pen pen = new Pen(fillColor))
					{
						// Draw the ellipse using the pen
						graphics.DrawEllipse(
							pen,
							new Rectangle(
								(int)(-ellipse.Width / 2.0), (int)(-ellipse.Height / 2.0),
								(int)ellipse.Width,
								(int)ellipse.Height));
					}
				}
			}
		}

		/// <summary>
		/// Renders the specified line points.
		/// </summary>		
		private void InitialRenderLine(Bitmap bitmap, Point[] points, Color fillColor)
		{
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{									
				using (Pen pen = new Pen(fillColor))
				{
					graphics.DrawLine(pen, points[0], points[1]);
				}				
			}
		}

		/// <summary>
		/// Calculates the color of the specified pixel from the fast pixel bitmap.
		/// </summary>		
		private void CalculatePixelFlip(int x, int y, int bufferHt, FastPixel.FastPixel bitmap, IPixelFrameBuffer frameBuffer)
		{
			if (x >= 0 && y >= 0 && y < _bufferHt && x < _bufferWi)
			{
				Color color = bitmap.GetPixel(x, bufferHt - y - 1);

				if (!_emptyColor.Equals(color))
				{
					frameBuffer.SetPixel(x, y, color);
				}
			}
		}

		/// <summary>
		/// Expands the pattern morph polygon based on the repeating pattern.
		/// </summary>
		private void ExpandPatternMorphPolygons()
		{
			// Create a new list of expanded morph polygons
			_patternExpandedMorphPolygons = new List<IMorphPolygon>();
			
			// Transfer the Fill Type to the morph polygon
			MorphPolygons[0].FillType = FillType;

			// Transfer the Fill color to the morph polygon
			MorphPolygons[0].FillColor = FillColor;

			// Add the user drawn polygon to the collection
			_patternExpandedMorphPolygons.Add(MorphPolygons[0]);
			
			// The first polygon gets the entire effect duration
			MorphPolygons[0].StartOffset = 0;

			// Create morph polygons for the repeating pattern
			for (int index = 0; index < RepeatCount; index++)
			{
				// Clone the previous morph polygon
				IMorphPolygon morphPolygon = (IMorphPolygon)_patternExpandedMorphPolygons.Last().Clone();

				// Calculate the start delay of the wipe
				morphPolygon.StartOffset = Stagger * (index + 1);
				
				// Add the cloned morph polygon to the collection
				_patternExpandedMorphPolygons.Add(morphPolygon);

				// Move the morph polygon based on the repeat direction
				switch (RepeatDirection)
				{
					case WipeRepeatDirection.Down:
						morphPolygon.GetPointBasedShape().MovePoints(0, RepeatSkip);
						UpdateEllipseForYOffset(morphPolygon.Ellipse, RepeatSkip);
						break;
					case WipeRepeatDirection.Up:
						morphPolygon.GetPointBasedShape().MovePoints(0, -RepeatSkip);
						UpdateEllipseForYOffset(morphPolygon.Ellipse, -RepeatSkip);
						break;
					case WipeRepeatDirection.Left:
						morphPolygon.GetPointBasedShape().MovePoints(-RepeatSkip, 0);
						UpdateEllipseForXOffset(morphPolygon.Ellipse, -RepeatSkip);
						break;
					case WipeRepeatDirection.Right:
						morphPolygon.GetPointBasedShape().MovePoints(RepeatSkip, 0);
						UpdateEllipseForXOffset(morphPolygon.Ellipse, RepeatSkip);
						break;
					default:
						Debug.Assert(false, "Unsupported Direction");
						break;
				}
			}			
		}

		/// <summary>
		/// Renders the effect for for pattern polygon mode.
		/// </summary>		
		private void RenderEffectPattern(int frameNum, IPixelFrameBuffer frameBuffer, double intervalPos)
		{
			// If the polygon fill type is Wipe then...
			if (FillType == PolygonFillType.Wipe)
			{				
				// Render the morph polygons/lines using a wipe
				// Note this method handles the repeating pattern
				RenderEffectWipes(frameNum, frameBuffer, intervalPos, GetWipePolygons());
			}
			else
			{
				// If the pattern polygons have not been expanded then...
				if (_patternExpandedMorphPolygons == null)
				{
					// Expand the pattern polygon/line into multiple morph polygons
					ExpandPatternMorphPolygons();
				}

				// If the morph polygon is a polygon or a line then...
				if (MorphPolygons[0].Polygon != null || MorphPolygons[0].Line != null)
				{
					// Render the solid or outline polygon/line
					RenderStaticPolygons(frameBuffer, intervalPos, _patternExpandedMorphPolygons);
				}
				// Otherwise the morph polygon is an ellipse
				else
				{
					// Render the solid or outline ellipse
					RenderStaticEllipse(frameBuffer, intervalPos, _patternExpandedMorphPolygons);
				}
			}
		}

		/// <summary>
		/// Renders the effect for free form polygon mode.
		/// </summary>		
		private void RenderEffectFreeForm(int frameNum, IPixelFrameBuffer frameBuffer, double intervalPos)
		{
			// Render the wipe polygons/lines/ellipses
			List<IMorphPolygon> wipePolygons = GetWipePolygons();				
			RenderEffectWipes(frameNum, frameBuffer, intervalPos, wipePolygons);

			// Render the solid and outline polygons/lines
			List<IMorphPolygon> staticPolygons = MorphPolygons.Where(mp => (
				mp.Polygon != null || mp.Line != null)  && mp.FillType != PolygonFillType.Wipe).ToList();			
			RenderStaticPolygons(frameBuffer, intervalPos, staticPolygons);

			// Render the solid and outline ellipses
			List<IMorphPolygon> ellipses = MorphPolygons.Where(
				mp => mp.Ellipse != null && mp.FillType != PolygonFillType.Wipe).ToList();
			RenderStaticEllipse(frameBuffer, intervalPos, ellipses);
		}
		
		/// <summary>
		/// Returns the length of the wipe polygon.  Note not looking at the start or end sides of the polygon.
		/// </summary>		
		private int GetLengthOfWipePolygon(MorphWipePolygonRenderData wipeRenderData)
		{
			// Determine which side of the polygon is the longest in the x direction
			// Note this is NOT the start and stop sides.
			int length = wipeRenderData.X1Points.Count;

			if (wipeRenderData.X2Points.Count > length)
			{
				length = wipeRenderData.X2Points.Count;
			}

			return length;
		}

		/// <summary>
		/// Renders the wipes for the specified morph polygons.
		/// </summary>		
		private void RenderEffectWipes(
			int frameNum, 
			IPixelFrameBuffer frameBuffer, 
			double intervalPos, 
			List<IMorphPolygon> morphPolygons)
		{
			// Loop over the morph polygons
			for (int polygonIndex = 0; polygonIndex < morphPolygons.Count; polygonIndex++)
			{
				// Get the specified morph polygon
				MorphPolygon morphPolygon = (MorphPolygon)morphPolygons[polygonIndex];

				// Determine which side of the polygon is the longest in the x direction
				// Note this is NOT the start and stop sides.
				int length = GetLengthOfWipePolygon(_wipePolygonRenderData[polygonIndex]);

				// Initialize the time for the wipe
				double time = TimeSpan.TotalMilliseconds;

				// Make copies of all the points
				List<int> x1PointsCopy = UpdatePointList(_wipePolygonRenderData[polygonIndex].X1Points, 0);
				List<int> x2PointsCopy = UpdatePointList(_wipePolygonRenderData[polygonIndex].X2Points, 0);
				List<int> y1PointsCopy = UpdatePointList(_wipePolygonRenderData[polygonIndex].Y1Points, 0);
				List<int> y2PointsCopy = UpdatePointList(_wipePolygonRenderData[polygonIndex].Y2Points, 0);

				Ellipse ellipseMask = null;

				// If the morph polygon contains an ellipse then...
				if (morphPolygon.Ellipse != null)
				{
					// Make a copy of the ellipse to draw the mask.  Need a copy because it is going to modified
					// as it is repeated.
					ellipseMask = morphPolygon.Ellipse.Clone();
				}

				// Draw a wipe for each repeated polygon
				for (int index = 0; index < RepeatCount + 1; index++)
				{
					// Determine the time in the wipe
					double timeInWipe = frameNum * FrameTime;

					// If this is Free Form polygon then....
					if (RepeatCount == 0 && morphPolygon.StartOffset != 0)
					{
						// Adjust the time for the stagger
						timeInWipe = timeInWipe - ((_wipePolygonRenderData[polygonIndex].Stagger / 100.0) * time);
					}
					else
					{
						// Adjust the time for the stagger
						timeInWipe = timeInWipe - (index * (_wipePolygonRenderData[polygonIndex].Stagger / 100.0) * time);
					}

					// If the wipe time is valid (positive) then...
					if (timeInWipe >= 0.0)
					{							
						// Calculate the wipe head position using the acceleration and initial velocity			
						double headPosition = GetIncreasingHeadPosition(
							_wipePolygonRenderData[polygonIndex].HeadAcceleration,
							timeInWipe,
							_wipePolygonRenderData[polygonIndex].HeadVelocityZero);

						// Calculate the interval position factor (0-100.0)
						double intervalPosFactor = intervalPos * 100;

						// Once the head has completed the wipe set a flag so that it doesn't back up
						// back onto the display element when de-accelerating.
						if (headPosition > length + morphPolygon.HeadLength - 1)
						{
							_wipePolygonRenderData[polygonIndex].HeadIsDone[index] = true;								
						}

						// If the head has wiped across the polygon then....
						if (_wipePolygonRenderData[polygonIndex].HeadIsDone[index])
						{
							// Set its position just past the end of the polygon so that the tail
							// can wipe off the polygon							
							headPosition = length + morphPolygon.HeadLength;
						}

						// Calculate the tail position					
						int tailPosition = (int)GetIncreasingHeadPosition(
							_wipePolygonRenderData[polygonIndex].TailAcceleration,
							timeInWipe,
							_wipePolygonRenderData[polygonIndex].TailVelocityZero);
								
						// Calculate the position in the wipe
						double intervalHeadPos = headPosition / length;
						double intervalHeadPosFactor = intervalHeadPos * 100;

						// Retrieve the head and tail colors from the morph polygon gradients
						Color headColor = GetHeadColor(intervalPos, morphPolygon);
						Color tailColor = GetTailColor(intervalPos, morphPolygon);

						IPixelFrameBuffer maskFrameBuffer = null;
 					
						// If the morph polygon contains an ellipse then...
						if (morphPolygon.Ellipse != null)
						{
							// Create a frame buffer for the ellipse mask
							maskFrameBuffer = new PixelFrameBuffer(_bufferWi, _bufferHt);

							// Convert the ellipse points into Microsoft Drawing Points
							List<Point> points = morphPolygon.Ellipse.Points
								.Select(pt => new Point((int)Math.Round(pt.X), (int)Math.Round(pt.Y))).ToList();

							// Render the ellipse mask
							RenderStaticEllipse(
								maskFrameBuffer,
								points,
								morphPolygon.Ellipse.Angle,
								ellipseMask,
								true,
								Color.Red); // Could pick any color here
						}

						// Determine which set of points is longer
						if (_wipePolygonRenderData[polygonIndex].X1Points.Count > _wipePolygonRenderData[polygonIndex].X2Points.Count)
						{
							DrawWipe(
								intervalHeadPos,
								frameBuffer,
								maskFrameBuffer,
								tailPosition,
								x2PointsCopy, // Short Points
								y2PointsCopy,
								x1PointsCopy, // Long Points
								y1PointsCopy,
								length,
								_wipePolygonRenderData[polygonIndex].Direction,
								headColor,
								tailColor,
								morphPolygon.HeadLength);
						}
						else
						{
							DrawWipe(
								intervalHeadPos,
								frameBuffer,
								maskFrameBuffer,
								tailPosition,
								x1PointsCopy, // Short Points
								y1PointsCopy, 
								x2PointsCopy, // Long Points
								y2PointsCopy,
								length,
								_wipePolygonRenderData[polygonIndex].Direction,
								headColor,
								tailColor,
								morphPolygon.HeadLength);
						}
					}

					// Update the points based on the repeating pattern
					switch (RepeatDirection)
					{
						case WipeRepeatDirection.Down:
							y1PointsCopy = UpdatePointList(y1PointsCopy, -RepeatSkip);
							y2PointsCopy = UpdatePointList(y2PointsCopy, -RepeatSkip);
							UpdateEllipseForYOffset(ellipseMask, -RepeatSkip);
							break;
						case WipeRepeatDirection.Up:
							y1PointsCopy = UpdatePointList(y1PointsCopy, RepeatSkip);
							y2PointsCopy = UpdatePointList(y2PointsCopy, RepeatSkip);
							UpdateEllipseForYOffset(ellipseMask, RepeatSkip);
							break;
						case WipeRepeatDirection.Left:
							x1PointsCopy = UpdatePointList(x1PointsCopy, -RepeatSkip);
							x2PointsCopy = UpdatePointList(x2PointsCopy, -RepeatSkip);
							UpdateEllipseForXOffset(ellipseMask, -RepeatSkip);
							break;
						case WipeRepeatDirection.Right:
							x1PointsCopy = UpdatePointList(x1PointsCopy, RepeatSkip);
							x2PointsCopy = UpdatePointList(x2PointsCopy, RepeatSkip);
							UpdateEllipseForXOffset(ellipseMask, RepeatSkip);
							break;
						default:
							Debug.Assert(false, "Unsupported Direction");
							break;
					}
				}				
			}
		}

		/// <summary>
		/// Updates the ellipse points and center for the specified Y offset.
		/// </summary>
		/// <param name="ellipse">Ellipse to update</param>
		/// <param name="yOffset">Y offset</param>
		private void UpdateEllipseForYOffset(Ellipse ellipse, int yOffset)
		{
			// If the ellipse mask is in use then...
			if (ellipse != null)
			{
				// Loop over the ellipse points
				for (int index = 0; index < ellipse.Points.Count; index++)
				{
					// Update the Y coordinate
					ellipse.Points[index].Y += yOffset;
				}

				// Update the center of the ellipse
				ellipse.Center.Y += yOffset;
			}
		}

		/// <summary>
		/// Updates the ellipse points and center for the specified X offset.
		/// </summary>
		/// <param name="ellipse">Ellipse to update</param>
		/// <param name="xOffset">X offset</param>
		private void UpdateEllipseForXOffset(Ellipse ellipse, int xOffset)
		{
			// If the ellipse mask is in use then...
			if (ellipse != null)
			{
				// Loop over the ellipse points
				for (int index = 0; index < ellipse.Points.Count; index++)
				{
					// Update the X coordinate
					ellipse.Points[index].X += xOffset;
				}

				// Update the center of the ellipse
				ellipse.Center.X += xOffset;
			}
		}

		/// <summary>
		/// Update the following points with the specified offset.
		/// </summary>		
		private List<int> UpdatePointList(List<int> ptList, int offset)
		{
			List<int> ptListCopy = new List<int>();

			foreach (int pt in ptList)
			{
				ptListCopy.Add(pt + offset);
			}

			return ptListCopy;
		}

		/// <summary>
		/// Calculates the intensity of the specified position of the wipe tail.		
		/// </summary>		
		double GetTailIntensity(int x, int tailStartPosition, int tailEndPosition)
		{												
			// Calculate 0-1 intensity based on where the position is on the tail
			double intensity = (x - tailStartPosition + 1.0) / ((tailEndPosition- tailStartPosition));

			// Adjust the first half of the tail to full intensity
			if (intensity > .5)
			{
				intensity = 1.0;
			}
			// Then the second half of the tail fades from full intensity zero
			else
			{
				intensity = intensity / .5;
			}			
			
			return intensity;
		}

		/// <summary>
		/// Draws the specified wipe.
		/// </summary>		
		private void DrawWipe(			
			double intervalHeadPos,
			IPixelFrameBuffer frameBuffer,
			IPixelFrameBuffer maskFrameBuffer,
			int tailPosition,
			List<int> xShortPoints,
			List<int> yShortPoints,
			List<int> xLongPoints,
			List<int> yLongPoints,
			int length,
			bool direction,
			Color headColor,
			Color tailColor,
			int headLength)
		{								
			// Calculate the end of the head			
			int endOfHead = (int)Math.Round(length * intervalHeadPos);
			
			// Calculate the start of the head based on the current head length
			int startOfHead = endOfHead - headLength;
			
			// Don't allow the head to start off the display element so that
			// the tail is calculated properly.
			if (startOfHead < 0)
			{
				startOfHead = 0;
			}

			// If the start of the head is off the display element then...
			if (startOfHead > length - 1)						
			{
				// Set the start of the head to just off the display element
				startOfHead = length;
			}

			// Draw tail
			for (double x = tailPosition; x < startOfHead; x+=0.1)
			{				
				// Calculate the intensity of the tail x position
				HSV hsv = HSV.FromRGB(tailColor);
				int index = (int)x;
				hsv.V *= GetTailIntensity(index, tailPosition, startOfHead);

				DrawThickLine(x, length, xShortPoints, yShortPoints, xLongPoints, yLongPoints, hsv.ToRGB(), direction, frameBuffer, maskFrameBuffer);				
			}

			// If the start of the head is NOT off the display element then...
			// Drawing the head last so that if the head and tail overlap the head will win
			if (startOfHead < length)
			{
				// Don't let the end of the head extend off the display element
				if (endOfHead > length - 1)
				{
					endOfHead = length - 1;
				}

				// Loop over the head lines
				for (double x = startOfHead; x < endOfHead; x+=0.1)
				{
					DrawThickLine(x, length, xShortPoints, yShortPoints, xLongPoints, yLongPoints, headColor, direction, frameBuffer, maskFrameBuffer);
				}
			}			
		}

		/// <summary>
		/// Draws a line between the two specified points.
		/// </summary>		
		private void DrawThickLine(
			double longIndex,
			int length,
			List<int> xShortPoints,
			List<int> yShortPoints,
			List<int> xLongPoints,
			List<int> yLongPoints,
			Color color, 
			bool direction,
			IPixelFrameBuffer frameBuffer,
			IPixelFrameBuffer maskFrameBuffer)
		{
			// Get the length of the short side
			int shortLength = xShortPoints.Count;

			// Calculate what percentage of the long side position
			double pct = longIndex / length;

			// Calculate the short side index based on the same percentage as the long side
			int shortIndex = (int)(shortLength * pct);

			// Convert the long side index to an integer						
			int integerLongIndex = (int)longIndex;

			// Draw a line from the short side to the long side
			DrawThickLine(xShortPoints[shortIndex], yShortPoints[shortIndex], xLongPoints[integerLongIndex], yLongPoints[integerLongIndex], color, direction, frameBuffer, maskFrameBuffer);
		}
		
		/// <summary>
		/// Gets all the polygons configured with the Wipe fill.
		/// </summary>		
		private List<IMorphPolygon> GetWipePolygons()
		{
			// Find all the morph polygons that are a polygon with 3 or 4 points OR
			// is a line OR
			// is an ellipse
			return MorphPolygons.Where(mp => mp.FillType == PolygonFillType.Wipe &&
			                           ((mp.Polygon != null && mp.Polygon.Points.Count == 4 ||
										 mp.Polygon != null && mp.Polygon.Points.Count == 3 || 
									     mp.Line != null ||
									     mp.Ellipse != null))).ToList();					
		}

		/// <summary>
		/// Updates the visibility of fields.
		/// </summary>
		private void UpdateAttributes()
		{			
			UpdateStringOrientationAttributes();
			UpdatePolygonTypeAttributes(true);
			TypeDescriptor.Refresh(this);
		}
											
		/// <summary>
		/// Gets the head color of the morph polygon.  This property only applies to pattern mode.
		/// </summary>		
		private Color GetHeadColor(double intervalPos, MorphPolygon morphPolygon)
		{
			Color headColor = morphPolygon.HeadColor.GetColorAt(intervalPos);

			double intervalPosFactor = intervalPos * 100;

			// Adjust the color for brightness setting
			HSV hsv = HSV.FromRGB(headColor);
			hsv.V *= morphPolygon.HeadBrightness.GetValue(intervalPosFactor) / 100;
			headColor = hsv.ToRGB();

			return headColor;
		}

		/// <summary>
		/// Gets the tail color of the morph polygon.  This property only applies to pattern mode.
		/// </summary>		
		private Color GetTailColor(double intervalPos, IMorphPolygon morphPolygon)
		{
			Color tailColor = morphPolygon.TailColor.GetColorAt(intervalPos);

			double intervalPosFactor = intervalPos * 100;

			// Adjust the color for brightness setting
			HSV hsv = HSV.FromRGB(tailColor);
			hsv.V *= morphPolygon.TailBrightness.GetValue(intervalPosFactor) / 100;
			tailColor = hsv.ToRGB();

			return tailColor;
		}

		/// <summary>
		/// Gets the fill color with the specified intensity.  This method only applies to time based mode.
		/// </summary>		
		private Color GetFillColor(double intervalPos, Curve intensity)
		{
			Color fillColor = FillColor.GetColorAt(intervalPos);
			
			double intervalPosFactor = intervalPos * 100;

			// Adjust the color for brightness setting
			HSV hsv = HSV.FromRGB(fillColor);
			hsv.V *= intensity.GetValue(intervalPosFactor) / 100;
			fillColor = hsv.ToRGB();

			return fillColor;
		}

		/// <summary>
		/// Gets the fill color of the specified morph polygon.  This method applies to free form mode and
		/// when the pattern mode is expanded out into morph polygons.
		/// </summary>		
		private Color GetFillColor(double intervalPos, IMorphPolygon morphPolygon)
		{
			Color fillColor = morphPolygon.FillColor.GetColorAt(intervalPos);

			double intervalPosFactor = intervalPos * 100;

			// Adjust the color for brightness setting
			HSV hsv = HSV.FromRGB(fillColor);
			hsv.V *= morphPolygon.FillBrightness.GetValue(intervalPosFactor) / 100;
			fillColor = hsv.ToRGB();

			return fillColor;
		}

		/// <summary>
		/// Converts from the model morph polygon data to the serialized morph polygon data.
		/// </summary>
		private void UpdateMorphSerializedData()
		{
			// Clear the collection of wave forms
			_data.MorphPolygonData.Clear();

			// Loop over the polygons in the model polygon collection
			foreach (IMorphPolygon morphPolygon in MorphPolygons.ToList())
			{
				// Create a new serialized polygon
				MorphPolygonData serializedPolygon = new MorphPolygonData();

				// Transfer the properties from the polygon model to the serialized polygon data
				serializedPolygon.HeadLength = morphPolygon.HeadLength;
				serializedPolygon.HeadDuration = morphPolygon.HeadDuration;
				serializedPolygon.Acceleration = morphPolygon.Acceleration;
				serializedPolygon.HeadColor = new ColorGradient(morphPolygon.HeadColor);
				serializedPolygon.TailColor =  new ColorGradient(morphPolygon.TailColor);
				serializedPolygon.Time = morphPolygon.Time;
				serializedPolygon.FillType = morphPolygon.FillType;
				serializedPolygon.FillColor = new ColorGradient(morphPolygon.FillColor);
				serializedPolygon.Label = morphPolygon.Label;
				serializedPolygon.StartOffset = morphPolygon.StartOffset;
				serializedPolygon.TailBrightness = new Curve(morphPolygon.TailBrightness);
				serializedPolygon.HeadBrightness = new Curve(morphPolygon.HeadBrightness);
				serializedPolygon.FillBrightness = new Curve(morphPolygon.FillBrightness);

				if (morphPolygon.Polygon != null)
				{
					serializedPolygon.Polygon = morphPolygon.Polygon.Clone();					
				}
				else if (morphPolygon.Line != null)
				{
					serializedPolygon.Line = morphPolygon.Line.Clone();						
				}
				else if (morphPolygon.Ellipse != null)
				{
					serializedPolygon.Ellipse = morphPolygon.Ellipse.Clone();
				}

				// Add the serialized polygon to the collection
				_data.MorphPolygonData.Add(serializedPolygon);
			}
		}
		
		/// <summary>
		/// Converts from the serialized polygon data to the model polygon data.
		/// </summary>		
		private void UpdatePolygonModel(MorphData morphData)
		{
			// Clear the model polygon collection
			MorphPolygons.Clear();

			// Loop over the polygons in the serialized effect data
			foreach (MorphPolygonData serializedPolygon in morphData.MorphPolygonData)
			{
				// Create a new morph polygon in the model
				IMorphPolygon morphPolygon = new MorphPolygon();

				// Transfer the properties from the serialized effect data to the morph polygon model
				morphPolygon.HeadLength = serializedPolygon.HeadLength;
				morphPolygon.HeadDuration = serializedPolygon.HeadDuration;
				morphPolygon.Acceleration = serializedPolygon.Acceleration;
				morphPolygon.HeadColor = new ColorGradient(serializedPolygon.HeadColor);
				morphPolygon.TailColor = new ColorGradient(serializedPolygon.TailColor);
				morphPolygon.Time = serializedPolygon.Time;
				morphPolygon.FillType = serializedPolygon.FillType;
				morphPolygon.FillColor = new ColorGradient(serializedPolygon.FillColor);
				morphPolygon.Label = serializedPolygon.Label;
				morphPolygon.StartOffset = serializedPolygon.StartOffset;
				morphPolygon.FillBrightness = new Curve(serializedPolygon.FillBrightness);
				morphPolygon.HeadBrightness = new Curve(serializedPolygon.HeadBrightness);
				morphPolygon.TailBrightness = new Curve(serializedPolygon.TailBrightness);

				if (serializedPolygon.Polygon != null)
				{
					morphPolygon.Polygon = serializedPolygon.Polygon.Clone();						
				}
				else if (serializedPolygon.Line != null)
				{
					morphPolygon.Line = serializedPolygon.Line.Clone();												
				}
				else if (serializedPolygon.Ellipse != null)
				{
					morphPolygon.Ellipse = serializedPolygon.Ellipse.Clone();
				}

				// Add the polygon to the effect's collection
				MorphPolygons.Add(morphPolygon);				
			}
		}
		
		/// <summary>
		/// Updates which morph polygon attributes are visible.
		/// </summary>		
		private void UpdatePolygonTypeAttributes(bool refresh)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(16)
				{					
					{ nameof(RepeatCount), PolygonType == PolygonType.Pattern },
					{ nameof(RepeatSkip), PolygonType == PolygonType.Pattern },
					{ nameof(Stagger), PolygonType == PolygonType.Pattern },
					{ nameof(FillColor), PolygonType == PolygonType.TimeBased },
					{ nameof(MorphPolygons), PolygonType == PolygonType.FreeForm },
					{ nameof(RepeatDirection), PolygonType == PolygonType.Pattern },

					{ nameof(HeadColor), PolygonType == PolygonType.Pattern },
					{ nameof(TailColor), PolygonType == PolygonType.Pattern },
					{ nameof(HeadDuration), PolygonType == PolygonType.Pattern },
					{ nameof(HeadLength), PolygonType == PolygonType.Pattern },
					{ nameof(Acceleration), PolygonType == PolygonType.Pattern },		
					{ nameof(FillPolygon), PolygonType == PolygonType.TimeBased },
					{ nameof(FillType), PolygonType == PolygonType.Pattern},

					{ nameof(HeadBrightness), PolygonType == PolygonType.Pattern},
					{ nameof(TailBrightness), PolygonType == PolygonType.Pattern},
					{ nameof(FillBrightness), PolygonType == PolygonType.TimeBased}
				};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Updates the browseable state of properties related to polygon fill type.
		/// </summary>
		private void UpdatePolygonFillTypeAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(7)
			{
				{nameof(HeadLength), FillType == PolygonFillType.Wipe },
				{nameof(HeadDuration), FillType == PolygonFillType.Wipe },
				{nameof(Acceleration), FillType == PolygonFillType.Wipe },
				{nameof(HeadColor), FillType == PolygonFillType.Wipe },
				{nameof(TailColor), FillType == PolygonFillType.Wipe },
				{nameof(Stagger), FillType == PolygonFillType.Wipe },
				{nameof(FillColor), (FillType == PolygonFillType.Solid || FillType == PolygonFillType.Outline) },				
			};
			SetBrowsable(propertyStates);
			
			TypeDescriptor.Refresh(this);			
		}
	}

	#endregion
}

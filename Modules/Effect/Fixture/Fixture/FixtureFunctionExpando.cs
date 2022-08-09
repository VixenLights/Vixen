using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
using Vixen.Data.Value;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using ZedGraph;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Maintains the properties of a fixture function for the catch-all fixture effect.
	/// </summary>
	[ExpandableObject]
	public class FixtureFunctionExpando : ExpandoObjectBase, IFixtureFunctionExpando
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureFunctionExpando()
		{
			// If the main fixture effect is exposing a fixture function to add then...
			if (FixtureModule.ActiveFunction != null)
			{
				// Retrieve the fixture functions from the parent effect
				FixtureFunctions = FixtureModule.FixtureFunctions;

				// Set the function identity based on the parent effect active function
				FunctionIdentity = FixtureModule.ActiveFunction.FunctionIdentity;

				// Set the function name based on the parent active function
				FunctionName = FixtureModule.ActiveFunction.Name;

				// Set the function type based on the parent active function
				FunctionType = FixtureModule.ActiveFunction.FunctionType;

				// Initialize the range to zero
				Range = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));

				// Set the index data to the index data from the parent active function
				IndexData = FixtureModule.ActiveFunction.IndexData;
				
				// If there is is index data then...
				if (IndexData.Count > 0)
				{
					// Select the first index value
					IndexValue = IndexData.FirstOrDefault().Name;
				}

				// Set the color index data to the color wheel data from the parent active function
				ColorWheelIndexData = FixtureModule.ActiveFunction.ColorWheelData;

				// If there is color wheel data then...
				if (ColorWheelIndexData.Count > 0)
				{
					// Set the selected color to the first color
					ColorIndexValue = ColorWheelIndexData.FirstOrDefault().Name;
				}

				// Initialize the color to white
				Color = new ColorGradient(System.Drawing.Color.White);

				// Initialize the timeline color
				TimelineColor = FixtureModule.ActiveFunction.TimelineColor;
			}
			// Otherwise just default the properties
			else
			{
				FunctionIdentity = FunctionIdentity.Custom;
				Range = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
				IndexData = new List<FixtureIndex>();
				ColorWheelIndexData = new List<FixtureColorWheel>();
				Color = new ColorGradient(System.Drawing.Color.White);
				TimelineColor = System.Drawing.Color.White;
			}

			InitAllAttributes();
		}

		#endregion

		#region Private Properties

		/// <summary>
		/// Indicates that the selected index is associated with a range / curve.
		/// </summary>
		private bool IndexMapsToRange { get; set; }

		#endregion

		#region IFixtureFunctionExpando

		private string _functionName;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[Browsable(false)]
		public string FunctionName 
		{ 
			get
			{
				return _functionName;
			}
			set
			{
				// Store off the fixture function name
				_functionName = value;

				// Function name will be null if the Functions drop down is blank
				if (_functionName != null)
				{
					// Find the fixture function by name
					FixtureFunction fixtureFunction = FixtureFunctions.SingleOrDefault(fn => fn.Name == FunctionName);

					if (fixtureFunction != null)
					{
						// Retrieve the index data from the function
						IndexData = fixtureFunction.IndexData;

						// Retrieve the color wheel data from the function
						ColorWheelIndexData = fixtureFunction.ColorWheelData;
					}
				}
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[Browsable(false)]
		public FunctionIdentity FunctionIdentity { get; set; }

		/// <summary>
		/// Backing field for the function type.
		/// </summary>
		private FixtureFunctionType _functionType;

		/// <summary>
		/// Refer interface documentation.
		/// </summary>
		[Browsable(false)]
		public FixtureFunctionType FunctionType
		{
			get
			{
				return _functionType;
			}
			set
			{
				_functionType = value;
				
				// Update the visibility of the controls
				UpdateFunctionTypeAttributes();
			}
		}

		/// <summary>
		/// Fixture functions associated with the fixture specification.
		/// </summary>
		[Browsable(false)]
		public List<App.Fixture.FixtureFunction> FixtureFunctions { get; set; }

		private string _indexValue;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[ProviderDisplayName(@"FixtureIndexValue")]
		[ProviderDescription(@"FixtureIndexValue")]
		[TypeConverter(typeof(FixtureIndexCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(1)]
		public string IndexValue
		{
			get
			{
				return _indexValue;
			}
			set
			{
				// Store off the selected index value
				_indexValue = value;

				// Default the index to NOT being associated with a curve
				// (Disable the curve control)
				IndexMapsToRange = false;

				// If the index value is NOT empty then...
				if (!string.IsNullOrEmpty(_indexValue))
				{
					// Find the fixture index object
					FixtureIndex fixtureIndex = IndexData.SingleOrDefault(item => item.Name == _indexValue);

					// If the index entry was not found then...
					if (fixtureIndex == null)
					{
						// Default to the first entry
						fixtureIndex = IndexData.First();
						_indexValue = fixtureIndex.Name;
					}

					// If the fixture index has a range (curve) then...
					if (fixtureIndex.UseCurve)
					{
						// Enable the range / curve
						IndexMapsToRange = true;
					}
				}

				// Update the visibility of the controls
				UpdateFunctionTypeAttributes();
			}
		}
		
		private string _colorIndexValue;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[ProviderDisplayName(@"FixtureColorIndex")]
		[ProviderDescription(@"FixtureColorIndex")]
		[TypeConverter(typeof(ColorIndexConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
		public string ColorIndexValue 
		{ 
			get
			{
				return _colorIndexValue;
			}
			set
			{
				// Save off the new color index value
				_colorIndexValue = value;

				// If the index value is NOT empty then...
				if (!string.IsNullOrEmpty(_colorIndexValue))
				{
					// Find the color wheel index object
					FixtureColorWheel fixtureColorWheel = ColorWheelIndexData.SingleOrDefault(item => item.Name == _colorIndexValue);

					// If the color wheel entry is NOT found then...
					if (fixtureColorWheel == null)
					{
						// Default the color to the first entry
						fixtureColorWheel = ColorWheelIndexData.First();
						_colorIndexValue = fixtureColorWheel.Name;
					}

					// If the fixture index has a range (curve) then...
					if (fixtureColorWheel.UseCurve)
					{
						// Enable the range / curve
						IndexMapsToRange = true;
					}

					// Update the visibility of the controls
					UpdateFunctionTypeAttributes();
				}
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[ProviderDisplayName(@"FixtureRange")]
		[ProviderDescription(@"FixtureRange")]
		[PropertyOrder(3)]
		public Curve Range { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[ProviderDisplayName(@"FixtureColor")]
		[ProviderDescription(@"FixtureColor")]
		[PropertyOrder(15)]
		public ColorGradient Color { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[Browsable(false)]
		public Color TimelineColor { get; set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Index data associated with the fixture function.
		/// </summary>
		[Browsable(false)]
		public List<FixtureIndex> IndexData { get; private set; }		

		/// <summary>
		/// Color wheel data associated with the fixture function.
		/// </summary>
		[Browsable(false)]
		public List<FixtureColorWheel> ColorWheelIndexData { get; private set; }

		#endregion
								
		#region Public Methods

		/// <summary>
		/// Returns the function name.
		/// </summary>
		/// <returns>Function name</returns>
		public override string ToString()
		{
			return FunctionName;
		}
		
		/// <summary>
		/// Returns a clone of the fixture function.
		/// </summary>		
		public IFixtureFunctionExpando CreateInstanceForClone()
		{
			IFixtureFunctionExpando result = new FixtureFunctionExpando()
			{
				IndexData = IndexData,
				ColorWheelIndexData = ColorWheelIndexData,
				FunctionIdentity = FunctionIdentity,
				Range = new Curve(Range),
				FunctionName = FunctionName,
				FunctionType = FunctionType,
				IndexValue = IndexValue,
				ColorIndexValue  = ColorIndexValue,				
				Color = new ColorGradient(Color),		
				TimelineColor = TimelineColor,	
			};

			return result;
		}
		
		#endregion

		#region Implementation of ICloneable

		/// <inheritdoc />
		public object Clone()
		{
			return CreateInstanceForClone();
		}
						
		#endregion

		#region Private Methods
		
		/// <summary>
		/// Updates the browseable state of fixture function properties.
		/// </summary>
		private void UpdateFunctionTypeAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(7)
			{
				{ nameof(IndexValue), FunctionType == FixtureFunctionType.Indexed },
				{ nameof(Range), FunctionType == FixtureFunctionType.Range || 
				                 IndexMapsToRange || 								 
								 FunctionType == FixtureFunctionType.RGBWColor
				},
				{ nameof(ColorIndexValue), FunctionType == FixtureFunctionType.ColorWheel },				
				{ nameof(Color), (FunctionType == FixtureFunctionType.RGBColor || FunctionType == FixtureFunctionType.RGBWColor) },
			};
			SetBrowsable(propertyStates);
		}
		
		/// <summary>
		/// Updates the visibility of the fixture function properties.
		/// </summary>
		private void InitAllAttributes()
		{
			UpdateFunctionTypeAttributes();						
			TypeDescriptor.Refresh(this);
		}

		#endregion
	}
}

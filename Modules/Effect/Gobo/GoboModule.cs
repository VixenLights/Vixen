using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.App.FixtureSpecificationManager;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.IntelligentFixture;

namespace VixenModules.Effect.Gobo
{
	/// <summary>
	/// Intelligent fixture Gobo effect.
	/// </summary>
	public class GoboModule : FixtureIndexEffectBase<GoboData>
	{       
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public GoboModule() :
			// Give the base class the online help URL
			base("http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/intelligent-fixture/gobo/")
		{						
		}

		#endregion

		#region Public Effect Properties

		/// <summary>
		/// Gets or sets the selected gobo for the fixture(s).
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Function")]
		[ProviderDescription(@"Function")]
		[PropertyEditor("SelectionEditor")]
		[TypeConverter(typeof(GoboFunctionNameConverter))]
		[PropertyOrder(1)]
		public string GoboFunctionName
		{
			get => Data.GoboFunctionName;
			set
			{
				Data.GoboFunctionName = value;
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets the selected gobo index value for the fixture(s).
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Gobo")]
		[ProviderDescription(@"Gobo")]
		[TypeConverter(typeof(GoboFixtureIndexCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
		public string GoboIndexValue
		{
			get => Data.GoboIndexValue;
			set
			{
				// Save off the gobo value
				Data.GoboIndexValue = value;

				// Update whether the index item supports a curve
				UpdateSupportsCurve(value, GoboFunctionName, FunctionIdentity.Gobo);
								
				// Update the effect property visibility status
				UpdateAttributes();

				// Mark the effect dirty
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets the gobo rotation curve.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"RotationSpeed")]
		[ProviderDescription(@"RotationSpeed")]
		[PropertyOrder(3)]
		public Curve GoboCurve
		{
			get => Data.GoboCurve;
			set
			{
				Data.GoboCurve = value;
				IsDirty = true;
			}
		}

		#endregion
		
		#region Protected Methods

		/// <summary>
		/// Updates the visibility of gobo attributes.
		/// </summary>
		protected override void UpdateAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{ nameof(GoboIndexValue), SupportsIndexFunction},
				{ nameof(GoboCurve), IndexHasCurve},
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		
		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override void PreRenderInternal(CancellationTokenSource cancellationToken = null)
		{
			PreRenderInternal(
				GoboFunctionName,
				GoboIndexValue,
				GoboCurve,
				FunctionIdentity.Gobo,
				cancellationToken);			
		}
												
		/// <summary>
		/// Determines if the fixture supports the ability to zoom.
		/// </summary>
		protected override void UpdateFixtureCapabilities()
		{
			string functionName = GoboFunctionName;
			string indexValue = GoboIndexValue;
			UpdateFixtureCapabilities(ref functionName, ref indexValue, FunctionIdentity.Gobo);
			GoboFunctionName = functionName;
			GoboIndexValue = GoboIndexValue;
		}
				
		#endregion
		
		#region Public Methods

		/// <summary>
		/// Gets the gobo function names associated with the target nodes.
		/// </summary>
		/// <returns>Gobo function names associated with the target nodes</returns>
		public List<string> GetGoboFunctionNames()
		{
			return GetMatchingFunctionNames(FunctionIdentity.Gobo);
		}

		#endregion

		#region Public Overrides of BaseEffect		

		/// <summary>
		/// Generates the visual representation of the effect on the timeline.
		/// </summary>
		/// <param name="graphics">Graphics context</param>
		/// <param name="clipRectangle">Clipping rectangle of the effect on the timeline</param>
		public override void GenerateVisualRepresentation(Graphics graphics, Rectangle clipRectangle)
		{
			// Create a flag indicating the visual representation is using a gobo image
			bool useGoboImage = false;

			// If a gobo value has been selected then...
			if (!string.IsNullOrEmpty(GoboIndexValue))
			{
				// Get the first function associated with the gobo index
				FixtureIndex fixtureIndex = (FixtureIndex)GetFunctionIndex(GoboIndexValue, GoboFunctionName, FunctionIdentity.Gobo);

				// If there an image associated with gobo index then...
				if (!string.IsNullOrEmpty(fixtureIndex.Image))
				{
					// Create the path to the gobo image
					string imagePath = Path.Combine(FixtureSpecificationManager.Instance().GetGoboImageDirectory(), fixtureIndex.Image);

					// If the gobo image exists then...
					if (File.Exists(imagePath))
					{
						// Remember we displaying a gobo image
						useGoboImage = true;

						// Load the image from the file system
						Image sourceImage = Image.FromFile(imagePath);

						// Make the width of the image the same as the height
						int width = clipRectangle.Height;

						// Repeat the image to fill the entire effect timeline rectangle						
						for (int totalWidth = 0; totalWidth < clipRectangle.Width; totalWidth += width)
						{
							// Draw the gobo image
							graphics.DrawImage(sourceImage, totalWidth, clipRectangle.Top, width, clipRectangle.Height);
						}
					}
				}
			}
			
			// If not using a gobo image then...
			if (!useGoboImage)
			{
				// Indicate the effect is the gobo effect
				DrawText(graphics, clipRectangle, Color.Red, "Gobo", 0, clipRectangle.Y, clipRectangle.Height);
			}			
		}

		#endregion	
	}
}
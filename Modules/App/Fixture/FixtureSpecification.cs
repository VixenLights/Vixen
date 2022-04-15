using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Data.Value;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains meta-data (channels and functions) of an intelligent fixture.
	/// </summary>
    [DataContract]
	public class FixtureSpecification 
	{
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public FixtureSpecification()
		{
			// Create the channel definition collection
			ChannelDefinitions = new List<FixtureChannel>();
			
			// Create the function definition collection
			FunctionDefinitions = new List<FixtureFunction>();			

			// Initialize who created the fixture specification
			CreatedBy = Environment.UserName;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Name of the fixture.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Company that produces or sells the fixture.
		/// </summary>
		[DataMember]
		public string Manufacturer { get; set; }

		/// <summary>
		/// Name of the person that created the fixture profile.
		/// </summary>
		[DataMember]
		public string CreatedBy { get; set; }

		/// <summary>
		/// Revision information about the fixture profile.
		/// </summary>
		[DataMember]
		public string Revision { get; set; }
		
		/// <summary>
		/// Collection of channel definitions.
		/// </summary>
		[DataMember]
		public List<FixtureChannel> ChannelDefinitions { get; set; }

		/// <summary>
		/// Collection of function definitions.
		/// </summary>
		[DataMember]
		public List<FixtureFunction> FunctionDefinitions { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the filename of the fixture.
		/// This method removes any special characters that are not allowed by Windows as a file name.
		/// </summary>
		/// <returns>File name of the fixture</returns>
		public string GetFileName()
		{
			// < (less than)
			string fileName = Name.Replace('<', '(');

			// > (greater than)
			fileName = fileName.Replace('>', '(');

			// : (colon - sometimes works, but is actually NTFS Alternate Data Streams)
			fileName = fileName.Replace(':', '_');

			// " (double quote)
			fileName = fileName.Replace('"', '_');

			// / (forward slash)
			fileName = fileName.Replace('/', '_');

			// \ (backslash)
			fileName = fileName.Replace('\\', '_');

			// | (vertical bar or pipe)
			fileName = fileName.Replace('|', '_');

			// ? (question mark)
			fileName = fileName.Replace('?', '_');

			// * (asterisk)
			fileName = fileName.Replace('*', '_');

			// Append the .xml file extension
			fileName += ".xml";

			return fileName;
		}

		/// <summary>
		/// Clones the fixture specification.
		/// </summary>
		/// <returns>Clone of the fixture specification</returns>
		public FixtureSpecification CreateInstanceForClone()
		{
			// Create a new fixture specification
			FixtureSpecification clone = new FixtureSpecification();

			// Clone the name of the fixture
			clone.Name = Name;

			// Copy the manufacturer
			clone.Manufacturer = Manufacturer;

			// Copy the name of the user that created the fixture profile
			clone.CreatedBy = CreatedBy;

			// Copy the revision information 
			clone.Revision = Revision;
			
			// Loop over the channel definitions
			foreach (FixtureChannel channel in ChannelDefinitions)
			{
				// Clone the channel definition
				clone.ChannelDefinitions.Add(channel.CreateInstanceForClone());
			}

			// Loop over the function definitions
			foreach (FixtureFunction function in FunctionDefinitions)
			{
				// Clone the function definition
				clone.FunctionDefinitions.Add(function.CreateInstanceForClone());
			}

			return clone;
		}

		/// <summary>
		/// Returns true if the specified function name is supported by the fixture.
		/// </summary>
		/// <param name="functionName">Name of the function to check</param>
		/// <returns>True if the function is supported</returns>
		public bool SupportsFunction(string functionName)
		{
			// Return whether the function is supported
			return FunctionDefinitions.Any(item => item.Name == functionName);
		}

		/// <summary>
		/// Adds a function to the fixture specification.
		/// </summary>
		/// <param name="name">Name of the function</param>
		/// <param name="functionType">Type of the function</param>
		/// <param name="identity">Preview identity of the function</param>
		/// <returns></returns>
		public FixtureFunction AddFunctionType(
			string name,
			FixtureFunctionType functionType,
			FunctionIdentity identity)
		{
			// Create the new function
			FixtureFunction function = new FixtureFunction();

			// Configure the name on the function
			function.Name = name;

			// Configure the function type
			function.FunctionType = functionType;

			// Configure the function identity
			function.FunctionIdentity = identity;

			// Add the function to the fixture specification
			FunctionDefinitions.Add(function);

			return function;
		}

		/// <summary>
		/// Initialize the built in functions.
		/// </summary>
		public void InitializeBuiltInFunctions()
		{
			// Clear the function definition collection
			FunctionDefinitions.Clear();

			// Add the pan function 
			FixtureFunction pan = AddFunctionType(
				"Pan",
				FixtureFunctionType.Range,
				FunctionIdentity.Pan);
			pan.RotationLimits = new FixtureRotationLimits();
			
			// Add the tilt function 
			FixtureFunction tilt = AddFunctionType(
				"Tilt",
				FixtureFunctionType.Range,
				FunctionIdentity.Tilt);
			tilt.RotationLimits = new FixtureRotationLimits();
			
			// Add an empty color wheel function
			AddFunctionType(
				"Color Wheel",
				FixtureFunctionType.ColorWheel,
				FunctionIdentity.Custom);

			// Add a dimmer function
			AddFunctionType(
				"Dimmer",
				FixtureFunctionType.Range,
				FunctionIdentity.Dim);

			// Add a color mixing function
			AddFunctionType(
				"Color",
				FixtureFunctionType.RGBWColor,
				FunctionIdentity.Custom);

			// Add zoom function
			AddFunctionType(
				"Zoom",
				FixtureFunctionType.Range,
				FunctionIdentity.Zoom);

			// Add shutter function
			AddFunctionType(
				"Shutter",
				FixtureFunctionType.Indexed,
				FunctionIdentity.Shutter);

			// Add a None function so that channels can be included in the specification but generally ignored
			AddFunctionType(
				"None",
				FixtureFunctionType.None,
				FunctionIdentity.Custom);
		}
		
		/// <summary>
		/// Returns true if the fixture is color mixing (RGB or RGBW).
		/// </summary>
		/// <returns></returns>
		public bool IsColorMixing()
        {
			// Default to NOT color mixing
			bool colorMixing = false;

			// Loop over all the channel definitions
			foreach(FixtureChannel channel in ChannelDefinitions)
            {
				// Retrieve the associated fixture function
				FixtureFunction fnc = FunctionDefinitions.Single(function => function.Name == channel.Function);

				// If the function is either RGB or RGBW then...
				if (fnc.FunctionType == FixtureFunctionType.RGBColor ||
				    fnc.FunctionType == FixtureFunctionType.RGBWColor)
                {
					// The fixture supports color mixing
					colorMixing = true;
					break;
                }
			}
			
			return colorMixing;
        }

		/// <summary>
		/// Returns true if the fixture contains a color wheel.
		/// </summary>
		/// <returns></returns>
		public bool ContainsColorWheel()
		{
			// Default to NOT containing a color wheel
			bool containsColorWheel = false;

			// Loop over all the channel definitions
			foreach (FixtureChannel channel in ChannelDefinitions)
			{
				// Retrieve the associated fixture function
				FixtureFunction fnc = FunctionDefinitions.Single(function => function.Name == channel.Function);
				
				// If the function is a color wheel then...
				if (fnc.FunctionType == FixtureFunctionType.ColorWheel)					
				{
					// Indicate the fixture contains a color wheel
					containsColorWheel = true;
					break;
				}
			}

			return containsColorWheel;
		}

		/// <summary>
		/// Returns true if the fixture supports RGBW.
		/// </summary>
		/// <returns>True if the fixture supports RGBW</returns>
		public bool IsRGBW()
		{
			// Default to NOT RGBW
			bool isRGBW = false;

			// Loop over the channels in the specification
			foreach (FixtureChannel channel in ChannelDefinitions)
			{
				// Retrieve the associated function
				FixtureFunction fnc = FunctionDefinitions.Single(function => function.Name == channel.Function);

				// If the function is RGBW then...
				if (fnc.FunctionType == FixtureFunctionType.RGBWColor)
				{
					// Indicate the fixture supports RGBW
					isRGBW = true;
					break;
				}
			}

			return isRGBW;
		}
		
		#endregion		
	}
}

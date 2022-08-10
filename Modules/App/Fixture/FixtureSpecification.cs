using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains meta-data (channels and functions) of an intelligent fixture.
	/// </summary>
    [DataContract]
	public class FixtureSpecification : IDataModel 
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

			// Default the revision to 1.0
			Revision = "1.0";

			// Set the schema version
			// This version value is also located in <c>ObjectVersion</c> class
			version = "1";
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Version of the FixtureSpecification schema.
		/// </summary>		
		[DataMember, XmlAttribute]
		public string version { get; set; } 

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

			// Copy this object's state into the clone
			clone.CopyInto(this);

			// Return the cloned object
			return clone;
		}

		/// <summary>
		/// Returns true if the specified function name is supported by the fixture.
		/// </summary>
		/// <param name="functionName">Name of the function to check</param>
		/// <returns>True if the function is supported</returns>
		public bool SupportsFunction(string functionName)
		{
			// Return whether the function is defined and
			// referenced on at least one channel
			return FunctionDefinitions.Any(item => item.Name == functionName) &&
				   ChannelDefinitions.Any(channel => channel.Function == functionName);
		}
		
		/// <summary>
		/// Returns the fixture function with the specified name and identity.
		/// </summary>
		/// <param name="functionName">Function name to find</param>
		/// <param name="functionIdentity">Function identity to find</param>
		/// <returns>Fixture function with the specified name and identity</returns>
		public FixtureFunction GetInUseFunction(string functionName, FunctionIdentity functionIdentity)
		{
			// Default the function to null indicating it was not found
			FixtureFunction function = null;

			// If the function is referenced on one of the fixture's channels then...
			if (IsFunctionUsed(functionName, functionIdentity))
			{
				// Find the fixture function with the specified name and identity
				function = GetFunction(functionName, functionIdentity);
			}

			return function;
		}

		/// <summary>
		/// Returns true if the specified function name and identity is used on the fixture.
		/// </summary>
		/// <param name="functionName">Name of the function</param>
		/// <param name="functionIdentity">Identity of the function</param>
		/// <returns></returns>
		public bool IsFunctionUsed(string functionName, FunctionIdentity functionIdentity)
		{
			// Default to the function not being used
			bool isUsed = false;

			// Find the function associated with the effect
			FixtureFunction func = GetFunction(functionName, functionIdentity);

			// If the function was found then...
			if (func != null)
			{
				// If the function is mapped to a channel then...
				isUsed = ChannelDefinitions.Any(channel => channel.Function == functionName);
			}

			// Returns whether the function is referenced on one of the fixture's channels
			return isUsed;
		}
		
		/// <summary>
		/// Returns true if the specified function identity is supported by the fixture.
		/// </summary>
		/// <param name="functionName">Name of the function to check</param>
		/// <returns>True if the function is supported</returns>
		public bool SupportsFunction(FunctionIdentity functionIdentity)
		{
			// Default to not supporting the function
			bool supported = false;

			// Retrieve the first function that matches the identity
			FixtureFunction function = FunctionDefinitions.FirstOrDefault(item => item.FunctionIdentity == functionIdentity);
			
			// If the function was found then...
			if (function != null)
			{
				// Check to make sure the function is used on at least one channel
				supported = ChannelDefinitions.Any(channel => channel.Function == function.Name);
			}

			// Return whether the function is supported
			return supported;
		}

		/// <summary>
		/// Adds a function to the fixture specification.
		/// </summary>
		/// <param name="name">Name of the function</param>
		/// <param name="functionType">Type of the function</param>
		/// <param name="identity">Preview identity of the function</param>		
		/// <param name="timelineColor">Color to use on the timeline for some effects</param>
		/// <returns>New fixture function</returns>
		public FixtureFunction AddFunctionType(
			string name,
			FixtureFunctionType functionType,
			FunctionIdentity identity,
			Color timelineColor)
		{
			// Create the new function
			FixtureFunction function = new FixtureFunction();

			// Configure the name on the function
			function.Name = name;

			// Configure the function type
			function.FunctionType = functionType;

			// Configure the function identity
			function.FunctionIdentity = identity;

			// Configure the timeline color
			function.TimelineColor = timelineColor;

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
				FunctionIdentity.Pan,
				Color.Red);
			pan.RotationLimits = new FixtureRotationLimits();
			
			// Add the tilt function 
			FixtureFunction tilt = AddFunctionType(
				"Tilt",
				FixtureFunctionType.Range,
				FunctionIdentity.Tilt,
				Color.Green);
				tilt.RotationLimits = new FixtureRotationLimits();
			
			// Add an empty color wheel function
			AddFunctionType(
				"Color Wheel",
				FixtureFunctionType.ColorWheel,
				FunctionIdentity.Custom,
				Color.White);

			// Add a dimmer function
			AddFunctionType(
				"Dimmer",
				FixtureFunctionType.Range,
				FunctionIdentity.Dim,
				Color.Purple);

			// Add a color mixing function
			AddFunctionType(
				"Color",
				FixtureFunctionType.RGBWColor,
				FunctionIdentity.Custom,
				Color.DarkGray);

			// Add zoom function
			AddFunctionType(
				"Zoom",
				FixtureFunctionType.Range,
				FunctionIdentity.Zoom,
				Color.Blue);

			// Add shutter function
			AddFunctionType(
				"Shutter",
				FixtureFunctionType.Indexed,
				FunctionIdentity.Shutter,
				Color.Black);

			// Add gobo wheel function
			AddFunctionType(
				"Gobo Wheel",
				FixtureFunctionType.Indexed,
				FunctionIdentity.Gobo,
				Color.Pink);

			// Add prism function
			AddFunctionType(
				"Open Close Prism",
				FixtureFunctionType.Indexed,
				FunctionIdentity.OpenClosePrism,
				Color.Orange);

			// Add (rotating) prism function
			AddFunctionType(
				"Prism",
				FixtureFunctionType.Indexed,
				FunctionIdentity.Prism,
				Color.Yellow);

			// Add frost function
			AddFunctionType(
				"Frost",
				FixtureFunctionType.Range,
				FunctionIdentity.Frost,
				Color.LightBlue);

			// Add a None function so that channels can be included in the specification but generally ignored
			AddFunctionType(
				"None",
				FixtureFunctionType.None,
				FunctionIdentity.Custom,
				Color.Transparent);
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

		#region Private Methods

		/// <summary>
		/// Returns the function with the specified name and identity.
		/// </summary>
		/// <param name="functionName">Function name to search for</param>
		/// <param name="functionIdentity">Function identity to search for</param>
		/// <returns></returns>
		private FixtureFunction GetFunction(string functionName, FunctionIdentity functionIdentity)
		{
			// Find the fixture function with the specified name and identity
			return FunctionDefinitions.SingleOrDefault(
				function => (function.FunctionIdentity == functionIdentity &&
				function.Name == functionName));
		}

		#endregion

		#region IDataModel

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void CopyInto(IDataModel source)
		{
			// Get FixtureSpecification reference to the source object
			FixtureSpecification src = (FixtureSpecification)source;
			
			// Copy the name of the fixture
			Name = src.Name;

			// Copy the manufacturer
			Manufacturer = src.Manufacturer;

			// Copy the name of the user that created the fixture profile
			CreatedBy = src.CreatedBy;

			// Copy the revision information 
			Revision = src.Revision;

			// Loop over the channel definitions
			foreach (FixtureChannel channel in src.ChannelDefinitions)
			{
				// Clone the channel definition
				ChannelDefinitions.Add(channel.CreateInstanceForClone());
			}

			// Loop over the function definitions
			foreach (FixtureFunction function in src.FunctionDefinitions)
			{
				// Clone the function definition
				FunctionDefinitions.Add(function.CreateInstanceForClone());
			}		
		}

		#endregion
	}
}

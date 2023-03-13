using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Vixen.Sys;

namespace Vixen.IO.Xml.Sequence
{
	/// <summary>
	/// Converts from XML (XElement) to a FixtureSpecification object.
	/// </summary>
	/// <typeparam name="T">Type of the FixtureSpecification</typeparam>
	internal class FixtureSpecificationXElementWriter<T> : IObjectContentWriter
		where T : IDataModel
	{			
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="filePath">Path to the fixture specification</param>		
		public FixtureSpecificationXElementWriter(string filePath)
		{
			// If the file path is NOT valid then...
			if (string.IsNullOrWhiteSpace(filePath))
			{
				// Throw an argument exception
				throw new ArgumentNullException("filePath");
			}
		}

		#endregion

		#region IObjectContentWriter

		/// <summary>
		/// Assigns the specified XML content to properties on the specified fixture specification.
		/// </summary>
		/// <param name="content">XElement content to set on the specified fixture specification</param>
		/// <param name="obj">Fixture specification to write into</param>		
		public void WriteContentToObject(object content, object obj)
		{
			// Cast the arguments to their strong typed equivalents
			XElement xmlContent = content as XElement;
			IDataModel fixtureSpecification = obj as IDataModel;

			// Type check / error check the input arguments
			if (xmlContent == null)
			{
				throw new InvalidOperationException("Content must be an XElement.");
			}
			if (fixtureSpecification == null)
			{
				throw new InvalidOperationException("Object must be an IDataModel.");
			}

			// Copy the XML contents into the fixture specification
			fixtureSpecification.CopyInto(ConvertXElementToFixtureSpecification(xmlContent));			
		}

		/// <summary>
		/// Get the version of the fixture specification file.
		/// </summary>
		/// <param name="content">Fixture specification XElement to extract the version from</param>
		/// <returns>Version number of the fixture specification</returns>		
		public int GetContentVersion(object content)
		{
			// Cast the argument to its strong typed equivalent
			XElement xmlContent = content as XElement;

			// Type check the input arguments
			if (xmlContent == null)
			{
				throw new InvalidOperationException("Content must be an XElement.");
			}

			// Extract the version from the XML (XElement)
			return  XmlRootAttributeVersion.GetVersion(xmlContent);			
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Converts the specified XElement into a FixtureSpecification object.
		/// </summary>
		/// <param name="xmlContent">XElement content of a fixture specification</param>
		/// <returns>Populated FixtureSpecification</returns>
		private T ConvertXElementToFixtureSpecification(XElement xmlContent)
		{			
			// 
			using (XmlReader xmlReader = xmlContent.CreateReader())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));		
				T fixtureSpecification = (T)serializer.Deserialize(xmlReader);
				
				return fixtureSpecification;
			}
		}

		#endregion
	}
}
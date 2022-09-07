using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Vixen.Extensions;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.Fixture;

namespace VixenModules.App.FixtureSpecificationManager
{
	/// <summary>
	/// Manages a repository of intelligent fixtures.
	/// This component is a singleton.
	/// </summary>
	public class FixtureSpecificationManager : IFixtureSpecificationManager
	{
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        private FixtureSpecificationManager()
		{			
			// Create the collection of fixture specifications
			FixtureSpecifications = new List<FixtureSpecification>();	
			
			// Initialize the profile path
			_profilePath = Paths.DataRootPath;

			// Load the fixtures from the fixture directory
			LoadFixtureSpecifications();
		}

        #endregion

        #region Private Static Fields

		/// <summary>
		/// Maintains a single instance of the component.
		/// </summary>
        private static IFixtureSpecificationManager _instance;

        #endregion

        #region Private Static Fields

		/// <summary>
		/// Active profile path used to retrieve the fixture specifications.
		/// </summary>
        private static string _profilePath;

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the complete path to the fixture specification directory.
		/// </summary>
		/// <returns>Fixture specification directory</returns>
		private string GetFixtureDirectory()
		{
			// Create the path to the fixtures
			return _profilePath + @"\Fixtures\";					
		}

		/// <summary>
		/// Refer to <see cref="IFixtureSpecificationManager"/> documentation.
		/// </summary>
		private void LoadFixtureSpecifications()
		{
			// If the fixture directory does NOT exist then...
			if (!Directory.Exists(GetFixtureDirectory()))
			{
				// Create the fixture directory
				Directory.CreateDirectory(GetFixtureDirectory());
			}

			// If the fixture gobo image directory does NOT exist then...
			if (!Directory.Exists(GetGoboImageDirectory()))
			{
				// Create the fixture gobo image directory
				Directory.CreateDirectory(GetGoboImageDirectory());
			}

			// Create a directory info object pointing at the fixture directory
			DirectoryInfo directoryInfo = new DirectoryInfo(GetFixtureDirectory());

			// Get all the XML files in the fixture directory
			FileInfo[] specificationFiles = directoryInfo.GetFiles("*.xml");

			// Loop over all the fixture specification in the folder
			foreach (FileInfo fileInfo in specificationFiles)
			{								
				try
				{
					// Call the Deserialize method to load the fixture specification
					FixtureSpecification fixture = FixtureSpecificationService.Load<FixtureSpecification>(fileInfo.FullName);

					//(FixtureSpecification)serializer.Deserialize(reader);
					FixtureSpecifications.Add(fixture);
				}
				catch (Exception e)
				{
					// If we encounter a malformed XML file just ignore it and log an error
					Logger logging = LogManager.GetCurrentClassLogger();
					logging.Error(e, fileInfo.FullName + "is malformed!");						
				}							
			}
		

			//
			// This commented out code is used to support development and testing of the intelligent fixtures.
			// The classes below create fixture specifications to avoid having to perform manual data entry
			// or XML surgery after a schema change.
			//
			//if (!FixtureSpecifications.Any(fixture => fixture.Name == ADJHydroBeamX1Data.GetFixture().Name))
			//{
			//	FixtureSpecifications.Add(ADJHydroBeamX1Data.GetFixture());
			//}
			//
			//if (!FixtureSpecifications.Any(fixture => fixture.Name == ADJHydroWashX7_17FixtureData.GetFixture().Name))
			//{
			//	FixtureSpecifications.Add(ADJHydroWashX7_17FixtureData.GetFixture());

			// Sort the fixtures by name
			Sort();
		}

		/// <summary>
		/// Sorts the fixture collection by name.
		/// </summary>
		private void Sort()
		{
			// Sort the fixture by name
			IList<FixtureSpecification> sortedCollection = FixtureSpecifications.OrderBy(item => item.Name).ToList();

			// Clear the collection
			FixtureSpecifications.Clear();

			// Add back the sorted items
			FixtureSpecifications = sortedCollection;
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Returns the single instance of the fixture specification manager.
		/// </summary>
		/// <returns>Single instance of the fixture specification manager</returns>
		public static IFixtureSpecificationManager Instance()
		{
			// If an instance has not yet been created then...
			if (_instance == null)
			{
				// Create the singleton instance
				_instance = new FixtureSpecificationManager();
			}

			return _instance;
		}

		#endregion
		
		#region IFixtureSpecificationManager

		/// <summary>
		/// Refer to <see cref="IFixtureSpecificationManager"/> documentation.
		/// </summary>
		public IList<FixtureSpecification> FixtureSpecifications { get; private set; }
		
		/// <summary>
		/// Refer to <see cref="IFixtureSpecificationManager"/> documentation.
		/// </summary>
		public void Save(FixtureSpecification fixture)
        {
			// Create the XML settings
			XmlWriterSettings xmlsettings = new XmlWriterSettings()
			{
				Indent = true,
				IndentChars = "\t",
			};

			// Get the path to the fixture specification directory
			string pathName = GetFixtureDirectory();

			// Add the file name to the path
			pathName += fixture.GetFileName();

			// Create an XML writer
			using (XmlWriter xmlWriter = XmlWriter.Create(pathName, xmlsettings))
			{
				// Create an XML serializer
				XmlSerializer serializer = new XmlSerializer(typeof(FixtureSpecification));

				// Save the fixture to the XML file
				serializer.Serialize(xmlWriter, fixture);
			}

			// Attempt to find the fixture in the collection
			FixtureSpecification cachedItem = FixtureSpecifications.SingleOrDefault(item => item.Name == fixture.Name);

			// If the fixture was found then...
			if (cachedItem != null)
			{
				// Remove the old copy from the collection
				FixtureSpecifications.Remove(cachedItem);
			}

			// Add the new fixture to the collection
			FixtureSpecifications.Add(fixture);

			// Sort the fixtures by name
			Sort();
		}

		/// <summary>
		/// Refer to <see cref="IFixtureSpecificationManager"/> documentation.
		/// </summary>		
		public IList<string> GetGoboImages()
		{
			// Create the collection of gobo images
			List<string> goboImages = new List<string>();

			// Get the list of files in the gobo image directory
			string[] images = Directory.GetFiles(GetGoboImageDirectory());

			// Loop over the image files
			foreach(string imagePath in images)
			{
				// Extract just the filename from the image path
				goboImages.Add(Path.GetFileName(imagePath));
			}

			// Return the collection of gobo images
			return goboImages;
		}

		/// <summary>
		/// Refer to <see cref="IFixtureSpecificationManager"/> documentation.
		/// </summary>		
		public string GetGoboImageDirectory()
		{
			// Construct the path to the gobo images
			return GetFixtureDirectory() + @"\Images\";
		}

		#endregion
	}
}

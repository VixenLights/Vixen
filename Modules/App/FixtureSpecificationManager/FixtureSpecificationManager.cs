using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
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

			// Create a directory info object pointing at the fixture directory
			DirectoryInfo directoryInfo = new DirectoryInfo(GetFixtureDirectory());

			// Get all the XML files in the fixture directory
			FileInfo[] specificationFiles = directoryInfo.GetFiles("*.xml");

			// Loop over all the fixture specification in the folder
			foreach (FileInfo fileInfo in specificationFiles)
			{
				// Create an XML serializer for a fixture specification
				XmlSerializer serializer = new XmlSerializer(typeof(FixtureSpecification));

				// Create a file reader for the fixture specification
				using (Stream reader = new FileStream(fileInfo.FullName, FileMode.Open))
				{
					try
					{
						// Call the Deserialize method to load the fixture specification
						FixtureSpecification fixture = (FixtureSpecification)serializer.Deserialize(reader);
						FixtureSpecifications.Add(fixture);
					}
					catch (Exception e)
					{
						// If we encounter a malformed XML file just ignore it and log an error
						Logger logging = LogManager.GetCurrentClassLogger();
						logging.Error(e, fileInfo.FullName + "is malformed!");						
					}				
				}
			}

			// TODO: Remove development code!
			if (!FixtureSpecifications.Any(fixture => fixture.Name == ADJHydroBeamX1Data.GetFixture().Name))
			{
				FixtureSpecifications.Add(ADJHydroBeamX1Data.GetFixture());
			}

			if (!FixtureSpecifications.Any(fixture => fixture.Name == ADJHydroWashX7_17FixtureData.GetFixture().Name))
			{
				FixtureSpecifications.Add(ADJHydroWashX7_17FixtureData.GetFixture());
			}			
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
		}

		/// <summary>
		/// Refer to <see cref="IFixtureSpecificationManager"/> documentation.
		/// </summary>
		public void InitializeProfilePath(string profilePath)
        {
			// Save off the active profile path
			_profilePath = profilePath;

			// Load the fixtures from the fixture directory
			LoadFixtureSpecifications();
		}

		#endregion
	}
}

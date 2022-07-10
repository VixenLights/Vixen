using Vixen.IO.Factory;

namespace Vixen.IO.Loader
{
	/// <summary>
	/// Maintains a Fixture Specification loader.
	/// </summary>
	/// <typeparam name="T">Type of the FixtureSpecification</typeparam>
	public class FixtureSpecificationLoader<T> : IObjectLoader<T> where T : class, new()
	{
		#region IObjectLoader<T>

		/// <summary>
		/// Loads the FixtureSpecification from the specified file.
		/// </summary>
		/// <param name="filePath">Path to the fixture specification</param>
		/// <returns>Fixture specification contained in the file</returns>
		public T LoadFromFile(string filePath)
		{			
			// Create the file reader
			IFileReader fileReader = FileReaderFactory.Instance.CreateFileReader();

			// Create the object that converts the XElement into a FixtureSpecification object
			IObjectContentWriter contentWriter = ObjectContentWriterFactory.Instance.CreateFixtureSpecificationContentWriter<T>(filePath);

			// Content migrator responsible for updating the XML for schema changes
			IContentMigrator contentMigrator = ContentMigratorFactory.Instance.CreateFixtureSpecificationContentMigrator();

			// Have the migrating object loader service load the fixture specification
			T fixtureSpecification = MigratingObjectLoaderService.Instance.LoadFromFile<T>(
				filePath,
				fileReader,
				contentWriter,
				contentMigrator,
				ObjectVersion.FixtureSpecification);

			return fixtureSpecification;
		}

		#endregion

		#region IObjectLoader 

		/// <summary>
		/// Loads the FixtureSpecification from the specified file.
		/// </summary>
		/// <param name="filePath">Path to the fixture specification</param>
		/// <returns>Fixture specification contained in the file</returns>
		object IObjectLoader.LoadFromFile(string filePath)
		{		
			return LoadFromFile(filePath);
		}

		#endregion
	}
}

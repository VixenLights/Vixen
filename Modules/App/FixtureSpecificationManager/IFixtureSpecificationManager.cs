using System.Collections.Generic;
using VixenModules.App.Fixture;

namespace VixenModules.App.FixtureSpecificationManager
{
    /// <summary>
    /// Manages a repository of intelligent fixtures.
    /// </summary>
    public interface IFixtureSpecificationManager
    {
	    /// <summary>
        /// Saves the specified fixture to the fixture respository.
        /// </summary>
        /// <param name="fixture">Fixture specification to save</param>
        void Save(FixtureSpecification fixture);

        /// <summary>
        /// Gets the fixture specifications in the respository.
        /// </summary>
        IList<FixtureSpecification> FixtureSpecifications { get; }

        /// <summary>
        /// Retrieves the list of Gobo images in the profile.
        /// </summary>
        /// <returns>List of gobo images</returns>
        IList<string> GetGoboImages();

        /// <summary>
        /// Returns the path to the Gobo images folder.
        /// </summary>
        /// <returns>Path to the Gobo images folder</returns>
        string GetGoboImageDirectory();
    }
}

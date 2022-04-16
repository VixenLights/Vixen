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
        /// Initializes the manager with the active profile path.
        /// </summary>
        /// <param name="profilePath">Active profile path</param>
        void InitializeProfilePath(string profilePath);

        /// <summary>
        /// Saves the specified fixture to the fixture respository.
        /// </summary>
        /// <param name="fixture">Fixture specification to save</param>
        void Save(FixtureSpecification fixture);

        /// <summary>
        /// Gets the fixture specifications in the respository.
        /// </summary>
        IList<FixtureSpecification> FixtureSpecifications { get; }
    }
}

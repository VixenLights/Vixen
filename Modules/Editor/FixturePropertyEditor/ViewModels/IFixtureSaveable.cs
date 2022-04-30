namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Validates fixture view model to determine if it safe to save.
    /// </summary>
    public interface IFixtureSaveable
    {
        /// <summary>
		/// Returns true if all required function data has been populated.
		/// </summary>
		/// <returns>True if all required function data has been populated</returns>
        bool CanSave();
        
        /// <summary>
        /// Returns the results of the data validation.
        /// </summary>
        /// <returns>Results of the data validation.</returns>
        string GetValidationResults();
    }
}

namespace Vixen.Services
{
	/// <summary>
	/// Loads FixtureSpecification from disk.
	/// </summary>
	public class FixtureSpecificationService
	{
		/// <summary>
		/// Loads fixture specification from disk.
		/// </summary>
		/// <typeparam name="T">Type of the FixtureSpecification</typeparam>
		/// <param name="filePath">Path to the file</param>
		/// <returns>FixtureSpecification object from the file</returns>
		public static T Load<T>(string filePath) where T : class, new()		
		{
			return FileService.Instance.LoadFixtureSpecification<T>(filePath);
		}
	}
}

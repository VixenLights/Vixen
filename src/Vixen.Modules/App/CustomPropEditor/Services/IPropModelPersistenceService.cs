using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Services;

/// <summary>
/// Defines asynchronous persistence operations for Custom Prop files.
/// </summary>
public interface IPropModelPersistenceService
{
	/// <summary>
	/// Loads a Custom Prop from a supported package or legacy LiteDB v4 file.
	/// </summary>
	/// <param name="path">The path of the prop file to load.</param>
	/// <param name="cancellationToken">A token that cancels the load operation.</param>
	/// <returns>A task that represents the asynchronous operation and contains the loaded prop.</returns>
	Task<Prop> LoadAsync(string path, CancellationToken cancellationToken = default);

	/// <summary>
	/// Saves a Custom Prop as a validated schema-1 package.
	/// </summary>
	/// <param name="prop">The prop to save.</param>
	/// <param name="path">The destination path of the prop file.</param>
	/// <param name="cancellationToken">A token that cancels the save operation before publication.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	Task SaveAsync(Prop prop, string path, CancellationToken cancellationToken = default);
}

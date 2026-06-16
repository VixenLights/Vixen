namespace VixenModules.Property.State.Setup.Services
{
	/// <summary>
	/// Provides modal prompts used to manage State definitions.
	/// </summary>
	public interface IStateDefinitionDialogService
	{
		/// <summary>
		/// Requests a State definition name from the user.
		/// </summary>
		/// <param name="title">The title displayed by the prompt.</param>
		/// <param name="initialName">The initial name displayed by the prompt.</param>
		/// <param name="existingNames">The existing names that must remain unique.</param>
		/// <param name="currentName">The current name being edited, or <see langword="null" /> when creating a new definition.</param>
		/// <returns>The accepted name, or <see langword="null" /> when the prompt is canceled.</returns>
		Task<string?> RequestNameAsync(
			string title,
			string initialName,
			IReadOnlyCollection<string> existingNames,
			string? currentName);

		/// <summary>
		/// Confirms deletion of a State definition.
		/// </summary>
		/// <param name="name">The name of the State definition to delete.</param>
		/// <returns><see langword="true" /> if deletion is confirmed; otherwise, <see langword="false" />.</returns>
		Task<bool> ConfirmDeleteAsync(string name);

		/// <summary>
		/// Confirms deletion of one State item.
		/// </summary>
		/// <param name="name">The name of the State item to delete.</param>
		/// <returns><see langword="true" /> if deletion is confirmed; otherwise, <see langword="false" />.</returns>
		Task<bool> ConfirmDeleteStateItemAsync(string name);

		/// <summary>
		/// Confirms deletion of multiple State items.
		/// </summary>
		/// <param name="count">The number of State items to delete.</param>
		/// <returns><see langword="true" /> if deletion is confirmed; otherwise, <see langword="false" />.</returns>
		Task<bool> ConfirmDeleteStateItemsAsync(int count);
	}
}

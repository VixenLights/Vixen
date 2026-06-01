using Common.Messages.LivePreview;

namespace VixenModules.App.LivePreview
{
	/// <summary>Provides live element control operations against named <c>LiveContext</c> instances.</summary>
	/// <remarks>
	/// All methods accept an optional <paramref name="contextName"/>. When <see langword="null"/> the
	/// default <c>"LivePreview"</c> context is used. Named contexts are created on first use and
	/// released when the module unloads.
	/// </remarks>
	public interface ILivePreviewService
	{
		/// <summary>Turns on a single element using a SetLevel effect with the specified parameters.</summary>
		/// <param name="id">The unique identifier of the element node to activate.</param>
		/// <param name="seconds">The duration in seconds; <c>0</c> keeps the element on indefinitely.</param>
		/// <param name="intensity">The intensity level as a percentage from 0 to 100.</param>
		/// <param name="color">An HTML color string such as <c>"#FF0000"</c>, or empty to use the element's default.</param>
		/// <param name="contextName">The name of the target live context, or <see langword="null"/> to use the default context.</param>
		void TurnOnElement(Guid id, int seconds, double intensity, string color, string? contextName = null);

		/// <summary>Turns on multiple elements using SetLevel effects with the specified parameters.</summary>
		/// <param name="states">The desired states for each element to activate.</param>
		/// <param name="contextName">The name of the target live context, or <see langword="null"/> to use the default context.</param>
		void TurnOnElements(IEnumerable<ElementState> states, string? contextName = null);

		/// <summary>Terminates all active effects on a single element.</summary>
		/// <param name="id">The unique identifier of the element node to deactivate.</param>
		/// <param name="contextName">The name of the target live context, or <see langword="null"/> to use the default context.</param>
		void TurnOffElement(Guid id, string? contextName = null);

		/// <summary>Terminates all active effects on multiple elements.</summary>
		/// <param name="states">The states identifying the elements to deactivate.</param>
		/// <param name="contextName">The name of the target live context, or <see langword="null"/> to use the default context.</param>
		void TurnOffElements(IEnumerable<ElementState> states, string? contextName = null);

		/// <summary>Clears all active effects from the specified live context.</summary>
		/// <param name="contextName">The name of the target live context, or <see langword="null"/> to use the default context.</param>
		void ClearActiveEffects(string? contextName = null);
	}
}

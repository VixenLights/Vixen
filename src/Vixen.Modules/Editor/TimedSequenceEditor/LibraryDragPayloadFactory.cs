using System.Drawing;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Editor.TimedSequenceEditor
{
	/// <summary>
	/// Creates independent payloads for library drag-and-drop operations.
	/// </summary>
	internal static class LibraryDragPayloadFactory
	{
		/// <summary>
		/// Creates a payload from a library value.
		/// </summary>
		/// <param name="sourceValue">The library value to copy.</param>
		/// <param name="libraryItemName">The library item name to apply when the payload is linked.</param>
		/// <param name="linkToLibrary"><see langword="true" /> to link the payload to <paramref name="libraryItemName" />; otherwise, <see langword="false" />.</param>
		/// <returns>An independent Curve or ColorGradient copy, or the supplied Color value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="sourceValue" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="libraryItemName" /> is empty when <paramref name="linkToLibrary" /> is <see langword="true" />, or <paramref name="sourceValue" /> is not supported.</exception>
		internal static object Create(object sourceValue, string libraryItemName, bool linkToLibrary)
		{
			ArgumentNullException.ThrowIfNull(sourceValue);

			if (linkToLibrary && string.IsNullOrWhiteSpace(libraryItemName))
			{
				throw new ArgumentException("A linked payload requires a library item name.", nameof(libraryItemName));
			}

			return sourceValue switch
			{
				Curve curve => CreateCurvePayload(curve, libraryItemName, linkToLibrary),
				ColorGradient colorGradient => CreateColorGradientPayload(colorGradient, libraryItemName, linkToLibrary),
				Color color => color,
				_ => throw new ArgumentException("The source value type is not supported for a library drag payload.", nameof(sourceValue))
			};
		}

		private static Curve CreateCurvePayload(Curve sourceCurve, string libraryItemName, bool linkToLibrary)
		{
			var payload = new Curve(sourceCurve)
			{
				LibraryReferenceName = linkToLibrary ? libraryItemName : string.Empty,
				IsCurrentLibraryCurve = false
			};

			return payload;
		}

		private static ColorGradient CreateColorGradientPayload(ColorGradient sourceGradient, string libraryItemName, bool linkToLibrary)
		{
			var payload = new ColorGradient(sourceGradient)
			{
				LibraryReferenceName = linkToLibrary ? libraryItemName : string.Empty,
				IsCurrentLibraryGradient = false
			};

			return payload;
		}
	}
}

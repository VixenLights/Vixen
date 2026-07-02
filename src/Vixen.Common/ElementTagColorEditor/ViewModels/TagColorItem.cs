using System.ComponentModel;
using System.Runtime.CompilerServices;
using Vixen.Sys;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Common.ElementTagColorEditor.ViewModels
{
	/// <summary>
	/// Wraps a single <see cref="ElementTagDefinition"/> for editing in <see cref="Views.ElementTagColorEditorWindow"/>.
	/// </summary>
	/// <remarks>
	/// Color edits are staged on <see cref="Color"/> only; the wrapped <see cref="ElementTagDefinition"/> is left
	/// untouched until <see cref="CommitColor"/> is called, so a Cancel can simply discard this item without
	/// having mutated any shared state.
	/// </remarks>
	public sealed class TagColorItem : INotifyPropertyChanged
	{
		private Color _color;

		public TagColorItem(ElementTagDefinition tagDefinition)
		{
			TagDefinition = tagDefinition;
			_color = ParseColor(tagDefinition.DisplayColor);
		}

		/// <summary>
		/// Gets the tag definition this item edits.
		/// </summary>
		public ElementTagDefinition TagDefinition { get; }

		/// <summary>
		/// Gets the tag's display name.
		/// </summary>
		public string Name => TagDefinition.Name;

		/// <summary>
		/// Gets or sets the staged color for this tag, as chosen in the color picker.
		/// </summary>
		public Color Color
		{
			get => _color;
			set
			{
				if (_color == value)
					return;
				_color = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Writes <see cref="Color"/> back to <see cref="TagDefinition"/>'s <see cref="ElementTagDefinition.DisplayColor"/>
		/// as a 6-digit hex string, matching the format already used by <see cref="Vixen.Sys.BuiltInElementTags"/>.
		/// </summary>
		public void CommitColor()
		{
			TagDefinition.DisplayColor = $"#{_color.R:X2}{_color.G:X2}{_color.B:X2}";
		}

		private static Color ParseColor(string displayColor)
		{
			if (string.IsNullOrEmpty(displayColor))
				return Colors.White;

			try
			{
				return (Color)ColorConverter.ConvertFromString(displayColor);
			}
			catch (FormatException)
			{
				return Colors.White;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

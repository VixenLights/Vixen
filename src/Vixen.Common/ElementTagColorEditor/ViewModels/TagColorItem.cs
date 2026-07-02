using System;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using ColorConverter = System.Windows.Media.ColorConverter;
using ColorPicker = Common.Controls.ColorManagement.ColorPicker.ColorPicker;
using XYZ = Common.Controls.ColorManagement.ColorModels.XYZ;

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
	public class TagColorItem : ViewModelBase
	{
		public TagColorItem(ElementTagDefinition tagDefinition)
		{
			TagDefinition = tagDefinition;
			Color = ParseColor(tagDefinition.DisplayColor);
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
		/// Gets or sets the staged color for this tag, as chosen via <see cref="PickColorCommand"/>.
		/// </summary>
		public Color Color
		{
			get => GetValue<Color>(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		/// <summary>
		/// Color property data.
		/// </summary>
		public static readonly IPropertyData ColorProperty = RegisterProperty<Color>(nameof(Color));

		private Command _pickColorCommand;

		/// <summary>
		/// Gets the command that opens a color picker to choose <see cref="Color"/>.
		/// </summary>
		public Command PickColorCommand => _pickColorCommand ??= new Command(PickColor);

		/// <summary>
		/// Method to invoke when <see cref="PickColorCommand"/> is executed.
		/// </summary>
		private void PickColor()
		{
			using ColorPicker colorPicker = new ColorPicker();
			colorPicker.Color = XYZ.FromRGB(System.Drawing.Color.FromArgb(Color.R, Color.G, Color.B));
			if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				System.Drawing.Color chosen = colorPicker.Color.ToRGB();
				Color = Color.FromRgb(chosen.R, chosen.G, chosen.B);
			}
		}

		/// <summary>
		/// Writes <see cref="Color"/> back to <see cref="TagDefinition"/>'s <see cref="ElementTagDefinition.DisplayColor"/>
		/// as a 6-digit hex string, matching the format already used by <see cref="Vixen.Sys.BuiltInElementTags"/>.
		/// </summary>
		public void CommitColor()
		{
			TagDefinition.DisplayColor = $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";
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
	}
}

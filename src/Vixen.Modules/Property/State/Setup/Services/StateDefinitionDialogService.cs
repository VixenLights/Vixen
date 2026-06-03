using System.Windows;
using System.Windows.Controls;
using WpfButton = System.Windows.Controls.Button;
using WpfMessageBox = System.Windows.MessageBox;
using WpfOrientation = System.Windows.Controls.Orientation;
using WpfTextBox = System.Windows.Controls.TextBox;

namespace VixenModules.Property.State.Setup.Services
{
	/// <summary>
	/// Displays WPF prompts used to manage State definitions.
	/// </summary>
	public sealed class StateDefinitionDialogService : IStateDefinitionDialogService
	{
		/// <inheritdoc />
		public Task<string?> RequestNameAsync(
			string title,
			string initialName,
			IReadOnlyCollection<string> existingNames,
			string? currentName)
		{
			var window = new Window
			{
				Title = title,
				Width = 360,
				Height = 150,
				MinWidth = 320,
				MinHeight = 150,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				ResizeMode = ResizeMode.NoResize
			};

			var root = new Grid { Margin = new Thickness(12) };
			root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			root.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			var nameLabel = new TextBlock
			{
				Text = "Name",
				Margin = new Thickness(0, 3, 8, 3),
				VerticalAlignment = VerticalAlignment.Center
			};
			var nameBox = new WpfTextBox
			{
				Text = initialName,
				Margin = new Thickness(0, 0, 0, 4),
				MinWidth = 220
			};
			Grid.SetColumn(nameBox, 1);

			var validationText = new TextBlock
			{
				Foreground = System.Windows.Media.Brushes.Firebrick,
				Margin = new Thickness(0, 0, 0, 8),
				TextWrapping = TextWrapping.Wrap
			};
			Grid.SetRow(validationText, 1);
			Grid.SetColumnSpan(validationText, 2);

			var warningText = new TextBlock
			{
				Foreground = System.Windows.Media.Brushes.DarkGoldenrod,
				Margin = new Thickness(0, 0, 0, 8),
				TextWrapping = TextWrapping.Wrap
			};
			Grid.SetRow(warningText, 2);
			Grid.SetColumnSpan(warningText, 2);

			var buttons = new StackPanel
			{
				Orientation = WpfOrientation.Horizontal,
				HorizontalAlignment = System.Windows.HorizontalAlignment.Right
			};
			Grid.SetRow(buttons, 3);
			Grid.SetColumnSpan(buttons, 2);

			var okButton = new WpfButton
			{
				Content = "OK",
				MinWidth = 72,
				Margin = new Thickness(0, 0, 8, 0),
				IsDefault = true
			};
			var cancelButton = new WpfButton
			{
				Content = "Cancel",
				MinWidth = 72,
				IsCancel = true
			};
			buttons.Children.Add(okButton);
			buttons.Children.Add(cancelButton);

			root.Children.Add(nameLabel);
			root.Children.Add(nameBox);
			root.Children.Add(validationText);
			root.Children.Add(warningText);
			root.Children.Add(buttons);
			window.Content = root;

			void ValidateName()
			{
				var name = nameBox.Text.Trim();
				var duplicate = existingNames.Any(existingName =>
					!existingName.Equals(currentName, StringComparison.Ordinal) &&
					existingName.Equals(name, StringComparison.Ordinal));
				var caseOnlyDuplicate = existingNames.Any(existingName =>
					!existingName.Equals(currentName, StringComparison.Ordinal) &&
					existingName.Equals(name, StringComparison.OrdinalIgnoreCase) &&
					!existingName.Equals(name, StringComparison.Ordinal));

				if (string.IsNullOrWhiteSpace(name))
				{
					validationText.Text = "State definition name is required.";
					okButton.IsEnabled = false;
				}
				else if (duplicate)
				{
					validationText.Text = "State definition names must be unique.";
					okButton.IsEnabled = false;
				}
				else
				{
					validationText.Text = string.Empty;
					okButton.IsEnabled = true;
				}

				warningText.Text = caseOnlyDuplicate
					? "State definition names differ only by casing. Check you don't have a typo."
					: string.Empty;
			}

			nameBox.TextChanged += (_, _) => ValidateName();
			okButton.Click += (_, _) =>
			{
				window.Tag = nameBox.Text.Trim();
				window.DialogResult = true;
			};
			window.Loaded += (_, _) =>
			{
				nameBox.SelectAll();
				nameBox.Focus();
				ValidateName();
			};

			var result = window.ShowDialog();
			return Task.FromResult(result == true ? (string?)window.Tag : null);
		}

		/// <inheritdoc />
		public Task<bool> ConfirmDeleteAsync(string name)
		{
			var result = WpfMessageBox.Show(
				$"Delete State definition '{name}'?",
				"Delete State Definition",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question);
			return Task.FromResult(result == MessageBoxResult.Yes);
		}
	}
}

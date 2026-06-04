using System.Windows;
using Catel.IoC;
using Catel.Services;
using VixenModules.Property.State.Setup.ViewModels;
using VixenModules.Property.State.Setup.Views;
using WpfApplication = System.Windows.Application;

namespace VixenModules.Property.State.Setup.Services
{
	/// <summary>
	/// Displays prompts used to manage State definitions.
	/// </summary>
	public sealed class StateDefinitionDialogService : IStateDefinitionDialogService
	{
		private readonly IMessageService _messageService;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateDefinitionDialogService"/> class.
		/// </summary>
		public StateDefinitionDialogService()
			: this(ServiceLocator.Default.ResolveType<IMessageService>() ??
				throw new InvalidOperationException("The Catel message service is not registered."))
		{
		}

		internal StateDefinitionDialogService(IMessageService messageService)
		{
			_messageService = messageService;
		}

		/// <inheritdoc />
		public Task<string?> RequestNameAsync(
			string title,
			string initialName,
			IReadOnlyCollection<string> existingNames,
			string? currentName)
		{
			var viewModel = new StateDefinitionNameDialogViewModel(
				title,
				initialName,
				existingNames,
				currentName);
			var window = new StateDefinitionNameDialogView(viewModel)
			{
				Owner = GetOwnerWindow()
			};
			if (window.Owner == null)
			{
				window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			}

			var result = window.ShowDialog();
			return Task.FromResult(result == true ? viewModel.ResultName : null);
		}

		/// <inheritdoc />
		public async Task<bool> ConfirmDeleteAsync(string name)
		{
			var result = await _messageService.ShowAsync(
				$"Delete State definition '{name}'?",
				"Delete State Definition",
				MessageButton.YesNo,
				MessageImage.Question);
			return result == MessageResult.Yes;
		}

		private static Window? GetOwnerWindow()
		{
			return WpfApplication.Current?.Windows
				.OfType<Window>()
				.FirstOrDefault(window => window.IsActive) ??
				WpfApplication.Current?.MainWindow;
		}
	}
}

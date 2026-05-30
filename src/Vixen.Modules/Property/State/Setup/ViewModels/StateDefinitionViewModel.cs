using System.Collections.ObjectModel;
using Catel.MVVM;
using VixenModules.Property.State.Setup.Models;

namespace VixenModules.Property.State.Setup.ViewModels
{
	internal class StateDefinitionViewModel: ViewModelBase
	{
		public StateDefinitionViewModel(List<StateDefinition> stateDefinitions)
		{
			StateDefinitions = new ObservableCollection<StateDefinition>(stateDefinitions);
		}

		public ObservableCollection<StateDefinition> StateDefinitions { get; set; }
	}
}

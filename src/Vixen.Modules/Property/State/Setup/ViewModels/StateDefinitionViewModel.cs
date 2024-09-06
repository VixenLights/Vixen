using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.MVVM;
using VixenModules.Property.State.Setup.Models;

namespace VixenModules.Property.State.Setup.ViewModels
{
	internal class StateDefinitionViewModel: ViewModelBase
	{
		public StateDefinitionViewModel(List<StateDefinition> stateDefinitions)
		{
			
		}

		public ObservableCollection<StateDefinition> StateDefinitions { get; set; }
	}
}

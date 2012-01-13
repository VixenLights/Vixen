namespace VixenModules.Output.E131.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using VixenModules.Output.E131.Model;

    public class UniverseSetupViewModel : ViewModelBase
    {
        private UniverseEntry _selectedUniverseEntry;

        public UniverseSetupViewModel(ObservableCollection<UniverseEntry> universeEntries)
        {
            UniverseEntries = universeEntries;
            UniverseEntries.Add(new UniverseEntry(true, 100, 150, 300, "unicast", "multicast", 30));
            AddUniverseEntryCommand = new RelayCommand(x => AddUniverseEntry());
            DeleteUniverseEntryCommand = new RelayCommand(x => DeleteUniverseEntry(), x => CanDeleteUniverseEntryCommand());
            CloseCommand = new RelayCommand(x => InvokeRequestClose());
            MoveUpCommand = new RelayCommand(x => MoveUp(), x => CanMoveUp());
            MoveDownCommand = new RelayCommand(x => MoveDown(), x => CanMoveDown());
        }

        public ICommand AddUniverseEntryCommand { get; private set; }
        public ICommand DeleteUniverseEntryCommand { get; private set; }
        public ICommand MoveDownCommand { get; private set; }
        public ICommand MoveUpCommand { get; private set; }

        public UniverseEntry SelectedUniverseEntry
        {
            get
            {
                return _selectedUniverseEntry;
            }

            set
            {
                _selectedUniverseEntry = value;
                OnPropertyChanged("SelectedUniverseEntry");
            }
        }

        public ObservableCollection<UniverseEntry> UniverseEntries { get; private set; }
        protected RelayCommand CloseCommand { get; private set; }

        private void AddUniverseEntry()
        {
            var universeEntry = new UniverseEntry(true, 2, 3, 4, string.Empty, string.Empty, 30);
            UniverseEntries.Add(universeEntry);
        }

        private bool CanDeleteUniverseEntryCommand()
        {
            return SelectedUniverseEntry != null;
        }

        private bool CanMoveDown()
        {
            var currentDisplayElement = SelectedUniverseEntry;
            return currentDisplayElement != null && UniverseEntries.IndexOf(currentDisplayElement) != UniverseEntries.Count - 1;
        }

        private bool CanMoveUp()
        {
            var currentDisplayElement = SelectedUniverseEntry;
            return currentDisplayElement != null && UniverseEntries.IndexOf(currentDisplayElement) != 0;
        }

        private void DeleteUniverseEntry()
        {
            var universeEntry = SelectedUniverseEntry;
            SelectedUniverseEntry = null;
            if (universeEntry != null
                && UniverseEntries.Contains(universeEntry))
            {
                UniverseEntries.Remove(universeEntry);
            }
        }

        private void MoveDown()
        {
            var currentDisplayElement = SelectedUniverseEntry;
            var index = UniverseEntries.IndexOf(currentDisplayElement);
            UniverseEntries.Move(index, index + 1);
        }

        private void MoveUp()
        {
            var currentDisplayElement = SelectedUniverseEntry;
            var index = UniverseEntries.IndexOf(currentDisplayElement);
            UniverseEntries.Move(index, index - 1);
        }
    }
}

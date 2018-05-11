using System.Collections.ObjectModel;
using System.Linq;
using Catel.Data;
using Catel.MVVM;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	public class MarkDockerViewModel:ViewModelBase
	{
		public MarkDockerViewModel(ObservableCollection<MarkCollection> markCollections)
		{
			MarkCollections = markCollections;
		}

		#region MarkCollections model property

		/// <summary>
		/// Gets or sets the MarkCollections value.
		/// </summary>
		[Model]
		public ObservableCollection<MarkCollection> MarkCollections
		{
			get { return GetValue<ObservableCollection<MarkCollection>>(MarkCollectionsProperty); }
			private set { SetValue(MarkCollectionsProperty, value); }
		}

		/// <summary>
		/// MarkCollections property data.
		/// </summary>
		public static readonly PropertyData MarkCollectionsProperty = RegisterProperty("MarkCollections", typeof(ObservableCollection<MarkCollection>));

		#endregion

		public void DeleteCollection(MarkCollection markCollection)
		{
			MarkCollections.Remove(markCollection);
			if (markCollection.IsDefault && MarkCollections.Any())
			{
				//we need to find a new default
				var mc = MarkCollections.FirstOrDefault(x => x.IsEnabled);
				if (mc != null)
				{
					mc.IsDefault = true;
				}
				else
				{
					MarkCollections.First().IsDefault = true;
				}
			}
		}

		#region AddCollection command

		private Command _addCollectionCommand;

		/// <summary>
		/// Gets the AddCollection command.
		/// </summary>
		public Command AddCollectionCommand
		{
			get { return _addCollectionCommand ?? (_addCollectionCommand = new Command(AddCollection)); }
		}

		/// <summary>
		/// Method to invoke when the AddCollection command is executed.
		/// </summary>
		private void AddCollection()
		{
			var mc = new MarkCollection();
			if (!MarkCollections.Any())
			{
				mc.IsDefault = true;
			}
			MarkCollections.Add(mc);
			
		}

		#endregion

		#region ImportCollecton command

		private Command _importCollectonCommand;

		/// <summary>
		/// Gets the ImportCollecton command.
		/// </summary>
		public Command ImportCollectionCommand
		{
			get { return _importCollectonCommand ?? (_importCollectonCommand = new Command(ImportCollection)); }
		}

		/// <summary>
		/// Method to invoke when the ImportCollecton command is executed.
		/// </summary>
		private void ImportCollection()
		{
			// TODO: Handle command logic here
		}

		#endregion

		#region ExportCollection command

		private Command _exportCollectionCommand;

		/// <summary>
		/// Gets the ExportCollection command.
		/// </summary>
		public Command ExportCollectionCommand
		{
			get { return _exportCollectionCommand ?? (_exportCollectionCommand = new Command(ExportCollection, CanExportCollection)); }
		}

		/// <summary>
		/// Method to invoke when the ExportCollection command is executed.
		/// </summary>
		private void ExportCollection()
		{
			// TODO: Handle command logic here
		}

		/// <summary>
		/// Method to check whether the ExportCollection command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanExportCollection()
		{
			return true;
		}

		#endregion

	}
}

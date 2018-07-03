using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Common.Controls;
using Vixen.Marks;
using VixenModules.App.Marks;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	public class MarkDockerViewModel:ViewModelBase
	{
		private bool _lineToggleState = true;
		private bool _markBarToggleState = true;
		public MarkDockerViewModel(ObservableCollection<IMarkCollection> markCollections)
		{
			MarkCollections = markCollections;
			if (MarkCollections.All(x => x.ShowGridLines))
			{
				_lineToggleState = false;
			}
			if (MarkCollections.All(x => x.ShowMarkBar))
			{
				_markBarToggleState = false;
			}
		}

		#region MarkCollections model property

		/// <summary>
		/// Gets or sets the MarkCollections value.
		/// </summary>
		[Model]
		public ObservableCollection<IMarkCollection> MarkCollections
		{
			get { return GetValue<ObservableCollection<IMarkCollection>>(MarkCollectionsProperty); }
			set { SetValue(MarkCollectionsProperty, value); }
		}

		/// <summary>
		/// MarkCollections property data.
		/// </summary>
		public static readonly PropertyData MarkCollectionsProperty = RegisterProperty("MarkCollections", typeof(ObservableCollection<IMarkCollection>));

		#endregion

		public void DeleteCollection(IMarkCollection markCollection)
		{
			MarkCollections.Remove(markCollection);
			if (markCollection.IsDefault && MarkCollections.Any())
			{
				//we need to find a new default
				var mc = MarkCollections.FirstOrDefault(x => x.IsVisible);
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
			var aDialog = new AudacityImportDialog();

			if (aDialog.ShowDialog() == DialogResult.OK)
			{
				if (aDialog.IsVixen3BeatSelection)
					MarkImportExportService.ImportVixen3Beats(MarkCollections);
				if (aDialog.IsVampBarSelection || aDialog.IsAudacityBeatSelection)
					MarkImportExportService.LoadBarLabels(MarkCollections);
				if (aDialog.IsVampBeatSelection)
					MarkImportExportService.LoadBeatLabels(MarkCollections);
				if (aDialog.IsXTimingSelection)
					MarkImportExportService.LoadXTiming(MarkCollections);
				if (aDialog.IsPapagayoSelection)
					MarkImportExportService.ImportPapagayoTracks(MarkCollections);
			}
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
			var bDialog = new BeatMarkExportDialog();

			if (bDialog.ShowDialog() == DialogResult.OK)
			{
				if (bDialog.IsVixen3Selection)
					MarkImportExportService.ExportMarkCollections("vixen3", MarkCollections);
				if (bDialog.IsAudacitySelection)
					MarkImportExportService.ExportMarkCollections("audacity", MarkCollections);
				if (!bDialog.IsVixen3Selection && !bDialog.IsAudacitySelection)
				{
					var messageBox = new MessageBoxForm("No export type selected", "Warning", MessageBoxButtons.OK, SystemIcons.Warning);
					messageBox.ShowDialog();
				}
			}
		}

		/// <summary>
		/// Method to check whether the ExportCollection command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanExportCollection()
		{
			return MarkCollections.Any();
		}

		

		#endregion

		#region EnableAllLines command

		private Command _enableAllLinesCommand;

		/// <summary>
		/// Gets the EnableAllLines command.
		/// </summary>
		public Command ToggleLineStateCommand
		{
			get { return _enableAllLinesCommand ?? (_enableAllLinesCommand = new Command(ToggleLineState)); }
		}

		/// <summary>
		/// Method to invoke when the EnableAllLines command is executed.
		/// </summary>
		private void ToggleLineState()
		{
			MarkCollections.ForEach(x => x.ShowGridLines = _lineToggleState);
			_lineToggleState = !_lineToggleState;
		}

		#endregion

		#region ToggleMarkBarState command

		private Command _toggleMarkBarStateCommand;

		/// <summary>
		/// Gets the ToggleMarkBarState command.
		/// </summary>
		public Command ToggleMarkBarStateCommand
		{
			get { return _toggleMarkBarStateCommand ?? (_toggleMarkBarStateCommand = new Command(ToggleMarkBarState, CanToggleMarkBarState)); }
		}

		/// <summary>
		/// Method to invoke when the ToggleMarkBarState command is executed.
		/// </summary>
		private void ToggleMarkBarState()
		{
			MarkCollections.ForEach(x => x.ShowMarkBar = _markBarToggleState);
			_markBarToggleState = !_markBarToggleState;
		}

		/// <summary>
		/// Method to check whether the ToggleMarkBarState command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanToggleMarkBarState()
		{
			return MarkCollections.Any();
		}

		#endregion

	}
}

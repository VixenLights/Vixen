using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Common.Controls;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	public class MarkDockerViewModel:ViewModelBase
	{
		private bool _lineToggleState = true;
		private bool _markBarToggleState = true;
		public MarkDockerViewModel(ObservableCollection<MarkCollection> markCollections)
		{
			MarkCollections = markCollections;
			if (MarkCollections.All(x => x.IsEnabled))
			{
				_lineToggleState = false;
			}
			if (MarkCollections.All(x => x.Decorator.CompactMode))
			{
				_markBarToggleState = false;
			}
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
			var bDialog = new BeatMarkExportDialog();

			if (bDialog.ShowDialog() == DialogResult.OK)
			{
				if (bDialog.IsVixen3Selection)
					ExportMarkCollections("vixen3");
				if (bDialog.IsAudacitySelection)
					ExportMarkCollections("audacity");
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

		//Beat Mark Collection Export routine 2-7-2014 JMB
		//In the audacity section, if the MarkCollections.Count = 1 then we assume the collection is bars and iMarkCollection++
		//Other wise its beats, at least from the information I have studied, and we do not iMarkCollection++ to keep the collections together properly.
		private void ExportMarkCollections(string exportType)
		{
			var saveFileDialog = new SaveFileDialog();
			if (exportType == "vixen3")
			{
				saveFileDialog.DefaultExt = ".v3m";
				saveFileDialog.Filter = @"Vixen 3 Mark Collection (*.v3m)|*.v3m|All Files (*.*)|*.*";
				saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					var xmlsettings = new XmlWriterSettings()
					{
						Indent = true,
						IndentChars = "\t",
					};

					DataContractSerializer ser = new DataContractSerializer(typeof(List<MarkCollection>));
					var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
					ser.WriteObject(writer, MarkCollections);
					writer.Close();
				}
			}

			if (exportType == "audacity")
			{
				int iMarkCollection = 0;
				List<string> beatMarks = new List<string>();
				foreach (MarkCollection mc in MarkCollections)
				{
					iMarkCollection++;
					foreach (Mark mark in mc.Marks)
					{
						beatMarks.Add(mark.StartTime.TotalSeconds.ToString("0000.000") + "\t" + mark.StartTime.TotalSeconds.ToString("0000.000") + "\t" + iMarkCollection);
						if (MarkCollections.Count == 1)
							iMarkCollection++;
					}
				}

				saveFileDialog.DefaultExt = ".txt";
				saveFileDialog.Filter = @"Audacity Marks (*.txt)|*.txt|All Files (*.*)|*.*";
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					string name = saveFileDialog.FileName;

					using (System.IO.StreamWriter file = new System.IO.StreamWriter(name))
					{
						foreach (string bm in beatMarks.OrderBy(x => x))
						{
							file.WriteLine(bm);
						}
					}
				}
			}
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
			MarkCollections.ForEach(x => x.IsEnabled = _lineToggleState);
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
			MarkCollections.ForEach(x => x.Decorator.CompactMode = _markBarToggleState);
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

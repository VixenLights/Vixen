using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Vixen.Module.Editor;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;

namespace VixenModules.App.Shows
{
	public partial class SequenceTypeEditor : TypeEditorBase
	{
		public ShowItem _showItem;

		public SequenceTypeEditor(ShowItem showItem)
		{
			InitializeComponent();
			_showItem = showItem;
		}

		private void textBoxSequence_TextChanged(object sender, EventArgs e)
		{
			_showItem.Sequence_FileName = (sender as TextBox).Text;
			textBoxSequence.Text = _showItem.Sequence_FileName;
			if (System.IO.File.Exists(_showItem.Sequence_FileName))
			{
				labelSequence.Text = System.IO.Path.GetFileName(_showItem.Sequence_FileName);
				_showItem.Name = "Run sequence: " + System.IO.Path.GetFileName(_showItem.Sequence_FileName);
			}
			else
			{
				labelSequence.Text = "(sequence not found)";
				_showItem.Name = labelSequence.Text;
			}
			FireChanged(_showItem.Name);
		}

		private void buttonSelectSequence_Click(object sender, EventArgs e)
		{
			openFileDialog.InitialDirectory = SequenceService.SequenceDirectory;

			// configure the open file dialog with a filter for currently available sequence types
			string filter = "";
			string allTypes = "";
			IEnumerable<ISequenceTypeModuleDescriptor> sequenceDescriptors =
				ApplicationServices.GetModuleDescriptors<ISequenceTypeModuleInstance>().Cast<ISequenceTypeModuleDescriptor>();
			foreach (ISequenceTypeModuleDescriptor descriptor in sequenceDescriptors)
			{
				filter += descriptor.TypeName + " (*" + descriptor.FileExtension + ")|*" + descriptor.FileExtension + "|";
				allTypes += "*" + descriptor.FileExtension + ";";
			}
			filter += "All files (*.*)|*.*";
			filter = "All Sequence Types (" + allTypes + ")|" + allTypes + "|" + filter;

			openFileDialog.Filter = filter;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				textBoxSequence.Text = openFileDialog.FileName;
			}
		}

		private void SequenceTypeEditor_Load(object sender, EventArgs e)
		{
			textBoxSequence.Text = _showItem.Sequence_FileName;
		}

	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Vixen.Services;
using Vixen.Sys;

namespace VixenModules.App.Scheduler
{
	public partial class ProgramForm : BaseForm
	{
		private Program _originalProgram;
		private Program _editingProgram;

		public ProgramForm(Program program)
		{
			if (program == null) throw new ArgumentNullException("program");
			InitializeComponent();

			_Program = program;
			_SystemSequences = SequenceService.Instance.GetAllSequenceFileNames().Select(x => new ItemData(x));
		}

		private Program _Program
		{
			get { return _editingProgram; }
			set
			{
				_originalProgram = value;
				_editingProgram = new Program(_originalProgram);
				_ProgramName = _editingProgram.Name;
				_ProgramSequences = _editingProgram.Select(x => new ItemData(x.FilePath));
			}
		}

		private IEnumerable<ItemData> _SystemSequences
		{
			get { return listBoxSequences.Items.Cast<ItemData>(); }
			set
			{
				listBoxSequences.Items.Clear();
				listBoxSequences.Items.AddRange(value.Cast<Object>().ToArray());
			}
		}

		private IEnumerable<ItemData> _ProgramSequences
		{
			get { return listBoxProgram.Items.Cast<ItemData>(); }
			set
			{
				listBoxProgram.Items.Clear();
				listBoxProgram.Items.AddRange(value.Cast<Object>().ToArray());
			}
		}

		private string _ProgramName
		{
			get { return textBoxProgramName.Text; }
			set { textBoxProgramName.Text = value; }
		}

		private void _Move(ListBox from, ListBox to)
		{
			if (from.SelectedItem != null) {
				to.Items.Add(from.SelectedItem);
				from.Items.Remove(from.SelectedItem);
			}
		}

		private void listBoxSequences_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonMoveRight.Enabled = _CanMoveRight;
		}

		private void listBoxProgram_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonMoveLeft.Enabled = _CanMoveLeft;
			buttonMoveUp.Enabled = _CanMoveUp;
			buttonMoveDown.Enabled = _CanMoveDown;
			buttonDelete.Enabled = _CanDelete;
		}

		private bool _CanMoveLeft
		{
			get { return listBoxProgram.SelectedItem != null; }
		}

		private bool _CanMoveRight
		{
			get { return listBoxSequences.SelectedItem != null; }
		}

		private bool _CanMoveUp
		{
			get
			{
				return
					listBoxProgram.SelectedItems.Count == 1 &&
					listBoxProgram.SelectedIndex > 0;
			}
		}

		private bool _CanMoveDown
		{
			get
			{
				return
					listBoxProgram.SelectedItems.Count == 1 &&
					listBoxProgram.SelectedIndex < listBoxProgram.Items.Count - 1;
			}
		}

		private bool _CanDelete
		{
			get { return listBoxProgram.SelectedItem != null; }
		}

		private void buttonMoveRight_Click(object sender, EventArgs e)
		{
			_Move(listBoxSequences, listBoxProgram);
		}

		private void _MoveUp()
		{
			object item = listBoxProgram.SelectedItem;
			int index = listBoxProgram.SelectedIndex;

			listBoxProgram.Items.RemoveAt(index);
			index--;
			listBoxProgram.Items.Insert(index, item);
		}

		private void _MoveDown()
		{
			object item = listBoxProgram.SelectedItem;
			int index = listBoxProgram.SelectedIndex;

			listBoxProgram.Items.RemoveAt(index);
			index++;
			listBoxProgram.Items.Insert(index, item);
		}

		private bool _Validate()
		{
			List<string> messages = new List<string>();

			if (string.IsNullOrWhiteSpace(_ProgramName)) {
				messages.Add("* Program does not have a name.");
			}

			if (_ProgramSequences.Count() == 0) {
				messages.Add("* The program has no sequences.");
			}

			if (messages.Count > 0) {
				messages.Insert(0, "The following problems need to be corrected:");
				messages.Insert(1, Environment.NewLine);
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Hand; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(string.Join(Environment.NewLine, messages), "Vixen Program", false, false);
				messageBox.ShowDialog();
				return false;
			}

			return true;
		}

		private void buttonMoveLeft_Click(object sender, EventArgs e)
		{
			_Move(listBoxProgram, listBoxSequences);
		}

		private void buttonMoveUp_Click(object sender, EventArgs e)
		{
			_MoveUp();
		}

		private void buttonMoveDown_Click(object sender, EventArgs e)
		{
			_MoveDown();
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (_Validate()) {
				Cursor = Cursors.WaitCursor;
				try {
					_originalProgram.Clear();
					_originalProgram.Sequences.AddRange(_ProgramSequences.Select(x => SequenceService.Instance.Load(x.Path)));
					_originalProgram.Save(_ProgramName);
				}
				catch (Exception ex) {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Exclamation; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm(ex.Message, "Vixen Program", false, false);
					messageBox.ShowDialog();
				}
				finally {
					Cursor = Cursors.Default;
				}
			}
		}

		internal struct ItemData
		{
			private readonly string Name;
			public string Path;

			public ItemData(string path)
			{
				Name = System.IO.Path.GetFileName(path);
				Path = path;
			}

			public override string ToString()
			{
				return Name;
			}
		}
	}
}
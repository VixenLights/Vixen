using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Execution;
using Vixen.Sys;
using Vixen.Module.Editor;
using Vixen.Module.Sequence;
using CommonElements.Timeline;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm : Form
	{
		#region Member Variables

		private TimedSequence _sequence;
		private Dictionary<CommandNode, List<TimelineElement>> _commandNodeToElements;
		//private Dictionary<TimelineElement, CommandNode> _elementToCommandNode;
		private Dictionary<ChannelNode, List<TimelineRow>> _channelNodeToRows;

		#endregion



		public TimedSequenceEditorForm()
		{
			InitializeComponent();

			_commandNodeToElements = new Dictionary<CommandNode, List<TimelineElement>>();
			//_elementToCommandNode = new Dictionary<TimelineElement, CommandNode>();
			_channelNodeToRows = new Dictionary<ChannelNode, List<TimelineRow>>();

			LoadSystemNodesToRows();
		}




		#region IEditor implementation
		// (yes, even though this doesn't implement IEditor, it's wrapped by the TimedSequenceEditor,
		// which does. The TimedSequenceEditor will just pass all calls through to this form, effectively.)


		// TODO: what is this supposed to be for?
		public EditorValues EditorValues
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsModified { get; private set; }

		// TODO: what is this supposed to do; doesn't the framwork/main application
		// set the sequence of the editor?
		public void NewSequence()
		{
			throw new NotImplementedException();
		}

		public void RefreshSequence()
		{
			throw new NotImplementedException();
		}

		public void Save(string filePath = null)
		{
			throw new NotImplementedException();
		}

		public ISelection Selection
		{
			get { throw new NotImplementedException(); }
		}

		public Vixen.Sys.ISequence Sequence
		{
			get { return _sequence; }
			set { LoadSequence(value);  }
		}

		#endregion






		#region Saving / Loading Methods

		/// <summary>
		/// Loads all nodes (groups/channels) currently in the system as rows in the timeline control.
		/// </summary>
		private void LoadSystemNodesToRows()
		{
			foreach (ChannelNode node in Vixen.Sys.Execution.Nodes.RootNodes) {
				AddNodeAsRow(node, null);
			}
		}

		/// <summary>
		/// Adds a single given channel node as a row in the timeline control. Recursively adds all
		/// child nodes of the given node as children, if needed.
		/// </summary>
		/// <param name="node">The node to generate a row for.</param>
		/// <param name="parentRow">The parent node the row should belong to, if any.</param>
		private void AddNodeAsRow(ChannelNode node, TimelineRow parentRow)
		{
			// made the new row from the given node and add it to the control.
			TimelineRow newRow = timelineControl.AddRow(node.Name, parentRow);

			// Tag it with the node it refers to, and take note of which row the given channel node will refer to.
			newRow.Tag = node;
			if (_channelNodeToRows.ContainsKey(node))
				_channelNodeToRows[node].Add(newRow);
			else
				_channelNodeToRows[node] = new List<TimelineRow> { newRow };

			// iterate through all if its children, adding them as needed
			foreach (ChannelNode child in node.Children) {
				AddNodeAsRow(node, newRow);
			}
		}

		private void LoadSequence(Vixen.Sys.ISequence sequence)
		{
			// check if it's the right type of sequence that we know how to deal with. If not, complain bitterly.
			if (sequence is TimedSequence)
				_sequence = (TimedSequence)sequence;
			else {
				throw new NotImplementedException("Cannot use sequence type with a Timed Sequence Editor");
			}

			// clear out all the old data
			// TODO

			// load the new data: get all the commands in the sequence, and make a new element for each of them.
			foreach (CommandNode node in _sequence.Data.GetCommands()) {
				// for each command, make a new element for every separate row its in; even though there's only
				// a single command, if there are multiple copies of the same channel node that it is in (ie.
				// belonging to different groups, etc.) then add a new element for each row. We can just track the
				// elements, and change the other "identical" elements if one of them changes.
				if (_channelNodeToRows.ContainsKey(node.TargetNodes.First())) {
					List<TimelineRow> targetRows = _channelNodeToRows[node.TargetNodes.First()];
					// make a new element for each row that represents the channel this command is in.
					foreach (TimelineRow row in targetRows) {
						TimelineElement element = new TimelineElement();
						element.StartTime = TimeSpan.FromMilliseconds(node.StartTime);
						element.Duration = TimeSpan.FromMilliseconds(node.TimeSpan);
						if (_commandNodeToElements.ContainsKey(node))
							_commandNodeToElements[node].Add(element);
						else
							_commandNodeToElements[node] = new List<TimelineElement> { element };
					}
				}
			}
		}

		private void SaveSequence()
		{
			if (_sequence != null)
				_sequence.Save();
		}








		#endregion



		#region Tool Strip Menu Items

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		#endregion
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Xml;
using Common.Controls;
using Common.Controls.ControlsEx.ValueControls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Resources;
using Common.Resources.Properties;
using NLog;
using Vixen;
using Vixen.Cache.Sequence;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Module.App;
using VixenModules.App.Curves;
using VixenModules.App.LipSyncApp;
using VixenModules.Effect.Video;
using VixenModules.Media.Audio;
using VixenModules.Effect.LipSync;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;
using Vixen.Sys.State;
using VixenModules.Analysis.BeatsAndBars;
using VixenModules.App.ColorGradients;
using VixenModules.Editor.EffectEditor;
using VixenModules.Editor.TimedSequenceEditor.Undo;
using VixenModules.Sequence.Timed;
using WeifenLuo.WinFormsUI.Docking;
using Element = Common.Controls.Timeline.Element;
using Timer = System.Windows.Forms.Timer;
using VixenModules.Property.Color;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm
	{

		private void AddContextCollectionsMenu()
		{
			ToolStripMenuItem contextMenuItemCollections = new ToolStripMenuItem("Collections") { Image = Resources.collection };

			if ((TimelineControl.SelectedElements.Count() > 1 
				|| TimelineControl.SelectedElements.Count() == 1 && SupportsColorLists(TimelineControl.SelectedElements.FirstOrDefault())) 
				&& _colorCollections.Any())
			{
				ToolStripMenuItem contextMenuItemColorCollections = new ToolStripMenuItem("Colors") { Image = Resources.colors };
				ToolStripMenuItem contextMenuItemRandomColors = new ToolStripMenuItem("Random") {Image = Resources.randomColors };
				ToolStripMenuItem contextMenuItemSequentialColors = new ToolStripMenuItem("Sequential") { Image = Resources.sequentialColors };

				contextMenuItemCollections.DropDown.Items.Add(contextMenuItemColorCollections);
				contextMenuItemColorCollections.DropDown.Items.Add(contextMenuItemRandomColors);
				contextMenuItemColorCollections.DropDown.Items.Add(contextMenuItemSequentialColors);

				foreach (ColorCollection collection in _colorCollections)
				{
					if (collection.Color.Any())
					{
						ToolStripMenuItem contextMenuItemRandomColorItem = new ToolStripMenuItem(collection.Name);
						contextMenuItemRandomColorItem.ToolTipText = collection.Description;
						contextMenuItemRandomColorItem.Click += (mySender, myE) => ApplyColorCollection(collection, true);
						contextMenuItemRandomColors.DropDown.Items.Add(contextMenuItemRandomColorItem);

						ToolStripMenuItem contextMenuItemSequentialColorItem = new ToolStripMenuItem(collection.Name);
						contextMenuItemSequentialColorItem.ToolTipText = collection.Description;
						contextMenuItemSequentialColorItem.Click += (mySender, myE) => ApplyColorCollection(collection, false);
						contextMenuItemSequentialColors.DropDown.Items.Add(contextMenuItemSequentialColorItem);	
					}
				}

				if (contextMenuItemCollections.DropDownItems.Count > 0)
				{
					_contextMenuStrip.Items.Add("-");
					_contextMenuStrip.Items.Add(contextMenuItemCollections);
				}
			}
		}

	}
}

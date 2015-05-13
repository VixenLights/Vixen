using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Controls.WpfPropertyGrid.Themes;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Vixen.Execution.Context;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Editor.TimedSequenceEditor.Undo;
using VixenModules.EffectEditor.EffectTypeEditors;
using WeifenLuo.WinFormsUI.Docking;
using Action = System.Action;
using Element = Common.Controls.Timeline.Element;
using Timer = System.Timers.Timer;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FormEffectEditor : DockContent
	{
		private readonly List<Element> _elements = new List<Element>();
		private readonly TimedSequenceEditorForm _sequenceEditorForm;
		private readonly ToolStripButton _toolStripButtonPreview;
		private LiveContext _previewContext;
		private readonly Timer _previewLoopTimer = new Timer();
		private readonly PropertyEditorGrid _propertyEditorGridEffectPropertiesEditor;

		public FormEffectEditor(TimedSequenceEditorForm sequenceEditorForm)
		{
			
			if (System.Windows.Application.Current == null)
			{
				// create the Application object
				new System.Windows.Application();
			}
			ResourceDictionary dict = new ResourceDictionary();

			dict.Source = new Uri("/System.Windows.Controls.WpfPropertyGrid.Themes;component/Kaxaml/Theme.xaml", UriKind.Relative);

			System.Windows.Application.Current.Resources.MergedDictionaries.Add(dict);
			InitializeComponent();
			
			_sequenceEditorForm = sequenceEditorForm;
			var host = new ElementHost { Dock = DockStyle.Fill };

			_propertyEditorGridEffectPropertiesEditor = new PropertyEditorGrid
			{
				Layout = new System.Windows.Controls.WpfPropertyGrid.Design.CategorizedLayout(),
				ShowReadOnlyProperties = true
			};
			//System.Windows.Forms.PropertyGrid
			
			//KaxamlTheme theme = new KaxamlTheme();
			
			host.Child = _propertyEditorGridEffectPropertiesEditor;

			Controls.Add(host);
			//foreach (var control in _propertyEditorGridEffectPropertiesEditor.Controls)
			//{
			//	if (control is ToolStrip)
			//	{
			//		ToolStrip ts = control as ToolStrip;
			//		foreach (ToolStripItem item in ts.Items)
			//		{
			//			item.Visible = false;
			//		}
			//		_toolStripButtonPreview = new ToolStripButton
			//		{
			//			CheckOnClick = true,
			//			ForeColor = Color.WhiteSmoke,
			//			Text = @"Preview"
			//		};
			//		_toolStripButtonPreview.Click += toolStripButtonPreview_Click;
			//		ts.Items.Add(_toolStripButtonPreview);
			//	}
			//}

			sequenceEditorForm.TimelineControl.SelectionChanged += timelineControl_SelectionChanged;
			_propertyEditorGridEffectPropertiesEditor.PropertyValueChanged += PropertyEditorValueChanged;
			_previewLoopTimer.Elapsed += PreviewLoopTimerOnElapsed;
		}

		
		void PropertyEditorValueChanged(object sender, System.Windows.Controls.WpfPropertyGrid.PropertyValueChangedEventArgs e)
		{
			Dictionary<Element, Tuple<Object, PropertyDescriptor>> elementValues = new Dictionary<Element, Tuple<object, PropertyDescriptor>>();

			int i = 0;
			foreach (var element in _elements)
			{
				element.UpdateNotifyContentChanged();
				elementValues.Add(element, new Tuple<object, PropertyDescriptor>(e.OldValue[i], e.Property.UnderLyingPropertyDescriptor(i)));
				i++;
			}

			var undo = new EffectsPropertyModifiedUndoAction(elementValues);
			_sequenceEditorForm.AddEffectsModifiedToUndo(undo);
		}

		internal IEnumerable<Element> Elements
		{
			get { return _elements; }
			set
			{
				_elements.Clear();
				_elements.AddRange(value);
				//SetPreviewState();
				
				_propertyEditorGridEffectPropertiesEditor.SelectedObjects = _elements.Select(x => x.EffectNode.Effect).ToArray();
			} 
		}

		#region Events
		
		private void timelineControl_SelectionChanged(object sender, EventArgs e)
		{
			Elements = _sequenceEditorForm.TimelineControl.SelectedElements;
		}

		#endregion

		#region Preview

		private void SetPreviewState()
		{
			PreviewStop();
			if (_elements.Any() && _toolStripButtonPreview.Checked)
			{
				PreviewPlay();
			}
		}

		
		private void PreviewLoopTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			if (InvokeRequired)
			{
				if (_toolStripButtonPreview.Checked && _elements.Any())
				{
					BeginInvoke(new Action(PreviewPlay));	
				}
				
			}

		}

		private void PreviewPlay()
		{
			if (_previewContext == null)
			{
				_previewContext = VixenSystem.Contexts.CreateLiveContext("Effect Preview");
				_previewContext.Start();

			}
			IEnumerable<EffectNode> orderedNodes = Elements.Select(x => x.EffectNode).OrderBy(x => x.StartTime);
			TimeSpan startOffset = orderedNodes.First().StartTime;
			TimeSpan duration = orderedNodes.Last().EndTime - startOffset;
			List<EffectNode> nodesToPlay = orderedNodes.Select(effectNode => new EffectNode(effectNode.Effect, effectNode.StartTime - startOffset)).ToList();
			_previewContext.Execute(nodesToPlay);
			_previewLoopTimer.Interval = duration.TotalMilliseconds;
			_previewLoopTimer.Start();
		}

		private void PreviewStop()
		{
			_previewLoopTimer.Stop();
			if (_previewContext != null)
			{
				_previewContext.Clear();
			}
		}

		private void toolStripButtonPreview_Click(object sender, EventArgs e)
		{
			TogglePreviewState();
		}

		private void TogglePreviewState()
		{
			if (_toolStripButtonPreview.Checked)
			{
				if (_elements.Any())
				{
					PreviewPlay();
				}
			}
			else
			{
				PreviewStop();
			}
		}

		#endregion
	}

}

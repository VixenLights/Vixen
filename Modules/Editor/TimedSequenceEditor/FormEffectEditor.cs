using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Vixen.Execution.Context;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Editor.EffectEditor;
using VixenModules.Editor.EffectEditor.Editors;
using VixenModules.Editor.TimedSequenceEditor.Undo;
using WeifenLuo.WinFormsUI.Docking;
using Application = System.Windows.Application;
using Element = Common.Controls.Timeline.Element;
using PropertyValueChangedEventArgs = VixenModules.Editor.EffectEditor.PropertyValueChangedEventArgs;
using Timer = System.Timers.Timer;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FormEffectEditor : DockContent
	{
		private readonly List<Element> _elements = new List<Element>();
		private readonly TimedSequenceEditorForm _sequenceEditorForm;
		private LiveContext _previewContext;
		private readonly Timer _previewLoopTimer = new Timer();
		private readonly EffectPropertyEditorGrid _effectPropertyEditorGridEffectEffectPropertiesEditor;
		private bool _previewState;

		public FormEffectEditor(TimedSequenceEditorForm sequenceEditorForm)
		{
			
			if (Application.Current == null)
			{
				// create the Application object
				new Application();
			}
			ResourceDictionary dict = new ResourceDictionary
			{
				Source = new Uri("/VixenModules.Editor.EffectEditor;component/Themes/Theme.xaml", UriKind.Relative)
			};


			Application.Current.Resources.MergedDictionaries.Add(dict);
			InitializeComponent();
			
			_sequenceEditorForm = sequenceEditorForm;
			var host = new ElementHost { Dock = DockStyle.Fill };

			_effectPropertyEditorGridEffectEffectPropertiesEditor = new EffectPropertyEditorGrid
			{
				ShowReadOnlyProperties = true,
				PropertyFilterVisibility = Visibility.Hidden
			};
			
			host.Child = _effectPropertyEditorGridEffectEffectPropertiesEditor;

			Controls.Add(host);
			
			sequenceEditorForm.TimelineControl.SelectionChanged += timelineControl_SelectionChanged;
			_effectPropertyEditorGridEffectEffectPropertiesEditor.PropertyValueChanged += EffectPropertyEditorValueChanged;
			_effectPropertyEditorGridEffectEffectPropertiesEditor.PreviewChanged += EffectPropertyEditorGridEffectEffectPropertiesEditorPreviewChanged;
			_previewLoopTimer.Elapsed += PreviewLoopTimerOnElapsed;
		}

		internal IEnumerable<Element> Elements
		{
			get { return _elements; }
			set
			{
				_elements.Clear();
				_elements.AddRange(value);
				SetPreviewState();
				
				_effectPropertyEditorGridEffectEffectPropertiesEditor.SelectedObjects = _elements.Select(x => x.EffectNode.Effect).ToArray();
			} 
		}

		#region Events
		
		private void timelineControl_SelectionChanged(object sender, EventArgs e)
		{
			Elements = _sequenceEditorForm.TimelineControl.SelectedElements;
		}

		private void EffectPropertyEditorGridEffectEffectPropertiesEditorPreviewChanged(object sender, PreviewStateEventArgs e)
		{
			_previewState = e.State;
			TogglePreviewState();
		}


		private void EffectPropertyEditorValueChanged(object sender, PropertyValueChangedEventArgs e)
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

		#endregion

		#region Preview

		private void SetPreviewState()
		{
			PreviewStop();
			if (_elements.Any() && _previewState)
			{
				PreviewPlay();
			}
		}

		
		private void PreviewLoopTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			if (InvokeRequired)
			{
				if (_previewState && _elements.Any())
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

		private void TogglePreviewState()
		{
			if (_previewState)
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

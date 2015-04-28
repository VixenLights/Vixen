using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using Vixen.Execution.Context;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.EffectEditor.EffectTypeEditors;
using WeifenLuo.WinFormsUI.Docking;
using Action = System.Action;
using Element = Common.Controls.Timeline.Element;
using Timer = System.Timers.Timer;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FormEffectEditor : DockContent
	{
		private readonly Dictionary<Element, EffectModelCandidate> _elements = new Dictionary<Element, EffectModelCandidate>();
		private readonly TimedSequenceEditorForm _sequenceEditorForm;
		private readonly ToolStripButton _toolStripButtonPreview;
		private LiveContext _previewContext;
		private readonly Timer _previewLoopTimer = new Timer();

		public FormEffectEditor(TimedSequenceEditorForm sequenceEditorForm)
		{
			InitializeComponent();
			//This should be refactored out of here and into the actual type classes, but due to some circular dependencies it is not possible until the 
			//old effect editors are removed.
			Hashtable table = new Hashtable
			{
				{typeof (ColorGradient), typeof (EffectColorGradientTypeEditor).AssemblyQualifiedName},
				{typeof (Curve), typeof (EffectCurveTypeEditor).AssemblyQualifiedName}
			};

			TypeDescriptor.AddEditorTable(typeof(UITypeEditor), table);
			//End refactor block

			_sequenceEditorForm = sequenceEditorForm;
			foreach (var control in propertyGridEffectProperties.Controls)
			{
				if (control is ToolStrip)
				{
					ToolStrip ts = control as ToolStrip;
					foreach (ToolStripItem item in ts.Items)
					{
						item.Visible = false;
					}
					_toolStripButtonPreview = new ToolStripButton
					{
						CheckOnClick = true,
						ForeColor = Color.WhiteSmoke,
						Text = @"Preview"
					};
					_toolStripButtonPreview.Click += toolStripButtonPreview_Click;
					ts.Items.Add(_toolStripButtonPreview);
				}
			}

			sequenceEditorForm.TimelineControl.SelectionChanged += timelineControl_SelectionChanged;
			propertyGridEffectProperties.PropertyValueChanged += propertyGridEffectProperties_PropertyValueChanged;
			_previewLoopTimer.Elapsed += PreviewLoopTimerOnElapsed;
		}

		internal IEnumerable<Element> Elements
		{
			get { return _elements.Keys; }
			set
			{
				RemoveElementContentChangedListener();
				_elements.Clear();
				AddElements(value);
				SetPreviewState();
				propertyGridEffectProperties.SelectedObjects = _elements.Keys.Select(x => x.EffectNode.Effect).ToArray();
			} 
		}

		private void AddElements(IEnumerable<Element> elements)
		{
			foreach (var element in elements)
			{
				_elements.Add(element, new EffectModelCandidate(element.EffectNode.Effect));
				AddElementContentChangedListener(element);
			}
		}

		private void AddElementContentChangedListener(Element element)
		{
			
			element.ContentChanged += element_ContentChanged;
			
		}

		private void RemoveElementContentChangedListener()
		{
			foreach (var element in Elements)
			{
				element.ContentChanged -= element_ContentChanged;
			}
		}

		#region Events
		
		private void timelineControl_SelectionChanged(object sender, EventArgs e)
		{
			Elements = _sequenceEditorForm.TimelineControl.SelectedElements;
		}

		private void propertyGridEffectProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			_sequenceEditorForm.AddEffectsModifiedToUndo(new Dictionary<Element, EffectModelCandidate>(_elements), e.ChangedItem.Label);
			var keys = new List<Element>(Elements);
			foreach (var element in keys)
			{
				element.UpdateNotifyContentChanged();
				_elements[element] = new EffectModelCandidate(element.EffectNode.Effect);
			}

		}

		private void element_ContentChanged(object sender, EventArgs e)
		{
			propertyGridEffectProperties.Refresh();
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

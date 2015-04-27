using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Timeline;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.EffectEditor.EffectTypeEditors;
using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FormEffectEditor : DockContent
	{
		//private IEnumerable<Element> _elements = new List<Element>();
		private Dictionary<Element, EffectModelCandidate> _elements = new Dictionary<Element, EffectModelCandidate>();
		private readonly TimedSequenceEditorForm _sequenceEditorForm;
		public FormEffectEditor(TimedSequenceEditorForm sequenceEditorForm)
		{
			InitializeComponent();
			Hashtable table = new Hashtable();
			table.Add(typeof(ColorGradient), typeof(EffectColorGradientTypeEditor).AssemblyQualifiedName);
			table.Add(typeof(Curve), typeof(EffectCurveTypeEditor).AssemblyQualifiedName);
			
			TypeDescriptor.AddEditorTable(typeof(UITypeEditor), table);
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
					ToolStripItem toolStripButtonPreview = new ToolStripButton();
					toolStripButtonPreview.Text = @"Preview";
					toolStripButtonPreview.Click += toolStripButtonPreview_Click;
					ts.Items.Add(toolStripButtonPreview);
				}
			}

			sequenceEditorForm.TimelineControl.SelectionChanged += timelineControl_SelectionChanged;
			propertyGridEffectProperties.PropertyValueChanged += propertyGridEffectProperties_PropertyValueChanged;
		}

		void timelineControl_SelectionChanged(object sender, EventArgs e)
		{
			Elements = _sequenceEditorForm.TimelineControl.SelectedElements;
		}

		void toolStripButtonPreview_Click(object sender, EventArgs e)
		{
			
		}

		void propertyGridEffectProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			_sequenceEditorForm.AddEffectsModifiedToUndo(new Dictionary<Element, EffectModelCandidate>(_elements), e.ChangedItem.Label);
			var keys = new List<Element>(Elements);
			foreach (var element in keys)
			{
				element.UpdateNotifyContentChanged();
				_elements[element]=new EffectModelCandidate(element.EffectNode.Effect);
			}
			
		}

		public IEnumerable<Element> Elements
		{
			get { return _elements.Keys; }
			set
			{
				RemoveElementContentChangedListener();
				_elements.Clear();
				AddElements(value);
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

		private void element_ContentChanged(object sender, EventArgs e)
		{
			propertyGridEffectProperties.Refresh();
		}
	}
}

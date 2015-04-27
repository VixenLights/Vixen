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
		private IEnumerable<Element> _elements = new List<Element>();
		private TimelineControl _timelineControl;
		public FormEffectEditor(TimelineControl timelineControl)
		{
			InitializeComponent();
			Hashtable table = new Hashtable();
			table.Add(typeof(ColorGradient), typeof(EffectColorGradientTypeEditor).AssemblyQualifiedName);
			table.Add(typeof(Curve), typeof(EffectCurveTypeEditor).AssemblyQualifiedName);
			
			TypeDescriptor.AddEditorTable(typeof(UITypeEditor), table);
			_timelineControl = timelineControl;
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

			timelineControl.SelectionChanged += timelineControl_SelectionChanged;
			propertyGridEffectProperties.PropertyValueChanged += propertyGridEffectProperties_PropertyValueChanged;
		}

		void timelineControl_SelectionChanged(object sender, EventArgs e)
		{
			Elements = _timelineControl.SelectedElements;
		}

		void toolStripButtonPreview_Click(object sender, EventArgs e)
		{
			
		}

		void propertyGridEffectProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			foreach (var element in _elements)
			{
				element.UpdateNotifyContentChanged();	
			}
			
		}

		public IEnumerable<Element> Elements
		{
			get { return _elements; }
			set
			{
				RemoveElementContentChangedListener();
				_elements = value;
				AddElementContentChangedListener();
				propertyGridEffectProperties.SelectedObjects = _elements.Select(x => x.EffectNode.Effect).ToArray();
			} 
		}

		private void AddElementContentChangedListener()
		{
			foreach (var element in _elements)
			{
				element.ContentChanged += element_ContentChanged;
			}
		}

		private void RemoveElementContentChangedListener()
		{
			foreach (var element in _elements)
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

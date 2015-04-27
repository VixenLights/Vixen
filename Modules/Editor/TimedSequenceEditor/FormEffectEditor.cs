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
		private IEnumerable<Element> _elements;
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
			_elements = _timelineControl.SelectedElements;
			propertyGridEffectProperties.SelectedObjects = _timelineControl.SelectedElements.Select(x => x.EffectNode.Effect).ToArray();
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
				_elements = value;
				propertyGridEffectProperties.SelectedObjects = _elements.Select(x => x.EffectNode.Effect).ToArray();
			} 
		}
	}
}

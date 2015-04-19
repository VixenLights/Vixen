using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.PropertyEditor;
using Vixen.Module.Effect;
using WeifenLuo.WinFormsUI.Docking;
using Common.Controls.Timeline;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.EffectEditor.EffectTypeEditors;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FormEffectEditor : DockContent
	{
		private IEnumerable<Element> _elements;
		private TimelineControl _timelineControl;
		//private EffectEditor _editor;
		public FormEffectEditor(TimelineControl timelineControl)
		{
			InitializeComponent();
			Hashtable table = new Hashtable();
			table.Add(typeof(ColorGradient), typeof(EffectColorGradientTypeEditor).AssemblyQualifiedName);
			table.Add(typeof(Curve), typeof(EffectCurveTypeEditor).AssemblyQualifiedName);
			
			TypeDescriptor.AddEditorTable(typeof(UITypeEditor), table);
			//propertyGridEffectProperties.AutoScroll = true;
			//_editor = new EffectEditor();
			//_editor.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
			//_editor.AutoSize = true;
			//panelEditControls.Controls.Add(_editor);
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

			propertyGridEffectProperties.PropertyValueChanged += propertyGridEffectProperties_PropertyValueChanged;
		}

		void toolStripButtonPreview_Click(object sender, System.EventArgs e)
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
				//EffectEditor editor = new EffectEditor();
				//_editor.SelectedObjects = _elements.Select(x => x.EffectNode.Effect).ToArray();
				propertyGridEffectProperties.SelectedObjects = _elements.Select(x => x.EffectNode.Effect).ToArray();
			} 
		}
	}
}

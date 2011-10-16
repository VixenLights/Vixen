using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorEffectEditor : Form
	{
		private EffectNode _effectNode;
		private List<IEffectEditorControl> _controls;
		private bool _usedSingleControl;

		public TimedSequenceEditorEffectEditor(EffectNode effectNode)
		{
			InitializeComponent();

			_effectNode = effectNode;
			IEnumerable<IEffectEditorControl> controls = ApplicationServices.GetEffectEditorControls(_effectNode.Effect.Descriptor.TypeId);

			if (controls == null) {
				Label l = new Label();
				l.Text = "Can't find any effect editors that can edit this effect!";
				l.Anchor = AnchorStyles.None;
				tableLayoutPanelEffectEditors.Controls.Add(l);
				tableLayoutPanelEffectEditors.SetColumnSpan(l, 2);
				return;
			}

			_controls = new List<IEffectEditorControl>();
			object[] values = _effectNode.Effect.ParameterValues;

			// if there were multiple controls returned, or there was only a single control needed (ie. the efffect parameters had only
			// a single item) then add controls inside a ParameterEditor wrapper using editors for that type, and label them appropriately.
			// if it was only a single control returned, it must have matched the entire effect (by GUID or signature), so just dump the
			// control it, and let it deal with everything it needs to.
			if (controls.Count() > 1 || (controls.Count() == 1 && (_effectNode.Effect.Descriptor as IEffectModuleDescriptor).Parameters.Count == 1)) {
				_usedSingleControl = false;
				int i = 0;
				foreach (IEffectEditorControl ec in controls) {
					// as it's a single control for a single parameter, it *should* take the corresponding indexed parameter from the effect.
					// so pull that individual parameter out, and give it to the control as a single item. This is a bit of an assumption
					// and seems...... prone to breaking. TODO: review.
					ec.EffectParameterValues = values[i].AsEnumerable().ToArray();

					Label l = new Label();
					l.Width = 1;
					l.Height = 1;
					l.Text = _effectNode.Effect.Parameters[i].Name + ":";
					l.AutoSize = true;
					l.Anchor = AnchorStyles.None;
					tableLayoutPanelEffectEditors.Controls.Add(l);

					(ec as Control).Anchor = AnchorStyles.None;
					tableLayoutPanelEffectEditors.Controls.Add(ec as Control);

					// save the editor control into a list we can use as a reference later on, to pull the data back out of them in the right order.
					_controls.Add(ec);
					i++;
				}
			} else {
				_usedSingleControl = true;
				IEffectEditorControl control = controls.First();
				control.EffectParameterValues = _effectNode.Effect.ParameterValues;
				tableLayoutPanelEffectEditors.Controls.Add(control as Control);
				tableLayoutPanelEffectEditors.SetColumnSpan((control as Control), 2);

				_controls.Add(control);
			}
		}

		private void TimedSequenceEditorEffectEditor_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (DialogResult == System.Windows.Forms.DialogResult.OK && _controls != null) {
				if (_usedSingleControl) {
					_effectNode.Effect.ParameterValues = _controls.First().EffectParameterValues;
				} else {
					object[] values = new object[_controls.Count];
					for (int i = 0; i < _controls.Count; i++) {
						values[i] = _controls[i].EffectParameterValues.First();
					}
					_effectNode.Effect.ParameterValues = values;
				}
			}
		}
	}
}

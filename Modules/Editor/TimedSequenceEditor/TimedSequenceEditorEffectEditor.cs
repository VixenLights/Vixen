using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorEffectEditor : Form
	{
		private EffectNode _effectNode;
		private IEnumerable<EffectNode> _effectNodes;
		private List<IEffectEditorControl> _controls;
		private bool _usedSingleControl;
		private object[] _cleanValues;

		public TimedSequenceEditorEffectEditor(IEnumerable<EffectNode> effectNodes)
			: this(effectNodes.First())
		{
			if (effectNodes != null && effectNodes.Count() > 1) {
				_effectNodes = effectNodes;
				Text = "Edit Multiple Effects";

				// show a warning if multiple effect types are selected
				EffectNode displayedEffect = effectNodes.First();
				if (displayedEffect != null) {
					foreach (EffectNode node in effectNodes) {
						if (node.Effect.TypeId != displayedEffect.Effect.TypeId) {
							MessageBox.Show("The selected effects contain multiple types. Once you finish editing, these values will " +
							                "only be applied to the effects of type '" + displayedEffect.Effect.Descriptor.TypeName + "'.",
							                "Warning", MessageBoxButtons.OK);
							break;
						}
					}
				}
			}
		}

		public TimedSequenceEditorEffectEditor(EffectNode effectNode)
		{
			InitializeComponent();

			_effectNode = effectNode;
			_effectNodes = null;
			IEnumerable<IEffectEditorControl> controls =
				ApplicationServices.GetEffectEditorControls(_effectNode.Effect.Descriptor.TypeId);

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
			if (_effectNode.Effect.EffectName.Equals("Nutcracker"))
			{
				var data = _effectNode.Effect.ParameterValues.First() as ICloneable;
				values = new []{data.Clone()};
			}
			
			
			_cleanValues = _effectNode.Effect.ParameterValues;

			// if there were multiple controls returned, or there was only a single control needed (ie. the efffect parameters had only
			// a single item) then add controls inside a ParameterEditor wrapper using editors for that type, and label them appropriately.
			// if it was only a single control returned, it must have matched the entire effect (by GUID or signature), so just dump the
			// control it, and let it deal with everything it needs to.
			if (controls.Count() > 1 ||
			    (controls.Count() == 1 && (_effectNode.Effect.Descriptor as IEffectModuleDescriptor).Parameters.Count == 1)) {
				_usedSingleControl = false;
				int i = 0;
				foreach (IEffectEditorControl ec in controls) {
					// as it's a single control for a single parameter, it *should* take the corresponding indexed parameter from the effect.
					// so pull that individual parameter out, and give it to the control as a single item. This is a bit of an assumption
					// and seems...... prone to breaking. TODO: review.
					ec.EffectParameterValues = values[i].AsEnumerable().ToArray();

					ec.TargetEffect = effectNode.Effect;

					if (_effectNode.Effect.Parameters[i].ShowLabel) {
						Label l = new Label();
						l.Width = 1;
						l.Height = 1;
						l.Text = string.Format("{0}:", _effectNode.Effect.Parameters[i].Name );
						l.AutoSize = true;
						l.Anchor = AnchorStyles.None;
						tableLayoutPanelEffectEditors.Controls.Add(l);
					}

					(ec as Control).Anchor = AnchorStyles.None;
					tableLayoutPanelEffectEditors.Controls.Add(ec as Control);

					// save the editor control into a list we can use as a reference later on, to pull the data back out of them in the right order.
					_controls.Add(ec);
					i++;
				}
			}
			else {
				_usedSingleControl = true;
				IEffectEditorControl control = controls.First();
				control.EffectParameterValues = _effectNode.Effect.ParameterValues;
				control.TargetEffect = effectNode.Effect;
				tableLayoutPanelEffectEditors.Controls.Add(control as Control);
				tableLayoutPanelEffectEditors.SetColumnSpan((control as Control), 2);

				_controls.Add(control);
			}
		}

		private void TimedSequenceEditorEffectEditor_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (DialogResult == System.Windows.Forms.DialogResult.OK && _controls != null)
			{
				IEnumerable<EffectNode> nodes = _effectNodes ?? new EffectNode[] { _effectNode };

				int changedEFfects = 0;
				foreach (EffectNode node in nodes)
				{
					if (node.Effect.TypeId != _effectNode.Effect.TypeId)
						continue;

					if (_usedSingleControl)
					{
						if (node.Effect.EffectName == "Nutcracker")
						{
							node.Effect.ParameterValues = _controls.First().EffectParameterValues;
							continue;
						}

						object[] values = new object[_controls.First().EffectParameterValues.Count()];
						for (int i = 0; i < _controls.First().EffectParameterValues.Count(); i++)
						{
							if (Object.Equals(_controls.First().EffectParameterValues[i], _cleanValues[i]))
							{
								values[i] = node.Effect.ParameterValues[i];
							}
							else
							{
								values[i] = _controls.First().EffectParameterValues[i];
							}
						}
						node.Effect.ParameterValues = values; //_controls.First().EffectParameterValues;
					}
					else
					{
						object[] values = new object[_controls.Count];
						for (int i = 0; i < _controls.Count; i++)
						{
							if (Object.Equals(_controls[i].EffectParameterValues.First(), _cleanValues[i]) && node.Effect.EffectName != "Nutcracker")
							{
								values[i] = node.Effect.ParameterValues[i];
							}
							else
							{
								values[i] = _controls[i].EffectParameterValues.First();
							}
						}
						node.Effect.ParameterValues = values;
					}
					changedEFfects++;
				}

				if (nodes.Count() > 1)
				{
					MessageBox.Show(changedEFfects + " effects modified.", "", MessageBoxButtons.OK);
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Vixen.Sys;
using Vixen.Module.Effect;

namespace VixenTestbed {
	partial class Form1 {
		private IEffectModuleInstance _SelectedEffectModule {
			get { return moduleListEffect.GetSelectedModule<IEffectModuleInstance>(); }
		}

		private IEnumerable<ChannelNode> _SelectedEffectTargetNodes {
			get { return checkedListBoxEffectTargetNodes.CheckedItems.Cast<ChannelNode>(); }
		}

		private void moduleListEffect_SelectedModuleChanged(object sender, EventArgs e) {
			buttonRenderEffect.Enabled = _ValidateEffectControls();
			if(_SelectedEffectModule != null) {
				using(Graphics g = pictureBoxEffectImage.CreateGraphics()) {
					IEffectModuleDescriptor descriptor = _SelectedEffectModule.Descriptor as IEffectModuleDescriptor;
					pictureBoxEffectImage.Image = descriptor.GetRepresentativeImage(pictureBoxEffectImage.Width, pictureBoxEffectImage.Height);
				}
			}
		}

		private void checkedListBoxEffectTargetNodes_ItemCheck(object sender, ItemCheckEventArgs e) {
			buttonRenderEffect.Enabled = _ValidateEffectControls(e.NewValue == CheckState.Checked);
		}

		private bool _ValidateEffectControls(bool hasNewlyCheckedItem = false) {
			return
				_SelectedEffectModule != null &&
				(checkedListBoxEffectTargetNodes.CheckedItems.Count > 0 || hasNewlyCheckedItem);
		}

		private void buttonRenderEffect_Click(object sender, EventArgs e) {
			try {
				using(EffectEditorContainerForm editorContainerForm = new EffectEditorContainerForm(_SelectedEffectModule)) {
					if(editorContainerForm.ShowDialog() == DialogResult.OK) {
						IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(_SelectedEffectModule.Descriptor.TypeId);
						TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)numericUpDownEffectRenderTimeSpan.Value);

						effect.TargetNodes = _SelectedEffectTargetNodes.ToArray();
						effect.TimeSpan = timeSpan;
						effect.ParameterValues = editorContainerForm.GetValues();

						try {
							EffectNode commandNode = new EffectNode(effect, TimeSpan.Zero);
							Vixen.Sys.Execution.Write(new[] { commandNode });
						} catch(Exception ex) {
							MessageBox.Show(ex.Message);
						}
					}
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
	}
}

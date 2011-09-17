using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Transform;
using Vixen.Sys;
using CommandStandard.Types;

namespace TestTransform {
	public class Dimming : TransformModuleInstanceBase {
		private DimmingCurve _identityCurve;
		private DimmingCurve _curve = null;
		private DimmingData _moduleData;

		public Dimming() {
			_curve = _identityCurve = new DimmingCurve("Identity");
		}

		override public IModuleDataModel ModuleData {
			get { return _moduleData; }
			set {
				if((_moduleData = value as DimmingData) != null) {
					_curve = DimmingCurve.Load(_moduleData.DimmingCurveName) ?? _identityCurve;
				}
			}
		}

		public string DimmingCurveName {
			get { return _moduleData.DimmingCurveName; }
			set { _moduleData.DimmingCurveName = value; }
		}

		override public void Transform(Command command) {
			CommandParameterReference paramRef;
			if(_curve != null && !command.IsEmpty && CommandsAffected.TryGetValue(command.CommandIdentifier, out paramRef)) {
				foreach(int index in paramRef.ParameterIndexes) {
					// Get the existing value.
					Level level = (Level)command.ParameterValues[index];
					// Lookup the new value and replace.
					double value;
					if(_curve.Find(level, out value)) {
						level = value;
					}
					command.ParameterValues[index] = level;
				}
			}
		}

		override public void Setup() {
			using(SetupDialog setupDialog = new SetupDialog()) {
				setupDialog.DimmingCurveName = this.DimmingCurveName;
				if(setupDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					this.DimmingCurveName = setupDialog.DimmingCurveName;
				}
			}
		}
	}
}

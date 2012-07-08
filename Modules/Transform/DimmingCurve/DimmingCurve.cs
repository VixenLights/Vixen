using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Transform;
using VixenModules.App.Curves;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.Transform.DimmingCurve
{
	class DimmingCurve : TransformModuleInstanceBase
	{
		DimmingCurveData _data;

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as DimmingCurveData; }
		}

		public override void Setup()
		{
			if (_data == null) {
				VixenSystem.Logging.Warning("DimmingCurve: Null Data object when trying to Setup().");
				return;
			}

			CurveEditor editor = new CurveEditor(_data.Curve);
			if (editor.ShowDialog() == DialogResult.OK) {
				_data.Curve = new CachingCurve(editor.Curve);
			}
		}

		public override void Transform(Command command)
		{
			CommandParameterReference paramRef = _GetAffectedParameters(command);
			if (_data != null && _data.Curve != null) {
				foreach (int index in paramRef.ParameterIndexes) {
					command.SetParameterValue(index, (Level)_data.Curve.GetValue((Level)command.GetParameterValue(index)));
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Commands;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.RDS {
	public class RDSModule : EffectModuleInstanceBase {
		private EffectIntents _elementData = null;
		private RDSData _data;

		public RDSModule() {
			_data = new RDSData();
#if DEBUG
			_data.Text = DateTime.Now.ToString();
#endif
		}

		protected override void _PreRender() {
			_elementData = new EffectIntents();
#if DEBUG
			if (string.IsNullOrWhiteSpace(_data.Text))
				_data.Text="Debug Command " + DateTime.Now.ToString();
#endif
			//var intent = new CommandIntent(new Vixen.Data.Value.CommandValue(new UnknownValueCommand(_data.Text)), this.TimeSpan);
			var intent = new DynamicIntent(new Vixen.Data.Value.DynamicValue() { Value= _data.Text }, TimeSpan);
			foreach (ElementNode node in TargetNodes.Where(n => n != null)) {
				_elementData.AddIntentForElement(node.Id, intent, TimeSpan.Zero);

			}
		}

		protected override Vixen.Sys.EffectIntents _Render() {
			return _elementData;
		}
		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as RDSData; }
		}
	}
}

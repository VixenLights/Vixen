using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.RDS
{
	public class RDSModule : EffectModuleInstanceBase
	{
		private EffectIntents _elementData = null;
		private RDSData _data;

		public RDSModule()
		{
			_data = new RDSData();
#if DEBUG
			_data.Title = DateTime.Now.ToString();
#endif
		}

		protected override void _PreRender()
		{
			_elementData = new EffectIntents();
#if DEBUG
			if (string.IsNullOrWhiteSpace(_data.Title))
				_data.Title="Debug Command " + DateTime.Now.ToString();
#endif

			CommandValue value = new CommandValue(new StringCommand(_data.Title));

			foreach (ElementNode node in TargetNodes) {
				IIntent i = new CommandIntent(value, TimeSpan);
				_elementData.AddIntentForElement(node.Element.Id, i, TimeSpan.Zero);
			}
		}

		protected override Vixen.Sys.EffectIntents _Render()
		{
			return _elementData;
		}
		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as RDSData; }
		}
	}
}

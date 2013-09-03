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
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.RDS
{
	public class RDSModule : EffectModuleInstanceBase
	{
		private EffectIntents _elementData = null;
		private RDSData _data;

		public RDSModule()
		{
			_data = new RDSData();

		}

		protected override void _PreRender()
		{
			_elementData = new EffectIntents();

			CommandValue value = new CommandValue(new StringCommand(_data.Title));

			foreach (ElementNode node in TargetNodes) {
				IIntent i = new CommandIntent(value, TimeSpan);
				_elementData.AddIntentForElement(node.Element.Id, i, TimeSpan.Zero);
			}
		}

		[Value]
		public string Title
		{
			get { return _data.Title; }
			set
			{
				_data.Title=value;
				IsDirty=true;
			}
		}

		[Value]
		public string Artist
		{
			get
			{
				return _data.Artist;
			}
			set
			{
				_data.Artist=value;
				IsDirty=true;
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

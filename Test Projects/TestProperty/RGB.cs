using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Property;

namespace TestProperty {
	public class RGB : PropertyModuleInstanceBase {
		private RGBData _data;

		override public void Setup() {
			OutputChannel[] channels = Owner.GetChannelEnumerator().ToArray();

			using(RGBSetup setup = new RGBSetup(_data, channels)) {
				setup.ShowDialog();
			}
		}
		
		override public IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as RGBData; }
		}

		public Guid RedChannelId {
			get { return _data.RedChannelId; }
		}

		public Guid GreenChannelId {
			get { return _data.GreenChannelId; }
		}

		public Guid BlueChannelId {
			get { return _data.BlueChannelId; }
		}
	}
}

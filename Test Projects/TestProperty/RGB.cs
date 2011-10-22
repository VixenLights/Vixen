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

		public override void SetDefaultValues() {
			Channel[] channels = Owner.GetChannelEnumerator().ToArray();
			_data.RedChannelId = (channels.Length > 0) ? channels[0].Id : Guid.Empty;
			_data.GreenChannelId = (channels.Length > 1) ? channels[1].Id : Guid.Empty;
			_data.BlueChannelId = (channels.Length > 2) ? channels[2].Id : Guid.Empty;
		}

		public override bool HasSetup {
			get {
				return true;
			}
		}
		override public void Setup() {
			Channel[] channels = Owner.GetChannelEnumerator().ToArray();

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

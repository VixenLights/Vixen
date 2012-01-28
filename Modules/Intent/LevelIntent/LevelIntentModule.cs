using System;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module;
using Vixen.Module.Intent;
using Vixen.Sys;

namespace LevelIntent {
	public class LevelIntentModule : IntentModuleInstanceBase {
		private LevelIntentData _data;

		public override Command GetCurrentState(TimeSpan timeOffset) {
			if(timeOffset < TimeSpan) {
				double percent = timeOffset.TotalMilliseconds / TimeSpan.TotalMilliseconds;
				Level level = StartLevel + (EndLevel - StartLevel) * percent;
				return new Lighting.Monochrome.SetLevel(level);
			}
			return null;
		}

		public override TimeSpan TimeSpan { get; set; }

		//public override object[] Values {
		//    get {
		//        return new object[] {
		//            StartLevel,
		//            EndLevel
		//        };
		//    }
		//    set {
		//        if(value.Length != 2) throw new InvalidOperationException("Incorrect number of parameters.");
		//        StartLevel = value[0].DynamicCast<Level>();
		//        EndLevel = value[1].DynamicCast<Level>();
		//    }
		//}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = (LevelIntentData)value; }
		}

		[Value]
		public Level StartLevel {
			get { return _data.StartLevel; }
			set { _data.StartLevel = value; }
		}

		[Value]
		public Level EndLevel {
			get { return _data.EndLevel; }
			set { _data.EndLevel = value; }
		}
	}
}

using System;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module.PreFilter;

namespace FadeOut {
	public class FadeOutModule : PreFilterModuleInstanceBase {
		public override Command Affect(Command command, TimeSpan filterRelativeTime) {
			Lighting.Monochrome.SetLevel setLevelCommand = command as Lighting.Monochrome.SetLevel;
			if(setLevelCommand != null) {
				double percent = filterRelativeTime.TotalMilliseconds / TimeSpan.TotalMilliseconds;
				// The command instance is likely being cached, so don't modify the original.
				Level newLevel = setLevelCommand.Level - setLevelCommand.Level * percent;
				setLevelCommand = new Lighting.Monochrome.SetLevel(newLevel);
			}
			return setLevelCommand;
		}
	}
}

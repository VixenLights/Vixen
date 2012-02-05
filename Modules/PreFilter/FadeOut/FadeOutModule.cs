using System;
using System.Drawing;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module.PreFilter;

namespace FadeOut {
	public class FadeOutModule : PreFilterModuleInstanceBase {
		public override Command Affect(Command command, TimeSpan filterRelativeTime) {
			double percentOff = filterRelativeTime.TotalMilliseconds / TimeSpan.TotalMilliseconds;
			double percentOn = 1d - percentOff;

			Lighting.Monochrome.SetLevel setLevelCommand = command as Lighting.Monochrome.SetLevel;
			if(setLevelCommand != null) {
				// The command instance is likely being cached, so don't modify the original.
				Level newLevel = setLevelCommand.Level - setLevelCommand.Level * percentOff;
				command = new Lighting.Monochrome.SetLevel(newLevel);
			} else {
				Lighting.Polychrome.SetColor setColorCommand = command as Lighting.Polychrome.SetColor;
				if(setColorCommand != null) {
					Color newColor = Color.FromArgb((int)(setColorCommand.Color.R * percentOn), (int)(setColorCommand.Color.G * percentOn), (int)(setColorCommand.Color.B * percentOn));
					command = new Lighting.Polychrome.SetColor(newColor);
				}
			}
			return command;
		}
	}
}

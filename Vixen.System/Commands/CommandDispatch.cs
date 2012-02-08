namespace Vixen.Commands {
	abstract public class CommandDispatch {
		public virtual void DispatchCommand(Lighting.Monochrome.SetLevel command) {
		}

		public virtual void DispatchCommand(Lighting.Polychrome.SetColor command) {
		}

		public virtual void DispatchCommand(Animatronics.BasicPositioning.SetPosition command) {
		}

		public virtual void DispatchCommand(Animatronics.TimedPositioning.SetPosition command) {
		}
	}
}

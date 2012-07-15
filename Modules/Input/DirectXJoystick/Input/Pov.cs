using SlimDX.DirectInput;

namespace VixenModules.Input.DirectXJoystick.Input {
	class Pov : JoystickInput {
		private IPovBehavior _povBehavior;

		public enum PovType {
			Cardinal,
			Polar
		};

		public Pov(string name, int index, PovType type = PovType.Polar)
			: base(name) {
			PovIndex = index;
			Type = type;
		}

		public int PovIndex { get; private set; }

		private PovType _povType;

		public PovType Type {
			get { return _povType; }
			set {
				_povType = value;
				switch(_povType) {
					case PovType.Cardinal:
						_povBehavior = new CardinalPovBehavior();
						break;
					case PovType.Polar:
						_povBehavior = new PolarPovBehavior();
						break;
				}
			}
		}

		protected override double _GetValue(JoystickState joystickState) {
			return _povBehavior.GetValue(joystickState, PovIndex);
		}
	}
}

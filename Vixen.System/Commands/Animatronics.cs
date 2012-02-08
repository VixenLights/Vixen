using System;
using Vixen.Commands.KnownDataTypes;

namespace Vixen.Commands {
	public class Animatronics {
		static public readonly byte Value = 2;

		public class BasicPositioning {
			static public readonly byte Value = 0;

			public class SetPosition : Command {
				private ParameterSignature _signature = new ParameterSignature(
					new ParameterSpecification("Position", typeof(Position)));
				
				static public readonly byte Value = 0;

				static private CommandIdentifier _identifier = new CommandIdentifier(Animatronics.Value, BasicPositioning.Value, SetPosition.Value);

				static public CommandIdentifier CommandIdentifier { get { return _identifier; } }

				public SetPosition() { }

				public SetPosition(Position position) {
					Position = position;
				}

				override public CommandIdentifier Identifier { get { return CommandIdentifier; } }

				public override ParameterSignature Signature {
					get { return _signature; }
				}

				public override object GetParameterValue(int index) {
					return Position;
				}

				public override void SetParameterValue(int index, object value) {
					if(value is Position) {
						Position = (Position)value;
					}
				}

				public Position Position { get; set; }

				// Won't know the intent of the positioning, such as which direction
				// it's going in, so we're going to allow the default behavior (first wins).
				//public override Command Combine(Command other) {
				//    SetPosition otherCommand = other as SetPosition;

				//}

				public override Command Clone() {
					return new SetPosition(Position);
				}

				// Must be done in the derived classes.
				public override void Dispatch(CommandDispatch commandDispatch) {
					if(commandDispatch != null)
						commandDispatch.DispatchCommand(this);
				}
			}
		}

		public class TimedPositioning {
			static public readonly byte Value = 1;

			public class SetPosition : Command {
				private ParameterSignature _signature = new ParameterSignature(
					new ParameterSpecification("Position", typeof(Position)),
					new ParameterSpecification("TimeSpan", typeof(TimeSpan)));

				static public readonly byte Value = 0;

				static private CommandIdentifier _identifier = new CommandIdentifier(Animatronics.Value, TimedPositioning.Value, SetPosition.Value);

				static public CommandIdentifier CommandIdentifier { get { return _identifier; } }

				public SetPosition() { }

				public SetPosition(Position position, TimeSpan timeSpan) {
					Position = position;
					TimeSpan = timeSpan;
				}

				override public CommandIdentifier Identifier { get { return CommandIdentifier; } }

				public override ParameterSignature Signature {
					get { return _signature; }
				}

				public override object GetParameterValue(int index) {
					switch(index) {
						case 0:
							return Position;
						case 1:
							return TimeSpan;
					}

					return null;
				}

				public override void SetParameterValue(int index, object value) {
					switch(index) {
						case 0:
							if(value is Position) {
								Position = (Position)value;
							}
							break;
						case 1:
							if(value is TimeSpan) {
								TimeSpan = (TimeSpan)value;
							}
							break;
					}
				}

				public Position Position { get; set; }

				public TimeSpan TimeSpan { get; set; }

				// Won't know the intent of the positioning, such as which direction
				// it's going in, so we're going to allow the default behavior (first wins).
				//public override Command Combine(Command other) {
				//    SetPosition otherCommand = other as SetPosition;

				//}

				public override Command Clone() {
					return new SetPosition(Position, TimeSpan);
				}

				// Must be done in the derived classes.
				public override void Dispatch(CommandDispatch commandDispatch) {
					if(commandDispatch != null)
						commandDispatch.DispatchCommand(this);
				}
			}
		}

		public class Custom {
			static public readonly byte Value = CustomCommandBehavior.Value;

			static private ICustomCommandBehavior _behavior = new CustomCommandBehavior();

			static public Command Get(string extensionName) {
				return _behavior.Get(Animatronics.Value, extensionName);
			}
		}
	}
}

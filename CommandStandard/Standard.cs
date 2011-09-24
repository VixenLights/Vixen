using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CommandStandard.Types;

namespace CommandStandard {
	static public class Standard {
		public const byte CustomCategory = 0xFF;

		static public string AssemblyFileName {
			get { return Assembly.GetExecutingAssembly().Location; }
		}

		#region Private methods
		static private bool _IsPlatform(Type type) {
			return (type != null) ? type.HasAttribute(typeof(PlatformAttribute)) : false;
		}

		static private bool _IsCategory(Type type) {
			return (type != null) ? type.HasAttribute(typeof(CategoryAttribute)) : false;
		}

		static private bool _IsCommand(Type type) {
			return (type != null) ? type.HasAttribute(typeof(CommandAttribute)) : false;
		}

		static private string _GetCommandId(Type commandType) {
			// Construct by climbing the parental hierarchy
			if(_IsCommand(commandType)) {
				Type categoryType = _GetCategory(commandType);
				Type platformType = _GetPlatform(categoryType);
				return CommandIdentifier.FormatCommandIdentifierString(
					_GetPlatformValue(platformType),
					_GetCategoryValue(categoryType),
					_GetCommandValue(commandType)
					);
			}
			return string.Empty;
		}

		static private Type _FindPlatform(byte platform) {
			Type type = typeof(Standard).FindAttributedTypes(typeof(PlatformAttribute)).FirstOrDefault(x => x.HasFieldOfType<byte>(typeof(ValueAttribute), platform));
			if(type == null) throw new InvalidOperationException("Invalid platform value.");
			return type;
		}

		static private Type _FindCategory(Type platformType, byte category) {
			Type type = platformType.FindAttributedTypes(typeof(CategoryAttribute)).FirstOrDefault(x => x.HasFieldOfType<byte>(typeof(ValueAttribute), category));
			if(type == null) throw new InvalidOperationException("Invalid category value.");
			return type;
		}

		static private Type _FindCommand(Type categoryType, byte index) {
			Type type = categoryType.FindAttributedTypes(typeof(CommandAttribute)).FirstOrDefault(x => x.HasFieldOfType<byte>(typeof(ValueAttribute), index));
			if(type == null) throw new InvalidOperationException("Invalid command value.");
			return type;
		}

		static private byte _GetValue(Type standardClassType) {
			return (byte)standardClassType.GetField("Value").GetValue(null);
		}

		static private byte _GetPlatformValue(Type platformType) {
			if(_IsPlatform(platformType)) {
				return (byte)platformType.GetField("Value").GetValue(null);
			}
			return 0;
		}

		static private byte _GetCategoryValue(Type categoryType) {
			if(_IsCategory(categoryType)) {
				return (byte)categoryType.GetField("Value").GetValue(null);
			}
			return 0;
		}

		static private byte _GetCommandValue(Type commandType) {
			if(_IsCommand(commandType)) {
				return (byte)commandType.GetField("Value").GetValue(null);
			}
			return 0;
		}

		static private Type[] _GetPlatformCategories(Type platformType) {
			if(_IsPlatform(platformType)) {
				return platformType.FindAttributedTypes(typeof(CategoryAttribute)).ToArray();
			}
			return null;
		}

		static private Type[] _GetCategoryCommands(Type categoryType) {
			if(_IsCategory(categoryType)) {
				return categoryType.FindAttributedTypes(typeof(CommandAttribute)).ToArray();
			}
			return null;
		}

		static private CommandParameterSpecification[] _GetCommandParameters(Type commandType) {
			if(_IsCommand(commandType)) {
				object value = null;
				CommandParameterSignature parameters =
					(from fi in commandType.GetFields(BindingFlags.Static | BindingFlags.Public)
					 where ((value = fi.GetValue(null)) != null) && (value is CommandParameterSignature)
					 select value as CommandParameterSignature).FirstOrDefault();
				// Valid command, but no parameters specified.
				if(parameters == null) return new CommandParameterSpecification[] { };
				return parameters.ToArray();
			}
			// Invalid command.
			return null;
		}

		static private Type _GetCategory(Type commandType) {
			Type declaringType = commandType.DeclaringType;
			return _IsCategory(declaringType) ? declaringType : null;
		}

		static private Type _GetPlatform(Type categoryType) {
			Type declaringType = categoryType.DeclaringType;
			return _IsPlatform(declaringType) ? declaringType : null;
		}

		static private CommandSignature _GetCommandSignature(Type commandType) {
			// Name
			string name = commandType.Name;

			// Parameters
			CommandParameterSpecification[] parameters = _GetCommandParameters(commandType);

			// Ancestry
			Type categoryType = _GetCategory(commandType);
			Type platformType = _GetPlatform(categoryType);
			byte platformValue = _GetPlatformValue(platformType);
			byte categoryValue = _GetCategoryValue(categoryType);
			byte commandValue = _GetCommandValue(commandType);

			return new CommandSignature(name, platformValue, categoryValue, commandValue, parameters);
		}
		#endregion

		#region Public methods
		static public IEnumerable<byte> GetPlatformValues() {
			return
				(from platformType in typeof(Standard).FindAttributedTypes(typeof(PlatformAttribute))
				 select _GetValue(platformType));
		}

		static public IEnumerable<byte> GetCategoryValues(byte platformValue) {
			Type platformType = _FindPlatform(platformValue);
			return
				(from categoryType in platformType.FindAttributedTypes(typeof(CategoryAttribute))
				 select _GetValue(categoryType));
		}

		static public IEnumerable<byte> GetCommandValues(byte platformValue, byte categoryValue) {
			Type platformType = _FindPlatform(platformValue);
			Type categoryType = _FindCategory(platformType, categoryValue);
			return
				(from commandType in categoryType.FindAttributedTypes(typeof(CommandAttribute))
				 select _GetValue(commandType));
		}

		static public CommandSignature[] GetCommandSignatures(byte platformValue, byte categoryValue) {
			Type platformType;
			Type categoryType;

			if((platformType = _FindPlatform(platformValue)) != null) {
				if((categoryType = _FindCategory(platformType, categoryValue)) != null) {
					return _GetCategoryCommands(categoryType).Select(x => _GetCommandSignature(x)).ToArray();
				}
			}
			return null;
		}

		static public CommandSignature GetCommandSignature(byte platformValue, byte categoryValue, byte commandValue) {
			string name = null;
			CommandParameterSpecification[] parameters = null;

			Type platformType;
			Type categoryType;
			Type commandType;

			if((platformType = _FindPlatform(platformValue)) != null) {
				if((categoryType = _FindCategory(platformType, categoryValue)) != null) {
					if((commandType = _FindCommand(categoryType, commandValue)) != null) {
						name = commandType.Name;
						parameters = _GetCommandParameters(commandType);
					} else {
						return null;
					}
				}
			}

			return new CommandSignature(name, platformValue, categoryValue, commandValue, parameters);
		}

		static public CommandParameterCombiner GetCommandParameterCombiner(byte platformValue, byte categoryValue, byte commandValue) {
			//*** make a better way than a massive switch statement
			switch(platformValue) {
				case Lighting.Value:
					switch(categoryValue) {
						case Lighting.Monochrome.Value:
							switch(commandValue) {
								case Lighting.Monochrome.SetLevel.Value:
									return Lighting.Monochrome.SetLevel.Combine;
							}
							break;
						case Lighting.Polychrome.Value:
							switch(commandValue) {
								case Lighting.Polychrome.SetColor.Value:
									break;
							}
							break;
					}
					break;
				case Media.Value:
					break;
				case Animatronics.Value:
					break;
				case Readerboard.Value:
					break;
			}

			return null;
		}

		static public string GetPlatformName(byte platformValue) {
			return _FindPlatform(platformValue).Name;
		}

		static public string GetCategoryName(byte platformValue, byte categoryValue) {
			return _FindCategory(_FindPlatform(platformValue), categoryValue).Name;
		}

		static public string GetCommandName(byte platformValue, byte categoryValue, byte commandIndex) {
			return _FindCommand(_FindCategory(_FindPlatform(platformValue), categoryValue), commandIndex).Name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="leftParams"></param>
		/// <param name="rightParams"></param>
		/// <returns>Parameters for the combined value.</returns>
		static public object[] Combine(CommandIdentifier command, object[] leftParams, object[] rightParams) {
			// Account for null command, which means it has no effect.
			if(command == null) return leftParams;

			CommandParameterCombiner combiner = GetCommandParameterCombiner(command.Platform, command.Category, command.CommandIndex);
			if(combiner != null) {
				return combiner(leftParams, rightParams);
			}

			// Default to the left-hand parameters.
			return leftParams;
		}
		#endregion

		#region Standard implementation

		[Platform]
		static public class Lighting {
			[Value]
			public const byte Value = 0;
			[Category]
			static public class Monochrome {
				[Value]
				public const byte Value = 0;

				[Command]
				static public class SetLevel {
					[Value]
					public const byte Value = 0;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("level", typeof(Types.Level))
						});
					static public string Id = _GetCommandId(typeof(SetLevel));
					static public object[] Combine(object[] leftParams, object[] rightParams) {
						if(leftParams == null || leftParams.Length == 0) return rightParams;
						if(rightParams == null || rightParams.Length == 0) return leftParams;
						double leftLevel = (Level)leftParams[0];
						double rightLevel = (Level)rightParams[0];
						return new object[] { Math.Max(leftLevel, rightLevel) };
					}
				}
			}

			[Category]
			static public class Polychrome {
				[Value]
				public const byte Value = 1;

				[Command]
				static public class SetColor {
					[Value]
					public const byte Value = 0;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("color", typeof(Types.Color))
						});
					static public string Id = _GetCommandId(typeof(SetColor));
				}
			}

			[Category]
			static public class Custom {
				[Value]
				public const byte Value = CustomCategory;
			}

		}

		[Platform]
		static public class Media {
			[Value]
			public const byte Value = 1;

			[Category]
			static public class Audio {
				[Value]
				public const byte Value = 0;

				// *Nothing that affects execution or positioning*

				[Command]
				static public class FadeUp {
					[Value]
					public const byte Value = 4;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("time", typeof(Types.Time))
						});
					static public string Id = _GetCommandId(typeof(FadeUp));
				}
				[Command]
				static public class FadeDown {
					[Value]
					public const byte Value = 5;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("time", typeof(Types.Time))
						});
					static public string Id = _GetCommandId(typeof(FadeDown));
				}
				[Command]
				static public class SetVolume {
					[Value]
					public const byte Value = 6;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("level", typeof(Types.Level))
						});
					static public string Id = _GetCommandId(typeof(SetVolume));
				}
			}

			[Category]
			static public class Video {
				[Value]
				public const byte Value = 1;

				// *Nothing that affects execution or positioning*

				[Command]
				static public class FadeUp {
					[Value]
					public const byte Value = 4;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("time", typeof(Types.Time))
						});
					static public string Id = _GetCommandId(typeof(FadeUp));
				}
				[Command]
				static public class FadeDown {
					[Value]
					public const byte Value = 5;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("time", typeof(Types.Time))
						});
					static public string Id = _GetCommandId(typeof(FadeDown));
				}
			}

			[Category]
			static public class Custom {
				[Value]
				public const byte Value = CustomCategory;
			}
		}

		[Platform]
		static public class Animatronics {
			[Value]
			public const byte Value = 2;

			[Category]
			static public class BasicPositioning {
				[Value]
				public const byte Value = 0;

				[Command]
				static public class SetPosition {
					[Value]
					public const byte Value = 0;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("position", typeof(Types.Position))
						});
					static public string Id = _GetCommandId(typeof(SetPosition));
				}
			}

			[Category]
			static public class TimedPositioning {
				[Value]
				public const byte Value = 1;

				[Command]
				static public class SetPosition {
					[Value]
					public const byte Value = 0;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("position", typeof(Types.Position)),
							new CommandParameterSpecification("time", typeof(Types.Time))
						});
					static public string Id = _GetCommandId(typeof(SetPosition));
				}
			}

			[Category]
			static public class Custom {
				[Value]
				public const byte Value = CustomCategory;
			}
		}

		[Platform]
		static public class Readerboard {
			[Value]
			public const byte Value = 3;

			[Category]
			static public class Text {
				[Value]
				public const byte Value = 0;

				[Command]
				static public class ScrollText {
					[Value]
					public const byte Value = 0;
					static public CommandParameterSignature Parameters =
						new CommandParameterSignature(new CommandParameterSpecification[] {
							new CommandParameterSpecification("text", typeof(string)),
							new CommandParameterSpecification("extent", typeof(int)),
							new CommandParameterSpecification("direction", typeof(int)),
							new CommandParameterSpecification("time", typeof(Types.Time))
						});
					static public string Id = _GetCommandId(typeof(ScrollText));
				}
			}


			[Category]
			static public class Custom {
				[Value]
				public const byte Value = CustomCategory;
			}
		}
		#endregion
	}

	public delegate object[] CommandParameterCombiner(object[] left, object[] right);

	[AttributeUsage(AttributeTargets.Class)]
	internal class PlatformAttribute : Attribute { }
	[AttributeUsage(AttributeTargets.Class)]
	internal class CategoryAttribute : Attribute { }
	[AttributeUsage(AttributeTargets.Class)]
	internal class CommandAttribute : Attribute { }
	[AttributeUsage(AttributeTargets.Field)]
	internal class ValueAttribute : Attribute { }
}

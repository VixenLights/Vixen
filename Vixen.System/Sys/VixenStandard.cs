using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;
using Vixen.Module.CommandStandardExtension;

namespace Vixen.Sys {
	static class VixenStandard {
		//tree[platform][category] = Command collection
		static private PlatformDictionary _commandTree = new PlatformDictionary();

		static VixenStandard() {
			foreach(byte platform in Standard.GetPlatformValues()) {
				_AddPlatform(platform);
				foreach(byte category in Standard.GetCategoryValues(platform)) {
					_AddCategory(platform, category);
					foreach(byte commandIndex in Standard.GetCommandValues(platform, category)) {
						CommandSignature command = Standard.GetCommandSignature(platform, category, commandIndex);
						CommandParameterCombiner combiner = Standard.GetCommandParameterCombiner(platform, category, commandIndex);
						_AddCommand(command.Name, platform, category, commandIndex, command.Parameters, combiner);
					}
				}
			}
		}

		static public void AddCustomCommand(ICommandStandardExtension command) {
			_AddCommand(command.Name, command.CommandPlatform, Standard.CustomCategory, command.CommandIndex, command.Parameters, command.ParameterCombiner);
		}

		static public IEnumerable<CommandSignature> GetAvailableCommands() {
			return
				(from platform in _commandTree.Values
				 from category in platform.Values
				 from command in category.Values
				 select command);
		}

		static private CategoryDictionary _AddPlatform(byte platformValue) {
			CategoryDictionary categories;
			if(!_commandTree.TryGetValue(platformValue, out categories)) {
				_commandTree[platformValue] = categories = new CategoryDictionary();
			}
			return categories;
		}

		static private CommandCollection _AddCategory(byte platformValue, byte categoryValue) {
			CategoryDictionary categories = _AddPlatform(platformValue);
			CommandCollection commands;
			if(!categories.TryGetValue(categoryValue, out commands)) {
				categories[categoryValue] = commands = new CommandCollection();
			}
			return commands;
		}

		static private VixenCommandSignature _AddCommand(string name, byte platformValue, byte categoryValue, byte commandIndexValue, CommandParameterSpecification[] commandParameters, CommandParameterCombiner combiner) {
			CommandCollection commands = _AddCategory(platformValue, categoryValue);
			VixenCommandSignature command;
			commands[commandIndexValue] = command = new VixenCommandSignature(name, platformValue, categoryValue, commandIndexValue, commandParameters, combiner);
			return command;
		}
	}

	class CommandCollection : Dictionary<byte, VixenCommandSignature> { }
	class CategoryDictionary : Dictionary<byte, CommandCollection> { }
	class PlatformDictionary : Dictionary<byte, CategoryDictionary> { }
	class VixenCommandSignature : CommandSignature {
		public VixenCommandSignature(string name, byte platformValue, byte categoryValue, byte commandIndexValue, CommandParameterSpecification[] commandParameters, CommandParameterCombiner combiner)
			: base(name, platformValue, categoryValue, commandIndexValue, commandParameters) {
			Combiner = combiner;
		}

		public CommandParameterCombiner Combiner { get; private set; }
	}
}

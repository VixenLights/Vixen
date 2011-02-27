using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;
using Vixen.Module.CommandStandardExtension;

namespace Vixen.Sys {
	static class Standard {
		//tree[platform][category] = Command collection
		static private PlatformDictionary _commandTree = new PlatformDictionary();

		static Standard() {
			CommandSignature command;
			foreach(byte platform in CommandStandard.Standard.GetPlatformValues()) {
				_AddPlatform(platform);
				foreach(byte category in CommandStandard.Standard.GetCategoryValues(platform)) {
					_AddCategory(platform, category);
					foreach(byte commandIndex in CommandStandard.Standard.GetCommandValues(platform, category)) {
						command = CommandStandard.Standard.GetCommandSignature(platform, category, commandIndex);
						_AddCommand(command.Name, platform, category, commandIndex, command.Parameters);
					}
				}
			}
		}

		static public void AddCustomCommand(ICommandStandardExtension command) {
			_AddCommand(command.Name, command.CommandPlatform, CommandStandard.Standard.CustomCategory, command.CommandIndex, command.Parameters);
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

		static private CommandSignature _AddCommand(string name, byte platformValue, byte categoryValue, byte commandIndexValue, CommandParameterSpecification[] commandParameters) {
			CommandCollection commands = _AddCategory(platformValue, categoryValue);
			CommandSignature command;
			commands[commandIndexValue] = command = new CommandSignature(name, platformValue, categoryValue, commandIndexValue, commandParameters);
			return command;
		}
	}

	class CommandCollection : Dictionary<byte, CommandSignature> { }
	class CategoryDictionary : Dictionary<byte, CommandCollection> { }
	class PlatformDictionary : Dictionary<byte, CategoryDictionary> { }
}

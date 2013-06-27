using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.Script;
using Vixen.Services;
using Vixen.Module.Effect;

namespace VixenModules.Script.VB
{
	public partial class VB_ScriptFramework : IScriptFrameworkGenerator
	{
		public string Generate(string nameSpace, string className)
		{
			Namespace = nameSpace;
			ClassName = className;
			Effects = _GenerateEffectsDictionary();

			GenerationEnvironment.Clear();

			return TransformText();
		}

		public string Namespace { get; set; }

		public string ClassName { get; set; }

		public Dictionary<string, IEffectModuleDescriptor> Effects { get; set; }

		private Dictionary<string, IEffectModuleDescriptor> _GenerateEffectsDictionary()
		{
			IEffectModuleDescriptor[] effects =
				ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>().ToArray();
			Dictionary<string, IEffectModuleDescriptor> effectDictionary = new Dictionary<string, IEffectModuleDescriptor>();

			foreach (var effectGrouping in effects.GroupBy(x => x.EffectName)) {
				if (effectGrouping.Count() == 1) {
					// Name is unique;
					effectDictionary[_Mangle(effectGrouping.Key)] = effectGrouping.First();
				}
				else {
					// Name is not unique.
					// Append an index to each to make it unique.
					int i = 1;
					foreach (var effect in effectGrouping) {
						effectDictionary[_Mangle(effectGrouping.Key) + "_" + i++] = effect;
					}
				}
			}

			return effectDictionary;
		}

		private string _Mangle(string str)
		{
			if (string.IsNullOrWhiteSpace(str)) throw new ArgumentException("Name cannot be empty.");

			List<char> chars = new List<char>();

			if (!char.IsLetter(str[0]) && str[0] != '_') {
				chars.Add('_');
			}

			chars.AddRange(str.Select(x => _IsValidSymbolChar(x) ? x : '_'));

			return new string(chars.ToArray());
		}

		private bool _IsValidSymbolChar(char ch)
		{
			return char.IsLetterOrDigit(ch) || ch == '_';
		}

		private string _Fix(string str, List<string> usedSet)
		{
			str = _Mangle(str);
			str = _EnsureUnique(str, usedSet);
			usedSet.Add(str);
			return str;
		}

		private string _EnsureUnique(string str, List<string> usedSet)
		{
			int index = 2;
			while (usedSet.Contains(str)) {
				str = str + "_" + index++;
			}
			return str;
		}
	}
}
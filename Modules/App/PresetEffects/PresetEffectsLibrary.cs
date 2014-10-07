using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using Vixen.Module;
using Vixen.Module.App;


namespace VixenModules.App.PresetEffects
{

	public class PresetEffectsLibrary : AppModuleInstanceBase, IEnumerable<KeyValuePair<Guid, PresetEffect>>
	{
		private PresetEffectsLibraryStaticData _data;
		public event EventHandler PresetEffectsChanged;

		public override void Loading()
		{
		}

		public override void Unloading()
		{
		}

		public override Vixen.Sys.IApplication Application
		{
			set { }
		}

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = value as PresetEffectsLibraryStaticData; }
		}

		public Dictionary<Guid, PresetEffect> Library
		{
			get { return _data.Library; }
		}

		public bool Contains(Guid guid)
		{
			return Library.ContainsKey(guid);
		}

		/// <summary>
		/// Retruns boolean value if the type of effect is reconized and is saveable.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool CanSaveType(string typeName)
		{
			string[] saveableTypes =
			{
				"Alternating",
				"Candle Flicker",
				"Chase",
				"Custom Value",
				"Nutcracker",
				"Pulse",
				"Set Level",
				"Spin",
				"Twinkle",
				"Wipe",
				"Launcher",
				"RDS"
				
			};

			return saveableTypes.Contains(typeName) || typeName.Contains("LipSync");
		}
		public PresetEffect GetPresetEffect(Guid guid)
		{
			if (Library.ContainsKey(guid))
				return Library[guid];
			else
				return null;
		}

		public bool AddPresetEffect(Guid guid, PresetEffect presetEffect)
		{
			if (guid == Guid.Empty ||
				presetEffect.Name == string.Empty ||
				presetEffect.EffectTypeName == string.Empty ||
				presetEffect.EffectTypeGuid == Guid.Empty ||
				presetEffect.ParameterValues == null)
				return false;

			if (Contains(guid))
				return false;

			Library[guid] = presetEffect;
			_PresetEffectsChanged(guid);
			return true;
		}

		public bool RemovePresetEffect(Guid guid)
		{
			bool removed = _RemovePresetEffect(guid);
			if (removed)
			{
				_PresetEffectsChanged(guid);
			}
			return removed;
		}

		private bool _RemovePresetEffect(Guid guid)
		{
			if (!Contains(guid))
				return false;

			Library.Remove(guid);
			return true;
		}

		//Upon editing, we check if custom values exist, if so, we load them to the editor form

		//public bool EditLibraryCustomEffectDefault(string name)
		//{
		//	object[] CustomEffectDefault = GetCustomEffectDefault(name);
		//	if (CustomEffectDefault == null)
		//		return false;

			//CustomEffectDefaultEditor editor = new CustomEffectDefaultEditor(CustomEffectDefault);
			//editor.LibraryCustomEffectDefaultName = name;

			//if (editor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			//{
			//	_RemoveCustomEffectDefault(name);
			//	AddCustomEffectDefault(name, editor.CustomEffectDefault);
			//	return true;
			//}

			//return false;
		//}

		public IEnumerator<KeyValuePair<Guid, PresetEffect>> GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		private void _PresetEffectsChanged(Guid guid)
		{
			if (PresetEffectsChanged != null)
				PresetEffectsChanged(this, new PresetEffectsLibraryEventArgs(guid));
		}
	}

	public class PresetEffectsLibraryEventArgs : EventArgs
	{
		public PresetEffectsLibraryEventArgs(Guid guid)
		{
			_guid = guid;
		}

		public Guid _guid { get; private set; }
	}

}

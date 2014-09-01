using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;


namespace VixenModules.App.CustomEffectDefaults
{

	public class CustomEffectDefaultLibrary : AppModuleInstanceBase, IEnumerable<KeyValuePair<string, object[]>>
	{
		private CustomEffectDefaultLibraryStaticData _data;
		public event EventHandler CustomEffectDefaultChanged;

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
			set { _data = value as CustomEffectDefaultLibraryStaticData; }
		}

		public Dictionary<string, object[]> Library
		{
			get { return _data.Library; }
		}

		public bool Contains(string name)
		{
			return Library.ContainsKey(name);
		}

		public object[] GetCustomEffectDefault(string name)
		{
			if (Library.ContainsKey(name))
				return Library[name];
			else
				return null;
		}

		public bool AddCustomEffectDefault(string name, object[] CustomEffectDefault)
		{
			if (name == string.Empty)
				return false;

			bool inLibrary = Contains(name);

			Library[name] = CustomEffectDefault;
			_CustomEffectDefaultChanged(name);
			return inLibrary;
		}

		public bool RemoveCustomEffectDefault(string name)
		{
			bool removed = _RemoveCustomEffectDefault(name);
			if (removed)
			{
				_CustomEffectDefaultChanged(name);
			}
			return removed;
		}

		private bool _RemoveCustomEffectDefault(string name)
		{
			if (!Contains(name))
				return false;

			Library.Remove(name);
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

		public IEnumerator<KeyValuePair<string, object[]>> GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		private void _CustomEffectDefaultChanged(string name)
		{
			if (CustomEffectDefaultChanged != null)
				CustomEffectDefaultChanged(this, new CustomEffectDefaultLibraryEventArgs(name));
		}
	}

	public class CustomEffectDefaultLibraryEventArgs : EventArgs
	{
		public CustomEffectDefaultLibraryEventArgs(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}

}

using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.Curves
{
	[Serializable]
	public class CurveLibrary : AppModuleInstanceBase, IEnumerable<KeyValuePair<string, Curve>>
	{
		private CurveLibraryStaticData _data;
		public event EventHandler CurveChanged;

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
			set { _data = value as CurveLibraryStaticData; }
		}

		public Dictionary<string, Curve> Library
		{
			get { return _data.Library; }
		}

		public bool Contains(string name)
		{
			return Library.ContainsKey(name);
		}

		public Curve GetCurve(string name)
		{
			if (Library.ContainsKey(name))
				return Library[name];
			else
				return null;
		}

		public bool AddCurve(string name, Curve curve)
		{
			if (name == string.Empty)
				return false;

			bool inLibrary = Contains(name);
			if (inLibrary) {
				Library[name].IsCurrentLibraryCurve = false;
			}
			curve.IsCurrentLibraryCurve = true;
			curve.LibraryReferenceName = string.Empty;
			Library[name] = curve;
			_CurveChanged(name);
			return inLibrary;
		}

		public bool RemoveCurve(string name)
		{
			bool removed = _RemoveCurve(name);
			if (removed)
			{
				_CurveChanged(name);
			}
			return removed;
		}

		private bool _RemoveCurve(string name)
		{
			if (!Contains(name))
				return false;

			Library[name].IsCurrentLibraryCurve = false;
			Library.Remove(name);
			return true;	
		}

		public bool EditLibraryCurve(string name)
		{
			Curve curve = GetCurve(name);
			if (curve == null)
				return false;

			CurveEditor editor = new CurveEditor(curve);
			editor.LibraryCurveName = name;

			if (editor.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				_RemoveCurve(name);
				AddCurve(name, editor.Curve);
				return true;
			}

			return false;
		}

		public IEnumerator<KeyValuePair<string, Curve>> GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		private void _CurveChanged(string name)
		{
			if (CurveChanged != null)
				CurveChanged(this, new CurveLibraryEventArgs(name));
		}
	}

	public class CurveLibraryEventArgs : EventArgs
	{
		public CurveLibraryEventArgs(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}
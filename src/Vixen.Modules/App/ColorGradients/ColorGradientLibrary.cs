using NLog;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;

namespace VixenModules.App.ColorGradients
{
	[Serializable]
	public class ColorGradientLibrary : AppModuleInstanceBase, IEnumerable<KeyValuePair<string, ColorGradient>>
	{
		private ColorGradientLibraryStaticData _data;
		private bool _bulkUpdating;
		public event EventHandler GradientsChanged;
		private HashSet<string> _bulkGradientChangeNames;
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		public override void Loading()
		{
		}

		public override void Unloading()
		{
		}

		public override IApplication Application
		{
			set { }
		}

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set { _data = value as ColorGradientLibraryStaticData; }
		}


		public Dictionary<string, ColorGradient> Library
		{
			get { return _data.Library; }
		}

		public bool Contains(string name)
		{
			return Library.ContainsKey(name);
		}

		public ColorGradient GetColorGradient(string name)
		{
			if (Library.ContainsKey(name))
				return Library[name];
			else
				return null;
		}

		public async Task<bool> AddColorGradientAsync(string name, ColorGradient cg)
		{
			if (name == string.Empty)
				return false;

			bool inLibrary = Contains(name);
			if (inLibrary) {
				Library[name].IsCurrentLibraryGradient = false;
			}
			cg.IsCurrentLibraryGradient = true;
			cg.LibraryReferenceName = string.Empty;
			Library[name] = cg;
			
			if (!_bulkUpdating)
			{
				_GradientsChanged(new []{name});
				await VixenSystem.SaveModuleConfigAsync();
			}
			else
			{
				_bulkGradientChangeNames.Add(name);
			}
			return inLibrary;
		}

		public async Task<bool> RemoveColorGradientAsync(string name)
		{
			bool removed = _RemoveColorGradient(name);
			if (removed)
			{
				if (!_bulkUpdating)
				{
					_GradientsChanged(new []{name});
					await VixenSystem.SaveModuleConfigAsync();
				}
				else
				{
					_bulkGradientChangeNames.Add(name);
				}
			}

			return removed;
		}

		public bool _RemoveColorGradient(string name)
		{
			if (!Contains(name))
				return false;

			Library[name].IsCurrentLibraryGradient = false;
			Library.Remove(name);

			return true;
		}

		public async Task<bool> EditLibraryItemAsync(string name)
		{
			ColorGradient cg = GetColorGradient(name);
			if (cg == null)
				return false;

			ColorGradientEditor editor = new ColorGradientEditor(cg, false, null);
			editor.LibraryItemName = name;

			if (await editor.ShowDialogAsync() == DialogResult.OK) {
				_RemoveColorGradient(name);
				await AddColorGradientAsync(name, editor.Gradient);
				return true;
			}

			return false;
		}

		public IEnumerator<KeyValuePair<string, ColorGradient>> GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Library.GetEnumerator();
		}

		private void _GradientsChanged(IEnumerable<string> names)
		{
			GradientsChanged?.Invoke(this, new ColorGradientLibraryEventArgs(names));
		}

		/// <summary>
		/// Called to begin a bulk update operation
		/// </summary>
		public void BeginBulkUpdate()
		{
			_bulkUpdating = true;
			_bulkGradientChangeNames = new HashSet<string>();
		}

		/// <summary>
		/// Called to complete a bulk update operation
		/// </summary>
		public async void EndBulkUpdate()
		{
			try
			{
				_bulkUpdating = false;
				_GradientsChanged(_bulkGradientChangeNames);
				_bulkGradientChangeNames = null;
				await VixenSystem.SaveModuleConfigAsync();
			}
			catch (Exception e)
			{
				Logging.Error(e, "Error saving color gradient library.");
			}
		}

	}

	public class ColorGradientLibraryEventArgs : EventArgs
	{
		public ColorGradientLibraryEventArgs(IEnumerable<string> names)
		{
			Names = names;
		}

		public IEnumerable<string> Names { get; private set; }
	}
}
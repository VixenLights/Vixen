using System.Collections.Generic;
using System.IO;
using Vixen.Module;
using Vixen.Module.PostFilter;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
	class PostFilterTemplate {
		private List<PostFilterCollection> _outputFilters;

		private const string DIRECTORY_NAME = "Template\\PostFilter";

		[DataPath]
		static public string Directory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		public const string Extension = ".pos";

		public PostFilterTemplate() {
			DataSet = new ModuleLocalDataSet();
			_outputFilters = new List<PostFilterCollection>();
		}

		public ModuleLocalDataSet DataSet;

		public void AddOutputFilters(PostFilterCollection filters) {
			_AddFilterToDataSet(filters);

			_outputFilters.Add(filters);
		}

		public void AddOutputFilters(IEnumerable<PostFilterCollection> filterCollections) {
			foreach(PostFilterCollection filterCollection in filterCollections) {
				AddOutputFilters(filterCollection);
			}
		}

		public void RemoveOutputFilters(int outputIndex) {
			if(outputIndex < _outputFilters.Count) {
				PostFilterCollection filters = _outputFilters[outputIndex];
				_outputFilters.RemoveAt(outputIndex);

				_RemoveFiltersFromDataSet(filters);
			}
		}

		public void InsertOutputFilters(int outputIndex, PostFilterCollection filters) {
			_AddFilterToDataSet(filters);

			_outputFilters.Insert(outputIndex, filters);
		}

		public void SetOutputFilters(int outputIndex, PostFilterCollection filters) {
			RemoveOutputFilters(outputIndex);
			InsertOutputFilters(outputIndex, filters);
		}

		public IEnumerable<PostFilterCollection> OutputFilters {
			get { return _outputFilters; }
		}

		public PostFilterCollection this[int index] {
			get {
				if(index < _outputFilters.Count) {
					return _outputFilters[index];
				}
				return null;
			}
		}

		public void ClearOutputFilters() {
			_outputFilters.Clear();
			DataSet.Clear();
		}

		private void _AddFilterToDataSet(IEnumerable<IPostFilterModuleInstance> filters) {
			foreach(IPostFilterModuleInstance module in filters) {
				DataSet.AssignModuleInstanceData(module);
			}
		}

		private void _RemoveFiltersFromDataSet(IEnumerable<IPostFilterModuleInstance> filters) {
			foreach(IPostFilterModuleInstance module in filters) {
				DataSet.RemoveModuleInstanceData(module);
			}
		}
	}
}

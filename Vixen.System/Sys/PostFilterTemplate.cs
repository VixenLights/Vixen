using System;
using System.Collections.Generic;
using System.IO;
using Vixen.IO;
using Vixen.IO.Result;
using Vixen.Module;
using Vixen.Module.PostFilter;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
	public class PostFilterTemplate {
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

		static public IEnumerable<PostFilterTemplate> GetAll() {
			foreach(string filePath in System.IO.Directory.GetFiles(PostFilterTemplate.Directory, "*" + Extension)) {
				yield return Load(filePath);
			}
		}

		static public PostFilterTemplate Load(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) return null;

			FileSerializer<PostFilterTemplate> serializer = SerializerFactory.Instance.CreatePostFilterTemplateSerializer();
			SerializationResult<PostFilterTemplate> result = serializer.Read(filePath);
			return result.Object;
		}

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");

			FileSerializer<PostFilterTemplate> serializer = SerializerFactory.Instance.CreatePostFilterTemplateSerializer();
			serializer.Write(this, filePath);

			FilePath = filePath;
		}

		public void Save() {
			Save(FilePath);
		}
		
		public string FilePath { get; set; }

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

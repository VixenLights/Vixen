using System;
using System.Collections.Generic;
using System.IO;
using Vixen.IO;
using Vixen.IO.Result;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
	public class OutputFilterTemplate {
		private List<OutputFilterCollection> _outputFilters;

		private const string DIRECTORY_NAME = "Template\\OutputFilter";

		[DataPath]
		static public string Directory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		public const string Extension = ".pos";

		public OutputFilterTemplate() {
			DataSet = new ModuleLocalDataSet();
			_outputFilters = new List<OutputFilterCollection>();
		}

		static public IEnumerable<OutputFilterTemplate> GetAll() {
			foreach(string filePath in System.IO.Directory.GetFiles(OutputFilterTemplate.Directory, "*" + Extension)) {
				yield return Load(filePath);
			}
		}

		static public OutputFilterTemplate Load(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) return null;

			FileSerializer<OutputFilterTemplate> serializer = SerializerFactory.Instance.CreateOutputFilterTemplateSerializer();
			SerializationResult<OutputFilterTemplate> result = serializer.Read(filePath);
			return result.Object;
		}

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");

			FileSerializer<OutputFilterTemplate> serializer = SerializerFactory.Instance.CreateOutputFilterTemplateSerializer();
			serializer.Write(this, filePath);

			FilePath = filePath;
		}

		public void Save() {
			Save(FilePath);
		}
		
		public string FilePath { get; set; }

		public ModuleLocalDataSet DataSet;

		public void AddOutputFilters(OutputFilterCollection filters) {
			_AddFilterToDataSet(filters);

			_outputFilters.Add(filters);
		}

		public void AddOutputFilters(IEnumerable<OutputFilterCollection> filterCollections) {
			foreach(OutputFilterCollection filterCollection in filterCollections) {
				AddOutputFilters(filterCollection);
			}
		}

		public void RemoveOutputFilters(int outputIndex) {
			if(outputIndex < _outputFilters.Count) {
				OutputFilterCollection filters = _outputFilters[outputIndex];
				_outputFilters.RemoveAt(outputIndex);

				_RemoveFiltersFromDataSet(filters);
			}
		}

		public void InsertOutputFilters(int outputIndex, OutputFilterCollection filters) {
			_AddFilterToDataSet(filters);

			_outputFilters.Insert(outputIndex, filters);
		}

		public void SetOutputFilters(int outputIndex, OutputFilterCollection filters) {
			RemoveOutputFilters(outputIndex);
			InsertOutputFilters(outputIndex, filters);
		}

		public IEnumerable<OutputFilterCollection> OutputFilters {
			get { return _outputFilters; }
		}

		public OutputFilterCollection this[int index] {
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

		private void _AddFilterToDataSet(IEnumerable<IOutputFilterModuleInstance> filters) {
			foreach(IOutputFilterModuleInstance module in filters) {
				DataSet.AssignModuleInstanceData(module);
			}
		}

		private void _RemoveFiltersFromDataSet(IEnumerable<IOutputFilterModuleInstance> filters) {
			foreach(IOutputFilterModuleInstance module in filters) {
				DataSet.RemoveModuleInstanceData(module);
			}
		}
	}
}

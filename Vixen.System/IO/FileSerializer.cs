using System;
using System.Collections.Generic;
using System.IO;
using Vixen.Sys;

namespace Vixen.IO {
	abstract class FileSerializer<ObjectType> : IFileSerializer<ObjectType, SerializationResult<ObjectType>>
		where ObjectType : class {
		private List<IFileOperationResult> _results;

		protected FileSerializer() {
			_results = new List<IFileOperationResult>();
		}

		public SerializationResult<ObjectType> Read(string filePath) {
			if(filePath == null) throw new ArgumentNullException("filePath");

			SerializationResult<ObjectType> serializationResult;

			_ClearResults();

			try {
				if(File.Exists(filePath)) {
					ObjectType value = _Read(filePath);
					serializationResult = new SerializationResult<ObjectType>(true, "File read successful.", value, _results);
				} else {
					serializationResult = new SerializationResult<ObjectType>(false, "File does not exist.", null, _results);
				}
			} catch(Exception ex) {
				VixenSystem.Logging.Debug(ex);
				serializationResult = new SerializationResult<ObjectType>(false, ex.Message, null, _results);
			}

			return serializationResult;
		}

		public SerializationResult<ObjectType> Write(ObjectType value, string filePath) {
			if(filePath == null) throw new ArgumentNullException("filePath");
			if(value == null) throw new ArgumentNullException("value");

			SerializationResult<ObjectType> serializationResult;

			_ClearResults();

			try {
				_Write(value, filePath);
				serializationResult = new SerializationResult<ObjectType>(true, "File write successful.", value, _results);
			} catch(Exception ex) {
				VixenSystem.Logging.Debug(ex);
				serializationResult = new SerializationResult<ObjectType>(false, ex.Message, null, _results);
			}

			return serializationResult;
		}

		abstract protected ObjectType _Read(string filePath);

		abstract protected void _Write(ObjectType value, string filePath);

		protected void _AddResults(IEnumerable<IFileOperationResult> operationResults) {
			_results.AddRange(operationResults);
		}

		private void _ClearResults() {
			_results.Clear();
		}
	}
}

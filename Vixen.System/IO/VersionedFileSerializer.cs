using System;
using System.Collections.Generic;
using Vixen.IO.Result;
using Vixen.Sys;

namespace Vixen.IO {
	sealed class VersionedFileSerializer : IFileSerializer {
		private IVersionedFileSerializer _serializer;
		private IMigrator _migrator;
		private CompositeResult _result;

		public VersionedFileSerializer(IVersionedFileSerializer serializer, IMigrator migrator) {
			if(serializer == null) throw new ArgumentNullException("serializer");
			if(migrator == null) throw new ArgumentNullException("migrator");

			_serializer = serializer;
			_migrator = migrator;
		}

		public ISerializationResult Read(string filePath) {
			if(filePath == null) throw new ArgumentNullException("filePath");

			_ClearResults();

			SerializationResult serializationResult;
			try {
				serializationResult = _ReadObject(filePath);
				_AddResults(serializationResult);

				if(serializationResult.Success) {
					IEnumerable<IResult> migrationResults = _EnsureContentIsUpToDate(serializationResult.Object, filePath);
					_AddResults(migrationResults);
				}
			} catch(Exception ex) {
				VixenSystem.Logging.Error(ex);
				serializationResult = new SerializationResult(false, ex.Message, null);
			}

			return serializationResult;
		}

		public ISerializationResult Write(object value, string filePath) {
			if(filePath == null) throw new ArgumentNullException("filePath");
			if(value == null) throw new ArgumentNullException("value");

			_ClearResults();

			return _WriteObject(value, filePath);
		}

		private void _AddResults(IResult operationResults) {
			_result.AddResults(operationResults);
		}

		private void _AddResults(IEnumerable<IResult> operationResults) {
			_result.AddResults(operationResults);
		}

		private void _ClearResults() {
			_result = new CompositeResult();
		}

		private SerializationResult _ReadObject(string filePath) {
			SerializationResult serializationResult;

			object value = _serializer.Read(filePath);
			if(value != null) {
				serializationResult = new SerializationResult(true, "File read successful.", value);
			} else {
				serializationResult = new SerializationResult(false, "File does not exist.", null);
			}

			return serializationResult;
		}

		private SerializationResult _WriteObject(object value, string filePath) {
			SerializationResult serializationResult;

			try {
				_serializer.Write(value, filePath);
				serializationResult = new SerializationResult(true, "File write successful.", value);
			} catch(Exception ex) {
				VixenSystem.Logging.Error(ex);
				serializationResult = new SerializationResult(false, ex.Message, null);
			}

			return serializationResult;
		}

		private IEnumerable<IResult> _EnsureContentIsUpToDate(object value, string originalFilePath) {
			GeneralFileMigrationPolicy migrationPolicy = new GeneralFileMigrationPolicy();
			migrationPolicy.MatureContent(value, _serializer.FileVersion, _serializer.ClassVersion, _migrator, originalFilePath);

			return migrationPolicy.MigrationResults;
		}

		object IFileSerializer.Read(string filePath) {
			return Read(filePath);
		}

		void IFileSerializer.Write(object value, string filePath) {
			Write(value, filePath);
		}
	}
}

namespace Vixen.IO {
	interface IObjectPersistor<in T> : IObjectPersistor {
		void SaveToFile(T obj, string filePath);
	}

	interface IObjectPersistor {
		void SaveToFile(object obj, string filePath);
	}
}

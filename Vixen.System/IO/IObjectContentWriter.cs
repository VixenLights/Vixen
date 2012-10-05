namespace Vixen.IO {
	interface IObjectContentWriter<in T, in U> : IObjectContentWriter
		where T : class
		where U : class {
		void WriteContentToObject(T content, U obj);
		int GetContentVersion(T content);
	}

	interface IObjectContentWriter {
		void WriteContentToObject(object content, object obj);
		int GetContentVersion(object content);
	}
}

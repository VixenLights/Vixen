namespace Vixen.IO {
	interface IObjectContentReader<out T, in U> : IObjectContentReader {
		T ReadContentFromObject(U obj);
	}

	interface IObjectContentReader {
		object ReadContentFromObject(object obj);
	}
}

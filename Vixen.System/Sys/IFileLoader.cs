namespace Vixen.Sys {
	interface IFileLoader<T> {
		T Load(string filePath);
	}
}

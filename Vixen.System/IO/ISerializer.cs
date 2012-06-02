namespace Vixen.IO {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of the object being serialized.</typeparam>
	/// <typeparam name="U">Type of what the object is being serialized to.</typeparam>
	interface ISerializer<T, U>
		where T : class 
		where U : class {
		U WriteObject(T value);
		T ReadObject(U element);
	}
}

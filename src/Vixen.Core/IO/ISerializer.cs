namespace Vixen.IO
{
	public interface ISerializer
	{
		object WriteObject(object value);
		object ReadObject(object source);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of the object being serialized.</typeparam>
	/// <typeparam name="U">Type of what the object is being serialized to.</typeparam>
	internal interface ISerializer<T, U>
	{
		//where T : class 
		//where U : class {
		U WriteObject(T value);
		T ReadObject(U source);
	}
}
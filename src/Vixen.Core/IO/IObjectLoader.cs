namespace Vixen.IO
{
	internal interface IObjectLoader<out T> : IObjectLoader
		where T : class, new()
	{
		new T LoadFromFile(string filePath);
	}

	internal interface IObjectLoader
	{
		object LoadFromFile(string filePath);
	}
}
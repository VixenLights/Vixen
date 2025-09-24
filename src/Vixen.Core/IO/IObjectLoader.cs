namespace Vixen.IO
{
	public interface IObjectLoader<out T> : IObjectLoader
		where T : class, new()
	{
		new T LoadFromFile(string filePath);
	}

	public interface IObjectLoader
	{
		object LoadFromFile(string filePath);
	}
}
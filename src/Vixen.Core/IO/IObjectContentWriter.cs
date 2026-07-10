namespace Vixen.IO
{
	internal interface IObjectContentWriter<in T, in U> : IObjectContentWriter
		where T : class
		where U : class
	{
		void WriteContentToObject(T content, U obj);
		int GetContentVersion(T content);
	}

	internal interface IObjectContentWriter
	{
		void WriteContentToObject(object content, object obj);
		int GetContentVersion(object content);
	}
}
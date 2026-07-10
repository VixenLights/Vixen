namespace Vixen.IO
{
	internal interface IObjectContentReader<out T, in U> : IObjectContentReader
	{
		T ReadContentFromObject(U obj);
	}

	internal interface IObjectContentReader
	{
		object ReadContentFromObject(object obj);
	}
}
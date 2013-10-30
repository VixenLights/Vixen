using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.IO
{
	internal interface IFileReader<out T> : IFileReader
		where T : class
	{
		new T ReadFile(string filePath);
	}

	internal interface IFileReader
	{
		object ReadFile(string filePath);
	}
}
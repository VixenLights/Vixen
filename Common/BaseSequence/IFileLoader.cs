using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseSequence
{
	internal interface IFileLoader
	{
		object Load(string filePath);
	}
}
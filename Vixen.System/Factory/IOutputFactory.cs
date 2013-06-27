using System;
using Vixen.Sys.Output;

namespace Vixen.Factory
{
	internal interface IOutputFactory
	{
		Output CreateOutput(string name, int index);
		Output CreateOutput(Guid id, string name, int index);
	}
}
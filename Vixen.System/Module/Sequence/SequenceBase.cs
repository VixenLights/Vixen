using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;
using Vixen.Execution;
using Vixen.IO;

namespace Vixen.Module.Sequence {
	// This class exists as a facade implementation of the Sequence generic.
	/// <summary>
	/// Base class for sequence type module implementations.
	/// </summary>
	[Executor(typeof(SequenceExecutor))]
	abstract public class SequenceBase : Sequence<SequenceReader, SequenceWriter, SequenceBase>, ISequenceModuleInstance {
		private const string DIRECTORY_NAME = "Sequence";

		// Has to be in the subclass because you can't perform the late-bound operations
		// on the generic base.
		[DataPath]
		static private readonly string _directory = System.IO.Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

		protected override string Directory {
			get { return _directory; }
		}

		abstract public Guid TypeId { get; }

		public Guid InstanceId { get; set; }

		virtual public void Dispose() { }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		abstract public string FileExtension { get; }
	}
}

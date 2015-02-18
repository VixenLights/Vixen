using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Analysis
{
	public abstract class AnalysisModuleDescriptorBase : ModuleDescriptorBase, IAnalysisModuleDescriptor,
													IEqualityComparer<IAnalysisModuleDescriptor>,
													IEquatable<IAnalysisModuleDescriptor>,
	                                                IEqualityComparer<AnalysisModuleDescriptorBase>,
													IEquatable<AnalysisModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		// App module types can't have instance data classes; they're singletons, so don't let anyone set one.
		public override sealed Type ModuleDataClass
		{
			get { return null; }
		}

		public bool Equals(IAnalysisModuleDescriptor x, IAnalysisModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IAnalysisModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IAnalysisModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(AnalysisModuleDescriptorBase x, AnalysisModuleDescriptorBase y)
		{
			return Equals(x as IAnalysisModuleDescriptor, y as IAnalysisModuleDescriptor);
		}

		public int GetHashCode(AnalysisModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IAnalysisModuleDescriptor);
		}

		public bool Equals(AnalysisModuleDescriptorBase other)
		{
			return Equals(other as IAnalysisModuleDescriptor);
		}
	}
}
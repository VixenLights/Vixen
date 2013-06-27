using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Script
{
	public abstract class ScriptModuleDescriptorBase : ModuleDescriptorBase, IScriptModuleDescriptor,
	                                                   IEqualityComparer<IScriptModuleDescriptor>,
	                                                   IEquatable<IScriptModuleDescriptor>,
	                                                   IEqualityComparer<ScriptModuleDescriptorBase>,
	                                                   IEquatable<ScriptModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public abstract string LanguageName { get; }

		public abstract string FileExtension { get; }

		public abstract Type SkeletonGenerator { get; }

		public abstract Type FrameworkGenerator { get; }

		public abstract Type CodeProvider { get; }

		public bool Equals(IScriptModuleDescriptor x, IScriptModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IScriptModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IScriptModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(ScriptModuleDescriptorBase x, ScriptModuleDescriptorBase y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(ScriptModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IScriptModuleDescriptor);
		}

		public bool Equals(ScriptModuleDescriptorBase other)
		{
			return Equals(other as IScriptModuleDescriptor);
		}
	}
}
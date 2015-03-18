using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Analysis
{
	public abstract class AnalysisModuleInstanceBase : ModuleInstanceBase, IAnalysisModuleInstance,
												  IEqualityComparer<IAnalysisModuleInstance>, IEquatable<IAnalysisModuleInstance>,
												  IEqualityComparer<AnalysisModuleInstanceBase>,
												  IEquatable<AnalysisModuleInstanceBase>
	{
		public abstract void Loading();

		public abstract void Unloading();

		public override IModuleInstance Clone()
		{
			// Singleton
			throw new NotSupportedException();
		}

		public bool Equals(IAnalysisModuleInstance x, IAnalysisModuleInstance y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IAnalysisModuleInstance obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(IAnalysisModuleInstance other)
		{
			return base.Equals(other);
		}

		public bool Equals(AnalysisModuleInstanceBase x, AnalysisModuleInstanceBase y)
		{
			return Equals(x as IAnalysisModuleInstance, y as IAnalysisModuleInstance);
		}

		public int GetHashCode(AnalysisModuleInstanceBase obj)
		{
			return GetHashCode(obj as IAnalysisModuleInstance);
		}

		public bool Equals(AnalysisModuleInstanceBase other)
		{
			return Equals(other as IAnalysisModuleInstance);
		}
	}
}
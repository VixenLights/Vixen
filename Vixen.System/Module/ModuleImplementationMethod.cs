using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vixen.Sys;

namespace Vixen.Module
{
	internal abstract class ModuleImplementationMethod
	{
		public object Instance { get; protected set; }
		public object Result { get; protected set; }

		public abstract void Invoke(params object[] parameters);
	}

	internal class ModuleImplementationMethod<T> : ModuleImplementationMethod
	{
		public ModuleImplementationMethod(MethodInfo mi, object instance)
		{
			Instance = instance;
			Method = DelegateFactory.Create<T>(mi);
		}

		public T Method;

		public override void Invoke(params object[] parameters)
		{
			if (Method is LateBoundMethod) {
				Result = (Method as LateBoundMethod)(Instance, parameters);
			}
			else if (Method is LateBoundProcedure) {
				(Method as LateBoundProcedure)(Instance, parameters);
			}
		}
	}
}
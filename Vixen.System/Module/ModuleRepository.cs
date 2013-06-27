using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using Vixen.Sys;

namespace Vixen.Module
{
	internal class ModuleRepository : DynamicObject
	{
		private Dictionary<string, ModuleImplementationMethod> _methods = new Dictionary<string, ModuleImplementationMethod>();

		public ModuleRepository(ModuleImplementation[] moduleImplemenations)
		{
			object obj;
			MethodInfo mi;
			foreach (ModuleImplementation moduleImplemenation in moduleImplemenations) {
				obj = moduleImplemenation.GetType().GetProperty("Repository").GetValue(moduleImplemenation, null);
				mi = obj.GetType().GetMethod("Add", new Type[] {typeof (Guid)});
				_methods.Add("Add" + moduleImplemenation.TypeOfModule, new ModuleImplementationMethod<LateBoundProcedure>(mi, obj));
				mi = obj.GetType().GetMethod("Get", new Type[] {typeof (Guid)});
				_methods.Add("Get" + moduleImplemenation.TypeOfModule, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
				mi = obj.GetType().GetMethod("GetAll");
				_methods.Add("GetAll" + moduleImplemenation.TypeOfModule, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
				mi = obj.GetType().GetMethod("Remove", new Type[] {typeof (Guid)});
				_methods.Add("Remove" + moduleImplemenation.TypeOfModule,
				             new ModuleImplementationMethod<LateBoundProcedure>(mi, obj));
			}
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			ModuleImplementationMethod moduleImplementation;
			if (_methods.TryGetValue(binder.Name, out moduleImplementation)) {
				result = null;
				moduleImplementation.Invoke(args);
				result = moduleImplementation.Result;
				return true;
			}
			return base.TryInvokeMember(binder, args, out result);
		}
	}
}
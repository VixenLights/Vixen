using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using Vixen.Sys;

namespace Vixen.Module {
	class ModuleManagement : DynamicObject {
		private Dictionary<string, ModuleImplementationMethod> _methods = new Dictionary<string, ModuleImplementationMethod>();

		public ModuleManagement(ModuleImplementation[] moduleImplemenations) {
			object obj;
			MethodInfo mi;
			foreach(ModuleImplementation moduleImplemenation in moduleImplemenations) {
				//Specific reflection, really fragile, dammit.
				obj = moduleImplemenation.GetType().GetProperty("Management").GetValue(moduleImplemenation, null);
				mi = obj.GetType().GetMethod("Get", new Type[] { typeof(Guid) });
				_methods.Add("Get" + moduleImplemenation.ModuleTypeName, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
				mi = obj.GetType().GetMethod("GetAll");
				_methods.Add("GetAll" + moduleImplemenation.ModuleTypeName, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
			}
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
			ModuleImplementationMethod moduleImplementation;
			if(_methods.TryGetValue(binder.Name, out moduleImplementation)) {
				result = null;
				try {
					moduleImplementation.Invoke(args);
					result = moduleImplementation.Result;
					return true;
				} catch {
					return false;
				}
			}
			return base.TryInvokeMember(binder, args, out result);
		}
	}
}

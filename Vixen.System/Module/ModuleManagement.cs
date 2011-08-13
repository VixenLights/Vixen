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

		public ModuleManagement(ModuleImplementation[] moduleImplementations) {
			object obj;
			MethodInfo mi;
			foreach(ModuleImplementation moduleImplementation in moduleImplementations) {
				//Specific reflection; really fragile, dammit.
				obj = moduleImplementation.GetType().GetProperty("Management").GetValue(moduleImplementation, null);
				mi = obj.GetType().GetMethod("Get", new Type[] { typeof(Guid) });
				_methods.Add("Get" + moduleImplementation.TypeOfModule, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
				mi = obj.GetType().GetMethod("GetAll");
				_methods.Add("GetAll" + moduleImplementation.TypeOfModule, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
				mi = obj.GetType().GetMethod("Clone", new Type[] { moduleImplementation.ModuleInstanceType });
				_methods.Add("Clone" + moduleImplementation.TypeOfModule, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
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

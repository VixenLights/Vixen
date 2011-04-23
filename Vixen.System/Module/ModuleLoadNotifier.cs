using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using Vixen.Sys;

// The goal was to be able to add module types by definition and let the system handle
// the wiring.
namespace Vixen.Module {
	class ModuleLoadNotifier : DynamicObject {
		private Dictionary<string, ModuleImplementationMethod> _methods = new Dictionary<string, ModuleImplementationMethod>();

		public ModuleLoadNotifier(ModuleImplementation[] moduleImplementations) {
			object obj;
			MethodInfo mi;
			foreach(ModuleImplementation moduleImplementation in moduleImplementations) {
				//Specific reflection, really fragile, dammit.
				obj = moduleImplementation.GetType().GetProperty("ModuleTypeLoader").GetValue(moduleImplementation, null);
				mi = obj.GetType().GetMethod("ModuleLoaded");
				_methods.Add(moduleImplementation.ModuleTypeName + "ModuleLoaded", new ModuleImplementationMethod<LateBoundProcedure>(mi, obj));
				mi = obj.GetType().GetMethod("ModuleUnloading");
				_methods.Add(moduleImplementation.ModuleTypeName + "ModuleUnloading", new ModuleImplementationMethod<LateBoundProcedure>(mi, obj));
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

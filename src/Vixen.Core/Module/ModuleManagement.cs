using System.Dynamic;
using System.Reflection;
using NLog;
using Vixen.Sys;

namespace Vixen.Module
{
	internal class ModuleManagement : DynamicObject
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		private Dictionary<string, ModuleImplementationMethod> _methods = new Dictionary<string, ModuleImplementationMethod>();

		public ModuleManagement(IEnumerable<ModuleImplementation> moduleImplementations)
		{
			foreach (ModuleImplementation moduleImplementation in moduleImplementations) {
				//Specific reflection; really fragile, dammit.
				object obj = moduleImplementation.GetType().GetProperty("Management").GetValue(moduleImplementation, null);
				MethodInfo mi = obj.GetType().GetMethod("Get", new[] {typeof (Guid)});
				_methods.Add("Get" + moduleImplementation.TypeOfModule, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
				mi = obj.GetType().GetMethod("GetAll");
				_methods.Add("GetAll" + moduleImplementation.TypeOfModule, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
				mi = obj.GetType().GetMethod("Clone", new[] {moduleImplementation.ModuleInstanceType});
				_methods.Add("Clone" + moduleImplementation.TypeOfModule, new ModuleImplementationMethod<LateBoundMethod>(mi, obj));
			}
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			ModuleImplementationMethod moduleImplementation;
			if (_methods.TryGetValue(binder.Name, out moduleImplementation)) {
				result = null;
				try {
					moduleImplementation.Invoke(args);
					result = moduleImplementation.Result;
					return true;
				}
				catch (Exception ex) {
					Logging.Error(ex);
					return false;
				}
			}
			return base.TryInvokeMember(binder, args, out result);
		}
	}
}
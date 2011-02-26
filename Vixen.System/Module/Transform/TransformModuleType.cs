using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Transform {
	class TransformModuleType : IModuleLoadNotification {
		public void ModuleLoaded(IModuleDescriptor descriptor) {
			// Pre-cache affected types and commands for the module.
			ITransformModuleDescriptor transformModuleDescriptor = descriptor as ITransformModuleDescriptor;

			var commandParameters =
				(from signature in Vixen.Sys.Standard.GetAvailableCommands()
				 from param in signature.Parameters.Select((spec, index) => new { Spec = spec, Index = index, CmdSig = signature })
				 select param);
			var affectedParameters =
				(from p in commandParameters
				 join affectedType in transformModuleDescriptor.TypesAffected
				 on p.Spec.Type equals affectedType
				 select p);
			var groupedParameters =
				(from p in affectedParameters
				 group p by p.CmdSig.Identifier into commandParams
				 select commandParams);
			var parameterReferences =
				(from grouping in groupedParameters
				 select new CommandParameterReference() {
					 CommandId = grouping.Key,
					 ParameterIndexes = grouping.Select(x => x.Index).ToArray()
				 });
			transformModuleDescriptor.CommandsAffected = parameterReferences.ToDictionary(x => x.CommandId, x => x);
		}

		public void ModuleUnloading(IModuleDescriptor descriptor) {
		}
	}
}

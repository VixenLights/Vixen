using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Transform {
	class TransformModuleRepository : GenericModuleRepository<ITransformModuleInstance> {
		public override void Add(Guid id) {
			ITransformModuleDescriptor descriptor = Modules.GetDescriptorById<ITransformModuleDescriptor>(id);

			// Build a dictionary of the command parameters affected by this transform based on types affected.
			var commandParameters =
				(from signature in VixenStandard.GetAvailableCommands()
				 from param in signature.Parameters.Select((spec, index) => new { Spec = spec, Index = index, CmdSig = signature })
				 select param);
			var affectedParameters =
				(from p in commandParameters
				 join affectedType in descriptor.TypesAffected
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

			descriptor.CommandsAffected = new CommandsAffected(parameterReferences.ToDictionary(x => x.CommandId, x => x));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;

namespace Vixen.Module.Transform {
	abstract public class TransformModuleInstanceBase : ModuleInstanceBase, ITransformModuleInstance, IEqualityComparer<ITransformModuleInstance>, IEquatable<ITransformModuleInstance>, IEqualityComparer<TransformModuleInstanceBase>, IEquatable<TransformModuleInstanceBase> {
		private CommandsAffected _commandsAffected = new CommandsAffected();
		
		abstract public void Setup();

		protected CommandParameterReference _GetAffectedParameters(Command command) {
			CommandParameterReference parameterReference;
			
			if(!_commandsAffected.TryGetValue(command.Identifier, out parameterReference)) {
				var commandParameters =
					(from param in command.Signature.Select((spec, index) => new { Spec = spec, Index = index })
					 select param);
				var affectedParameters =
					(from p in commandParameters
					 join affectedType in (Descriptor as ITransformModuleDescriptor).TypesAffected
					 on p.Spec.Type equals affectedType
					 select p);
				
				parameterReference = new CommandParameterReference() {
					CommandId = command.Identifier,
					ParameterIndexes = affectedParameters.Select(x => x.Index).ToArray()
				};

				_commandsAffected[command.Identifier] = parameterReference;
			}
			
			return parameterReference;
		}

		abstract public void Transform(Command command);

		public bool Equals(ITransformModuleInstance x, ITransformModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ITransformModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(ITransformModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(TransformModuleInstanceBase x, TransformModuleInstanceBase y) {
			return Equals(x as ITransformModuleInstance, y as ITransformModuleInstance);
		}

		public int GetHashCode(TransformModuleInstanceBase obj) {
			return GetHashCode(obj as ITransformModuleInstance);
		}

		public bool Equals(TransformModuleInstanceBase other) {
			return Equals(other as ITransformModuleInstance);
		}
	}
}

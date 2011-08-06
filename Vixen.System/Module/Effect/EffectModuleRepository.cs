using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;
using Vixen.Module.Property;

namespace Vixen.Module.Effect {
	class EffectModuleRepository : GenericModuleRepository<IEffectModuleInstance> {
		// Effect name : command handler instance
		private Dictionary<string, Guid> _nameLookup = new Dictionary<string, Guid>();

		public IEffectModuleInstance Get(string commandName) {
			Guid typeId;
			if(_nameLookup.TryGetValue(commandName, out typeId)) {
				return Get(typeId);
			}
			return null;
		}

		override public void Add(Guid id) {
			EffectModuleDescriptorBase descriptor = Modules.GetDescriptorById<EffectModuleDescriptorBase>(id);
			if(descriptor != null) {
				_nameLookup[descriptor.EffectName] = id;

				// Determine which, if any, of the effect's dependencies are properties.
				descriptor.PropertyDependencies = descriptor.Dependencies.Where(x => Modules.GetDescriptorById<IPropertyModuleDescriptor>(x) != null).ToArray();
			}
		}

		override public void Remove(Guid id) {
			IEffectModuleDescriptor descriptor = Modules.GetDescriptorById<IEffectModuleDescriptor>(id);
			if(descriptor != null) {
				_nameLookup.Remove(descriptor.EffectName);
			}
		}
	}
}

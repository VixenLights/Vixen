using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Vixen.Module;
using Vixen.Module.Transform;

namespace TestTemplate {
	[DataContract]
	public class OutputControllerTemplateData : IModuleDataModel {
		// This is not called when deserialized.
		public OutputControllerTemplateData() {
			Transforms = new List<InstanceReference>();
			TransformData = new ModuleDataSet();
		}

		// This is necessary because when a data contract type is deserialized,
		// the default constructor is NOT called.  And TransformData isn't
		// directly de/serialized.
		[OnDeserializing]
		private void OnDeserializing(StreamingContext streamingContext) {
			TransformData = new ModuleDataSet();
			// Don't need to create Transforms because the serializer does since it's
			// decorated with DataMemberAttribute and directly de/serialized.
		}

		[DataMember]
		public List<InstanceReference> Transforms { get; set; }

		[DataMember(Name="TransformModuleDatum")]
		private XElement _TransformModuleDatum {
			get {
				if(TransformData != null) {
					return TransformData.ToXElement();
				}
				return null;
			}
			set {
				if(value != null) {
					TransformData.Deserialize(value.ToString());
				}
			}
		}

		public IModuleDataSet TransformData { get; private set; }

		public Guid ModuleTypeId { get; set; }

		public Guid ModuleInstanceId { get; set; }

		public IModuleDataSet ModuleDataSet { get; set; }

		public IModuleDataModel Clone() {
			OutputControllerTemplateData newInstance = new OutputControllerTemplateData();
			newInstance._TransformModuleDatum = _TransformModuleDatum;
			newInstance.Transforms = new List<InstanceReference>(Transforms.Select(x => x.Clone()));
			return newInstance;
		}
	}

	//[DataContract]
	//public class TransformInstanceReference {
	//    public TransformInstanceReference(ITransformModuleInstance instance) {
	//        TypeId = instance.TypeId;
	//        InstanceId = instance.InstanceId;
	//    }
		
	//    private TransformInstanceReference(Guid typeId, Guid instanceId) {
	//        TypeId = typeId;
	//        InstanceId = instanceId;
	//    }
		
	//    [DataMember]
	//    public Guid TypeId;
	//    [DataMember]
	//    public Guid InstanceId;

	//    public TransformInstanceReference Clone() {
	//        return new TransformInstanceReference(TypeId, InstanceId);
	//    }
	//}
}

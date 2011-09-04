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
	public class OutputControllerTemplateData : ModuleDataModelBase {
		// This is not called when deserialized.
		public OutputControllerTemplateData() {
			Transforms = new List<InstanceReference>();
			TransformData = new ModuleLocalDataSet();
		}

		// This is necessary because when a data contract type is deserialized,
		// the default constructor is NOT called.  And TransformData isn't
		// directly de/serialized.
		[OnDeserializing]
		private void OnDeserializing(StreamingContext streamingContext) {
			TransformData = new ModuleLocalDataSet();
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

		override public IModuleDataModel Clone() {
			OutputControllerTemplateData newInstance = new OutputControllerTemplateData();
			newInstance._TransformModuleDatum = _TransformModuleDatum;
			newInstance.Transforms = new List<InstanceReference>(Transforms.Select(x => x.Clone()));
			return newInstance;
		}
	}
}

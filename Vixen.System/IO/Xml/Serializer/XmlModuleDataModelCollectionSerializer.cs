using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module;
using Vixen.Sys;

namespace Vixen.IO.Xml.Serializer
{
	// This is different from the other *CollectionSerializer classes.
	// Other classes: Serialize the entire collection, including the container of that collection.
	// This class: Provided to a dataset so that it can serialize its internal data models without
	//             needing to expose them.
	internal class XmlModuleDataModelCollectionSerializer : IModuleDataModelCollectionSerializer
	{
		private XElement _containerElement;

		//called by the serializer
		public XmlModuleDataModelCollectionSerializer(XElement containerElement)
		{
			if (containerElement == null) throw new ArgumentNullException("containerElement");

			_containerElement = containerElement;
		}

		//called by the dataset
		public void Write(IEnumerable<IModuleDataModel> dataModels)
		{
			XmlModuleDataModelSerializer dataModelSerializer = new XmlModuleDataModelSerializer();
			_containerElement.Add(dataModels.Select(dataModelSerializer.WriteObject).Where(x => x != null));
		}

		//called by the dataset
		public IEnumerable<IModuleDataModel> Read()
		{
			List<IModuleDataModel> dataModels = new List<IModuleDataModel>();

			XmlModuleDataModelSerializer dataModelSerializer = new XmlModuleDataModelSerializer();

			dataModels.AddRange(_containerElement.Elements().AsParallel().Select(dataModelSerializer.ReadObject).Where(x => x != null));

			return dataModels;
		}
	}
}
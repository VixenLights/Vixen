using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Module.Property;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlElementNodeSerializer : IXmlSerializer<ElementNode>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private readonly Dictionary<Guid, Element> _underlyingElementMap; 
		private const string ELEMENT_NODE = "Node";
		private const string ATTR_ID = "id";
		private const string ATTR_NAME = "name";
		private const string ATTR_ELEMENT_ID = "channelId";

		public XmlElementNodeSerializer(IEnumerable<Element> underlyingElementsForRead)
		{
			// VixenSystem.Elements has not yet been populated because the file is in the
			// middle of being read/written.  This serializer has a dependency on the
			// newly read element collection.
			//_underlyingElements = underlyingElementsForRead;
			_underlyingElementMap = underlyingElementsForRead?.ToDictionary(x => x.Id);
		}

		public XElement WriteObject(ElementNode value)
		{
			object elementXmlElements = null;

			if (value.IsLeaf) {
				// Leaf - reference the single element by id
				if (value.Element != null) {
					elementXmlElements = new XAttribute(ATTR_ELEMENT_ID, value.Element.Id.ToString());
				}
			}
			else {
				// Branch - include the child nodes inline
				elementXmlElements = value.Children.Select(WriteObject);
			}

			XmlPropertyCollectionSerializer propertyCollectionSerializer = new XmlPropertyCollectionSerializer();
			XElement propertyCollectionElement = propertyCollectionSerializer.WriteObject(value.Properties);
			//XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			//XElement propertyDataElement = dataSetSerializer.WriteObject(value.Properties.PropertyData);

			XElement result = new XElement(ELEMENT_NODE, new XAttribute(ATTR_NAME, value.Name), new XAttribute(ATTR_ID, value.Id));
			if (propertyCollectionElement != null)
				result.Add(propertyCollectionElement);
			if (elementXmlElements != null)
				result.Add(elementXmlElements);

			return result;
		}
		public static int readCount = 0;
		public ElementNode ReadObject(XElement element)
		{
			try {
				System.Threading.Interlocked.Increment(ref readCount);

				string name = XmlHelper.GetAttribute(element, ATTR_NAME);
				if (name == null)
					return null;

				Guid? id = XmlHelper.GetGuidAttribute(element, ATTR_ID);
				if (id == null)
					return null;

				// check if we have already loaded the node with this GUID (ie. if it's a node that's referenced twice,
				// in different groups). If we have, return that node instead, and don't go any deeper into child nodes.
				// (we'll just assume that the XML data for this one is identical to the earlier XML node that was written
				// out. To be a bit more proper, we should probably change the WriteXML() to not fully write out repeat
				// ElementNodes, and instead do some sort of soft reference to the first one (ie. GUID only). )
				ElementNode existingNode = VixenSystem.Nodes.GetElementNode(id.Value);
				if (existingNode != null) {
					return existingNode;
				}

				// Children or element reference
				ElementNode node = null;
				Guid? elementId = XmlHelper.GetGuidAttribute(element, ATTR_ELEMENT_ID);
				if (elementId == null) {
					// Branch
					IEnumerable<ElementNode> childNodes = element.Elements(ELEMENT_NODE).Select(ReadObject).Where(x => x != null);
					node = new ElementNode(id.Value, name, null, childNodes);
				}
				else {
					// Leaf
					Element elem;
					_underlyingElementMap.TryGetValue(elementId.GetValueOrDefault(), out elem);
					if (elem != null) {
						node = new ElementNode(id.Value, name, elem, null);
					}
				}

				if (node != null) {
					//// Property data
					//// It's not necessary to load the data before the properties, but it will
					//// save it from creating data for each module and then dumping it.
					//XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
					//node.Properties.PropertyData = dataSetSerializer.ReadObject(element);

					// Properties
					XmlPropertyCollectionSerializer propertyCollectionSerializer = new XmlPropertyCollectionSerializer();
					IEnumerable<IPropertyModuleInstance> properties = propertyCollectionSerializer.ReadObject(element);
					foreach (IPropertyModuleInstance instance in properties) {
						node.Properties.Add(instance);
					}
				}

				return node;
			} catch (Exception e) {
				logging.Error(e, "Error loading Element Node from XML");
				return null;
			}
		}
	}
}
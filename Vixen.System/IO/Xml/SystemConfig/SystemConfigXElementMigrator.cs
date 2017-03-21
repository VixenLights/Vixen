using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml.SystemConfig
{
	internal class SystemConfigXElementMigrator : IContentMigrator<XElement>
	{
		public SystemConfigXElementMigrator()
		{
			ValidMigrations = new[]
			                  	{
			                  		new MigrationSegment<XElement>(1, 2, _Version_1_to_2),
			                  		new MigrationSegment<XElement>(2, 3, _Version_2_to_3),
			                  		new MigrationSegment<XElement>(3, 4, _Version_3_to_4),
			                  		new MigrationSegment<XElement>(4, 5, _Version_4_to_5),
			                  		new MigrationSegment<XElement>(5, 6, _Version_5_to_6),
			                  		new MigrationSegment<XElement>(6, 7, _Version_6_to_7),
			                  		new MigrationSegment<XElement>(7, 8, _Version_7_to_8),
			                  		new MigrationSegment<XElement>(8, 9, _Version_8_to_9),
			                  		new MigrationSegment<XElement>(9, 10, _Version_9_to_10),
			                  		new MigrationSegment<XElement>(10, 11, _Version_10_to_11),
			                  		new MigrationSegment<XElement>(11, 12, _Version_11_to_12),
			                  		new MigrationSegment<XElement>(12, 13, _Version_12_to_13),
									new MigrationSegment<XElement>(13, 14, _Version_13_to_14),
									new MigrationSegment<XElement>(14, 15, _Version_14_to_15),
									new MigrationSegment<XElement>(15, 16, _Version_15_to_16)
			                  	};
		}

		public XElement MigrateContent(XElement content, int fromVersion, int toVersion)
		{
			IMigrationSegment<XElement> migrationSegment =
				ValidMigrations.FirstOrDefault(x => x.FromVersion == fromVersion && x.ToVersion == toVersion);
			if (migrationSegment == null) {
				throw new InvalidOperationException("Cannot migrate content from version " + fromVersion + " to version " +
				                                    toVersion);
			}
			content = migrationSegment.Execute(content);
			VixenSystem.MigrationOccured = true;
			return content;
		}

		object IContentMigrator.MigrateContent(object content, int fromVersion, int toVersion)
		{
			if (!(content is XElement)) throw new InvalidOperationException("Content must be an XElement.");

			return MigrateContent((XElement) content, fromVersion, toVersion);
		}

		public IEnumerable<IMigrationSegment<XElement>> ValidMigrations { get; private set; }

		IEnumerable<IMigrationSegment> IContentMigrator.ValidMigrations
		{
			get { return ValidMigrations; }
		}

		private XElement _Version_1_to_2(XElement content)
		{
			content.Add(new XElement("Identity", Guid.NewGuid().ToString()));
			return content;
		}

		private XElement _Version_2_to_3(XElement content)
		{
			content.Add(new XElement("Controllers"));
			return content;
		}

		private XElement _Version_3_to_4(XElement content)
		{
			//XElement controllersElement = _content.Element(XmlControllerCollectionSerializer.ELEMENT_CONTROLLERS);
			//if(controllersElement != null) {
			//    foreach(XElement controllerElement in controllersElement.Elements(XmlControllerCollectionSerializer.ELEMENT_CONTROLLER)) {
			//        XElement transformDataElement = controllerElement.Element(ELEMENT_TRANSFORM_DATA);
			//        string content = transformDataElement.InnerXml();
			//        transformDataElement.Remove();
			//        XElement moduleDataElement = new XElement(ELEMENT_MODULE_DATA, XElement.Parse(content));
			//        controllerElement.Add(moduleDataElement);
			//    }
			//}
			//return element;
			return content;
		}

		private XElement _Version_4_to_5(XElement content)
		{
			content.Add(new XElement("DisabledControllers"));
			return content;
		}

		private XElement _Version_5_to_6(XElement content)
		{
			content.Add(new XElement("ControllerLinks"));
			return content;
		}

		private XElement _Version_6_to_7(XElement content)
		{
			XElement controllersElement = content.Element("Controllers");
			if (controllersElement != null) {
				foreach (XElement controllerElement in controllersElement.Elements("Controller")) {
					XElement outputsElement = controllerElement.Element("Outputs");
					if (outputsElement != null) {
						foreach (XElement outputElement in outputsElement.Elements("Output")) {
							outputElement.Add(new XElement("FilterNodes"));
						}
					}
				}
			}
			return content;
		}

		private XElement _Version_7_to_8(XElement content)
		{
			content.Add(new XElement("Previews"));
			return content;
		}

		private XElement _Version_8_to_9(XElement content)
		{
			content.Add(new XElement("AllowFilterEvaluation", true));
			return content;
		}

		private XElement _Version_9_to_10(XElement content)
		{
			content.Add(new XElement("AllowSubordinateEffects", true));
			return content;
		}

		private XElement _Version_10_to_11(XElement content)
		{
			content.Add(new XElement("SmartControllers"));
			return content;
		}

		private XElement _Version_11_to_12(XElement content)
		{
			content = _FixControllerData_AddInstanceIds(content);
			content = _FixSmartControllerData_AddInstanceIds(content);
			content = _FixPreviewData_AddInstanceIds(content);
			return content;
		}

		private XElement _FixControllerData_AddInstanceIds(XElement contentElement)
		{
			return _FixDeviceData_AddInstanceIds(contentElement, _GetControllerElements);
		}

		private XElement _FixSmartControllerData_AddInstanceIds(XElement contentElement)
		{
			return _FixDeviceData_AddInstanceIds(contentElement, _GetSmartControllerElements);
		}

		private XElement _FixPreviewData_AddInstanceIds(XElement contentElement)
		{
			return _FixDeviceData_AddInstanceIds(contentElement, _GetPreviewElements);
		}

		private XElement _FixDeviceData_AddInstanceIds(XElement contentElement,
		                                               Func<XElement, IEnumerable<XElement>> deviceElementSelector)
		{
			Guid[] moduleTypeIds = _GetModuleTypeIds(contentElement, deviceElementSelector);
			if (moduleTypeIds == null) return null;

			Guid[] moduleInstanceIds = _GetModuleInstanceIds(moduleTypeIds);
			if (moduleInstanceIds == null) return null;

			return _AddInstanceIdsToDevices(contentElement, moduleTypeIds, moduleInstanceIds, deviceElementSelector);
		}

		private Guid[] _GetModuleTypeIds(XElement systemConfigContent,
		                                 Func<XElement, IEnumerable<XElement>> deviceElementSelector)
		{
			IEnumerable<XElement> deviceElements = deviceElementSelector(systemConfigContent);
			if (deviceElements == null) return null;

			var idStrings = deviceElements.Select(x => x.Attribute("hardwareId")).Where(x => x != null).Select(x => x.Value);
			return idStrings.Select(x => new Guid(x)).ToArray();
		}

		private IEnumerable<XElement> _GetControllerElements(XElement systemConfigContent)
		{
			XElement controllersElement = systemConfigContent.Element("Controllers");
			if (controllersElement == null) return null;

			return controllersElement.Elements("Controller");
		}

		private IEnumerable<XElement> _GetSmartControllerElements(XElement systemConfigContent)
		{
			XElement controllersElement = systemConfigContent.Element("SmartControllers");
			if (controllersElement == null) return null;

			return controllersElement.Elements("SmartController");
		}

		private IEnumerable<XElement> _GetPreviewElements(XElement systemConfigContent)
		{
			XElement controllersElement = systemConfigContent.Element("Previews");
			if (controllersElement == null) return null;

			return controllersElement.Elements("Preview");
		}

		private Guid[] _GetModuleInstanceIds(IEnumerable<Guid> moduleTypeIds)
		{
			return
				moduleTypeIds.SelectMany(
					x =>
					VixenSystem.ModuleStore.InstanceData.DataModels.Where(y => y.ModuleTypeId.Equals(x)).Select(z => z.ModuleInstanceId))
					.ToArray();
		}

		private XElement _AddInstanceIdsToDevices(XElement systemConfigContent, Guid[] moduleTypeIds, Guid[] moduleInstanceIds,
		                                          Func<XElement, IEnumerable<XElement>> deviceElementSelector)
		{
			XElement[] deviceElements = deviceElementSelector(systemConfigContent).ToArray();

			// There may be duplicate data in the module data store due to the bug that this is fixing, so we can't
			// assume that the number of devices will match the number of data elements for the devices.
			//if(moduleTypeIds.Length != moduleInstanceIds.Length || moduleInstanceIds.Length != deviceElements.Length) {
			//    throw new Exception("Module type id count: " + moduleTypeIds.Length + ", module instance id count: " + moduleInstanceIds.Length + ", device element count: " + deviceElements.Length);
			//}

			for (int i = 0; i < moduleTypeIds.Length; i++) {
				if (moduleTypeIds[i] != Guid.Parse(deviceElements[i].Attribute("hardwareId").Value)) {
					throw new Exception("Expected device type to match, but it did not.");
				}
				deviceElements[i].Add(new XAttribute("hardwareInstanceId", moduleInstanceIds[i]));
			}

			return systemConfigContent;
		}

		// Version 13: Changed the disabled controllers element to reflect disabled devices
		// (ie. controllers, smart controllers, and previews.) Changed name to suit.
		private XElement _Version_12_to_13(XElement content)
		{
			XElement disabledControllers = content.Element("DisabledControllers");
			XElement disabledDevices = new XElement("DisabledDevices");
			if (disabledControllers != null) {
				disabledDevices = new XElement(disabledControllers);
				disabledDevices.Name = "DisabledDevices";
				foreach (XElement disabledDevice in disabledDevices.Elements("Controller")) {
					disabledDevice.Name = "Device";
				}
				disabledControllers.Remove();
			}
			content.Add(disabledDevices);
			return content;
		}

		private XElement _Version_13_to_14(XElement content)
		{
			//Version 14 correct disconnected nodes that where not converted back after being elements.
			XElement nodes = content.Element("Nodes");
			if (nodes != null)
			{
				XElement channels = content.Element("Channels");

				foreach(XElement node in nodes.Elements())
				{
					IEnumerable<XElement> childNodes = node.Descendants("Node").Where(x => !x.Descendants("Node").Any());
					foreach (XElement childNode in childNodes)
					{
						if (!childNode.Attributes("channelId").Any() )
						{
							Guid channelId = Guid.NewGuid();
							childNode.SetAttributeValue("channelId", channelId);
							XElement channel = new XElement("Channel", 
								 new XAttribute("id",channelId),
								 new XAttribute("name", childNode.Attribute("name").Value) 
								 );
							channels.Add(channel);
						}
					}
				}
			}
			return content;
		}

		private XElement _Version_14_to_15(XElement content)
		{
			//Version 15 correct disconnected nodes that where not converted back after being elements.
			//Version 14 did not take care of the full depth of nested nodes possible.
			XElement nodes = content.Element("Nodes");
			if (nodes != null)
			{
				XElement channels = content.Element("Channels");

				foreach (XElement node in nodes.Elements())
				{
					IEnumerable<XElement> childNodes = node.Descendants("Node").Where(x => !x.Descendants("Node").Any());
					foreach (XElement childNode in childNodes)
					{
						if (!childNode.Attributes("channelId").Any())
						{
							Guid channelId = Guid.NewGuid();
							childNode.SetAttributeValue("channelId", channelId);
							XElement channel = new XElement("Channel",
								 new XAttribute("id", channelId),
								 new XAttribute("name", childNode.Attribute("name").Value)
								 );
							channels.Add(channel);
						}
					}
				}
			}
			return content;
		}

		private XElement _Version_15_to_16(XElement content)
		{
			//Version 16 move the default update interval back to a more normal 50ms if it is still 46ms.
			//If it is other than 46, the user probably made a choice of their own and we will honor that.
			XElement intervalElement = content.Element("DefaultUpdateInterval");
			if (intervalElement != null)
			{
				if (intervalElement.Value.Equals("46"))
				{
					intervalElement.Value = "50";	
				}
					
			}
			
			
			
			return content;
		}
		
	}
}
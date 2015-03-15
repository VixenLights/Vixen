using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Vixen.IO.Xml.ModuleStore
{
	internal class ModuleStoreXElementMigrator : IContentMigrator<XElement>
	{
		public ModuleStoreXElementMigrator()
		{
			ValidMigrations = new[]
			                  	{
			                  		new MigrationSegment<XElement>(1, 2, _Version_1_to_2)
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
			//  3/14/2015
			//Migrate full path name of the background image to just the filename. Code will now look 
 			//relative to the profile for the module path to the filenames
			var namespaces = new XmlNamespaceManager(new NameTable());
			XNamespace ns = "http://schemas.datacontract.org/2004/07/VixenModules.Preview.VixenPreview";
			namespaces.AddNamespace("ns", ns.NamespaceName);

			IEnumerable<XElement> fileNameElements =
				content.XPathSelectElements(
					"ModuleData/Module[@dataModelType='VixenModules.Preview.VixenPreview.VixenPreviewData, VixenPreview']/ns:VixenPreviewData/ns:BackgroundFileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				string fileName = Path.GetFileName(fileNameElement.Value);
				fileNameElement.SetValue(fileName);	
			}
			
			return content;
		}
	}
}
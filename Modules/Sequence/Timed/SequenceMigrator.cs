using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Vixen.IO;

namespace VixenModules.Sequence.Timed
{
	public class SequenceMigrator : IContentMigrator<XElement>
	{
		public SequenceMigrator()
		{
			ValidMigrations = new[]
				{
									new MigrationSegment<XElement>(0, 1, _EmptyMigration),
									new MigrationSegment<XElement>(1, 2, _EmptyMigration),
									new MigrationSegment<XElement>(2, 3, _Version_0_to_3)
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

		private XElement _EmptyMigration(XElement content)
		{
			return content;
		}

		private XElement _Version_0_to_3(XElement content)
		{
			//  3/14/2015
			//Migrate full path name of the background image to just the filename. Code will now look 
 			//relative to the profile for the module path to the filenames
			var namespaces = new XmlNamespaceManager(new NameTable());
			XNamespace ns = "http://schemas.datacontract.org/2004/07/VixenModules.Sequence.Timed";
			namespaces.AddNamespace("", ns.NamespaceName);
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Nutcracker";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);
			XNamespace d1p1 = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
			namespaces.AddNamespace("d1p1", d1p1.NamespaceName);

			//Fix the paths on the Nutcracker Picture filenames
			IEnumerable<XElement> fileNameElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType/d2p1:NutcrackerData/d2p1:Picture_FileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				string fileName = Path.GetFileName(fileNameElement.Value);
				fileNameElement.SetValue(fileName);
			}

			//Fix the paths on the Nutcracker Picture filenames
			fileNameElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType/d2p1:NutcrackerData/d2p1:PictureTile_FileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				if (!IsNutcrackerResource(fileNameElement.Value))
				{
					string fileName = Path.GetFileName(fileNameElement.Value);
					fileNameElement.SetValue(fileName);	
				}
				
			}

			//Fix the paths on the Nutcracker Glediator filenames
			fileNameElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType/d2p1:NutcrackerData/d2p1:Glediator_FileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				string fileName = Path.GetFileName(fileNameElement.Value);
				fileNameElement.SetValue(fileName);
			}
			
			return content;
		}

		public static bool IsNutcrackerResource(string s)
		{
			return s.Contains("VixenModules.Effect.Nutcracker");
		}
	}
}
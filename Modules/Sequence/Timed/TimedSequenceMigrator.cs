using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Vixen.IO;
using Vixen.Services;

namespace VixenModules.Sequence.Timed
{
	/// <summary>
	/// Class to conduct sequence migrations
	/// The Version atribute in the descriptor for the sequence determines the current version and is
	/// used to determine if migration should occur
	/// </summary>
	public class TimedSequenceMigrator : IContentMigrator<XElement>
	{
		public TimedSequenceMigrator()
		{
			ValidMigrations = new[]
				{
									new MigrationSegment<XElement>(0, 1, _Version_0_to_1)
									
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

		private XElement _Version_0_to_1(XElement content)
		{
			//  3/14/2015
			//Migrate full path name of the background image to just the filename. Code will now look 
 			//relative to the profile for the module path to the filenames
			//This is the first introduction of versioning for sequences so not versioned files are considered version 0
			//new files and migrated ones will have a version atribute in the root element
			var namespaces = new XmlNamespaceManager(new NameTable());
			XNamespace ns = "http://schemas.datacontract.org/2004/07/VixenModules.Sequence.Timed";
			namespaces.AddNamespace("", ns.NamespaceName);
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Nutcracker";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);
			XNamespace d1p1 = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
			namespaces.AddNamespace("d1p1", d1p1.NamespaceName);

			//Fix the paths on the Nutcracker Picture filenames so they are now relative to the profile instead of full paths
			IEnumerable<XElement> fileNameElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType/d2p1:NutcrackerData/d2p1:Picture_FileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				string fileName = Path.GetFileName(fileNameElement.Value);
				fileNameElement.SetValue(fileName);
			}

			//Fix the paths on the Nutcracker Picture filenames  so they are now relative to the profile instead of full paths
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

			//Fix the paths on the Nutcracker Glediator filenames so they are now relative to the profile instead of full paths
			fileNameElements =
				content.XPathSelectElements(
					"_dataModels/d1p1:anyType/d2p1:NutcrackerData/d2p1:Glediator_FileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				string fileName = Path.GetFileName(fileNameElement.Value);
				fileNameElement.SetValue(fileName);
			}


			//Fix the audio paths on Media so they are now relative to the profile instead of full paths
			fileNameElements =
				content.XPathSelectElements(
					"_mediaSurrogates/MediaSurrogate/FilePath",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				string filePath = fileNameElement.Value;
				string fileName = Path.GetFileName(filePath);
				fileNameElement.SetValue(fileName);
				fileNameElement.Name = "FileName";
				string newPath = Path.Combine(MediaService.MediaDirectory, fileName);
				if (File.Exists(filePath) && !File.Exists(newPath))
				{
					File.Copy(filePath, newPath);	
				}
					
			}

			//Fix the provider source name
			IEnumerable<XElement> timingElements =
				content.XPathSelectElements(
					"_selectedTimingProviderSurrogate",
					namespaces);

			foreach (var timingElement in timingElements)
			{
				XElement type = timingElement.Element("ProviderType");
				if (type != null && type.Value.Equals("Media"))
				{
					XElement source = timingElement.Element("SourceName");
					if (source != null)
					{
						string fileName = Path.GetFileName(source.Value);
						source.SetValue(fileName);		
					}
					
				}
				
			}
			
			return content;
		}

		private bool IsNutcrackerResource(string s)
		{
			return s.Contains("VixenModules.Effect.Nutcracker");
		}
	}
}
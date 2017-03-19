using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Vixen.Extensions;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.IO.Xml.ModuleStore
{
	internal class ModuleStoreXElementMigrator : IContentMigrator<XElement>
	{
		public ModuleStoreXElementMigrator()
		{
			ValidMigrations = new[]
			                  	{
			                  		new MigrationSegment<XElement>(1, 2, _Version_1_to_2),
									new MigrationSegment<XElement>(2, 3, _Version_2_to_3)
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

		private XElement _Version_2_to_3(XElement content)
		{
			//  3/12/2017
			//Migrate full path name of the sequences in the scheduler to just the relative to the sequence folder. Code will now look 
			//relative to the profile sequence folder path to the filenames
			var namespaces = new XmlNamespaceManager(new NameTable());
			XNamespace ns = "http://schemas.datacontract.org/2004/07/VixenModules.App.Shows";
			namespaces.AddNamespace("ns", ns.NamespaceName);

			IEnumerable<XElement> fileNameElements =
				content.XPathSelectElements(
					"ModuleData/Module[@dataModelType='VixenModules.App.Shows.ShowsData, Shows']/ns:ShowsData/ns:Shows/ns:Show/ns:Items/ns:ShowItem/ns:Sequence_FileName",
					namespaces);

			foreach (var fileNameElement in fileNameElements)
			{
				if (!string.IsNullOrEmpty(fileNameElement.Value))
				{
					
					if (!fileNameElement.Value.StartsWith(SequenceService.SequenceDirectory))
					{
						var fileInfo = CopyShowSequenceFileLocal(fileNameElement.Value);
						if (fileInfo.Item1)
						{
							fileNameElement.SetValue(fileInfo.Item2);
							continue;
						}
						MessageBox.Show(string.Format("A sequence path {0} referenced in a Show is unable to be migrated correctly."+
								" Check your Shows and correct any sequence paths necessary to reference sequences from the profile sequence folder.",fileNameElement.Value), "Show Migration", MessageBoxButton.OK, MessageBoxImage.Warning);
					}
					
					FileSystemInfo sequenceDirectory = new DirectoryInfo(SequenceService.SequenceDirectory);
					FileSystemInfo showPath = new FileInfo(fileNameElement.Value);
					string fileName = sequenceDirectory.GetRelativePathTo(showPath);
					fileNameElement.SetValue(fileName);
				}
				
			}

			return content;
		}

		private Tuple<bool, string> CopyShowSequenceFileLocal(string filePath)
		{
			bool success = false;
			string fileName = Path.GetFileName(filePath);
			if (!string.IsNullOrEmpty(fileName))
			{
				string newPath = Path.Combine(SequenceService.SequenceDirectory, fileName);
				if (File.Exists(newPath))
				{
					MessageBox.Show(string.Format("A sequence path {0} referenced in a Show was not pointed to a sequence in the current profile sequence folder. Another sequence in the current profile sequence folder has the same name so it could not be migrated." + 
						" The Show will be pointed to the sequence folder version.\n\nPlease verify your Show is using the correct version.", filePath), "Show Migration", MessageBoxButton.OK, MessageBoxImage.Warning);
					success = true;
				}
				else if (File.Exists(filePath))
				{
					File.Copy(filePath, newPath);
					success = true;
					MessageBox.Show(string.Format("A sequence path {0} referenced in a Show was not in the current profile." +
								" It was copied into the local profile sequence folder as part of a Scheduler migration and the Show was updated.", filePath), "Show Migration", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			
			return new Tuple<bool, string>(success, fileName);

		}

	}
}
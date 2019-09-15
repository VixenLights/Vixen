using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
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
									new MigrationSegment<XElement>(2, 3, _Version_2_to_3),
									new MigrationSegment<XElement>(3, 4, _Version_3_to_4)
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

		private XElement _Version_3_to_4(XElement content)
		{
			//  2/6/2018
			// Migrate existing LipSync Matrix Drawings to Bitmaps
			var namespaces = new XmlNamespaceManager(new NameTable());
			XNamespace ns = "http://schemas.datacontract.org/2004/07/VixenModules.App.LipSyncApp";
			namespaces.AddNamespace("ns", ns.NamespaceName);

			IEnumerable<XElement> lipSyncElements =
				content.XPathSelectElements(
					"ModuleData/Module[@dataModelType='VixenModules.App.LipSyncApp.LipSyncMapStaticData, LipSyncApp']/ns:LipSyncMapStaticData/ns:_library",
					namespaces);


			var mapElements = lipSyncElements.Elements();
			
			foreach (var mapElem in mapElements)
			{
				Dictionary<string, Bitmap> phonemeBitmaps = new Dictionary<string, Bitmap>();

				var keyElement =
					(mapElem.Elements().Select(a =>
					new { keyVal = a })
					.Where(b => b.keyVal.Name.LocalName.Contains("Key")))
					.First().keyVal;

				XElement valueElement = 
					(mapElem.Elements().Select(a => 
					new { valueElem = a, valueName = a.Name.LocalName.Contains("Value") })
					.Where(n => n.valueName))
					.First().valueElem;

				string mapName = valueElement.Element(ns + "LibraryReferenceName").Value;
				string moduleDataPath = Paths.ModuleDataFilesPath + "\\LipSync";
				string newDirName = moduleDataPath + "\\" + mapName;

				bool isMatrix= Convert.ToBoolean(valueElement.Element(ns + "IsMatrix").Value.ToString());

				if (isMatrix)
				{
					//Create a directory to save the space in the appropriate location. 
					if (Directory.Exists(newDirName))
					{
						try
						{
							Directory.Delete(newDirName, true);
						}
						catch (Exception) { }
					}

					//If this throws an exception, we want it to fall thru and abort the conversion 
					Directory.CreateDirectory(newDirName);
				}

				//Remove ZoomLevel
				XElement zoomLevel = valueElement.Element(ns + "ZoomLevel");
				if (null != zoomLevel)
				{
					zoomLevel.Remove();
				}

				//Read StringsareRows and determine matrix orientation
				bool stringsAreRows = false;
				XElement orientationElem = valueElement.Element(ns + "StringsAreRows");
				if (null != orientationElem)
				{
					stringsAreRows = Convert.ToBoolean(orientationElem.Value);
				}

				XElement matrixPixelsPerStringElem = valueElement.Element(ns +"MatrixPixelsPerString");
				XElement matrixStringCountElem = valueElement.Element(ns +"MatrixStringCount");

				var matrixPixelsPerString = Convert.ToInt32(matrixPixelsPerStringElem.Value);
				var matrixStringCount = Convert.ToInt32(matrixStringCountElem.Value);

				matrixPixelsPerStringElem.Remove();
				matrixStringCountElem.Remove();

				XElement mapItemsElem = valueElement.Element(ns + "MapItems");

				if ((isMatrix) && (matrixStringCount > 0) && (matrixPixelsPerString > 0))
				{
					int height = (stringsAreRows) ? matrixStringCount : matrixPixelsPerString;
					int width = (stringsAreRows) ? matrixPixelsPerString : matrixStringCount;
					int row = 0;
					int col = 0;

					IEnumerable<XElement> mapItemElemArray = mapItemsElem.Elements();
					foreach (XElement mapItemElem in mapItemElemArray)
					{
						XElement elementColor = mapItemElem.Element(ns + "ElementColor");
						if (null != elementColor)
						{
							elementColor.Remove();
						}

						string _stringName = mapItemElem.Element(ns + "_stringName").Value;
						string[] _stringNameTokens = _stringName.Split(' ');

						XElement elementColorsList = mapItemElem.Element(ns + "ElementColors");
						if (null != elementColorsList)
						{
							var elementColorsMappings =
								(elementColorsList.Elements()
								.Select(a => new { keyVal = a })
								.Where(b => b.keyVal.Name.LocalName.Contains("KeyValueOfPhonemeTypeColor")));

							foreach (var phonemeToColorMapping in elementColorsMappings)
							{
								string elementColorsListKey =
									(phonemeToColorMapping.keyVal.Elements()
									.Select(a => new { keyVal = a })
									.Where(b => b.keyVal.Name.LocalName.Contains("Key")))
									.First().keyVal.Value;

								var elementColorsListVal =
									(phonemeToColorMapping.keyVal.Elements()
									.Select(a => new { keyVal = a })
									.Where(b => b.keyVal.Name.LocalName.Contains("Value")))
									.First().keyVal;

								Bitmap phonemeBitmap;

								if (!phonemeBitmaps.TryGetValue(elementColorsListKey,out phonemeBitmap))
								{
									phonemeBitmaps[elementColorsListKey] = new Bitmap(width, height);
								}

								phonemeBitmap = phonemeBitmaps[elementColorsListKey];

								XElement knownColorElem = elementColorsListVal.Descendants().First();
								XElement nameElem = (XElement)knownColorElem.NextNode;
								XElement stateElem = (XElement)nameElem.NextNode;
								XElement valueElem = (XElement)stateElem.NextNode;

								//Read Value, knownColor,name,state,value
								KnownColor kc = (KnownColor)Convert.ToUInt32(knownColorElem.Value);
								int state = Convert.ToInt32(Convert.ToByte(stateElem.Value));
								int value = (int)(Convert.ToUInt32(valueElem.Value));

								Color c;
								if (1 == state)
								{
									c = Color.FromKnownColor(kc);
								}
								else if (2 == state)
								{
									c = Color.FromArgb(value);
								}
								else
								{
									c = Color.Black;
								}

								int x = (stringsAreRows) ? col : row; 
								int y = (stringsAreRows) ? row : col;

								int tmpY = y;
								y = (stringsAreRows) ?
									((matrixStringCount - 1) - y) :
									((matrixPixelsPerString - 1) - y);
								try
								{
									phonemeBitmap.SetPixel(x, y, c);
								}
								catch (Exception) { }
							}

							col = (++col) % matrixPixelsPerString;
							row = (0 == col) ? row + 1 : row;
							
						}
					}

					mapItemsElem.RemoveAll();

					foreach (KeyValuePair<string,Bitmap> kv in phonemeBitmaps)
					{
						string bmpName = newDirName + "\\" + kv.Key + ".bmp";
						kv.Value.Save(bmpName);
					}
				}

				//Add a Notes field
				XElement notesElem = valueElement.AddElement("Notes");
				if (null != notesElem)
				{
					notesElem.SetValue("");
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
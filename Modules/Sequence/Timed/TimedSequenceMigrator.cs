using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Common.Controls.ColorManagement.ColorModels;
using NLog.Targets;
using Vixen.IO;
using Vixen.Module;
using Vixen.Services;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Alternating;
using ZedGraph;

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
									new MigrationSegment<XElement>(0, 1, _Version_0_to_1),
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

		private XElement _Version_0_to_1(XElement content)
		{
			MessageBox.Show(
				@"Migrating sequence from version 0 to version 1.\nChanges include moving Nutcracker and Audio files to the common media folder.");
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

		private XElement _Version_1_to_2(XElement content)
		{
			MessageBox.Show(
				@"Migrating sequence from version 1 to version 2.\nChanges include upgrades to the Alternating effect to allow more than 2 colors.");
			//This migration deals with changing the Alternating effect to a Multi Alternating
			//Style that allows N number of colors. 
			var namespaces = new XmlNamespaceManager(new NameTable());
			XNamespace ns = "http://schemas.datacontract.org/2004/07/VixenModules.Sequence.Timed";
			namespaces.AddNamespace("", ns.NamespaceName);
			XNamespace d2p1 = "http://schemas.datacontract.org/2004/07/VixenModules.Effect.Alternating";
			namespaces.AddNamespace("d2p1", d2p1.NamespaceName);
			XNamespace d1p1 = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
			namespaces.AddNamespace("d1p1", d1p1.NamespaceName);
			XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
			namespaces.AddNamespace("i", i.NamespaceName);
			XNamespace a = "http://www.w3.org/2001/XMLSchema";
			namespaces.AddNamespace("a", a.NamespaceName);
			
			//Find the Alternating effects.
			IEnumerable<XElement> alternatingElements =
				content.XPathSelectElements(
				"_dataModels/d1p1:anyType[@i:type = 'd2p1:AlternatingData']",
					namespaces);

			var datamodel = content.XPathSelectElement("_dataModels",namespaces);

			foreach (var alternatingElement in alternatingElements.ToList())
			{
				//Find all the data points.
				XElement isStaticColor1 = alternatingElement.XPathSelectElement("d2p1:StaticColor1", namespaces);
				XElement isStaticColor2 = alternatingElement.XPathSelectElement("d2p1:StaticColor2", namespaces);
				XElement color1 = alternatingElement.XPathSelectElement("d2p1:Color1", namespaces);
				XElement level1 = alternatingElement.XPathSelectElement("d2p1:Level1", namespaces);
				XElement colorGradient1 = alternatingElement.XPathSelectElement("d2p1:ColorGradient1", namespaces);
				XElement curve1 = alternatingElement.XPathSelectElement("d2p1:Curve1", namespaces);
				XElement color2 = alternatingElement.XPathSelectElement("d2p1:Color2", namespaces);
				XElement level2 = alternatingElement.XPathSelectElement("d2p1:Level2", namespaces);
				XElement colorGradient2 = alternatingElement.XPathSelectElement("d2p1:ColorGradient2", namespaces);
				XElement curve2 = alternatingElement.XPathSelectElement("d2p1:Curve2", namespaces);
				XElement enable = alternatingElement.XPathSelectElement("d2p1:Enable", namespaces);
				XElement groupEffect = alternatingElement.XPathSelectElement("d2p1:GroupEffect", namespaces);
				XElement interval = alternatingElement.XPathSelectElement("d2p1:Interval", namespaces);
				XElement moduleInstanceId = alternatingElement.XPathSelectElement("ModuleInstanceId", namespaces);
				XElement moduleTypeId = alternatingElement.XPathSelectElement("ModuleTypeId", namespaces);

				//Determine which of the old types were used, colors vs gradients
				//The new only uses gradients and levels so take the two and build up the new object
				var gradientLevelPairs = new List<GradientLevelPair>();

				//Colors are used backwards in the old effect, so pick them off in reverse.
				if (isStaticColor2.Value.Equals("true"))
				{
					RGB c2 = DeSerializer<RGB>(color2);
					double l2 = DeSerializer<double>(level2);
					l2 *= 100;
					gradientLevelPairs.Add(new GradientLevelPair(c2,
						new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { l2, l2 }))));
				}
				else
				{
					ColorGradient cg2 = DeSerializer<ColorGradient>(colorGradient2);
					Curve c2 = DeSerializer<Curve>(curve2);
					gradientLevelPairs.Add(new GradientLevelPair(cg2, c2));
				}


				if (isStaticColor1.Value.Equals("true"))
				{
					RGB c1 = DeSerializer<RGB>(color1);
					double l1 = DeSerializer<double>(level1);
					l1 *= 100;
					gradientLevelPairs.Add(new GradientLevelPair(c1,
						new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {l1, l1}))));
				}
				else
				{
					ColorGradient cg1 = DeSerializer<ColorGradient>(colorGradient1);	
					Curve c1 = DeSerializer<Curve>(curve1);
					gradientLevelPairs.Add(new GradientLevelPair(cg1, c1));	
				}
				

				//Build the new data model
				int grouplevel = DeSerializer<int>(groupEffect);
				if (grouplevel > 0)
				{
					grouplevel -= 1;
				}
				AlternatingData data = new AlternatingData
				{
					Colors = gradientLevelPairs,
					ModuleInstanceId = DeSerializer<Guid>(moduleInstanceId),
					ModuleTypeId = DeSerializer<Guid>(moduleTypeId),
					EnableStatic = !DeSerializer<bool>(enable),
					DepthOfEffect = grouplevel,
					Interval = DeSerializer<int>(interval),
					IntervalSkipCount = 1
				};

				//Remove the old data model from the xml
				alternatingElement.Remove();

				//Build up a temporary container similar to the way sequences are stored to
				//make all the namespace prefixes line up.
				IModuleDataModel[] dm = {data};
				DataContainer dc = new DataContainer {_dataModels = dm};

				//Serialize the object into a xelement
				XElement glp = Serializer(dc);

				//Extract the new data model that we want and insert it in the tree
				datamodel.Add(glp.XPathSelectElement("//*[local-name()='anyType']", namespaces));
			}
		
			return content;
		}

		static T DeSerializer<T>(XElement element)
		{
			var serializer = new DataContractSerializer(typeof(T), element.Name.LocalName, element.Name.NamespaceName);
			return (T)serializer.ReadObject(element.CreateReader());
		}

		static XElement Serializer(DataContainer data)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				XmlWriterSettings settings = new XmlWriterSettings
				{
					//Encoding = Encoding.ASCII,
					Indent = true,
					NamespaceHandling = NamespaceHandling.OmitDuplicates
				};
				using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
				{
					_WriteSequenceDataToXmlWriter(data, xmlWriter);
					xmlWriter.Flush();
				}

				stream.Seek(0, SeekOrigin.Begin);

				using (var streamReader = new StreamReader(stream))
				{
					string result = streamReader.ReadToEnd();
					return XElement.Parse(result);
				}

				
			}
		}

		private static void _WriteSequenceDataToXmlWriter(DataContainer data, XmlWriter xmlWriter)
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(DataContainer), new[] { typeof(AlternatingData), typeof(IModuleDataModel[]), typeof(DataContainer) });
			serializer.WriteStartObject(xmlWriter, data);
			xmlWriter.WriteAttributeString("xmlns", "a", null, "http://www.w3.org/2001/XMLSchema");
			serializer.WriteObjectContent(xmlWriter, data);
			xmlWriter.WriteEndElement();
		}

		private bool IsNutcrackerResource(string s)
		{
			return s.Contains("VixenModules.Effect.Nutcracker");
		}

		
	}

	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/VixenModules.Sequence.Timed")]
	internal class DataContainer
	{
		[DataMember]
		internal IModuleDataModel[] _dataModels;
	}
}
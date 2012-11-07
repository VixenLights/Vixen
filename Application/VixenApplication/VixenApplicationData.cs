using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Vixen.Sys;

namespace VixenApplication
{
	// This is a class to handle persisting data for the VixenApplication. It does this by serializing out
	// data to the Vixen Data directory and reading it back from the same file. In future, it may need to
	// also handle migration of data if data formats change, etc., but for now it's probably not an issue.
	public class VixenApplicationData
	{
		private const string _DataFilename = "VixenApplicationData.xml";

		private const int DATA_FORMAT_VERSION_NUMBER = 3;


		public List<string> RecentSequences { get; set; }

		public Dictionary<Guid, FilterSetupFormShapePosition> FilterSetupFormShapePositions { get; set; }

		public bool FilterSetupFormHighQualityRendering { get; set; }

		private string DataFilepath
		{
			get { return Path.Combine(Vixen.Sys.Paths.DataRootPath, _DataFilename); }
		}

		public VixenApplicationData()
		{
			RecentSequences = new List<string>();
			FilterSetupFormShapePositions = new Dictionary<Guid, FilterSetupFormShapePosition>();
			FilterSetupFormHighQualityRendering = false;
			LoadData();
		}


		public void LoadData()
		{
			FileStream stream = null;

			if (!File.Exists(DataFilepath))
				return;

			try {
				stream = new FileStream(DataFilepath, FileMode.Open);

				XElement root = XElement.Load(stream);

				XElement versionElement = root.Element("DataFormatVersion");
				if (versionElement == null) {
					VixenSystem.Logging.Error("VixenApplication: loading application data: couldn't find data format version");
					return;
				}
				int dataFormatVersion = int.Parse(versionElement.Value);

				ReadData(dataFormatVersion, root);

			} catch(FileNotFoundException ex) {
				VixenSystem.Logging.Warning("VixenApplication: loading application data, but couldn't find file", ex);
			} catch (Exception ex) {
				VixenSystem.Logging.Error("VixenApplication: error loading application data", ex);
			} finally {
				if (stream != null)
					stream.Close();
			}
		}

		public void SaveData()
		{
			FileStream stream = null;
			try {
				stream = new FileStream(DataFilepath, FileMode.Create);

				XElement root = new XElement("VixenApplicationData");

				root.Add(new XElement("DataFormatVersion", DATA_FORMAT_VERSION_NUMBER));

				XElement recentSequencesElement = new XElement("RecentSequences");
				RecentSequences.ForEach(s => recentSequencesElement.Add(new XElement("SequenceFile", s)));
				root.Add(recentSequencesElement);

				XElement filterShapePositionsElement = new XElement("FilterShapePositions");
				foreach (KeyValuePair<Guid, FilterSetupFormShapePosition> pair in FilterSetupFormShapePositions) {
					filterShapePositionsElement.Add(
						new XElement("FilterPosition",
				             new XAttribute("FilterId", pair.Key),
				             new XElement("xPositionProportion", pair.Value.xPositionProportion),
				             new XElement("yPosition", pair.Value.yPosition)
						)
					);
				}
				root.Add(filterShapePositionsElement);

				XElement filterSetupFormHQRenderingElement = new XElement("FilterSetupFormHighQualityRendering");
				filterSetupFormHQRenderingElement.Add(new XAttribute("value", FilterSetupFormHighQualityRendering));
				root.Add(filterSetupFormHQRenderingElement);

				root.Save(stream);

			} catch (Exception ex) {
				VixenSystem.Logging.Error("VixenApplication: error saving application data", ex);
			} finally {
				if (stream != null)
				    stream.Close();
			}
		}

		public void ReadData(int dataVersion, XElement rootElement)
		{
			if (dataVersion > DATA_FORMAT_VERSION_NUMBER) {
				VixenSystem.Logging.Error("VixenApplication: error reading application data; given data version was too high: " + dataVersion);
				return;
			}

			// recent sequences: in all data formats
			XElement recentSequences = rootElement.Element("RecentSequences");
			if (recentSequences != null) {
				RecentSequences = new List<string>();
				foreach (XElement element in recentSequences.Elements("SequenceFile")) {
					RecentSequences.Add(element.Value);
				}
			}

			// filter shape positions: in data versions 2+
			if (dataVersion >= 2) {
				XElement filterShapePositionsElement = rootElement.Element("FilterShapePositions");
				if (filterShapePositionsElement != null) {
					FilterSetupFormShapePositions = new Dictionary<Guid, FilterSetupFormShapePosition>();
					foreach (XElement element in filterShapePositionsElement.Elements("FilterPosition")) {
						FilterSetupFormShapePosition position = new FilterSetupFormShapePosition();
						position.xPositionProportion = double.Parse(element.Element("xPositionProportion").Value);
						position.yPosition = int.Parse(element.Element("yPosition").Value);
						FilterSetupFormShapePositions.Add((Guid)element.Attribute("FilterId"), position);
					}
				}
			}

			// filter setup form HQ rendering added in data v3
			if (dataVersion >= 3) {
				XElement element = rootElement.Element("FilterSetupFormHighQualityRendering");
				if (element != null) {
					FilterSetupFormHighQualityRendering = Boolean.Parse(element.Attribute("value").Value);
				}
			}

		}
	}

	public class FilterSetupFormShapePosition
	{
		public double xPositionProportion { get; set; }
		public int yPosition { get; set; }
	}
}

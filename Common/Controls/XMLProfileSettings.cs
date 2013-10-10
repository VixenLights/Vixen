using System;
using System.Windows.Forms;
using System.Xml;

namespace Common.Controls
{
	public class XMLProfileSettings
	{
		private XmlDocument xmlDocument = new XmlDocument();
		private string documentPath;

		public XMLProfileSettings()
		{
			try {
				//documentPath = Application.UserAppDataPath + "\\settings.xml";
				documentPath =
					System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Vixen",
					                       "Settings.xml");
				if( !System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(documentPath)))
					System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(documentPath));
				xmlDocument.Load(documentPath);
			}
			catch {
				xmlDocument.LoadXml("<settings></settings>");
			}
		}

		public XMLProfileSettings(string path)
		{
			try {
				documentPath = path;
				xmlDocument.Load(documentPath);
			}
			catch {
				xmlDocument.LoadXml("<settings></settings>");
			}
		}

		public int GetSetting(string xPath, int defaultValue)
		{
			return Convert.ToInt16(GetSetting(xPath, Convert.ToString(defaultValue)));
		}

		public void PutSetting(string xPath, int value)
		{
			PutSetting(xPath, Convert.ToString(value));
		}

		public void PutSetting(string xPath, bool value)
		{
			PutSetting(xPath, Convert.ToString(value));
		}

		public bool GetSetting(string xPath, bool defaultValue)
		{
			return Convert.ToBoolean(GetSetting(xPath, Convert.ToString(defaultValue)));
		}

		public string GetSetting(string xPath, string defaultValue)
		{
			XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
			if (xmlNode != null) {
				return xmlNode.InnerText;
			}
			else {
				return defaultValue;
			}
		}

		public void PutSetting(string xPath, string value)
		{
			XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
			if (xmlNode == null) {
				xmlNode = createMissingNode("settings/" + xPath);
			}
			xmlNode.InnerText = value;
			xmlDocument.Save(documentPath);
		}

		private XmlNode createMissingNode(string xPath)
		{
			string[] xPathSections = xPath.Split('/');
			string currentXPath = string.Empty;
			XmlNode testNode = null;
			XmlNode currentNode = xmlDocument.SelectSingleNode("settings");
			foreach (string xPathSection in xPathSections) {
				currentXPath += xPathSection;
				testNode = xmlDocument.SelectSingleNode(currentXPath);
				if (testNode == null) {
					currentNode.InnerXml += "<" + xPathSection + "></" + xPathSection + ">";
				}
				currentNode = xmlDocument.SelectSingleNode(currentXPath);
				currentXPath += "/";
			}
			return currentNode;
		}
	}
}
using System;
using System.Xml;

namespace Common.Controls
{
	public class XMLProfileSettings
	{
		private readonly XmlDocument _xmlDocument = new XmlDocument();
		private readonly string _documentPath;
		private const string Root = "settings";

		public XMLProfileSettings()
		{
			try {
				//documentPath = Application.UserAppDataPath + "\\settings.xml";
				_documentPath =
					System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Vixen",
					                       "Settings.xml");
				if( !System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(_documentPath)))
					System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_documentPath));
				_xmlDocument.Load(_documentPath);
			}
			catch {
				_xmlDocument.LoadXml("<settings></settings>");
			}
		}

		public XMLProfileSettings(string path)
		{
			try {
				_documentPath = path;
				_xmlDocument.Load(_documentPath);
			}
			catch {
				_xmlDocument.LoadXml("<settings></settings>");
			}
		}

		public void RemoveNode(string xPath)
		{
			XmlNode xmlNode = _xmlDocument.SelectSingleNode(String.Format("{0}/{1}", Root, xPath));
			if (xmlNode != null)
			{
				xmlNode.RemoveAll();
				xmlNode.ParentNode.RemoveChild(xmlNode);
				_xmlDocument.Save(_documentPath);
			}		
		}

		public void PutSetting(SettingType type, string xPath, int value)
		{
			PutSetting(type, xPath, Convert.ToString(value));
		}

		public void PutSetting(SettingType type, string xPath, double  value)
		{
			PutSetting(type, xPath, Convert.ToString(value));
		}

		public void PutSetting(SettingType type, string xPath, bool value)
		{
			PutSetting(type, xPath, Convert.ToString(value));
		}

		public void PutSetting(SettingType type, string xPath, string value)
		{
			string path = String.Format("{0}/{1}/{2}", Root, type, xPath);
			XmlNode xmlNode = _xmlDocument.SelectSingleNode(path);
			if (xmlNode == null)
			{
				xmlNode = CreateMissingNode(path);
			}
			xmlNode.InnerText = value;
			_xmlDocument.Save(_documentPath);
		}

		public double GetSetting(SettingType type, string xPath, double defaultValue)
		{
			return Convert.ToDouble(GetSetting(type, xPath, Convert.ToString(defaultValue)));
		}

		public int GetSetting(SettingType type, string xPath, int defaultValue)
		{
			return Convert.ToInt32(GetSetting(type, xPath, Convert.ToString(defaultValue)));
		}

		public float GetSetting(SettingType type, string xPath, float defaultValue)
		{
			return (float)Convert.ToDouble(GetSetting(type, xPath, Convert.ToString(defaultValue)));
		}

		public bool GetSetting(SettingType type, string xPath, bool defaultValue)
		{
			return Convert.ToBoolean(GetSetting(type, xPath, Convert.ToString(defaultValue)));
		}

		public string GetSetting(SettingType type, string xPath, string defaultValue)
		{
			XmlNode xmlNode = _xmlDocument.SelectSingleNode(String.Format("{0}/{1}/{2}", Root, type, xPath));
			if (xmlNode != null)
			{
				return xmlNode.InnerText;
			}
			return defaultValue;
		}

		public void DeleteNode(SettingType type, string xPath)
		{
			string path = $"{Root}/{type}/{xPath}";
			XmlNode xmlNode = _xmlDocument.SelectSingleNode(path);
			if (xmlNode != null)
			{
				xmlNode.ParentNode?.RemoveChild(xmlNode);
				_xmlDocument.Save(_documentPath);
			}
			
		}

		public void RenameNode(SettingType type, string xPath, string newName)
		{
			string path = $"{Root}/{type}/{xPath}";
			XmlNode xmlNode = _xmlDocument.SelectSingleNode(path);
			if (xmlNode != null)
			{
				var newNode = _xmlDocument.CreateElement(newName);
				while (xmlNode.HasChildNodes)
				{
					newNode.AppendChild(xmlNode.FirstChild);
				}

				var parent = xmlNode.ParentNode;
				parent?.ReplaceChild(newNode, xmlNode);

				_xmlDocument.Save(_documentPath);
			}
		}

		private XmlNode CreateMissingNode(string xPath)
		{
			string[] xPathSections = xPath.Split('/');
			string currentXPath = string.Empty;
			XmlNode currentNode = _xmlDocument.SelectSingleNode(Root);
			foreach (string xPathSection in xPathSections) {
				currentXPath += xPathSection;
				XmlNode testNode = _xmlDocument.SelectSingleNode(currentXPath);
				if (testNode == null) {
					currentNode.InnerXml += string.Format("<{0}></{0}>" , xPathSection);
				}
				currentNode = _xmlDocument.SelectSingleNode(currentXPath);
				currentXPath += "/";
			}
			return currentNode;
		}

		/// <summary>
		/// Enum values for the types of settings the application can save.
		/// </summary>
		public enum SettingType
		{
			Profiles,
			AppSettings,
			Preferences
		};
		
	}

	
}
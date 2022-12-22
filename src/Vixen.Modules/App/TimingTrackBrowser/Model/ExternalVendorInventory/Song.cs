using System.Xml.Serialization;

namespace VixenModules.App.TimingTrackBrowser.Model.ExternalVendorInventory
{
    public class Song
    {
        public Song()
        {
            Artist = string.Empty;
            Title = string.Empty;
            Creator = string.Empty;
            Download = string.Empty;
            CategoryId = string.Empty;
            Weblink = string.Empty;
        }

        [XmlElement("artist")]
        public string Artist { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("creator")]
        public string Creator { get; set; }

        [XmlElement("download")]
        public string Download { get; set; }

        [XmlElement("categoryid")]
        public string CategoryId { get; set; }

        [XmlElement("weblink")]
        public string Weblink { get; set; }

    }
}

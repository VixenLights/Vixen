using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;
using System.Windows.Media.Media3D;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview
{
	[DataContract]
	public class VixenPreviewData : ModuleDataModelBase
	{
		private List<DisplayItem> _displayItems = new List<DisplayItem>();

		public override IModuleDataModel Clone()
		{
			//Console.WriteLine("Clone");
			VixenPreviewData result = (VixenPreviewData)MemberwiseClone();
			List<DisplayItem> displayItemCopy = new List<DisplayItem>(DisplayItems.Count);
			foreach (var displayItem in DisplayItems)
			{
				displayItemCopy.Add((DisplayItem)displayItem.Clone());
			}

			result.DisplayItems = displayItemCopy;
			return result;
		}

		[DataMember]
		public int BackgroundAlpha { get; set; } = 255;

		[DataMember]
		public string BackgroundFileName { get; set; }

		[DataMember]
		public int Top { get; set; }

		[DataMember]
		public int Left { get; set; }

		[DataMember]
		public int SetupTop { get; set; }

		[DataMember]
		public int SetupLeft { get; set; }

		[DataMember]
		public int SetupWidth { get; set; }

		[DataMember]
		public int SetupHeight { get; set; }

		[DataMember]
		public int Width { get; set; }

		[DataMember]
		public int Height { get; set; }

		[DataMember]
		public bool SaveLocations { get; set; } = true;

		[DataMember]
		public Vector3D LocationOffset { get; set; }

		[DataMember]
		public List<DisplayItem> DisplayItems
		{
			get { return _displayItems ?? (_displayItems = new List<DisplayItem>()); }

			set { _displayItems = value ?? new List<DisplayItem>(); }
		}

		[DataMember]
		public bool UseOpenGL { get; set; }
	}
}
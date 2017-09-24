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
		private int _backgroundAlpha = 255;
		private bool _saveLocations = true;

		public override IModuleDataModel Clone()
		{
			//Console.WriteLine("Clone");
			VixenPreviewData result = new VixenPreviewData
			                          	{
			                          		Width = 1024,
			                          		Height = 800
			                          	};
			return result;
		}

		[DataMember]
		public int BackgroundAlpha
		{
			get { return _backgroundAlpha; }
			set { _backgroundAlpha = value; }
		}

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
		public bool SaveLocations
		{
			get { return _saveLocations; }
			set { _saveLocations = value; }
		}

		[DataMember]
		public Vector3D LocationOffset { get; set; }

		[DataMember]
		public List<DisplayItem> DisplayItems
		{
			get { return _displayItems ?? (_displayItems = new List<DisplayItem>()); }

			set { _displayItems = value ?? new List<DisplayItem>(); }
		}
	}
}
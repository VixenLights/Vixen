using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using VixenModules.Preview.VixenPreview.Shapes;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview
{
	[DataContract]
	public class VixenPreviewData : ModuleDataModelBase
	{
		private int _setupTop, _setupLeft, _setupWidth, _setupHeight, _top, _left, _width, _height;
		private List<DisplayItem> _displayItems = new List<DisplayItem>();
		private string _backgroundFileName;
		private int _backgroundAlpha = 255;
		private bool _saveLocations = true;

		public VixenPreviewData()
		{
		}

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
		public string BackgroundFileName
		{
			get { return _backgroundFileName; }
			set { _backgroundFileName = value; }
		}

		[DataMember]
		public int Top
		{
			get { return _top; }
			set { _top = value; }
		}

		[DataMember]
		public int Left
		{
			get { return _left; }
			set { _left = value; }
		}

		[DataMember]
		public int SetupTop
		{
			get { return _setupTop; }
			set { _setupTop = value; }
		}

		[DataMember]
		public int SetupLeft
		{
			get { return _setupLeft; }
			set { _setupLeft = value; }
		}

		[DataMember]
		public int SetupWidth
		{
			get { return _setupWidth; }
			set { _setupWidth = value; }
		}

		[DataMember]
		public int SetupHeight
		{
			get { return _setupHeight; }
			set { _setupHeight = value; }
		}

		[DataMember]
		public int Width
		{
			get { return _width; }
			set { _width = value; }
		}

		[DataMember]
		public int Height
		{
			get { return _height; }
			set { _height = value; }
		}

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
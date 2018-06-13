using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module.Property;
using Vixen.Sys;
using VixenModules.App.LipSyncApp;

namespace VixenModules.Property.Face {
	public class FaceModule : PropertyModuleInstanceBase {

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private FaceData _data;

		public FaceModule() {
			_data = new FaceData();
		}

		public override void SetDefaultValues() {
			if (_data == null)
			{
				_data = new FaceData();
			}
			_data.PhonemeList = new Dictionary<string, bool>();
			_data.FaceComponents = new Dictionary<FaceComponent, bool>();
			DefaultColor = Color.White;
		}

		public override void CloneValues(IProperty sourceProperty)
		{
			var source = sourceProperty as FaceModule;
			if (source == null)
			{
				Logging.Error(
					"FaceModule: trying to CloneValues from another property, but it's not a FaceModule!");
				return;
			}

			PhonemeList = new Dictionary<string, bool>(source.PhonemeList);
			FaceComponents = new Dictionary<FaceComponent, bool>(source.FaceComponents);
			DefaultColor = source.DefaultColor;
		}

		public Dictionary<string, Boolean> PhonemeList
		{
			get { return _data.PhonemeList; }
			set { _data.PhonemeList = value; }
		}

		public Dictionary<FaceComponent, bool> FaceComponents
		{
			get { return _data.FaceComponents; }
			set { _data.FaceComponents = value; }
		}

		public Color DefaultColor
		{
			get { return _data.DefaultColor; }
			set { _data.DefaultColor = value; }
		}

		public override Vixen.Module.IModuleDataModel ModuleData {
			get {
				return _data;
			}
			set {
				_data = (FaceData)value;
			}
		}

		public static FaceModule GetFaceModuleForElement(ElementNode element)
		{
			return element.Properties.Get(FaceDescriptor.ModuleId) as FaceModule;
		}

		public Tuple<double, Color> ConfiguredColorAndIntensity()
		{
			HSV hsvVal = HSV.FromRGB(DefaultColor);
			hsvVal.V = 1;
			var colorRetVal = hsvVal.ToRGB().ToArgb();
			double intensityRetVal = HSV.VFromRgb(DefaultColor);

			return new Tuple<double, Color>(intensityRetVal, colorRetVal);
		}

		public double ConfiguredIntensity(LipSyncMapItem item)
		{
			return HSV.VFromRgb(DefaultColor);
		}

		public Color ConfiguredColor()
		{
			HSV hsvVal = HSV.FromRGB(DefaultColor);
			hsvVal.V = 1;
			var retVal = hsvVal.ToRGB().ToArgb();
				
			return retVal;
		}

		public bool PhonemeState(string phonemeName)
		{
			bool retVal = false;

			PhonemeList.TryGetValue(phonemeName, out retVal);

			return retVal;
		}

		public bool IsFaceComponentType(FaceComponent type)
		{
			bool retVal;

			FaceComponents.TryGetValue(type, out retVal);

			return retVal;
		}

		public override bool Setup() {
			//using (FaceSetupHelper faceSetupHelper = new FaceSetupHelper()) {
			//	if (faceSetupHelper.ShowDialog() == DialogResult.OK) {
			//		_data.PhonemeList = faceSetupHelper.PhonemeList;
			//		return true;
			//	}
			//	return false;
			//}

			return true;
		}

	}
}

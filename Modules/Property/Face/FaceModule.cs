using System;
using System.Collections.Generic;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module.Property;
using Vixen.Rule;
using Vixen.Sys;

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
			_data.PhonemeList = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
			_data.FaceComponents = new Dictionary<FaceComponent, bool>();
			DefaultColor = System.Drawing.Color.White;
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

			PhonemeList = new Dictionary<string, bool>(source.PhonemeList, StringComparer.OrdinalIgnoreCase);
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

		public System.Drawing.Color DefaultColor
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

		public static FaceModule GetFaceModuleForElement(IElementNode element)
		{
			return element.Properties.Get(FaceDescriptor.ModuleId) as FaceModule;
		}

		public Tuple<double, System.Drawing.Color> ConfiguredColorAndIntensity()
		{
			return new Tuple<double, System.Drawing.Color>(1, DefaultColor);
		}

		public double ConfiguredIntensity(FaceMapItem item)
		{
			return 1;
		}

		public System.Drawing.Color ConfiguredColor()
		{
			return DefaultColor;
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

		#region Overrides of PropertyModuleInstanceBase

		/// <inheritdoc />
		public override bool HasElementSetupHelper => true;

		/// <inheritdoc />
		public override bool SetupElements(IEnumerable<IElementNode> nodes)
		{
			var helper = new FaceSetupHelper();
			return helper.Perform(nodes);
		}

		#endregion
	}
}

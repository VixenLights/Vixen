using System.Collections.Generic;
using System.Windows.Forms;
using Vixen.Module.Property;
using Vixen.Sys;
using VixenModules.App.LipSyncApp;
using VixenModules.Property.Phoneme;

namespace VixenModules.Property.Face {
	public class FaceModule : PropertyModuleInstanceBase {

		private FaceData _data;

		public FaceModule() {
			_data = new FaceData();
		}

		public override void SetDefaultValues() {
			if (_data == null)
			{
				_data = new FaceData();
			}
			_data.Phonemes = new HashSet<PhonemeType>();
		}

		public HashSet<PhonemeType> Phonemes
		{
			get { return _data.Phonemes; }
			set { _data.Phonemes = value; }
		}

		public override Vixen.Module.IModuleDataModel ModuleData {
			get {
				return _data;
			}
			set {
				_data = (FaceData)value;
			}
		}

		public static HashSet<PhonemeType> GetPhonemesForElement(ElementNode element)
		{
			HashSet<PhonemeType> phonemes;
			var module = element.Properties.Get(FaceDescriptor.ModuleId) as FaceModule;
			if (module != null)
			{
				phonemes = module.Phonemes;
			}
			else
			{
				phonemes = new HashSet<PhonemeType>();
			}
			
			return phonemes;
		}

		public override bool Setup() {
			using (SetupForm setupForm = new SetupForm(_data)) {
				if (setupForm.ShowDialog() == DialogResult.OK) {
					_data.Phonemes = setupForm.Phonemes;
					return true;
				}
				return false;
			}
		}

	}
}

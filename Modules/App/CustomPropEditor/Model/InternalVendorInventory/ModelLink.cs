using System;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model.InternalVendorInventory
{
	public class ModelLink: BindableBase
	{
		private Uri _link;
		private ModelType _software;

		public ModelLink()
		{
			Software = ModelType.Prop;
		}

		public ModelLink(ModelType type, Uri link)
		{
			Link = link;
			Software = type;
		}

		/// <summary>
		/// Link to the model file
		/// </summary>
		public Uri Link
		{
			get => _link;
			set
			{
				if (Equals(value, _link)) return;
				_link = value;
				OnPropertyChanged(nameof(Link));
			}
		}

		/// <summary>
		/// Native software the for the model origin
		/// </summary>
		public ModelType Software
		{
			get => _software;
			set
			{
				if (value == _software) return;
				_software = value;
				OnPropertyChanged(nameof(Software));
			}
		}
	}
}

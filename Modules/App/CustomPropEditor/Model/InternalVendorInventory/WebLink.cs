using System;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model.InternalVendorInventory
{
	public class WebLink: BindableBase
	{
		private string _name;
		private Uri _link;

		public WebLink(string name, Uri link)
		{
			Name = name;
			Link = link;
		}

		/// <summary>
		/// Recognizable name for the Link
		/// </summary>
		public string Name
		{
			get => _name;
			set
			{
				if (value == _name) return;
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		/// <summary>
		/// Link to the site
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
	}
}

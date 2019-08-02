using System;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model.InternalVendorInventory
{
	public class ModelLink: BindableBase
	{
		private Uri _link;
		private ModelType _software;
		private string _name;
		private string _description;

		public ModelLink()
		{
			Name = "Default Model";
			Description = @"No description provided.";
			Software = ModelType.Prop;
		}

		public ModelLink(ModelType type, string name, string description, Uri link):this()
		{
			if (!string.IsNullOrEmpty(name))
			{
				Name = name;
			}

			if (!string.IsNullOrEmpty(description))
			{
				Description = description;
			}
			Link = link;
			Software = type;
		}

		/// <summary>
		/// The name of the model link
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
		/// Description for the model link
		/// </summary>
		public string Description
		{
			get => _description;
			set
			{
				if (value == _description) return;
				_description = value;
				OnPropertyChanged(nameof(Description));
			}
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

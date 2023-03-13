using Common.Controls;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;

namespace VixenApplication.Setup.ElementTemplates
{
	class SingleItem : IElementTemplate
	{
		private string _itemName;

		public SingleItem()
		{
			_itemName = "New Item";
		}

		public string TemplateName
		{
			get { return "Single Item"; }
		}

		public bool SetupTemplate(IEnumerable<ElementNode>? selectedNodes = null)
		{
			using (TextDialog td = new TextDialog("New Element Name?", "Element Name", _itemName, true))
			{
				DialogResult dr = td.ShowDialog();
				if (dr == DialogResult.OK)
				{
					_itemName = td.Response;
					if (_itemName == "")
					{
						_itemName = "New Item";
					}
					return true;
				}
			}
			return false;
		}

		public async Task<IEnumerable<ElementNode>> GenerateElements(IEnumerable<ElementNode>? selectedNodes = null)
		{
			ElementNode newNode = ElementNodeService.Instance.CreateSingle(null, _itemName);
			return await Task.FromResult(new[] { newNode });
		}

		#region IElementTemplate

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool ConfigureColor => true;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool ConfigureDimming => true;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool Cancelled => false;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public IEnumerable<ElementNode> GetElementsToDelete()
		{
			// By default there are no elements to delete
			return new List<ElementNode>();
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public virtual IEnumerable<ElementNode> GetLeafNodes()
		{
			// By default return an empty collection
			return new List<ElementNode>();
		}

		#endregion
	}
}

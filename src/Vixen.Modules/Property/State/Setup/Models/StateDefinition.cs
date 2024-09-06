using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Sys;

namespace VixenModules.Property.State.Setup.Models
{
	public class StateDefinition: ModelBase
	{
		public StateDefinition()
		{
			StateName = "State Name 1";
			StateItemName = "Item Name 1";
			StateColor = System.Drawing.Color.White;
		}

		#region Element property

		/// <summary>
		/// Gets or sets the Element associated the State.
		/// </summary>
		public ElementNode Element
		{
			get => GetValue<ElementNode>(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		/// <summary>
		/// SourceName property data.
		/// </summary>
		public static readonly IPropertyData ElementProperty = RegisterProperty<Element>(nameof(ElementProperty));

		#endregion

		#region StateColor property

		/// <summary>
		/// Gets or sets the Color associated to the State.
		/// </summary>
		public System.Drawing.Color StateColor
		{
			get => GetValue<System.Drawing.Color>(StateColorProperty);
			set => SetValue(StateColorProperty, value);
		}

		/// <summary>
		/// StateItemName property data.
		/// </summary>
		public static readonly IPropertyData StateColorProperty = RegisterProperty<System.Drawing.Color>(nameof(StateColorProperty));

		#endregion

		#region StateItemName property

		/// <summary>
		/// Gets or sets the Item Name associated to the State.
		/// </summary>
		public string StateItemName
		{
			get => GetValue<string>(StateItemNameProperty);
			set => SetValue(StateItemNameProperty, value);
		}

		/// <summary>
		/// StateItemName property data.
		/// </summary>
		public static readonly IPropertyData StateItemNameProperty = RegisterProperty<string>(nameof(StateItemNameProperty));

		#endregion

		#region StateName property

		/// <summary>
		/// Gets or sets the Name associated to the State.
		/// </summary>
		public string StateName
		{
			get => GetValue<string>(StateNameProperty);
			set => SetValue(StateNameProperty, value);
		}

		/// <summary>
		/// StateName property data.
		/// </summary>
		public static readonly IPropertyData StateNameProperty = RegisterProperty<string>(nameof(StateNameProperty));

		#endregion

	}
}

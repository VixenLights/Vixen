namespace VixenModules.Preview.DisplayPreview
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Windows.Media;
	using Vixen.Sys;

	public static class Extensions
	{
		public static void NotifyPropertyChanged(
			this PropertyChangedEventHandler propertyChangedEventHandler, string propertyName, object sender)
		{
			if (propertyChangedEventHandler != null) {
				propertyChangedEventHandler.Invoke(sender, new PropertyChangedEventArgs(propertyName));
			}
		}

		public static Dictionary<ElementNode, Color> ToMediaColor(
			this Dictionary<ElementNode, System.Drawing.Color> stateValues)
		{
			if (stateValues == null) {
				return new Dictionary<ElementNode, Color>();
			}

			var newValues =
				stateValues.ToList().Select(
					x =>
					new KeyValuePair<ElementNode, Color>(
						x.Key, Color.FromArgb(x.Value.A, x.Value.R, x.Value.G, x.Value.B))).ToDictionary(
							x => x.Key, x => x.Value);
			return newValues;
		}
	}
}
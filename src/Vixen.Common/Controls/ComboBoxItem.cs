﻿namespace Common.Controls
{
	public class ComboBoxItem
	{
		public string Text { get; set; }
		public object Value { get; set; }

		public ComboBoxItem(string text, object value)
		{
			Text = text;
			Value = value;
		}

		public override string ToString()
		{
			return Text;
		}
	}

}

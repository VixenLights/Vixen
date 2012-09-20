using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls
{
	public partial class NumberDialog : Form
	{
		public NumberDialog(string title, string prompt, int value, int minimum = 0, int maximum = int.MaxValue)
		{
			InitializeComponent();
			numericUpDownChooser.Value = value;
			numericUpDownChooser.Minimum = minimum;
			numericUpDownChooser.Maximum = maximum;
			Text = title;
			labelPrompt.Text = prompt;
		}

		public int Value
		{
			get { return decimal.ToInt32(numericUpDownChooser.Value); }
		}

	}
}

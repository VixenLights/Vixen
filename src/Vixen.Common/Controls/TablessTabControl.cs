﻿using System.ComponentModel;

namespace Common.Controls
{
	[ToolboxBitmap(typeof (TabControl))]
	public partial class TablessTabControl : TabControl
	{
		public TablessTabControl()
		{
			InitializeComponent();
			Multiline = true;
		}

		public TablessTabControl(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
			Multiline = true;
		}

		[Browsable(true)]
		public override Color BackColor
		{
			get { return base.BackColor; }
			set
			{
				base.BackColor = value;
				foreach (TabPage tabPage in TabPages) {
					tabPage.BackColor = value;
				}
			}
		}

		public override Rectangle DisplayRectangle
		{
			get { return new Rectangle(0, 0, Width, Height); }
		}
	}
}
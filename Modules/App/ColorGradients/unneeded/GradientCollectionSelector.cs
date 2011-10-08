using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CommonElements.ControlsEx.ListControls;

namespace VixenModules.App.ColorGradients
{
	/// <summary>
	/// gradient editor class
	/// </summary>
	public partial class GradientCollectionSelector : Form
	{
		/// <summary>
		/// collection for mirroring the changes to the ui gradient list
		/// </summary>
		private class GradientUICollection : GradientCollection
		{
			private GradientCollectionSelector _owner;
			public GradientUICollection(GradientCollectionSelector owner)
			{
				if (owner == null)
					throw new ArgumentNullException("owner");
				_owner = owner;
			}
			protected override void OnValidate(ColorGradient value)
			{
				if ((value as ColorGradient) == null)
					throw new ArgumentException("no gradient");
			}
			protected override void OnInsertComplete(int index, ColorGradient value)
			{
				_owner.lstPresets.Items.Insert(index, new GradientDisplayElement(value));
			}
			protected override void OnClearComplete()
			{
				_owner.lstPresets.Items.Clear();
			}
			protected override void OnRemoveComplete(int index, ColorGradient value)
			{
				if (index < _owner.lstPresets.Items.Count)
					_owner.lstPresets.Items.RemoveAt(index);
			}
			protected override void OnSetComplete(int index, ColorGradient oldValue, ColorGradient newValue)
			{
				_owner.lstPresets.Items[index] = new GradientDisplayElement(newValue);
			}
		}
		/// <summary>
		/// element of the ui gradient list
		/// </summary>
		private class GradientDisplayElement : DisplayItem
		{
			private ColorGradient _gradient;
			public GradientDisplayElement(ColorGradient grd)
			{
				if (grd == null)
					throw new ArgumentNullException("grd");
				_gradient = grd;
			}
			protected override void OnDraw(Graphics gr, Rectangle rct)
			{
				using (LinearGradientBrush lnbrs = new LinearGradientBrush(
						rct.Location,
						new Point(rct.Right, rct.Bottom),
						Color.Transparent, Color.Transparent)) {
					lnbrs.InterpolationColors = _gradient.GetColorBlend();
					gr.FillRectangle(lnbrs, rct);
				}
			}
			protected override void OnDrawUnscaled(Graphics gr, int x, int y)
			{
				using (LinearGradientBrush lnbrs = new LinearGradientBrush(
						Point.Empty, new Point(48, 48),
						Color.Transparent, Color.Transparent))
				{
					lnbrs.InterpolationColors = _gradient.GetColorBlend();
					gr.FillRectangle(lnbrs, new Rectangle(Point.Empty, Size));
				}
			}
			public ColorGradient Gradient
			{
				get { return _gradient; }
			}
			public override Size Size
			{
				get { return new Size(48, 48); }
			}
			public override string Text
			{
				get
				{
					return _gradient.Title;
				}
				set
				{
					_gradient.Title = value;
				}
			}
		}
		#region variables
		private GradientUICollection _gradients;
		private ColorGradient _selectedGradient;
		#endregion
		public GradientCollectionSelector()
		{
			_gradients = new GradientUICollection(this);
			InitializeComponent();
		}
		//
		private void lstPresets_SelectionChanged(object sender, EventArgs e)
		{
			GradientDisplayElement elem =
				lstPresets.SelectedItem as GradientDisplayElement;
			//
			_selectedGradient = elem != null ? elem.Gradient : null;
			labelName.Text = elem != null ? elem.Gradient.Title : null;
			mnuDelete.Enabled = elem != null;
		}
		private void mnuDelete_Click(object sender, EventArgs e)
		{
			if (lstPresets.SelectedItem != null)
				_gradients.Remove(((GradientDisplayElement)lstPresets.SelectedItem).Gradient);
		}
		#region properties
		/// <summary>
		/// gets a complete list of all loaded gradients
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)]
		public IList<ColorGradient> Gradients
		{
			get { return _gradients; }
			set
			{
				_gradients.Clear();
				foreach (ColorGradient g in value)
					_gradients.Add(g);
			}
		}
		/// <summary>
		/// gets the currently edited gradient object or null
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)]
		public ColorGradient SelectedGradient
		{
			get { return _selectedGradient; }
			set
			{
				_selectedGradient = value;
				lstPresets.SelectedIndex =
					_gradients.IndexOf(value);
			}
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CommonElements.ControlsEx.ListControls;

namespace CommonElements.ColorManagement.Gradients
{
	/// <summary>
	/// gradient editor class
	/// </summary>
	public partial class OriginalGradientCollectionEditor : Form
	{
		/// <summary>
		/// collection for mirroring the changes to the ui gradient list
		/// </summary>
		private class GradientUICollection : GradientCollection
		{
			private OriginalGradientCollectionEditor _owner;
			public GradientUICollection(OriginalGradientCollectionEditor owner)
			{
				if (owner == null)
					throw new ArgumentNullException("owner");
				_owner = owner;
			}
			protected override void OnValidate(Gradient value)
			{
				if ((value as Gradient) == null)
					throw new ArgumentException("no gradient");
			}
			protected override void OnInsertComplete(int index, Gradient value)
			{
				_owner.lstPresets.Items.Insert(index, new GradientDisplayElement(value));
			}
			protected override void OnClearComplete()
			{
				_owner.lstPresets.Items.Clear();
			}
			protected override void OnRemoveComplete(int index, Gradient value)
			{
				if (index < _owner.lstPresets.Items.Count)
					_owner.lstPresets.Items.RemoveAt(index);
			}
			protected override void OnSetComplete(int index, Gradient oldValue, Gradient newValue)
			{
				_owner.lstPresets.Items[index] = new GradientDisplayElement(newValue);
			}
		}
		/// <summary>
		/// element of the ui gradient list
		/// </summary>
		private class GradientDisplayElement : DisplayItem
		{
			private Gradient _gradient;
			public GradientDisplayElement(Gradient grd)
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
			public Gradient Gradient
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
		#endregion
		public OriginalGradientCollectionEditor()
		{
			_gradients = new GradientUICollection(this);
			InitializeComponent();
		}
		#region new gradient
		//enable or disable button
		private void tbNewPreset_TextChanged(object sender, EventArgs e)
		{
			btnNewPreset.Enabled = tbNewPreset.Text != null && tbNewPreset.Text != "";
		}
		//creates new gradient with the given title
		private void btnNewPreset_Click(object sender, EventArgs e)
		{
			Gradient grd = edit.Gradient;
			if (grd != null)
				grd = (Gradient)grd.Clone();
			else
			{
				grd = new Gradient();
				grd.Alphas.Add(new AlphaPoint(0, 0));
				grd.Alphas.Add(new AlphaPoint(255, 1));
				grd.Colors.Add(new ColorPoint(Color.Black, 0));
				grd.Colors.Add(new ColorPoint(Color.Black, 1));

			}
			grd.Title = tbNewPreset.Text;
			//add to collection
			_gradients.Add(grd);
			//update ui
			lstPresets.SelectedIndex = lstPresets.Items.Count - 1;
		}

		#endregion
		//
		private void lstPresets_SelectionChanged(object sender, EventArgs e)
		{
			GradientDisplayElement elem =
				lstPresets.SelectedItem as GradientDisplayElement;
			//
			edit.Gradient = elem != null ? elem.Gradient : null;
			tbNewPreset.Text = elem != null ? elem.Gradient.Title : null;
			mnuDelete.Enabled = elem != null;
		}
		//refresh the selected preset
		private void edit_GradientChanged(object sender, EventArgs e)
		{
			if (lstPresets.SelectedItem != null)
				lstPresets.SelectedItem.RaiseRefresh();
		}

		private void mnuDelete_Click(object sender, EventArgs e)
		{
			if (lstPresets.SelectedItem != null)
				_gradients.Remove(((GradientDisplayElement)lstPresets.SelectedItem).Gradient);
		}
		//save the current presets
		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				if (sDialog.ShowDialog() == DialogResult.OK)
					_gradients.Save(sDialog.FileName);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			try
			{
				if (_gradients.Count > 0 &&
					MessageBox.Show(this, "Overwrite?", this.Text, MessageBoxButtons.YesNo) == DialogResult.No)
					return;
				if (oDialog.ShowDialog() == DialogResult.OK)
				{
					_gradients.Clear();
					_gradients.Load(oDialog.FileName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.StackTrace);
			}
		}
		#region properties
		/// <summary>
		/// gets a complete list of all loaded gradients
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)]
		public IList<Gradient> Gradients
		{
			get { return _gradients; }
		}
		/// <summary>
		/// gets the currently edited gradient object or null
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)]
		public Gradient SelectedGradient
		{
			get { return edit.Gradient; }
			set
			{
				edit.Gradient = value;
				lstPresets.SelectedIndex =
					_gradients.IndexOf(value);
			}
		}
		#endregion
	}
}

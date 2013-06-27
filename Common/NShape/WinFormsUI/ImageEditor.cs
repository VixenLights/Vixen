/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.WinFormsUI
{
	/// <summary>
	/// UI type editor for properties of type NamedImage.
	/// </summary>
	public partial class ImageEditor : Form
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.ImageEditor" />.
		/// </summary>
		public ImageEditor()
		{
			InitializeComponent();
			Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.ImageEditor" />.
		/// </summary>
		public ImageEditor(string fileName)
			: this()
		{
			if (fileName == null) throw new ArgumentNullException("fileName");
			resultImage.Load(fileName);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.ImageEditor" />.
		/// </summary>
		public ImageEditor(Image image, string path)
			: this()
		{
			if (image == null) throw new ArgumentNullException("image");
			if (path == null) throw new ArgumentNullException("name");
			resultImage.Image = (Image) image.Clone();
			resultImage.Name = path;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.ImageEditor" />.
		/// </summary>
		public ImageEditor(Image image)
			: this(image, string.Empty)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.ImageEditor" />.
		/// </summary>
		public ImageEditor(NamedImage namedImage)
		{
			InitializeComponent();
			if (namedImage == null) throw new ArgumentNullException("namedImage");
			if (namedImage.Image != null)
				resultImage.Image = (Image) namedImage.Image.Clone();
			resultImage.Name = namedImage.Name;
		}


		/// <summary>
		/// Specifies the image selected by the user.
		/// </summary>
		public NamedImage Result
		{
			get { return resultImage; }
		}


		/// <override></override>
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			DisplayResult();
		}


		/// <override></override>
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);
			pictureBox.Image = null;
		}


		private Image Image
		{
			get { return resultImage.Image; }
			set
			{
				resultImage.Image = value;
				DisplayResult();
			}
		}


		private void okButton_Click(object sender, EventArgs e)
		{
			if (Modal) DialogResult = DialogResult.OK;
			else Close();
		}


		private void cancelButton_Click(object sender, EventArgs e)
		{
			if (Modal) DialogResult = DialogResult.Cancel;
			else Close();
		}


		private void clearButton_Click(object sender, EventArgs e)
		{
			this.SuspendLayout();
			resultImage.Image = null;
			resultImage.Name = string.Empty;
			DisplayResult();
			this.ResumeLayout();
		}


		private void browseButton_Click(object sender, EventArgs e)
		{
			openFileDialog.Filter =
				"Image Files|*.Bmp;*.Emf;*.Exif;*.Gif;*.Ico;*.Jpg;*.Jpeg;*.Png;*.Tiff;*.Wmf|All files (*.*)|*.*";
			if (nameTextBox.Text != string.Empty)
				openFileDialog.InitialDirectory = Path.GetDirectoryName(nameTextBox.Text);

			if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
				if (resultImage.Image != null)
					resultImage.Image.Dispose();
				resultImage.Load(openFileDialog.FileName);
				if (string.IsNullOrEmpty(nameTextBox.Text))
					nameTextBox.Text = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
				resultImage.Name = nameTextBox.Text;
				DisplayResult();
				this.ResumeLayout();
			}
		}


		private void nameTextBox_TextChanged(object sender, EventArgs e)
		{
			resultImage.Name = nameTextBox.Text;
		}


		private void DisplayResult()
		{
			this.SuspendLayout();
			pictureBox.Image = resultImage.Image;
			nameTextBox.Text = resultImage.Name;
			this.ResumeLayout();
		}


		private NamedImage resultImage = new NamedImage();
	}
}
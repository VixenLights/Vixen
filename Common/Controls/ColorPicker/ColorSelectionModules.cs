using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Common.Controls.ColorManagement.ColorModels;

namespace Common.Controls.ColorManagement.ColorPicker
{
	public abstract class ColorSelectionModule
	{
		#region colorfader

		//variables
		private ColorSelectionFader _fader;
		//update image
		protected void UpdateFaderImage()
		{
			if (_fader == null || _fader.Image == null)
				return;
			OnUpdateFaderImage(_fader.Image);
			//quantization
			_fader.Refresh();
		}

		protected abstract void OnUpdateFaderImage(Bitmap bmp);
		//update position
		protected void UpdateFaderPosition()
		{
			if (_fader == null || _fader.Image == null)
				return;
			OnUpdateFaderPosition(_fader);
		}

		protected abstract void OnUpdateFaderPosition(ColorSelectionFader fader);
		//update color
		protected void Fader_Scroll(object obj, EventArgs e)
		{
			if (_fader == null || _fader.Image == null)
				return;
			OnFaderScroll(_fader);
		}

		protected abstract void OnFaderScroll(ColorSelectionFader fader);
		//set & get plane
		public ColorSelectionFader ColorSelectionFader
		{
			get { return _fader; }
			set
			{
				if (_fader == value) return;
				if (_fader != null) {
					_fader.Scroll -= new EventHandler(Fader_Scroll);
				}
				_fader = value;
				if (_fader != null) {
					_fader.Scroll += new EventHandler(Fader_Scroll);
					_fader.ImageSizeChanged += _fader_SizeChanged;
				}
				UpdateFaderImage();
				UpdateFaderPosition();
			}
		}

		private void _fader_SizeChanged(object sender, EventArgs e)
		{
			UpdateFaderImage();
			UpdateFaderPosition();
		}

		#endregion

		#region colorplane

		//variables
		private ColorSelectionPlane _plane;
		//update image
		protected void UpdatePlaneImage()
		{
			if (_plane == null || _plane.Image == null)
				return;
			OnUpdatePlaneImage(_plane.Image);
			//quantization
			_plane.Refresh();
		}

		protected abstract void OnUpdatePlaneImage(Bitmap bmp);
		//update position
		protected void UpdatePlanePosition()
		{
			if (_plane == null || _plane.Image == null)
				return;
			OnUpdatePlanePosition(_plane);
		}

		protected abstract void OnUpdatePlanePosition(ColorSelectionPlane plane);
		//update color
		protected void Plane_Scroll(object obj, EventArgs e)
		{
			if (_plane == null || _plane.Image == null)
				return;
			OnPlaneScroll(_plane);
		}

		protected abstract void OnPlaneScroll(ColorSelectionPlane plane);
		//set & get plane
		public ColorSelectionPlane ColorSelectionPlane
		{
			get { return _plane; }
			set
			{
				if (_plane == value) return;
				if (_plane != null) {
					_plane.Scroll -= new EventHandler(Plane_Scroll);
				}
				_plane = value;
				if (_plane != null) {
					_plane.Scroll += new EventHandler(Plane_Scroll);
					_plane.ImageSizeChanged+=ImageSizeChanged;
				}
				UpdatePlaneImage();
				UpdatePlanePosition();
			}
		}

		private void ImageSizeChanged(object sender, EventArgs eventArgs)
		{
			UpdatePlaneImage();
			UpdatePlanePosition();
		}

		#endregion

		#region properties

		public abstract XYZ XYZ { get; set; }

		#endregion

		protected void RaiseColorChanged()
		{
			if (ColorChanged != null)
				ColorChanged(this, EventArgs.Empty);
		}

		public event EventHandler ColorChanged;
	}
}
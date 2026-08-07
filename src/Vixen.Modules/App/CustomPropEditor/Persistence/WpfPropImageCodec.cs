using System.IO;
using System.Windows.Media.Imaging;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal sealed class WpfPropImageCodec : IPropImageCodec
{
	public void EncodeJpeg(BitmapSource image, Stream destination)
	{
		ArgumentNullException.ThrowIfNull(image);
		ArgumentNullException.ThrowIfNull(destination);
		var encoder = new JpegBitmapEncoder();
		encoder.Frames.Add(BitmapFrame.Create(image));
		encoder.Save(destination);
	}

	public BitmapSource DecodeJpeg(Stream source)
	{
		ArgumentNullException.ThrowIfNull(source);
		var decoder = new JpegBitmapDecoder(source, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
		if (decoder.Frames.Count != 1) throw new PropPersistenceException("The prop package image is invalid.", "The JPEG contains an unexpected frame count.");
		var image = decoder.Frames[0];
		if (image.CanFreeze) image.Freeze();
		return image;
	}
}

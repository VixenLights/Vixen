using System.IO;
using System.Windows.Media.Imaging;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal interface IPropImageCodec
{
	void EncodeJpeg(BitmapSource image, Stream destination);
	BitmapSource DecodeJpeg(Stream source);
}

using System.Windows.Media.Imaging;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Persistence.Documents;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal interface IPropDocumentMapper
{
	PropPackageDocument ToDocument(Prop prop);
	Prop ToModel(PropPackageDocument document, BitmapSource image);
}

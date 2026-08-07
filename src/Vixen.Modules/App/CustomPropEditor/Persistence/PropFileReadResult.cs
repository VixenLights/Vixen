using System.Windows.Media.Imaging;
using VixenModules.App.CustomPropEditor.Persistence.Documents;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal sealed record PropFileReadResult(PropPackageDocument Document, BitmapSource Image, PropFileSourceFormat SourceFormat, VixenModules.App.CustomPropEditor.Model.Prop Prop = null);

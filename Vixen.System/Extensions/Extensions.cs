using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Vixen.Extensions
{
	public static class Extensions
	{
		public static string GetRelativePathFrom(this FileSystemInfo to, FileSystemInfo from)
		{
			return from.GetRelativePathTo(to);
		}

		public static string GetRelativePathTo(this FileSystemInfo from, FileSystemInfo to)
		{
			Func<FileSystemInfo, string> getPath = fsi =>
			{
				var d = fsi as DirectoryInfo;
				return d == null ? fsi.FullName : d.FullName.TrimEnd('\\') + "\\";
			};

			var fromPath = getPath(from);
			var toPath = getPath(to);

			var fromUri = new Uri(fromPath);
			var toUri = new Uri(toPath);

			var relativeUri = fromUri.MakeRelativeUri(toUri);
			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			return relativePath.Replace('/', Path.DirectorySeparatorChar);
		}

	    public static ImageSource ToImageSource(this Icon icon)
	    {
	        ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
	            icon.Handle,
	            Int32Rect.Empty,
	            BitmapSizeOptions.FromEmptyOptions());

	        return imageSource;
	    }

		public static string GetEnumDescription(this Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(
					typeof(DescriptionAttribute),
					false);

			if (attributes.Length > 0)
			{
				return attributes[0].Description;
			}

			return value.ToString();
		}

		public static string TrimStart(this string target, string trimString)
		{
			if (string.IsNullOrEmpty(trimString)) return target;

			string result = target;
			while (result.StartsWith(trimString))
			{
				result = result.Substring(trimString.Length);
			}

			return result;
		}

		public static string TrimEnd(this string target, string trimString)
		{
			if (string.IsNullOrEmpty(trimString)) return target;

			string result = target;
			while (result.EndsWith(trimString))
			{
				result = result.Substring(0, result.Length - trimString.Length);
			}

			return result;
		}
	}
}

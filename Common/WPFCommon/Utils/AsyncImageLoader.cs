using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Common.WPFCommon.Utils
{
	public class AsyncImageLoader : DependencyObject
	{
		public static Uri GetSourceUri(DependencyObject obj)
		{
			return (Uri) obj.GetValue(SourceUriProperty);
		}

		public static void SetSourceUri(DependencyObject obj, Uri value)
		{
			obj.SetValue(SourceUriProperty, value);
		}

		public static readonly DependencyProperty SourceUriProperty =
			DependencyProperty.RegisterAttached("SourceUri",
				typeof(Uri),
				typeof(AsyncImageLoader),
				new PropertyMetadata
				{
					PropertyChangedCallback = (obj, e) =>
						((Image) obj).SetBinding(
							Image.SourceProperty,
							new Binding("VerifiedUri")
							{
								Source = new AsyncImageLoader
								{
									_givenUri = (Uri) e.NewValue
								},
								IsAsync = true
							}
						)
				}
			);

		private Uri _givenUri;

		public Uri VerifiedUri
		{
			get
			{
				if (_givenUri != null)
				{
					try
					{
						System.Net.Dns.GetHostEntry(_givenUri.DnsSafeHost);
						return _givenUri;
					}
					catch (Exception)
					{
						return null;
					}
				}

				return null;
			}
		}
	}
}

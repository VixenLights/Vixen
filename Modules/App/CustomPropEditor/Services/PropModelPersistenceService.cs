using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Catel.Collections;
using LiteDB;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Services
{
	public class PropModelPersistenceService
	{
		private const string ImageFileName = "background.jpg";
		private const string LegacyImageFileName = "background.png"; //Bug workaround. There were two flavors of the image filename. This is the legacy one.

		public static bool SaveModel(Prop prop, string path)
		{
			using (var db = new LiteDatabase(path))
			{
				var col = db.GetCollection<Prop>("props");
				if (col.Count() > 0)
				{
					var props = col.FindAll().Select(x => x.Id);
					props.ForEach(x => col.Delete(x));

				}
				col.Insert(prop);
				db.FileStorage.Upload($"$/image/{ImageFileName}", ImageFileName, StreamFromBitmapSource(prop.Image));
			}

			return true;
		}

		public static bool UpdateModel(Prop prop, string path)
		{
			using (var db = new LiteDatabase(path))
			{
				var col = db.GetCollection<Prop>("props");
				col.Update(prop);
				CleanUpLegacyImages(db);
				db.FileStorage.Upload($"$/image/{ImageFileName}", ImageFileName, StreamFromBitmapSource(prop.Image));

				db.Shrink();
			}

			return true;
		}

		public static Prop GetModel(string path)
		{
			Prop p;
			using (var db = new LiteDatabase(path))
			{
				var col = db.GetCollection<Prop>("props");

				p = col.FindAll().FirstOrDefault();

				var file = db.FileStorage.FindById($"$/image/{ImageFileName}");
				if (file == null)
				{
					file = db.FileStorage.FindById($"$/image/{LegacyImageFileName}");
				}
				if (file != null)
				{
					Stream bmp = new MemoryStream();
					file.CopyTo(bmp);
					p.Image = BitmapSourceFromStream(bmp);
					bmp.Close();
				}
			}

			return p;
		}

		public static async Task<Prop> GetModelAsync(string path)
		{
			Task<Prop> t = Task<Prop>.Factory.StartNew(() =>
			{
				Prop p;
				using (var db = new LiteDatabase(path))
				{
					var col = db.GetCollection<Prop>("props");

					p = col.FindAll().FirstOrDefault();

					var fileName = "background.jpg";
					var file = db.FileStorage.FindById($"$/image/{fileName}");
					if (file != null)
					{
						Stream bmp = new MemoryStream();
						file.CopyTo(bmp);
						p.Image = BitmapSourceFromStream(bmp);
						bmp.Close();
					}
				}
				return p;
			});

			return await t;

		}


		private static Stream StreamFromBitmapSource(BitmapSource writeBmp)
		{
			Stream bmp = new MemoryStream();
			
			BitmapEncoder enc = new JpegBitmapEncoder();
			enc.Frames.Add(BitmapFrame.Create(writeBmp));
			enc.Save(bmp);
			bmp.Seek(0, SeekOrigin.Begin);
			return bmp;
		}

		private static BitmapSource BitmapSourceFromStream(Stream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);
			BitmapDecoder decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

			BitmapSource bmp = decoder.Frames[0];
			return bmp;
		}

		private static void CleanUpLegacyImages(LiteDatabase db)
		{
			var file = db.FileStorage.FindById($"$/image/{LegacyImageFileName}");
			if (file != null)
			{
				db.FileStorage.Delete($"$/image/{LegacyImageFileName}");
			}
		}
	}
}

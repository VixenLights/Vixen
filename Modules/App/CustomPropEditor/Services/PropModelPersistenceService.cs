using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LiteDB;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Services
{
	public class PropModelPersistenceService
	{
		public static bool SaveModel(Prop prop, string path)
		{
			using (var db = new LiteDatabase(path))
			{
				var col = db.GetCollection<Prop>("props");
				col.EnsureIndex(x => x.Name);
				col.Insert(prop);
				var fileName = "background.png";
				db.FileStorage.Upload($"$/image/{fileName}", fileName, StreamFromBitmapSource(prop.Image));
			}

			return true;
		}

		public static bool UpdateModel(Prop prop, string path)
		{
			using (var db = new LiteDatabase(path))
			{
				var col = db.GetCollection<Prop>("props");
				col.EnsureIndex(x => x.Name);
				col.Update(prop);

				var fileName = "background.jpg";
				db.FileStorage.Upload($"$/image/{fileName}", fileName, StreamFromBitmapSource(prop.Image));

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

				var fileName = "background.jpg";
				Stream bmp = new MemoryStream();
				var file = db.FileStorage.FindById($"$/image/{fileName}");
				file.CopyTo(bmp);
				p.Image = BitmapSourceFromStream(bmp);
				bmp.Close();
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
					Stream bmp = new MemoryStream();
					db.FileStorage.Download($"$/image/{fileName}", bmp);
					p.Image = BitmapSourceFromStream(bmp);
					bmp.Close();
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
	}
}

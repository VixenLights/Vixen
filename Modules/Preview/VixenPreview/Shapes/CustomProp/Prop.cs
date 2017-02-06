using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	[DataContract]
	public class Prop
	{

		public Prop()
		{
			Channels = new List<PropChannel>();
			BackgroundImageOpacity = 100;
		}

		[DataMember]
		public string BackgroundImage { get; set; }

		[DataMember]
		public int BackgroundImageOpacity { get; set; }

		public Prop(Prop obj)
		{

			this.Name = obj.Name;
			Channels = obj.Channels;

			this.BackgroundImage = obj.BackgroundImage;
			this.BackgroundImageOpacity = obj.BackgroundImageOpacity;
		}




		#region File IO Operations
		public void ToFile(string fileName)
		{
			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};
			var serializer = new DataContractSerializer(typeof(Prop));
			using (var dataWriter = XmlWriter.Create(fileName, xmlsettings))
			{
				serializer.WriteObject(dataWriter, this);
				dataWriter.Close();
			}

		}


		public static Prop FromFile(string fileName)
		{
			if (File.Exists(fileName))
			{
				using (FileStream reader = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(Prop));
					return (Prop)ser.ReadObject(reader);
				}
			}
			return null;
		}
		#endregion


		public DisplayItem ToDisplayItem(int x, int y)
		{
			//Theres got to be a better way to do this... LOL
			var prop = new PreviewCustomProp(new PreviewPoint(x, y), null, 1);
			prop.LoadProp(this);
			return new DisplayItem() { Shape = prop, ZoomLevel = 1 };
		}



		public int Height
		{

			get { return GetBounds().Height; }
		}

		public int Width
		{

			get { return GetBounds().Width; }
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<PropChannel> Channels { get; set; }



		public static List<Pixel> GetAllPixels(List<PropChannel> collection)
		{
			List<Pixel> result = collection.SelectMany(s => s.Pixels).ToList();
			collection.ForEach(c => result.AddRange(GetAllPixels(c.Children)));
			return result;
		}
		List<Pixel> _pixels = new List<Pixel>();
		string _currentHash = null;

		public Rectangle GetBounds()
		{
			var hash = ComputeHash(Channels);

			if (hash != _currentHash)
			{
				_currentHash = hash;
				_pixels = GetAllPixels(Channels);
			}

			var x_query = from Pixel p in _pixels select p.X;
			int xmin = x_query.Min();
			int xmax = x_query.Max();

			var y_query = from Pixel p in _pixels select p.Y;
			int ymin = y_query.Min();
			int ymax = y_query.Max();

			return new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
		}

		public Point Delta()
		{

			var bounds = GetBounds();

			int deltaX = bounds.X;
			int deltaY = bounds.Y;
			return new Point(deltaX, deltaY);

		}

		private static string ComputeHash(Object objectToSerialize)
		{
			if (objectToSerialize == null) return null;
			using (MemoryStream fs = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				var serializer = new DataContractSerializer(objectToSerialize.GetType());
				try
				{

					using (var dataWriter = XmlWriter.Create(fs))
					{
						serializer.WriteObject(dataWriter, objectToSerialize);
						dataWriter.Close();
					}

					MD5 md5 = new MD5CryptoServiceProvider();

					byte[] result = md5.ComputeHash(fs.ToArray());

					// Build the final string by converting each byte
					// into hex and appending it to a StringBuilder
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < result.Length; i++)
					{
						sb.Append(result[i].ToString("X2"));
					}

					// And return it
					return sb.ToString();

				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}

			}
			return null;
		}

		public string SelectedChannelId { get; set; }

		public string SelectedChannelName { get; set; }


	}
}

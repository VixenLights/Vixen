using System;
using System.IO;
using System.Xml.Serialization;

namespace Common.Preferences
{
	[Serializable]
	public abstract class PreferenceBase<T> : IPreference where T : IPreference, new()
	{
		protected PreferenceBase()
		{
			SetStandardValues();
		}

		public abstract void SetStandardValues();

		/// <inheritdoc />
		public static T Load(string path)
		{
			var fi = new FileInfo(path);
			T result;
			if (!fi.Exists || string.IsNullOrWhiteSpace(File.ReadAllText(fi.FullName)))
			{
				result = new T();
			}
			else
			{
				using (var reader = new StreamReader(path))
				{
					var deserializer = new XmlSerializer(typeof(T));
					result = (T) deserializer.Deserialize(reader);
				}
			}

			return result;
		}

		public void Save(string path)
		{
			using (var writer = new StreamWriter(path))
			{
				var serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(writer, this);
			}
		}
	}
}

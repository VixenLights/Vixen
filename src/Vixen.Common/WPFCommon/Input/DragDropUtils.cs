#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text.Json;
using System.Windows;

namespace Common.WPFCommon.Input
{
	/// <summary>
	/// Provides utility methods for serializing and deserializing drag-and-drop data using JSON format.
	/// This is for use in WPF applications to facilitate drag-and-drop operations with custom data types.
	/// </summary>
	/// <remarks>This class enables type-safe drag-and-drop operations by serializing objects to JSON and
	/// associating them with their type. It is intended for use with data transfer scenarios such as clipboard or
	/// drag-and-drop operations in Windows Forms or WPF applications. All methods are static and thread-safe.</remarks>
	public class DragDropUtils
	{
		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			IncludeFields = true
		};

		/// <summary>
		/// Creates a new DataObject containing the specified value serialized as JSON.
		/// </summary>
		/// <remarks>The value is serialized using the configured JSON options. The resulting DataObject stores the
		/// serialized data under the type of the original value, which can be used for type-specific retrieval.</remarks>
		/// <param name="value">The object to serialize and store in the DataObject. Cannot be null.</param>
		/// <returns>A DataObject instance containing the serialized representation of the specified value, with the data type set to
		/// the type of the value.</returns>
		public static DataObject CreateDataObject(object value)
		{
			DataObject dataObject = new DataObject();
			if (value is Color)
			{
				//Color is natively supported
				dataObject.SetData(value.GetType(), value);
			}
			else
			{
				var jsonValue = JsonSerializer.Serialize(value, JsonOptions);
				dataObject.SetData(value.GetType(), jsonValue);
			}
			
			return dataObject;
		}

		/// <summary>
		/// Attempts to retrieve drag-and-drop data of the specified type from the provided data object.
		/// </summary>
		/// <remarks>This method first attempts to extract the data as a JSON string and deserialize it to the
		/// specified type. If that fails, it attempts to cast the data directly to the specified type. Use this method to
		/// safely access strongly-typed drag-and-drop data without throwing exceptions if the data is not present or cannot
		/// be converted.</remarks>
		/// <typeparam name="T">The type of data to retrieve from the data object.</typeparam>
		/// <param name="dataObject">The data object containing drag-and-drop data. Must not be null.</param>
		/// <param name="value">When this method returns, contains the value of type T if the data is present and can be deserialized or cast;
		/// otherwise, the default value for the type.</param>
		/// <returns>true if the data of the specified type was successfully retrieved; otherwise, false.</returns>
		public static bool TryGetDragDropData<T>(IDataObject dataObject, [NotNullWhen(true)] out T? value)
		{
			value = default;
			var data = dataObject.GetData(typeof(T));
			if (data is string jsonValue)
			{
				value = JsonSerializer.Deserialize<T>(jsonValue, JsonOptions);
				return value != null;
			}

			if (data is T data1)
			{
				value = data1;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Attempts to retrieve data of the specified type from the given data object, deserializing from JSON if necessary.
		/// </summary>
		/// <remarks>If the data object contains a JSON string for the specified type, the method attempts to
		/// deserialize it. If the data is already of the requested type, it is returned directly. This method does not throw
		/// exceptions for missing or invalid data; callers should check the return value to determine success.</remarks>
		/// <param name="dataObject">The data object from which to retrieve the data. Cannot be null.</param>
		/// <param name="type">The type of data to retrieve from the data object. Cannot be null.</param>
		/// <param name="value">When this method returns, contains the retrieved data if successful; otherwise, null. The value is deserialized
		/// from JSON if the data is stored as a JSON string.</param>
		/// <returns>true if data of the specified type was successfully retrieved; otherwise, false.</returns>
		public static bool TryGetDragDropData(IDataObject dataObject, Type type, [NotNullWhen(true)] out object? value)
		{
			value = null;
			var data = dataObject.GetData(type);
			if (data is string jsonValue)
			{
				value = JsonSerializer.Deserialize(jsonValue, type, JsonOptions);
				return value != null;
			}
			
			if(type.IsInstanceOfType(data))
			{
				value = data;
				return true;
			}

			return false;
		}
	}
}
namespace Vixen.IO.Result {
	public class SerializationResult : ISerializationResult {
		public SerializationResult(bool success, string message, object obj) {
			Success = success;
			Message = message;
			Object = obj;
		}

		public bool Success { get; private set; }

		public string Message { get; private set; }

		public object Object { get; private set; }
	}

	public class SerializationResult<ObjectType> : SerializationResult {
		public SerializationResult(bool success, string message, ObjectType obj)
			: base(success, message, obj) {
			Object = obj;
		}

		new public ObjectType Object { get; private set; }
	}
}

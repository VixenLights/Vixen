using Catel.Services;

namespace Common.WPFCommon.Services
{
	public class MessageBoxResponse
	{
		public MessageBoxResponse(MessageResult result, string response)
		{
			Result = result;
			Response = response;
		}

		public MessageResult Result { get; private set; }

		public string Response { get; private set; }
	}
}

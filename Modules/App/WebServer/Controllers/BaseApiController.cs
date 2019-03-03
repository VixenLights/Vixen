using System.Net;
using System.Net.Http;
using System.Web.Http;
using VixenModules.App.WebServer.Filter;

namespace VixenModules.App.WebServer.Controllers
{
	[ArgumentExceptionFilter]
	public class BaseApiController: ApiController
	{
		protected void CreateResponseMessage(HttpStatusCode code, string content, string reason)
		{
			var resp = new HttpResponseMessage(code)
				{
					Content = new StringContent(content),
					ReasonPhrase = reason,
					StatusCode = code
				};
				throw new HttpResponseException(resp);
		}
	}
}

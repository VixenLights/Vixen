using Microsoft.AspNetCore.Mvc;
using VixenModules.App.WebServer.Filter;

namespace VixenModules.App.WebServer.Controllers
{
	[ArgumentExceptionFilter]
	public class BaseApiController: ControllerBase
	{
		
	}
}

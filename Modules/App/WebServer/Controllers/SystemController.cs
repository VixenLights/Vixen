using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using VixenModules.App.WebServer.Model;
using VixenModules.App.WebServer.Service;

namespace VixenModules.App.WebServer.Controllers
{
	public class SystemController:BaseApiController
	{
		public IEnumerable<Controller> GetControllers()
		{
			var helper = new SystemHelper();
			return helper.GetControllers();
		}

		public Controller Get(Guid id)
		{
			var helper = new SystemHelper();
			return helper.Get(id);
		}

		[HttpPost]
		public Status SetState(ControllerState state)
		{
			if (state == null)
			{
				CreateResponseMessage(HttpStatusCode.BadRequest, String.Empty, "Request is null.");
			}
			var s = new Status();
			var helper = new SystemHelper();
			var c = helper.Get(state.Id);

			if (c != null)
			{
				s.IsSuccessful = helper.SetState(state.Id, state.IsRunning);
				s.Message = $"{c.Name} state {(s.IsSuccessful?"Changed":"Not Changed")}.";
			}
			else
			{
				s.Message = @"Controller not found.";
				s.IsSuccessful = false;
			}

			return s;
		}
	}
}

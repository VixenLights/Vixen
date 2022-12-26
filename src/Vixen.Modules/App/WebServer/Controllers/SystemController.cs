﻿using Microsoft.AspNetCore.Mvc;
using VixenModules.App.WebServer.Model;
using VixenModules.App.WebServer.Service;
using Controller = VixenModules.App.WebServer.Model.Controller;

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
			return helper.GetController(id);
		}

		[HttpPost]
		public IActionResult SetControllerState(ControllerState state)
		{
			if (state == null)
			{
				//CreateResponseMessage(HttpStatusCode.BadRequest, String.Empty, "Request is null.");
				return BadRequest("Request is null.");
			}
			var s = new Status();
			var helper = new SystemHelper();
			var c = helper.GetController(state.Id);

			if (c != null)
			{
				s.IsSuccessful = helper.SetControllerState(state.Id, state.IsRunning);
				s.Message = $"{c.Name} state {(s.IsSuccessful?"Changed":"Not Changed")}.";
			}
			else
			{
				s.Message = @"Controller not found.";
				s.IsSuccessful = false;
			}

			return Ok(s);
		}

		[HttpPost]
		public Status SetAllControllersState(ControllerState state)
		{
			var s = new Status();
			var helper = new SystemHelper();
			s.IsSuccessful = helper.SetAllControllersState(state.IsRunning);
			s.Message = $"All controllers state {(s.IsSuccessful ? "Changed" : "Not Changed")}.";
			return s;
		}

		[HttpPost]
		public async Task<Status> Save()
		{
			var s = new Status();
			var helper = new SystemHelper();
			var success = await helper.Save();
			s.IsSuccessful = success;
			s.Message = $"Save {(s.IsSuccessful?"Successful":"Failed")}";
			return s;
		}
	}
}

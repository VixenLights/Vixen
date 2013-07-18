using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Commands
{
	public class CustomCommand : Dispatchable<CustomCommand>, ICommand<object>
	{
		public CustomCommand(object value)
		{
			CommandValue = value;
		}

		public object CommandValue { get; set; }


		 
	}

}

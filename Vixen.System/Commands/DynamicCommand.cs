using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Commands
{
	public class DynamicCommand : Dispatchable<DynamicCommand>, ICommand<DynamicValue>
	{
		public DynamicCommand(DynamicValue value)
		{
			CommandValue = value;
		}

		public DynamicValue CommandValue { get; set; }


		object ICommand.CommandValue
		{
			get
			{
				return CommandValue;
			}
			set
			{
				CommandValue =  (DynamicValue)value;
			}
		}
	}

}

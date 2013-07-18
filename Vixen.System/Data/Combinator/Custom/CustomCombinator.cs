using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Data.Value;

namespace Vixen.Data.Combinator.Commands {
	public class CustomCombinator : Combinator<CustomCombinator, object> {
		public override void Handle(CustomCommand obj) {
			
			if (CombinatorValue == null) {
				CombinatorValue = new CustomCommand(obj.CommandValue);
			}
	 
				
		}
	}
}

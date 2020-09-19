using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Effect.Morph
{
	public enum PolygonType
	{
		[Description("Pattern")]
		Pattern,

		[Description("Free Form")]
		FreeForm,

		[Description("Time Based")]
		TimeBased,
	}
}

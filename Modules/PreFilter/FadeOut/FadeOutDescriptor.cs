using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.SequenceFilter;

namespace FadeOut
{
	public class FadeOutDescriptor : SequenceFilterModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{E0E26570-6A01-4368-B996-E34576FF4910}");

		public override string TypeName
		{
			get { return "Fade out"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (FadeOutModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (FadeOutData); }
		}

		public override string Author
		{
			get { throw new NotImplementedException(); }
		}

		public override string Description
		{
			get { throw new NotImplementedException(); }
		}

		public override string Version
		{
			get { throw new NotImplementedException(); }
		}
	}
}
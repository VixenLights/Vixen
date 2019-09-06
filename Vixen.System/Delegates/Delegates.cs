using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen
{
	public class Delegates
	{
		public delegate void GenericDelegate();

		public delegate void GenericValue(object dyn);

		public delegate void GenericBool(bool val);

		public delegate string GenericString(string s);

		public delegate void GenericVoidString(string s);

		public delegate string GenericStringValue();
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.Editor.EffectEditor.Internal
{
	public class CollectionPropertyDescriptor: PropertyDescriptor
	{
		public CollectionPropertyDescriptor(string name, Attribute[] attrs) : base(name, attrs)
		{
		}

		public CollectionPropertyDescriptor(MemberDescriptor descr) : base(descr)
		{
		}

		public CollectionPropertyDescriptor(MemberDescriptor descr, Attribute[] attrs) : base(descr, attrs)
		{
		}

		public override bool CanResetValue(object component)
		{
			throw new NotImplementedException();
		}

		public override object GetValue(object component)
		{
			throw new NotImplementedException();
		}

		public override void ResetValue(object component)
		{
			throw new NotImplementedException();
		}

		public override void SetValue(object component, object value)
		{
			throw new NotImplementedException();
		}

		public override bool ShouldSerializeValue(object component)
		{
			throw new NotImplementedException();
		}

		public override Type ComponentType
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public override Type PropertyType
		{
			get { throw new NotImplementedException(); }
		}
	}
}

using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Model
{
	public class ElementModel : IEqualityComparer<ElementModel>, IEquatable<ElementModel>
	{
		public string Name { get; set; }

		public Guid Id { get; protected set; }

		public override string ToString()
		{
			return Name;
		}

		public bool Equals(ElementModel x, ElementModel y)
		{
			return y != null && x != null && x.Id == y.Id;
		}

		public int GetHashCode(ElementModel obj)
		{
			return obj.Id.GetHashCode();
		}

		public bool Equals(ElementModel other)
		{
			return other != null && Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		
	}
}

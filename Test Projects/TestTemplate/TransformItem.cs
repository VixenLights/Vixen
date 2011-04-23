using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestTemplate {
	class TransformItem {
		public TransformItem(Guid id, string description) {
			Id = id;
			Description = description;
		}

		public string Description { get; private set; }
		public Guid Id { get; private set; }
		public override string ToString() {
			return Description;
		}
	}
}

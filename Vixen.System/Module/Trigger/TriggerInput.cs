using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Vixen.Module.Trigger
{
	[DataContract]
	public abstract class TriggerInput : ITriggerInput
	{
		public event EventHandler Set;

		public TriggerInput()
		{
			// Required for serialization.
		}

		public TriggerInput(TriggerInputType type, Guid id)
		{
			this.Type = type;
			this.Id = id;
		}

		public TriggerInput(TriggerInputType type)
			: this(type, Guid.NewGuid())
		{
		}

		[DataMember]
		public TriggerInputType Type { get; set; }

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		public virtual double Value { get; set; }

		protected void OnTriggered(EventArgs e)
		{
			if (Set != null) {
				Set(this, e);
			}
		}
	}
}
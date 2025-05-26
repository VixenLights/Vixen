#nullable enable
using Vixen.Model;

namespace Vixen.Sys.Props
{
    [Serializable]
	public abstract class Prop: ModelBase, IProp
	{
		#region Constructors

        protected Prop() : this("Prop 1")
        {

        }

        protected Prop(string name)
        {   
            Id = Guid.NewGuid();
			Name = name;
            CreationDate = ModifiedDate = DateTime.Now;
            CreatedBy = Environment.UserName;
        }

		#endregion


		public Guid Id { get; init; } 
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; init; }
        public DateTime ModifiedDate { get; set; }
        public virtual PropType PropType { get; set; }

        // Add logic to manage Element structure into the regular element tree, including supported properties
        // like patching order, orientation, etc. 

        // Add logic to handle the visual structure and mapping for the preview.

        // Implementing Prop classes should utilize the logic in this class to interface with the core of Vixen
        // Implementing Prop classes with auto generate the standard structure for the type of the Prop.
        // Implementing Prop classes will define and shape the default model view. The preview will manage sizing and placing them. 
        // Location will need to be handled as an offset into any previews that the Prop participates in. 
        // * Prop should have a list of associated preview ids
        // * Prop should have a lookup of the offset by preview id. 
        

        // Custom definitions for targeted rendering will need to be handled in some form. TBD.

        // Add other properties to manage things like controller target, 
    }
}

#nullable enable
using Vixen.Model;

namespace Vixen.Sys.Props
{
    [Serializable]
	public abstract class Prop: BindableBase, IProp
	{
        private readonly Guid _id;
        private string _name;
        private string _createdBy;
        private DateTime _modifiedDate;
        private StringTypes _stringType;
        private IPropModel _propModel;

        #region Constructors

        protected Prop(string name, PropType propType)
        {   
            Id = Guid.NewGuid();
			Name = name;
            CreationDate = ModifiedDate = DateTime.Now;
            CreatedBy = Environment.UserName;
            PropType = propType;
            StringType = StringTypes.Pixel;
        }

		#endregion

        public Guid Id
        {
            get => _id;
            init => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string CreatedBy
        {
            get => _createdBy;
            set => SetProperty(ref _createdBy, value);
        }

        public DateTime CreationDate { get; init; }

        public DateTime ModifiedDate
        {
            get => _modifiedDate;
            set => SetProperty(ref _modifiedDate, value);
        }

        public PropType PropType { get; init; }

        public StringTypes StringType
        {
            get => _stringType;
            set => SetProperty(ref _stringType, value);
        }

        public IPropModel PropModel
        {
            get => _propModel;
            set => SetProperty(ref _propModel, value);
        }

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

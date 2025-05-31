using Vixen.Sys.Managers;

namespace Vixen.Sys.Props
{
    public interface IProp
    {
		/// <summary>
		/// Unique id of the Prop
		/// </summary>
		Guid Id { get; init; }

		/// <summary>
		/// Friendly name of the Prop
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Created by username
		/// </summary>
		string CreatedBy { get; set; }

		/// <summary>
		/// Creation date of the Prop
		/// </summary>
		DateTime CreationDate { get; init; }

		/// <summary>
		/// Last modified date of the Prop
		/// </summary>
		DateTime ModifiedDate { get; set; }

		/// <summary>
		/// Defined type of the Prop.
		/// </summary>
		PropType PropType { get; init; }

		/// <summary>
		/// String type of enum <see cref="StringTypes"/>
		/// </summary>
		StringTypes StringType { get; set; }

		/// <summary>
		/// Model for rendering the visual
		/// </summary>
		IPropModel PropModel { get; set; }
	}

    public enum StringTypes
    {
        Standard,
        Pixel
    }
}
using System.Drawing;
using System.Runtime.Serialization;
using VixenModules.App.Curves;

namespace VixenModules.App.ColorGradients
{
	[DataContract]
	public class GradientLevelPair
	{
		/// <summary>
		/// Creates a new pair with a default <see cref="ColorGradient"/> and <see cref="Curve"/>
		/// </summary>
		public GradientLevelPair()
		{
			ColorGradient = new ColorGradient();
			Curve = new Curve(CurveType.Flat100);
		}

		/// <summary>
		/// Creates a new pair with the provided <see cref="ColorGradient"/> and a default <see cref="Curve"/>
		/// </summary>
		/// <param name="cg"></param>
		public GradientLevelPair(ColorGradient cg)
		{
			ColorGradient = cg;
			Curve = new Curve();
		}

		/// <summary>
		/// Creates a new pair with the provided <see cref="ColorGradient"/> and <see cref="Curve"/>
		/// </summary>
		/// <param name="cg"></param>
		/// <param name="c"></param>
		public GradientLevelPair(ColorGradient cg, Curve c)
		{
			ColorGradient = cg;
			Curve = c;
		}

		/// <summary>
		/// Creates a new pair with the provided <see cref="ColorGradient"/> and <see cref="CurveType"/>
		/// </summary>
		/// <param name="cg"></param>
		/// <param name="type"></param>
		public GradientLevelPair(ColorGradient cg, CurveType type)
		{
			ColorGradient = cg;
			Curve = new Curve(type);
		}

		//Convienience constructors.

		/// <summary>
		/// Creates a new pair with a <see cref="ColorGradient"/> based on the provided <see cref="Color"/> and a default <see cref="Curve"/>
		/// </summary>
		/// <param name="c"></param>
		public GradientLevelPair(Color c)
		{
			ColorGradient = new ColorGradient(c);
			Curve = new Curve();
		}

		/// <summary>
		///  Creates a new pair with a <see cref="ColorGradient"/> based on the provided <see cref="Color"/> and <see cref="Curve"/>
		/// </summary>
		/// <param name="color"></param>
		/// <param name="c"></param>
		public GradientLevelPair(Color color, Curve c)
		{
			ColorGradient = new ColorGradient(color);
			Curve = c;
		}

		/// <summary>
		/// Creates a new pair with a <see cref="ColorGradient"/> based on the provided <see cref="Color"/> and <see cref="CurveType"/>
		/// </summary>
		/// <param name="c"></param>
		/// <param name="type"></param>
		public GradientLevelPair(Color c, CurveType type)
		{
			ColorGradient = new ColorGradient(c);
			Curve = new Curve(type);
		}

		[DataMember]
		public ColorGradient ColorGradient { get; set; }
		[DataMember]
		public Curve Curve { get; set; }

	}
}

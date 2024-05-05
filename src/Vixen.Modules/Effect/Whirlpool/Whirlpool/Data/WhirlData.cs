using System.Drawing;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Maintains properties of a whirl.
	/// </summary>
	[DataContract]
	public class WhirlData 
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public WhirlData()
		{
			Enabled = true;

			WhirlMode = WhirlpoolMode.RecurrentWhirls;
			TailLength = 10;
			StartLocation = WhirlpoolStartLocation.TopLeft;
			WhirlDirection = WhirlpoolDirection.In;
			Rotation = WhirlpoolRotation.Clockwise;
			ReverseDraw = false;
			Spacing = 2;
			Thickness = 2;
			Show3D = false;

			ColorMode = WhirlpoolColorMode.GradientOverTime;
			BandLength = 10;

			Colors = new List<ColorGradient> { new ColorGradient(Color.Red), new ColorGradient(Color.White) };
			LeftColor = new ColorGradient(Color.Blue);
			BottomColor = new ColorGradient(Color.Green);
			RightColor = new ColorGradient(Color.Yellow);
			TopColor = new ColorGradient(Color.Red);
			SingleColor = new ColorGradient(Color.Red);
		}

		#endregion

		#region Public Properties

		[DataMember]
		public bool Enabled { get; set; }

		[DataMember]
		public int X { get; set; }

		[DataMember]
		public int Y { get; set; }

		[DataMember]
		public int Width { get; set; }

		[DataMember]
		public int Height { get; set; }

		[DataMember]
		public WhirlpoolMode WhirlMode { get; set; }

		[DataMember]
		public int TailLength { get; set; }

		[DataMember]
		public WhirlpoolStartLocation StartLocation { get; set; }

		[DataMember]
		public WhirlpoolDirection WhirlDirection { get; set; }

		[DataMember]
		public WhirlpoolRotation Rotation { get; set; }

		[DataMember]
		public int Spacing { get; set; }

		[DataMember]
		public int Thickness { get; set; }

		[DataMember]
		public bool ReverseDraw { get; set; }

		[DataMember]
		public bool Show3D { get; set; }

		[DataMember]
		public WhirlpoolColorMode ColorMode { get; set; }

		[DataMember]
		public int BandLength { get; set; }

		[DataMember]
		public ColorGradient SingleColor { get; set; }

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public ColorGradient LeftColor { get; set; }

		[DataMember]
		public ColorGradient RightColor { get; set; }

		[DataMember]
		public ColorGradient TopColor { get; set; }

		[DataMember]
		public ColorGradient BottomColor { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Clones the whirl data.
		/// </summary>
		/// <returns>Clone of the whirl data.</returns>
		public WhirlData CreateInstanceForClone()
		{
			WhirlData result = new WhirlData
			{
				Enabled = Enabled,
				X = X,
				Y = Y,	
				Width = Width,
				Height = Height,

				WhirlMode = WhirlMode,
				TailLength = TailLength,
				StartLocation = StartLocation,
				WhirlDirection = WhirlDirection,
				Rotation = Rotation,
				ReverseDraw = ReverseDraw,
				Spacing = Spacing,
				Thickness = Thickness,
				Show3D = Show3D,

				ColorMode = ColorMode,
				BandLength = BandLength,

				Colors = Colors.ToList(),
				LeftColor = LeftColor,
				BottomColor = BottomColor,
				RightColor = RightColor,
				TopColor = TopColor,
				SingleColor = SingleColor,
			};

			return result;
		}

		#endregion
	}
}
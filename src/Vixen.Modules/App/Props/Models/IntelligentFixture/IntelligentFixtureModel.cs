using Vixen.Sys.Props;
using Vixen.Sys.Props.Model;
using VixenModules.App.Props.Models.IntelligentFixture;
using VixenModules.Editor.FixtureGraphics;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace VixenModules.App.Props.Models.IntellligentFixture
{
	public class IntelligentFixtureModel: BasePropModel, IPropModel
	{
               
        public IntelligentFixtureModel() 
        {
			// Default the movement constraints of the moving head
			PanStartPosition = DefaultPanStartPosition;
			PanStopPosition = DefaultPanStopPosition;
			TiltStartPosition = DefaultTiltStartPosition;
			TiltStartPosition = DefaultTiltStopPosition;
						
			// Default the beam length percentage
			BeamLength = DefaultBeamLength;

			// Default to partially transparent
			BeamTransparency = 40;
			
			// Default the top beam width to 8 times larger than the base width
			BeamWidthMultiplier = 8;

			// Default to showing the legend
			ShowLegend = true;
			
			// Default the fixture strobe minimum and maximum
			StrobeRateMinimum = DefaultStrobeRateMinimum;
			StrobeRateMaximum = DefaultStrobeRateMaximum;

			// Default the color wheel rotation speed minimum and maximum
			MinColorWheelRotationSpeed = DefaultMinColorWheelRotationSpeed;
			MaxColorWheelRotationSpeed = DefaultMaxColorWheelRotationSpeed;

			// Default the maximum strobe duration
			MaximumStrobeDuration = DefaultMaxStrobeDuration;

			// Default the maximum pan travel time
			MaxPanTravelTime = DefaultMaxPanTravelTime;

			// Default the maximum tilt travel time
			MaxTiltTravelTime = DefaultMaxTiltTravelTime;
		}

		#region Constants

		/// <summary>
		/// Default pan start position in degrees.
		/// </summary>
		const int DefaultPanStartPosition = 0;

		/// <summary>
		/// Default pan stop position in degrees.
		/// </summary>
		const int DefaultPanStopPosition = 360;

		/// <summary>
		/// Default tilt start position in degrees.
		/// </summary>
		const int DefaultTiltStartPosition = 0;

		/// <summary>
		/// Default tilt stop position in degrees.
		/// </summary>
		const int DefaultTiltStopPosition = 180;

		/// <summary>
		/// Default beam length factor (100%).
		/// </summary>
		const int DefaultBeamLength = 100;

		/// <summary>
		/// Default minimum strobe rate in Hz.
		/// </summary>
		private const int DefaultStrobeRateMinimum = 1;

		/// <summary>
		/// Default maximum strobe rate in Hz.
		/// </summary>
		private const int DefaultStrobeRateMaximum = 25;

		/// <summary>
		/// Default the strobe flash duration to 50ms.
		/// </summary>
		private const int DefaultMaxStrobeDuration = 50;

		/// <summary>
		/// Default time it takes to pan from starting position to max pan position.
		/// </summary>
		private const double DefaultMaxPanTravelTime = 4.6;

		/// <summary>
		/// Default time it takes to tilt from starting position to max tilt position.
		/// </summary>
		private const double DefaultMaxTiltTravelTime = 1.7;

		/// <summary>
		/// Default minimum color wheel rotation speed in seconds.
		/// </summary>
		private const double DefaultMinColorWheelRotationSpeed = 158.0;

		/// <summary>
		/// Default maximum color wheel rotation speed in seconds.
		/// </summary>
		private const double DefaultMaxColorWheelRotationSpeed = 1.0;

		#endregion


		/*
        public IntelligentFixtureModel(int nodeCount, int nodeSize = 2)
        {
            _nodeCount = nodeCount;
            _nodeSize = nodeSize;
			Nodes.AddRange<NodePoint>(GetArchPoints(_nodeCount, _nodeSize, RotationAngle));
			PropertyChanged += ArchModel_PropertyChanged;
        }
		*/

		//private void ArchModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		//{
		//TODO make this smarter to do the minimal to add, subtract, or update node size or rotation angle.
		//Nodes.Clear();
		//Nodes.AddRange<NodePoint>(GetArchPoints(_nodeCount, _nodeSize, RotationAngle));
		//}

		/*
        public int NodeSize
        {
            get => _nodeSize;
            set => SetProperty(ref _nodeSize, value);
        }

        public int NodeCount
        {
            get => _nodeCount;
            set => SetProperty(ref _nodeCount, value);
        }    
		*/

		public double MinColorWheelRotationSpeed { get; set; }
		public double MaxColorWheelRotationSpeed { get; set; }

		public double MaxPanTravelTime { get; set; }
		public double MaxTiltTravelTime { get; set; }

		public int StrobeRateMinimum { get; set; }
		public int StrobeRateMaximum { get; set; }

		public int MaximumStrobeDuration { get; set; }

		public int PanStartPosition { get; set; }
		public int PanStopPosition { get; set; }

		public int TiltStartPosition { get; set; }
		public int TiltStopPosition { get; set; }

		public int BeamLength { get; set; }

		public int BeamTransparency { get; set; }

		public int BeamWidthMultiplier { get; set; }

		public bool ZoomNarrowToWide { get; set; }

		public bool ShowLegend { get; set; }

		public YesNoType InvertPanDirection { get; set; }

		public YesNoType InvertTiltDirection { get; set; }

		public MountingPositionType MountingPosition { get; set; }	

	}
}

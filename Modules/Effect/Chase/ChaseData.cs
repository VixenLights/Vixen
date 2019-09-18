using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Chase
{
	[DataContract]
	public class ChaseData : EffectTypeModuleData
	{
		[DataMember]
		public ChaseColorHandling ColorHandling { get; set; }

		[DataMember]
		public int PulseOverlap { get; set; }

		[DataMember]
		public double DefaultLevel { get; set; }

		[DataMember]
		public bool EnableDefaultLevel { get; set; }

		private Color _staticColor;

		[DataMember]
		public Color StaticColor
		{
			get { return _staticColor; }
			set
			{
				_staticColor = value;
				StaticColorGradient = new ColorGradient(_staticColor);
			}
		}

		public ColorGradient StaticColorGradient { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		[DataMember]
		public Curve PulseCurve { get; set; }

		[DataMember]
		public Curve ChaseMovement { get; set; }

		[DataMember]
		public int DepthOfEffect { get; set; }

		[DataMember]
		public bool ExtendPulseToStart { get; set; }

		[DataMember]
		public bool ExtendPulseToEnd { get; set; }

		[DataMember]
		public TargetNodeSelection TargetNodeSelection { get; set; }

		public ChaseData()
		{
			ColorHandling = ChaseColorHandling.StaticColor;
			PulseOverlap = 0;
			DefaultLevel = 0;
			StaticColor = Color.White;
			ColorGradient = new ColorGradient(Color.White);
			PulseCurve = new Curve();
			ChaseMovement = new Curve();
			DepthOfEffect = 0;
			ExtendPulseToStart = false;
			ExtendPulseToEnd = false;
			EnableDefaultLevel = false;
			TargetNodeSelection = TargetNodeSelection.Group;
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			ChaseData result = new ChaseData();
			result.ColorHandling = ColorHandling;
			result.PulseOverlap = PulseOverlap;
			result.DefaultLevel = DefaultLevel;
			result.EnableDefaultLevel = EnableDefaultLevel;
			result.StaticColor = StaticColor;
			result.ColorGradient = new ColorGradient(ColorGradient);
			result.PulseCurve = new Curve(PulseCurve);
			result.ChaseMovement = new Curve(ChaseMovement);
			result.DepthOfEffect = DepthOfEffect;
			result.ExtendPulseToStart = ExtendPulseToStart;
			result.ExtendPulseToEnd = ExtendPulseToEnd;
			result.TargetNodeSelection = TargetNodeSelection;
			return result;
		}
	}
}
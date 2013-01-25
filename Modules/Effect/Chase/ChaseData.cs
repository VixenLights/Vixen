using System.Runtime.Serialization;
using System.ComponentModel;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;

namespace VixenModules.Effect.Chase
{
	[DataContract]
	public class ChaseData : ModuleDataModelBase
	{
		[DataMember]
		public ChaseColorHandling ColorHandling { get; set; }

		[DataMember]
		public int PulseOverlap { get; set; }

		[DataMember]
		public double DefaultLevel { get; set; }

		[DataMember]
		public Color StaticColor { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		[DataMember]
		public Curve PulseCurve { get; set; }

		[DataMember]
		public Curve ChaseMovement { get; set; }

		[DataMember]
		public DepthOfEffect DepthOfEffect { get; set; }

		public ChaseData()
		{
			ColorHandling = ChaseColorHandling.StaticColor;
			PulseOverlap = 0;
			DefaultLevel = 0;
			StaticColor = Color.White;
			ColorGradient = new ColorGradient();
			PulseCurve = new Curve();
			ChaseMovement = new Curve();
			DepthOfEffect = Effect.Chase.DepthOfEffect.LeafElements;
		}

		public override IModuleDataModel Clone()
		{
			ChaseData result = new ChaseData();
			result.ColorHandling = ColorHandling;
			result.PulseOverlap = PulseOverlap;
			result.DefaultLevel = DefaultLevel;
			result.StaticColor = StaticColor;
			result.ColorGradient = new ColorGradient(ColorGradient);
			result.PulseCurve = new Curve(PulseCurve);
			result.ChaseMovement = new Curve(ChaseMovement);
			result.DepthOfEffect = DepthOfEffect;
			return result;
		}
	}

	public enum ChaseColorHandling
	{
		StaticColor,
		GradientThroughWholeEffect,
		GradientForEachPulse,
		ColorAcrossItems
	}

	public enum DepthOfEffect
	{
		// Since LeafElements comes first, it ends up being the default if it doesn't currently exist in the serialized data
		//(If a different default is desired, see http://msdn.microsoft.com/en-us/library/ms733734.aspx)
		[Description("Leaf Elements")]
		LeafElements,
		[Description("Children (1 level deep)")]
		Children,
		[Description("Grandchildren (2 levels deep)")]
		Grandchildren
	}
}

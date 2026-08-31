using System.Runtime.Serialization;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Fire
{
	[DataContract]
	public class FireData: EffectTypeModuleData
	{
		public FireData()
		{
			Location = FireDirection.Bottom;
			Height = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			HueShiftCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			Orientation = StringOrientation.Vertical;
			DepthOfEffect = 0;
			TargetNodeSelection = TargetNodeSelection.Group;
		}

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public FireDirection Location { get; set; }

		[DataMember]
		public Curve Height { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int HueShift { get; set; }

		[DataMember]
		public Curve HueShiftCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		/// <summary>
		/// Gets or sets the target hierarchy depth used for individual target rendering.
		/// </summary>
		/// <value>The selected target hierarchy depth. The default is <c>0</c>.</value>
		[DataMember]
		public int DepthOfEffect { get; set; }

		/// <summary>
		/// Gets or sets the target-node handling mode for the Fire effect.
		/// </summary>
		/// <value>The target-node handling mode. The default is <see cref="TargetNodeSelection.Group" />.</value>
		[DataMember]
		public TargetNodeSelection TargetNodeSelection { get; set; }

		/// <summary>
		/// Restores data omitted by legacy Fire effect payloads and normalizes invalid target settings.
		/// </summary>
		/// <param name="context">The serialization context for the deserialized data.</param>
		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (HueShiftCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(HueShift, 100, 0);
				HueShiftCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
				HueShift = 0;
			}

			if (!Enum.IsDefined<TargetNodeSelection>(TargetNodeSelection))
			{
				TargetNodeSelection = TargetNodeSelection.Group;
			}

			if (DepthOfEffect < 0)
			{
				DepthOfEffect = 0;
			}
		}
		
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			FireData result = new FireData
			{
				Location = Location,
				Height = Height,
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve),
				HueShiftCurve = new Curve(HueShiftCurve),
				DepthOfEffect = DepthOfEffect,
				TargetNodeSelection = TargetNodeSelection
			};
			return result;
		}
	}
}

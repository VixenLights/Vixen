using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Sys;
using Vixen.Commands;

namespace Vixen.Module.Effect {
	// Effect instances are no longer singletons that render for all, they now contain
	// state necessary for rendering per-instance.
    public interface IEffect {
		bool IsDirty { get; }

		/// <summary>
		/// Nodes the effect is being applied to as a single collection.
		/// </summary>
		ChannelNode[] TargetNodes { get; set; }

		/// <summary>
		/// The length of the entire effect.
		/// </summary>
		TimeSpan TimeSpan { get; set; }

		/// <summary>
		/// Effect parameter values.
		/// </summary>
		object[] ParameterValues { get; set; }

		void PreRender();
		// Having two methods instead of a single one with default values so that the
		// effect doesn't have to check to see if there is a time frame restriction
		// with every call.
		ChannelData Render();
		ChannelData Render(TimeSpan restrictingOffsetTime, TimeSpan restrictingTimeSpan);
		string EffectName { get; }
		CommandParameterSignature Parameters { get; }
		void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle);
	}
}

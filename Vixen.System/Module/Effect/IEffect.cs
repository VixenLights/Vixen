using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using Vixen.Sys;

namespace Vixen.Module.Effect
{
	// Effect instances are no longer singletons that render for all, they now contain
	// state necessary for rendering per-instance.
	public interface IEffect: INotifyPropertyChanged
	{
		bool IsDirty { get; }

		/// <summary>
		/// Nodes the effect is being applied to as a single collection.
		/// </summary>
		ElementNode[] TargetNodes { get; set; }

		/// <summary>
		/// Calculated set of element ids that this effect has an impact on.
		/// </summary>
		IEnumerable<Guid> EffectedElementIds { get; set; }

		/// <summary>
		/// The length of the entire effect.
		/// </summary>
		TimeSpan TimeSpan { get; set; }

		/// <summary>
		/// Effect parameter values.
		/// </summary>
		object[] ParameterValues { get; set; }

		void PreRender(CancellationTokenSource cancellationToken = null);
		// Having two methods instead of a single one with default values so that the
		// effect doesn't have to check to see if there is a time frame restriction
		// with every call.
		EffectIntents Render();
		EffectIntents Render(TimeSpan restrictingOffsetTime, TimeSpan restrictingTimeSpan);
		string EffectName { get; }
		ParameterSignature Parameters { get; }
		void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle);
		ElementIntents GetElementIntents(TimeSpan currentTime);
	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using Vixen.Marks;
using Vixen.Module.Media;
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
		IElementNode[] TargetNodes { get; set; }

		/// <summary>
		/// The length of the entire effect.
		/// </summary>
		TimeSpan TimeSpan { get; set; }

		/// <summary>
		/// The layer in which the effect is active.
		/// </summary>
		TimeSpan StartTime { get; set; }

		/// <summary>
		/// Effect parameter values.
		/// </summary>
		object[] ParameterValues { get; set; }

		bool PreRender(CancellationTokenSource cancellationToken = null);
		// Having two methods instead of a single one with default values so that the
		// effect doesn't have to check to see if there is a time frame restriction
		// with every call.
		EffectIntents Render();
		EffectIntents Render(TimeSpan restrictingOffsetTime, TimeSpan restrictingTimeSpan);
		string EffectName { get; }
		ParameterSignature Parameters { get; }
		void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle);
		bool SupportsMedia { get; }
		string[] SupportsExtensions { get; }
		List<IMediaModuleInstance> Media { get; set; }
		bool SupportsMarks { get; }
		ObservableCollection<IMarkCollection> MarkCollections { get; set; }

	}
}
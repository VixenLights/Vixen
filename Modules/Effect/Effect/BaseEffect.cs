using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Vixen.Intent;
using Vixen.Marks;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Property.Color;
using VixenModules.Property.Orientation;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Base effect implementation to be used by all basic type effects. This provides 
	/// some utility methods to create intents and determine valid discrete colors
	/// </summary>
	public abstract class BaseEffect : EffectModuleInstanceBase
	{
		private bool _hasDiscreteColors;

		protected abstract EffectTypeModuleData EffectModuleData { get; }


		/// <summary>
		/// Indicates if there is any discrete colors assigned to any elements this effect targets. It does not mean all of the elements are discrete if true.
		/// Each effect should set this if it can work on discrete elements
		/// </summary>
		[Browsable(false)]
		public bool HasDiscreteColors
		{
			get { return _hasDiscreteColors; }
			set
			{
				_hasDiscreteColors = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets the list of valid colors this effect can use and sets the hasDiscreteColors flag if any of it's targeted elements are discrete and have a restricted list.
		/// </summary>
		/// <returns></returns>
		[Browsable(false)]
		public HashSet<Color> GetValidColors()
		{
			HashSet<Color> validColors = new HashSet<Color>();
			validColors.AddRange(TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
			if (validColors.Any())
			{
				HasDiscreteColors = true;
			}
			else
			{
				HasDiscreteColors = false;
			}
			return validColors;
		}

		protected Tuple<bool, StringOrientation> GetOrientation()
		{
			var value = new Tuple<bool, StringOrientation>(false, StringOrientation.Vertical);
			var node = TargetNodes.FirstOrDefault();
			if (node != null && node.Properties.Contains(OrientationDescriptor._typeId))
			{
				var orientation = OrientationModule.GetOrientationForElement(node);
				switch (orientation)
				{
					case Orientation.Horizontal:
						value = new Tuple<bool, StringOrientation>(true, StringOrientation.Horizontal);
						break;
					default:
						value = new Tuple<bool, StringOrientation>(true, StringOrientation.Vertical);
						break;
				}
			}

			return value;

		}

		protected bool IsElementDiscrete(ElementNode elementNode)
		{
			return ColorModule.isElementNodeDiscreteColored(elementNode);
		}

		protected EffectIntents CreateIntentsForElement(ElementNode element, double intensity, Color color, TimeSpan duration)
		{
			EffectIntents effectIntents = new EffectIntents();
			foreach (ElementNode elementNode in element.GetLeafEnumerator())
			{
				if (HasDiscreteColors && IsElementDiscrete(elementNode))
				{
					IEnumerable<Color> colors = ColorModule.getValidColorsForElementNode(elementNode, false);
					if (!colors.Contains(color))
					{
						continue;
					}
				}

				IIntent intent = CreateIntent(elementNode, color, intensity, duration);
				effectIntents.AddIntentForElement(elementNode.Element.Id, intent, TimeSpan.Zero);
			}

			return effectIntents;
		}

		protected IIntent CreateIntent(ElementNode node, Color color, double intensity, TimeSpan duration)
		{
			if (HasDiscreteColors && IsElementDiscrete(node))
			{
				return CreateDiscreteIntent(color, intensity, duration);
			}

			return CreateIntent(color, intensity, duration);
		}

		protected IIntent CreateIntent(ElementNode node, Color startColor, Color endColor, double startIntensity, double endIntensity, TimeSpan duration)
		{
			if (HasDiscreteColors && IsElementDiscrete(node))
			{
				return CreateDiscreteIntent(startColor, startIntensity, endIntensity, duration);
			}

			return CreateIntent(startColor, endColor, startIntensity, endIntensity, duration);
		}

		protected static IIntent CreateIntent(Color color, double intensity, TimeSpan duration)
		{
			return IntentBuilder.CreateIntent(color, intensity, duration);
		}

		protected static IIntent CreateIntent(Color startColor, Color endColor, double startIntensity, double endIntensity, TimeSpan duration)
		{
			return IntentBuilder.CreateIntent(startColor, endColor, startIntensity, endIntensity, duration);
		}

		protected static IIntent CreateDiscreteIntent(Color color, double intensity, TimeSpan duration)
		{
			return IntentBuilder.CreateDiscreteIntent(color, intensity, duration);
		}

		protected static IIntent CreateDiscreteIntent(Color color, double startIntensity, double endIntensity, TimeSpan duration)
		{
			return IntentBuilder.CreateDiscreteIntent(color, startIntensity, endIntensity, duration);
		}
		private static double ConvertRange(double originalStart, double originalEnd, double newStart, double newEnd, double value) // value to convert
		{
			double scale = (newEnd - newStart) / (originalEnd - originalStart);
			return newStart + (value - originalStart) * scale;
		}

		/// <summary>
		/// Takes an arbitrary value greater than equal to 0 and less than equal to 100 and translates it to a corresponding minimum - maximum value 
		/// suitable for use in a range 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="maximum"></param>
		/// <param name="minimum"></param>
		/// <returns></returns>
		protected static double ScaleCurveToValue(double value, double maximum, double minimum)
		{
			return ConvertRange(0, 100, minimum, maximum, value);
		}

		protected double GetEffectTimeIntervalPosition(TimeSpan startTime)
		{
			return startTime.TotalMilliseconds / TimeSpan.TotalMilliseconds;
		}

		protected static double DistanceFromPoint(Point origin, Point point)
		{
			return Math.Sqrt(Math.Pow((point.X - origin.X), 2) + Math.Pow((point.Y - origin.Y), 2));
		}

		#region Overrides of ModuleInstanceBase

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (SupportsMarks && MarkCollections != null)
				{
					foreach (var markCollection in MarkCollections)
					{
						RemoveMarkCollectionListeners(markCollection);
					}
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		#region IMark helpers

		/// <summary>
		/// Returns true if any marks in the list have a start time inclusive of the effect time. 
		/// </summary>
		/// <param name="marks"></param>
		/// <returns></returns>
		protected bool IsAnyMarksInclusiveOfEffectTimespan(IEnumerable<IMark> marks)
		{
			bool result = false;
			foreach (var mark in marks)
			{
				if (mark.StartTime >= StartTime && mark.StartTime <= StartTime + TimeSpan)
				{
					result = true;
					break;
				}
			}

			return result;
		}

		#endregion

		#region IMark change event handlers 

		/// <summary>
		/// Initialize the listeners on a specific collection
		/// </summary>
		/// <param name="markCollection"></param>
		protected void InitializeMarkCollectionListeners(IMarkCollection markCollection)
		{
			if (markCollection != null)
			{
				//Ensure we don't have any already attached listeners.
				RemoveMarkListeners(markCollection.Marks);
				AddMarkCollectionListeners(markCollection);
			}
		}

		private void ReloadMarkListeners(IMarkCollection collection)
		{
			var marks = collection?.Marks;
			if (marks != null)
			{
				RemoveMarkListeners(marks);
				AddMarkListeners(marks);
			}
		}

		private void RemoveMarkListeners(IEnumerable<IMark> marks)
		{
			if (marks == null) return;
			foreach (var mark in marks)
			{
				mark.PropertyChanged -= Mark_PropertyChanged;
			}
		}

		private void AddMarkListeners(IEnumerable<IMark> marks)
		{
			if (marks == null) return;
			foreach (var mark in marks)
			{
				mark.PropertyChanged += Mark_PropertyChanged;
			}
		}

		protected void AddMarkCollectionListeners(IMarkCollection mc)
		{
			if (mc == null) return;
			((INotifyCollectionChanged)mc.Marks).CollectionChanged += MarkCollectionPropertyChanged;
			AddMarkListeners(mc.Marks);
		}

		protected void RemoveMarkCollectionListeners(IMarkCollection mc)
		{
			if (mc == null) return;
			((INotifyCollectionChanged)mc.Marks).CollectionChanged -= MarkCollectionPropertyChanged;
			RemoveMarkListeners(mc.Marks);
		}

		/// <summary>
		/// Gets called when a mark property changes. The default implementation looks for changes in marks that have a start time
		/// inclusive of the effect timespan and have a change in time or text.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void Mark_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "StartTime" || e.PropertyName == "EndTime" || e.PropertyName == "Text")
			{
				if (!IsDirty)
				{
					var mark = sender as IMark;
					IsDirty = IsAnyMarksInclusiveOfEffectTimespan(new[] { mark });
				}
			}
		}

		/// <summary>
		/// Gets called when something changes in the overall collection being monitored. The default implementation adds/removes 
		/// mark listeners when marks are added or removed from the collection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void MarkCollectionPropertyChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			//We are just going to deal with adds and removes here. The individual event handlers will deal with moves.
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				var marks = e.NewItems.Cast<IMark>();
				AddMarkListeners(marks);
				if (!IsDirty)
				{
					IsDirty = IsAnyMarksInclusiveOfEffectTimespan(marks);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				var marks = e.OldItems.Cast<IMark>();
				RemoveMarkListeners(marks);
				if (!IsDirty)
				{
					IsDirty = IsAnyMarksInclusiveOfEffectTimespan(marks);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				ReloadMarkListeners(sender as IMarkCollection);
				IsDirty = true;
			}
		}

		#endregion

		#region Random

		protected int Rand()
		{
			return ThreadSafeRandom.Instance.Next();
		}

		protected int Rand(int minValue, int maxValue)
		{
			return ThreadSafeRandom.Instance.Next(minValue, maxValue);
		}

		protected double RandDouble()
		{
			return ThreadSafeRandom.Instance.NextDouble();
		}

		#endregion
	}
}

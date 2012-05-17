using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace Vixen.Interpolator {
	public abstract class Interpolator<T> {
		public bool Interpolate(TimeSpan timeOffset, TimeSpan timeSpan, T startValue, T endValue, out T value) {
			float percent = (float)(timeOffset.TotalMilliseconds / timeSpan.TotalMilliseconds);
			return Interpolate(percent, startValue, endValue, out value);
		}

		public bool Interpolate(double percentage, T startValue, T endValue, out T value) {
			value = default(T);

			if(percentage > 0 && percentage < 1) {
				value = InterpolateValue(percentage, startValue, endValue);
				return true;
			}

			return false;
		}

		abstract protected T InterpolateValue(double percent, T startValue, T endValue);
	}

	public static class Interpolator {
		static private Dictionary<Type, object> _interpolators;

		static Interpolator() {
			_interpolators = new Dictionary<Type, object>();
		}

		static public Interpolator<T> Create<T>() {
			return _FindInterpolator<T>(typeof(T));
		}

		private static Interpolator<T> _FindInterpolator<T>(Type type) {
			return
				_FindInDictionary<T>(type) ??
				_FindInAssembly<T>(type);
		}

		private static Interpolator<T> _FindInDictionary<T>(Type type) {
			object interpolator;
			_interpolators.TryGetValue(type, out interpolator);
			return (Interpolator<T>)interpolator;
		}

		private static Interpolator<T> _FindInAssembly<T>(Type type) {
			// Attribute : Interpolator type
			var interpolatorTypeLookup = Assembly.GetExecutingAssembly().GetAttributedTypes(typeof(InterpolatorAttribute)).ToDictionary(x => x.GetCustomAttributes(typeof(InterpolatorAttribute), true).First(), x => x);
			var interpolatorAttribute = interpolatorTypeLookup.Keys.Cast<InterpolatorAttribute>().FirstOrDefault(x => x.TargetType == type);
			object interpolator = null;
			if(interpolatorAttribute != null) {
				Type interpolatorType = interpolatorTypeLookup[interpolatorAttribute];
				try {
					interpolator = Activator.CreateInstance(interpolatorType);
				} catch {
					throw new Exception("Found an interpolator for type \"" + interpolatorType.Name + "\" but couldn't create it.");
				}
				_interpolators[type] = interpolator;
			}
			return (Interpolator<T>)interpolator;
		}
	}
}

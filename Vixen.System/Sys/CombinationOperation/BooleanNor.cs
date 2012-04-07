using System;
using System.Drawing;

namespace Vixen.Sys.CombinationOperation {
	public class BooleanNor : BooleanOperation {
		public BooleanNor() {
			RegisterCombiner(new LongValueCombiner());
			RegisterCombiner(new FloatValueCombiner());
			RegisterCombiner(new DoubleValueCombiner());
			RegisterCombiner(new DateTimeValueCombiner());
			RegisterCombiner(new ColorValueCombiner());
		}

		#region Long
		private class LongValueCombiner : Combiner<long> {
			override public void Handle(IIntentState<float> obj) {
				Value = ~(Value | (long)obj.GetValue());
			}

			override public void Handle(IIntentState<long> obj) {
				Value = ~(Value | obj.GetValue());
			}

			override public void Handle(IIntentState<double> obj) {
				Value = ~(Value | (long)(obj.GetValue() * 100));
			}
		}
		#endregion

		#region Float
		private class FloatValueCombiner : Combiner<float> {
			override public void Handle(IIntentState<float> obj) {
				Value = ~((int)Value | (int)obj.GetValue());
			}

			override public void Handle(IIntentState<long> obj) {
				Value = ~((int)Value | obj.GetValue());
			}

			override public void Handle(IIntentState<double> obj) {
				Value = ~((int)Value | (int)(obj.GetValue() * 100));
			}
		}
		#endregion

		#region Double (%)
		private class DoubleValueCombiner : CombinationOperation.DoubleValueCombiner {
			override public void Handle(IIntentState<float> obj) {
				Value = ~((int)Value | (int)obj.GetValue());
			}

			override public void Handle(IIntentState<long> obj) {
				Value = ~((int)Value | obj.GetValue());
			}

			override public void Handle(IIntentState<double> obj) {
				Value = ~((int)Value | (int)(obj.GetValue() * 100));
			}
		}
		#endregion

		#region DateTime
		private class DateTimeValueCombiner : Combiner<DateTime> {
		}
		#endregion

		#region Color
		private class ColorValueCombiner : Combiner<Color> {
			override public void Handle(IIntentState<Color> obj) {
				Color color = obj.GetValue();
				Value = Color.FromArgb(~(Value.R | color.R), ~(Value.G | color.G), ~(Value.B | color.B));
			}
		}
		#endregion
	}

	//public class BooleanNor : ICombinationOperation {
	//    private LongValueCombiner _longValueCombiner;
	//    private FloatValueCombiner _floatValueCombiner;
	//    private DoubleValueCombiner _doubleValueCombiner;
	//    private DateTimeValueCombiner _dateTimeValueCombiner;
	//    private ColorValueCombiner _colorValueCombiner;

	//    public BooleanNor() {
	//        _longValueCombiner = new LongValueCombiner();
	//        _floatValueCombiner = new FloatValueCombiner();
	//        _doubleValueCombiner = new DoubleValueCombiner();
	//        _dateTimeValueCombiner = new DateTimeValueCombiner();
	//        _colorValueCombiner = new ColorValueCombiner();
	//    }

	//    public long Combine(long value, IIntentState intentState) {
	//        return _longValueCombiner.Combine(value, intentState);
	//    }

	//    public float Combine(float value, IIntentState intentState) {
	//        return _floatValueCombiner.Combine(value, intentState);
	//    }

	//    public double Combine(double value, IIntentState intentState) {
	//        return _doubleValueCombiner.Combine(value, intentState);
	//    }

	//    public DateTime Combine(DateTime value, IIntentState intentState) {
	//        return _dateTimeValueCombiner.Combine(value, intentState);
	//    }

	//    public Color Combine(Color value, IIntentState intentState) {
	//        return _colorValueCombiner.Combine(value, intentState);
	//    }

	//    #region Long
	//    private class LongValueCombiner : IAnyIntentStateHandler {
	//        private long _value;

	//        public long Combine(long value, IIntentState intentState) {
	//            _value = value;
	//            intentState.Dispatch(this);
	//            return _value;
	//        }

	//        public void Handle(IIntentState<float> obj) {
	//            _value = ~(_value | (long)obj.GetValue());
	//        }

	//        public void Handle(IIntentState<DateTime> obj) {
	//        }

	//        public void Handle(IIntentState<Color> obj) {
	//        }

	//        public void Handle(IIntentState<long> obj) {
	//            _value = ~(_value | obj.GetValue());
	//        }

	//        public void Handle(IIntentState<double> obj) {
	//            _value = ~(_value | (long)(obj.GetValue() * 100));
	//        }
	//    }
	//    #endregion

	//    #region Float
	//    private class FloatValueCombiner : IAnyIntentStateHandler {
	//        private float _value;

	//        public float Combine(float value, IIntentState intentState) {
	//            _value = value;
	//            intentState.Dispatch(this);
	//            return _value;
	//        }

	//        public void Handle(IIntentState<float> obj) {
	//            _value = ~((int)_value | (int)obj.GetValue());
	//        }

	//        public void Handle(IIntentState<DateTime> obj) {
	//        }

	//        public void Handle(IIntentState<Color> obj) {
	//        }

	//        public void Handle(IIntentState<long> obj) {
	//            _value = ~((int)_value | obj.GetValue());
	//        }

	//        public void Handle(IIntentState<double> obj) {
	//            _value = ~((int)_value | (int)(obj.GetValue() * 100));
	//        }
	//    }
	//    #endregion

	//    #region Double (%)
	//    private class DoubleValueCombiner : IAnyIntentStateHandler {
	//        private double _value;

	//        public double Combine(double value, IIntentState intentState) {
	//            _value = value * 100;
	//            intentState.Dispatch(this);
	//            return _value / 100;
	//        }

	//        public void Handle(IIntentState<float> obj) {
	//            _value = ~((int)_value | (int)obj.GetValue());
	//        }

	//        public void Handle(IIntentState<DateTime> obj) {
	//        }

	//        public void Handle(IIntentState<Color> obj) {
	//        }

	//        public void Handle(IIntentState<long> obj) {
	//            _value = ~((int)_value | obj.GetValue());
	//        }

	//        public void Handle(IIntentState<double> obj) {
	//            _value = ~((int)_value | (int)(obj.GetValue() * 100));
	//        }
	//    }
	//    #endregion

	//    #region DateTime
	//    private class DateTimeValueCombiner : IAnyIntentStateHandler {
	//        private DateTime _value;

	//        // Does a boolean operation on a DateTime ever mean anything?

	//        public DateTime Combine(DateTime value, IIntentState intentState) {
	//            _value = value;
	//            intentState.Dispatch(this);
	//            return _value;
	//        }

	//        public void Handle(IIntentState<float> obj) {
	//        }

	//        public void Handle(IIntentState<DateTime> obj) {
	//        }

	//        public void Handle(IIntentState<Color> obj) {
	//        }

	//        public void Handle(IIntentState<long> obj) {
	//        }

	//        public void Handle(IIntentState<double> obj) {
	//        }
	//    }
	//    #endregion

	//    #region Color
	//    private class ColorValueCombiner : IAnyIntentStateHandler {
	//        private Color _value;

	//        public Color Combine(Color value, IIntentState intentState) {
	//            _value = value;
	//            intentState.Dispatch(this);
	//            return _value;
	//        }

	//        public void Handle(IIntentState<float> obj) {
	//        }

	//        public void Handle(IIntentState<DateTime> obj) {
	//        }

	//        public void Handle(IIntentState<Color> obj) {
	//            Color color = obj.GetValue();
	//            _value = Color.FromArgb(~(_value.R | color.R), ~(_value.G | color.G), ~(_value.B | color.B));
	//        }

	//        public void Handle(IIntentState<long> obj) {
	//        }

	//        public void Handle(IIntentState<double> obj) {
	//        }
	//    }
	//    #endregion
	//}
}

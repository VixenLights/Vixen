using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.ControlsEx.ValueControls
{
	/// <summary>
	/// ValueControl is the abstract base class for all Scrolling Controls
	/// </summary>
	[DefaultEvent("ValueChanged")]
	[DefaultProperty("Value")]
	public abstract class ValueControl:Control
	{
		#region variables
		private int _maximum = 100, _minimum = 0, _value = 0;
		#endregion
		#region public members
		public int GetPercentage(int width)
		{
			return width*(_value-_minimum)/Math.Max(1,_maximum-_minimum);
		}
		public bool SetPercentage(int pos, int width)
		{
			return SetValueCore(_minimum+(_maximum-_minimum)*pos/Math.Max(1,width));
		}
		#endregion
		#region controller
		protected bool SetValueCore(int value)
		{
			value=Math.Max(_minimum,Math.Min(_maximum,value));
			if (value==_value) return false;
			OnBeforeSetValue(value);
			_value=value;
			OnAfterSetValue();
			return true;
		}
		protected virtual void OnBeforeSetValue(int newvalue)
		{
			return;
		}
		protected virtual void OnAfterSetValue()
		{
			this.Refresh();
		}
		protected bool SetMaximumCore(int value)
		{
			value = Math.Max(_minimum + 1, value);
			if (value == _maximum) return false;
			_maximum = value;
			_value = Math.Min(_maximum, _value);
			OnSetMaximum();
			return true;
		}
		protected virtual void OnSetMaximum()
		{
			this.Refresh();
		}
		protected bool SetMinimumCore(int value)
		{
			value = Math.Min(_maximum - 1, value);
			if (value == _minimum) return false;
			_minimum = value;
			_value = Math.Max(_minimum, _value);
			OnSetMinimum();
			return true;
		}
		protected virtual void OnSetMinimum()
		{
			this.Refresh();
		}
		/// <summary>
		/// Refreshes only the specified rectangle
		/// </summary>
		public void Refresh(Rectangle rct)
		{
			this.Invalidate(rct);
			this.Update();
		}
		#endregion
		#region properties
		/// <summary>
		/// The Maximum value that is accepted
		/// </summary>
		[DefaultValue(100),
		Description("The Maximum value that is accepted")]
		public int Maximum
		{
			get { return _maximum; }
			set
			{
				this.SetMaximumCore(value);
			}
		}
		/// <summary>
		/// The Minimum value that is accepted
		/// </summary>
		[DefaultValue(0),
		Description("The Minimum value that is accepted")]
		public int Minimum
		{
			get { return _minimum; }
			set
			{
				this.SetMinimumCore(value);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(0),
		Description("The current value of the control")]
		public int Value
		{
			get { return _value; }
			set
			{
				this.SetValueCore(value);
			}
		}
		#endregion
		#region events
        protected virtual void RaiseValueChanged()
		{
			if (ValueChanged!=null)
				ValueChanged(this,ValueChangedEventArgs.Empty);
		}
		[Description("This event is raised when the value has changed")]
		public event ValueChangedEH ValueChanged;
		#endregion
	}
	/// <summary>
	/// delegate for ValueChanged event of a ValueUpDown control
	/// </summary>
	public delegate void ValueChangedEH(ValueControl sender, ValueChangedEventArgs e);
	/// <summary>
	/// EventArgs for a ValueChanged event
	/// </summary>
	public struct ValueChangedEventArgs
	{
		public static readonly ValueChangedEventArgs Empty = new ValueChangedEventArgs();
	}
	/// <summary>
	/// state of an element
	/// </summary>
	public enum ElementState
	{
		normal = 1, hot = 2, pushed = 3, disabled = 4
	}
	/// <summary>
	/// encapsulates bounds and state of an element
	/// </summary>
	public struct ElementInfo
	{
		public ElementInfo(ElementState state)
		{
			this.State=state;
			this.Bounds=Rectangle.Empty;
		}
		#region public members
		/// <summary>
		/// gets the buttonstate of the element
		/// </summary>
		public ButtonState ToButtonState()
		{
			switch (State)
			{
				case ElementState.pushed: return ButtonState.Pushed;
				case ElementState.disabled: return ButtonState.Inactive;
				default: return ButtonState.Normal;
			}
		}
		#endregion
		#region fields
		public ElementState State;
		public Rectangle Bounds;
		#endregion
	}
}

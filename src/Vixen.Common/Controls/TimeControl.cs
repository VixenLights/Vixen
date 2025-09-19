using Common.Controls.Theme;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

public partial class TimeControl : UserControl
{
	public TextBox textBox;
	private Button buttonUp;
	private Button buttonDown;
	private IntPtr focusCurrentControl = 0;
	static String timeFormat = @"mm\:ss\.fff";
	Font fontArrow;

	[LibraryImport("user32.dll")]
	internal static partial IntPtr GetFocus();

	[LibraryImport("user32.dll")]
	internal static partial IntPtr SetFocus(IntPtr hWnd);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool HideCaret(IntPtr hWnd);

	#region Properties
	/// <summary>
	/// Gets or sets the minimum allowable time value
	/// </summary>
	[Browsable(true)]
	public TimeSpan Minimum = TimeSpan.Zero;

	/// <summary>
	/// Gets or sets the maximum allowable time value
	/// </summary>
	[Browsable(true)]
	public TimeSpan Maximum = TimeSpan.MaxValue;

	/// <summary>
	/// Gets or sets the time value in a TimeSpan format
	/// </summary>
	[Browsable(true)]
	public TimeSpan TimeSpan
	{
		get
		{
			TimeSpan value;
			TimeSpan.TryParseExact(textBox.Text, timeFormat, CultureInfo.InvariantCulture, out value);
			return value;
		}
		set
		{ 
			textBox.Text = ValidateTime(value.ToString(timeFormat));
			SelectPos(-1);
		}
	}

	/// <summary>
	/// Gets or sets the time value in milliseconds
	/// </summary>
	[Browsable(true)]
	public double Milliseconds
	{
		get
		{
			TimeSpan value;
			TimeSpan.TryParseExact(textBox.Text, timeFormat, CultureInfo.InvariantCulture, out value);
			return value.TotalMilliseconds;
		}
		set
		{ 
			textBox.Text = ValidateTime(TimeSpan.FromMilliseconds(value).ToString(timeFormat));
			SelectPos(-1);
		}
	}

	/// <summary>
	/// Gets the time value in a String format
	/// </summary>
	[Browsable(true)]
	public new String ToString
	{	
		get => textBox.Text;
	}

	[Obsolete("This method is obsolete. Use TimeSpan or Milliseconds methods.", true)]
	public new String Text
	{
		set => throw new NotImplementedException();
		get => throw new NotImplementedException();
	}
	#endregion

	#region Constructors
	/// <summary>
	/// Initializes a new instance of the TimeControl class using defaults
	/// </summary>
	public TimeControl()
	{
		Initialize(TimeSpan.Zero, TimeSpan.MaxValue);
	}

	/// <summary>
	/// Initializes a new instance of the TimeControl class using the specified initial value and optional maximum value
	/// </summary>
	/// <param name="currentValue">Specifies the initial value</param>
	/// <param name="maxValue">Specifies the maximum value</param>
	public TimeControl(TimeSpan currentValue, TimeSpan? maxValue = null)
	{
		Initialize(currentValue, maxValue);
	}

	/// <summary>
	/// Initializes a new instance of the TimeControl class using the specified initial value and optional maximum value
	/// </summary>
	/// <param name="currentValue">Specifies the initial value</param>
	/// <param name="maxValue">Specifies the maximum value</param>
	public TimeControl(double value, TimeSpan? maxValue = null)
	{
		Initialize(TimeSpan.FromMilliseconds(value), maxValue);
	}

	/// <summary>
	/// Initializes a new instance of the TimeControl class using the specified initial value and optional maximum value
	/// </summary>
	/// <param name="currentValue">Specifies the initial value</param>
	/// <param name="maxValue">Specifies the maximum value</param>
	public TimeControl(double value, double? maxValue = null)
	{
		if (maxValue != null)
			Initialize(TimeSpan.FromMilliseconds(value), TimeSpan.FromMilliseconds((double)maxValue));
		else
			Initialize(TimeSpan.FromMilliseconds(value), null);
	}

	private void Initialize(TimeSpan value, TimeSpan? maxValue = null)
	{
		fontArrow = new Font(SystemFonts.DefaultFont.FontFamily, 4F, FontStyle.Regular);

		textBox = new TextBox { Dock = DockStyle.Fill };
		textBox.KeyDown += textBox_KeyDown;
		textBox.GotFocus += textBox_GotFocus;
		textBox.MouseDown += textBox_MouseDown;
		textBox.MouseMove += textBox_MouseMove;

		Panel panelUpDown = new Panel { Dock = DockStyle.Right, Width = 18 };
		buttonUp = new Button { Dock = DockStyle.Top, Width = panelUpDown.Width, Height = textBox.Height / 2};
		buttonUp.Click += ButtonUp_Click;
		buttonUp.MouseEnter += ButtonUpDown_MouseEnter;
		buttonUp.MouseLeave += ButtonUpDown_MouseLeave;
		buttonUp.Paint += ButtonUp_Paint;

		buttonDown = new Button { Dock = DockStyle.Bottom, Width = panelUpDown.Width, Height = textBox.Height / 2};
		buttonDown.Click += ButtonDown_Click;
		buttonDown.MouseEnter += ButtonUpDown_MouseEnter;
		buttonDown.MouseLeave += ButtonUpDown_MouseLeave;
		buttonDown.Paint += ButtonDown_Paint;

		panelUpDown.Controls.Add(buttonUp);
		panelUpDown.Controls.Add(buttonDown);

		Controls.Add(textBox);
		Controls.Add(panelUpDown);

		// Set the maximum, if available
		if (maxValue != null)
		{
			Maximum = (TimeSpan)maxValue;
		}

		// The value cannot be greater than the maximum
		if (value > Maximum)
		{
			value = Maximum;
		}

		// Set the value
		TimeSpan = value;
	}
	#endregion

	#region Operator Overloads
	[Browsable(true)]
	public static implicit operator TimeControl(TimeSpan value)
	{
		return new TimeControl(value);
	}

	public static implicit operator TimeControl(double value)
	{
		return new TimeControl(value, TimeSpan.MaxValue);
	}
	#endregion

	#region Event Handlers
	public event EventHandler ValueChanged;

	private void textBox_MouseMove(object sender, MouseEventArgs e)
	{
		int caretPos = textBox.SelectionStart;

		if (textBox.SelectionLength > 1)
		{
			// Get the position of the cursor in the text box
			int cursorPos = textBox.GetCharIndexFromPosition(new Point(e.X, e.Y));

			// If the cursor is moving to the left, make sure the highlight is only on digits
			if (cursorPos == textBox.SelectionStart)
			{
				caretPos = textBox.SelectionStart;
				if (caretPos < 0)
					caretPos = 0;
				else if (caretPos == 2)
					caretPos -= 2;
				else if (caretPos == 5)
					caretPos -= 2;
			}

			// If the cursor is moving to the right, make sure the highlight is only on digits
			else
			{
				caretPos = textBox.SelectionStart + textBox.SelectionLength - 1;
				if (caretPos == 2)
					caretPos++;
				else if (caretPos == 5)
					caretPos++;
				else if (caretPos == 9)
					caretPos = 8;
			}
		}

		SelectPos(caretPos);
	}

	private void textBox_MouseDown(object sender, MouseEventArgs e)
	{
		// The cursor can only the highlight digits
		int caretPos = textBox.SelectionStart;
		if (caretPos == 2)
			caretPos++;
		else if (caretPos == 5)
			caretPos++;
		else if (caretPos == 9)
			caretPos = 8;

		SelectPos(caretPos);
	}

	private void textBox_GotFocus(object sender, EventArgs e)
	{
		int caretPos;

		caretPos = textBox.SelectionStart;
		textBox.Select(caretPos, 1);

		HideCaret(textBox.Handle);
	}

	private void textBox_KeyDown(object sender, KeyEventArgs e)
	{
		int caretPos;

		caretPos = textBox.SelectionStart;

		e.Handled = true;

		if ('0' <= ConvertKeyToChar(e.KeyData) && ConvertKeyToChar(e.KeyData) <= '9')
		{
			ReplaceChar(caretPos, e.KeyData);

			// After inserting a character, move the caret right so the cursor can only the highlight digits
			caretPos++;
			if (caretPos == 2)
				caretPos ++;
			else if (caretPos == 5)
				caretPos ++;
			else if (caretPos >= 8)
				caretPos = 8;
			e.SuppressKeyPress = true;
		}

		else if (e.KeyCode == Keys.Right)
		{
			// The cursor can only the highlight digits
			if (caretPos == 1)
				caretPos += 2;
			else if (caretPos == 4)
				caretPos += 2;
			else if (caretPos >= 8)
			{ } // don't move past the end  
			else
				caretPos++;
		}

		else if (e.KeyCode == Keys.Left)
		{
			// The cursor can only the highlight digits
			if (caretPos <= 0)
			{ } // don't move before the beginning
			else if (caretPos == 3)
				caretPos -= 2;
			else if (caretPos == 6)
				caretPos -= 2;
			else
				caretPos--;
		}

		else if (e.KeyCode == Keys.Up)
		{
			int delta = 1;

			if (e.KeyData.HasFlag(Keys.Control))
				delta = 9;

			else if (e.KeyData.HasFlag(Keys.Alt))
				delta = 99;

			RotateValue(caretPos, delta);
		}

		else if (e.KeyCode == Keys.Down)
		{
			int delta = -1;

			if (e.KeyData.HasFlag(Keys.Control))
				delta = -9;

			else if (e.KeyData.HasFlag(Keys.Alt))
				delta = -99;

			RotateValue(caretPos, delta);
		}

		else if (e.KeyCode == Keys.Home)
		{
			caretPos = 0;
		}

		else if (e.KeyCode == Keys.End)
		{
			caretPos = 8;
		}

		else if (e.KeyCode == Keys.Insert || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
		{
			e.SuppressKeyPress = true;
		}

		else
		{
			e.SuppressKeyPress = true;
		}

		SelectPos(caretPos);
	}

	private void ButtonDown_Click(object sender, EventArgs e)
	{
		textBox.Focus();
		SendKeys.SendWait("{DOWN}");
	}

	private void ButtonUp_Click(object sender, EventArgs e)
	{
		textBox.Focus();
		SendKeys.SendWait("{UP}");
	}

	private void ButtonUpDown_MouseEnter(object sender, EventArgs e)
	{
		if (focusCurrentControl == 0)
			focusCurrentControl = GetFocus();

		textBox.Focus();
		SelectPos(textBox.SelectionStart);
	}

	private void ButtonUpDown_MouseLeave(object sender, EventArgs e)
	{
		SetFocus(focusCurrentControl);
		focusCurrentControl = 0;
	}

	private void ButtonUp_Paint(object sender, PaintEventArgs e)
	{
		Brush brushArrow;

		if (buttonUp.Enabled == true)
		{
			brushArrow = new SolidBrush(ThemeColorTable.ButtonTextColor);
		}
		else
		{
			brushArrow = new SolidBrush(ThemeColorTable.ForeColorDisabled);
		}
		e.Graphics.DrawString("\u25B2", fontArrow, brushArrow, (buttonUp.Width - fontArrow.Height) / 2, (buttonUp.Height - fontArrow.Height) / 2);
	}

	private void ButtonDown_Paint(object sender, PaintEventArgs e)
	{
		Brush brushArrow;

		if (buttonUp.Enabled == true)
		{
			brushArrow = new SolidBrush(ThemeColorTable.ButtonTextColor);
		}
		else
		{
			brushArrow = new SolidBrush(ThemeColorTable.ForeColorDisabled);
		}
		e.Graphics.DrawString("\u25BC", fontArrow, brushArrow, (buttonDown.Width - fontArrow.Height)/ 2, (buttonDown.Height - fontArrow.Height) / 2);
	}
	#endregion

	#region Private Methods
	private void ReplaceChar(int position, Keys key)
	{
		char newChar = ConvertKeyToChar(key);

		// Some positions can't be greater than 5 (i.e. time can be no greater than 59 minutes or seconds)
		if ((position == 0 || position == 3) && newChar > '5')
		{
			newChar = '5';
		}

		textBox.Text = ValidateTime(textBox.Text.Remove(textBox.SelectionStart, 1).Insert(textBox.SelectionStart, new String(newChar, 1)));
	}

	private void InsertChar(int position, Keys key)
	{
		textBox.Text = textBox.Text.Insert(textBox.SelectionStart, new String(ConvertKeyToChar(key), 1));
	}

	private char ConvertKeyToChar(Keys key)
	{
		char character = (char)0;

		if (Keys.NumPad0 <= key && key <= Keys.NumPad9)
		{
			character = (char)((int)key - (int)Keys.NumPad0 + (int)Keys.D0);
		}
		else if (Keys.D0 <= key && key <= Keys.D9)
		{
			character = (char)((int)key - (int)Keys.D0 + '0');
		}

		return character;
	}

	private void RotateValue(int position, int delta)
	{
		char[] text = textBox.Text.ToArray();
		int value;

		if (delta < -9)
		{
			while (++position < textBox.Text.Length)
			{
				if (Char.IsNumber(text[position]) == true)
				{
					text[position] = '0';
				}
			}
		}

		else if (delta > 9)
		{
			while (++position < textBox.Text.Length)
			{
				if (Char.IsNumber(text[position]) == true)
				{
					if (position == 0 || position == 3)
					{
						text[position] = '5';
					}
					else
					{
						text[position] = '9';
					}
				}
			}
		}

		else
		{
			value = text[position];
			value = value + delta;
			if (value < '0')
			{
				value = '0';
			}
			else if (value > '9')
			{
				value = '9';
			}

			if ((position == 0 || position == 3) && value >= '6')
			{
				value = '5';
			}
			text[position] = (char)value;
		}

		textBox.Text = ValidateTime(new String(text));
	}

	private void SelectPos(int position)
	{
		if (position < 0)
		{
			for (position = 0; position < textBox.Text.Length - 1; position++)
			{
				if (Char.IsNumber(textBox.Text[position]) == true && textBox.Text[position] != '0')
				{
					break;
				}
			}
		}

		textBox.Select(position, 1);
	}

	private String ValidateTime(String time)
	{
		TimeSpan value;
		TimeSpan.TryParseExact(time, timeFormat, CultureInfo.InvariantCulture, out value);
		if (value > Maximum)
			value = Maximum;
		else if (value < Minimum)
			value = Minimum;

		ValueChanged?.Invoke(this, EventArgs.Empty);

		return value.ToString(timeFormat);
	}
	#endregion
}



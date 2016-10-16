/*
 * Copyright © 2010, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VixenModules.Editor.EffectEditor.Utils
{
	/// <summary>
	///     Provides additional facilities for TextBox control.
	/// </summary>
	public static class TextBoxExtender
	{

		#region CommitOnFocusLost

		/// <summary>
		/// </summary>
		public static readonly DependencyProperty CommitOnFocusLostProperty = DependencyProperty.RegisterAttached(
			"CommitOnFocusLost", typeof(bool), typeof(TextBoxExtender),
			new FrameworkPropertyMetadata(false, OnCommitOnFocusLostChanged));

		private static void OnCommitOnFocusLostChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var textbox = sender as TextBox;
			if (textbox == null) return;

			var wasBound = (bool)(e.OldValue);
			var needToBind = (bool)(e.NewValue);

			if (wasBound)
				textbox.LostFocus -= TextBoxFocusLostCommitValue;

			if (needToBind)
			{
				textbox.LostFocus -= TextBoxFocusLostCommitValue;
				textbox.LostFocus += TextBoxFocusLostCommitValue;
			}
		}

		private static void TextBoxFocusLostCommitValue(object sender, RoutedEventArgs e)
		{
			var textbox = sender as TextBox;
			if (textbox == null) return;

			var expression = textbox.GetBindingExpression(TextBox.TextProperty);
			if (expression != null) expression.UpdateSource();
			e.Handled = true;
			
		}

		public static void SetCommitOnFocusLost(TextBox target, bool value)
		{
			target.SetValue(CommitOnFocusLostProperty, value);
		}

		public static bool GetCommitOnFocusLost(TextBox target)
		{
			return (bool)target.GetValue(CommitOnFocusLostProperty);
		}

		#endregion

		#region CommitOnEnter

		/// <summary>
		/// </summary>
		public static readonly DependencyProperty CommitOnEnterProperty = DependencyProperty.RegisterAttached(
			"CommitOnEnter", typeof (bool), typeof (TextBoxExtender),
			new FrameworkPropertyMetadata(false, OnCommitOnEnterChanged));

		private static void OnCommitOnEnterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var textbox = sender as TextBox;
			if (textbox == null) return;

			var wasBound = (bool) (e.OldValue);
			var needToBind = (bool) (e.NewValue);

			if (wasBound)
				textbox.KeyDown -= TextBoxCommitValue;

			if (needToBind)
			{
				textbox.KeyDown -= TextBoxCommitValue;
				textbox.KeyDown += TextBoxCommitValue;
			}
		}

		private static void TextBoxCommitValue(object sender, KeyEventArgs e)
		{
			var textbox = sender as TextBox;
			if (textbox == null) return;

			if ((e.Key == Key.Enter))
			{
				e.Handled = true;
				var expression = textbox.GetBindingExpression(TextBox.TextProperty);
				if (expression != null) expression.UpdateSource();
			}
		}

		public static void SetCommitOnEnter(TextBox target, bool value)
		{
			target.SetValue(CommitOnEnterProperty, value);
		}

		public static bool GetCommitOnEnter(TextBox target)
		{
			return (bool) target.GetValue(CommitOnEnterProperty);
		}

		#endregion

		#region CommitOnTyping

		public static readonly DependencyProperty CommitOnTypingProperty =
			DependencyProperty.RegisterAttached("CommitOnTyping", typeof (bool), typeof (TextBoxExtender),
				new FrameworkPropertyMetadata(false, OnCommitOnTypingChanged));

		private static void OnCommitOnTypingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var textbox = sender as TextBox;
			if (textbox == null) return;

			var wasBound = (bool) (e.OldValue);
			var needToBind = (bool) (e.NewValue);

			if (wasBound)
				textbox.KeyUp -= TextBoxCommitValueWhileTyping;

			if (needToBind)
				textbox.KeyUp += TextBoxCommitValueWhileTyping;
		}

		private static void TextBoxCommitValueWhileTyping(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Escape) //keep the escape for the roolback active
			{
				var textbox = sender as TextBox;
				if (textbox == null) return;
				var expression = textbox.GetBindingExpression(TextBox.TextProperty);
				if (expression != null) expression.UpdateSource();
				e.Handled = true;
			}
		}

		public static void SetCommitOnTyping(TextBox target, bool value)
		{
			target.SetValue(CommitOnTypingProperty, value);
		}

		public static bool GetCommitOnTyping(TextBox target)
		{
			return (bool) target.GetValue(CommitOnTypingProperty);
		}

		#endregion

		#region RollbackOnEscape

		public static readonly DependencyProperty RollbackOnEscapeProperty =
			DependencyProperty.RegisterAttached("RollbackOnEscape", typeof (bool), typeof (TextBoxExtender),
				new FrameworkPropertyMetadata(false, OnRollbackOnEscapeChanged));

		private static void OnRollbackOnEscapeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var textbox = sender as TextBox;
			if (textbox == null) return;

			var wasBound = (bool) (e.OldValue);
			var needToBind = (bool) (e.NewValue);

			if (wasBound)
				textbox.KeyUp -= TextBoxRollbackValue;

			if (needToBind)
				textbox.KeyUp += TextBoxRollbackValue;
		}

		public static void SetRollbackOnEscape(TextBox target, bool value)
		{
			target.SetValue(RollbackOnEscapeProperty, value);
		}

		public static bool GetRollbackOnEscape(TextBox target)
		{
			return (bool) target.GetValue(RollbackOnEscapeProperty);
		}

		private static void TextBoxRollbackValue(object sender, KeyEventArgs e)
		{
			var textbox = sender as TextBox;
			if (textbox == null) return;

			if (e.Key == Key.Escape)
			{
				var expression = textbox.GetBindingExpression(TextBox.TextProperty);
				if (expression != null) expression.UpdateTarget();
				e.Handled = true;
			}
		}

		#endregion

		#region SelectAllOnFocus

		/// <summary>
		///     Identifies the SelectAllOnFocus attached property.
		/// </summary>
		public static readonly DependencyProperty SelectAllOnFocusProperty =
			DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof (bool), typeof (TextBoxExtender),
				new FrameworkPropertyMetadata(false, OnSelectAllOnFocusChanged));

		/// <summary>
		///     Sets the select all on focus.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		public static void SetSelectAllOnFocus(DependencyObject target, bool value)
		{
			target.SetValue(SelectAllOnFocusProperty, value);
		}

		/// <summary>
		///     Determines whether the all the text should be selected on focus.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns>
		///     <c>true</c> if text should be selected on focus; otherwise, <c>false</c>.
		/// </returns>
		public static bool GetSelectAllOnFocus(DependencyObject target)
		{
			return (bool) target.GetValue(SelectAllOnFocusProperty);
		}

		private static void OnSelectAllOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var box = d as TextBox;

			if (box == null) return;

			var wasBound = (bool) (e.OldValue);
			var needToBind = (bool) (e.NewValue);

			if (wasBound)
				UnhookEvents(box);

			if (needToBind)
				HookEvents(box);
		}

		private static void HookEvents(TextBox box)
		{
			box.MouseDoubleClick += HandleGotFocus;
			box.GotKeyboardFocus += HandleGotFocus;
			box.PreviewMouseLeftButtonDown += SelectivelyIgnoreMouseButton;
		}

		private static void UnhookEvents(TextBox box)
		{
			box.MouseDoubleClick -= HandleGotFocus;
			box.GotKeyboardFocus -= HandleGotFocus;
			box.PreviewMouseLeftButtonDown -= SelectivelyIgnoreMouseButton;
		}

		private static void HandleGotFocus(object sender, RoutedEventArgs e)
		{
			var tb = (sender as TextBox);
			if (tb != null)
				tb.SelectAll();
		}

		private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
		{
			var tb = (sender as TextBox);

			if (tb != null)
			{
				if (!tb.IsKeyboardFocusWithin)
				{
					e.Handled = true;
					tb.Focus();
				}
			}
		}

		#endregion
	}
}
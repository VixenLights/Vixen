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

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
	/// <summary>
	/// Defines the search behavior of <see cref="SearchTextBox"/>.
	/// </summary>
	public enum SearchMode
	{
		/// <summary>
		/// Immediatelly fire search.
		/// </summary>
		Instant,

		/// <summary>
		/// Fire search with a delay.
		/// </summary>
		Delayed,
	}

	// http://davidowens.wordpress.com/2009/02/18/wpf-search-text-box/
	/// <summary>
	/// Property search box control.
	/// </summary>
	public class SearchTextBox : TextBox
	{
		private const string DefaultLabelText = "Search";

		/// <summary>
		/// Identifies the <see cref="LabelText"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LabelTextProperty =
			DependencyProperty.Register(
				"LabelText",
				typeof (string),
				typeof (SearchTextBox), new UIPropertyMetadata(DefaultLabelText, null));


		/// <summary>
		/// Identifies the <see cref="LabelTextColor"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty LabelTextColorProperty =
			DependencyProperty.Register(
				"LabelTextColor",
				typeof (Brush),
				typeof (SearchTextBox));

		/// <summary>
		/// Identifies the <see cref="SearchMode"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty SearchModeProperty =
			DependencyProperty.Register(
				"SearchMode",
				typeof (SearchMode),
				typeof (SearchTextBox),
				new PropertyMetadata(SearchMode.Instant));

		private static readonly DependencyPropertyKey HasTextPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"HasText",
				typeof (bool),
				typeof (SearchTextBox),
				new PropertyMetadata());

		/// <summary>
		/// Identifies the <see cref="HasText"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey IsMouseLeftButtonDownPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"IsMouseLeftButtonDown",
				typeof (bool),
				typeof (SearchTextBox),
				new PropertyMetadata());

		/// <summary>
		/// Identifies the <see cref="IsMouseLeftButtonDown"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsMouseLeftButtonDownProperty =
			IsMouseLeftButtonDownPropertyKey.DependencyProperty;

		/// <summary>
		/// Identifies the <see cref="SearchEventTimeDelay"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty SearchEventTimeDelayProperty =
			DependencyProperty.Register(
				"SearchEventTimeDelay",
				typeof (Duration),
				typeof (SearchTextBox),
				new FrameworkPropertyMetadata(
					new Duration(new TimeSpan(0, 0, 0, 0, 500)),
					new PropertyChangedCallback(OnSearchEventTimeDelayChanged)));

		/// <summary>
		/// Identifies the <see cref="Search"/> routed event.
		/// </summary>
		public static readonly RoutedEvent SearchEvent =
			EventManager.RegisterRoutedEvent(
				"Search",
				RoutingStrategy.Bubble,
				typeof (RoutedEventHandler),
				typeof (SearchTextBox));

		/// <summary>
		/// Initializes the <see cref="SearchTextBox"/> class.
		/// </summary>
		static SearchTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof (SearchTextBox),
				new FrameworkPropertyMetadata(typeof (SearchTextBox)));
		}

		private readonly DispatcherTimer _searchEventDelayTimer;

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchTextBox"/> class.
		/// </summary>
		public SearchTextBox()
		{
			_searchEventDelayTimer = new DispatcherTimer {Interval = SearchEventTimeDelay.TimeSpan};
			_searchEventDelayTimer.Tick += OnSeachEventDelayTimerTick;
		}

		private void OnSeachEventDelayTimerTick(object o, EventArgs e)
		{
			_searchEventDelayTimer.Stop();
			RaiseSearchEvent();
		}


		private static void OnSearchEventTimeDelayChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var stb = o as SearchTextBox;
			if (stb != null)
			{
				stb._searchEventDelayTimer.Interval = ((Duration) e.NewValue).TimeSpan;
				stb._searchEventDelayTimer.Stop();
			}
		}

		/// <summary>
		/// Is called when content in this editing control changes.
		/// </summary>
		/// <param name="e">The arguments that are associated with the <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged"/> event.</param>
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);

			HasText = Text.Length != 0;

			if (SearchMode == SearchMode.Instant)
			{
				_searchEventDelayTimer.Stop();
				_searchEventDelayTimer.Start();
			}
		}

		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var iconBorder = GetTemplateChild("PART_SearchIconBorder") as Border;
			if (iconBorder != null)
			{
				iconBorder.MouseLeftButtonDown += IconBorderMouseLeftButtonDown;
				iconBorder.MouseLeftButtonUp += IconBorderMouseLeftButtonUp;
				iconBorder.MouseLeave += IconBorderMouseLeave;
			}
		}

		private void IconBorderMouseLeftButtonDown(object obj, MouseButtonEventArgs e)
		{
			IsMouseLeftButtonDown = true;
		}

		private void IconBorderMouseLeftButtonUp(object obj, MouseButtonEventArgs e)
		{
			if (!IsMouseLeftButtonDown) return;

			if (HasText && SearchMode == SearchMode.Instant)
			{
				Text = "";
			}

			if (HasText && SearchMode == SearchMode.Delayed)
			{
				RaiseSearchEvent();
			}

			IsMouseLeftButtonDown = false;
		}

		private void IconBorderMouseLeave(object obj, MouseEventArgs e)
		{
			IsMouseLeftButtonDown = false;
		}

		/// <summary>
		/// Invoked whenever an unhandled <see cref="UIElement.KeyDown"/> attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
		/// </summary>
		/// <param name="e">Provides data about the event.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape && SearchMode == SearchMode.Instant)
			{
				Text = "";
			}
			else if ((e.Key == Key.Return || e.Key == Key.Enter) &&
			         SearchMode == SearchMode.Delayed)
			{
				RaiseSearchEvent();
			}
			else
			{
				base.OnKeyDown(e);
			}
		}

		private void RaiseSearchEvent()
		{
			var args = new RoutedEventArgs(SearchEvent);
			RaiseEvent(args);
		}

		/// <summary>
		/// Gets or sets the label text. This is a dependency property.
		/// </summary>
		/// <value>The label text.</value>
		public string LabelText
		{
			get { return (string) GetValue(LabelTextProperty); }
			set { SetValue(LabelTextProperty, value); }
		}

		/// <summary>
		/// Gets or sets the color of the label text. This is a dependency property.
		/// </summary>
		/// <value>The color of the label text.</value>
		public Brush LabelTextColor
		{
			get { return (Brush) GetValue(LabelTextColorProperty); }
			set { SetValue(LabelTextColorProperty, value); }
		}

		/// <summary>
		/// Gets or sets the search mode. This is a dependency property.
		/// </summary>
		/// <value>The search mode.</value>
		public SearchMode SearchMode
		{
			get { return (SearchMode) GetValue(SearchModeProperty); }
			set { SetValue(SearchModeProperty, value); }
		}

		/// <summary>
		/// Gets a value indicating whether this instance has text. This is a dependency property.
		/// </summary>
		/// <value><c>true</c> if this instance has text; otherwise, <c>false</c>.</value>
		public bool HasText
		{
			get { return (bool) GetValue(HasTextProperty); }
			private set { SetValue(HasTextPropertyKey, value); }
		}

		/// <summary>
		/// Gets or sets the search event time delay. This is a dependency property.
		/// </summary>
		/// <value>The search event time delay.</value>
		public Duration SearchEventTimeDelay
		{
			get { return (Duration) GetValue(SearchEventTimeDelayProperty); }
			set { SetValue(SearchEventTimeDelayProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether mouse left button is down.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if mouse left button is down; otherwise, <c>false</c>.
		/// </value>
		public bool IsMouseLeftButtonDown
		{
			get { return (bool) GetValue(IsMouseLeftButtonDownProperty); }
			private set { SetValue(IsMouseLeftButtonDownPropertyKey, value); }
		}

		/// <summary>
		/// Occurs when search is performed.
		/// </summary>
		public event RoutedEventHandler Search
		{
			add { AddHandler(SearchEvent, value); }
			remove { RemoveHandler(SearchEvent, value); }
		}

		/// <summary>
		/// Called when one or more of the dependency properties that exist on the element have had their effective values changed.
		/// </summary>
		/// <param name="e">Arguments for the associated event.</param>
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == VisibilityProperty)
				Text = string.Empty;

			base.OnPropertyChanged(e);
		}
	}
}
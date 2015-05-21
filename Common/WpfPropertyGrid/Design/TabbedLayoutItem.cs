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

namespace System.Windows.Controls.WpfPropertyGrid.Design
{
	/// <summary>
	/// Native Tab item for <see cref="TabbedLayout"/>
	/// </summary>
	public partial class TabbedLayoutItem : TabItem
	{
		/// <summary>
		/// Initializes the <see cref="TabbedLayoutItem"/> class.
		/// </summary>
		static TabbedLayoutItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (TabbedLayoutItem),
				new FrameworkPropertyMetadata(typeof (TabbedLayoutItem)));
		}

		/// <summary>
		/// Identifies the <see cref="CanClose"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty CanCloseProperty =
			DependencyProperty.Register("CanClose", typeof (bool), typeof (TabbedLayoutItem), new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a value indicating whether this instance can close.
		/// </summary>
		/// <value><c>true</c> if this instance can close; otherwise, <c>false</c>.</value>
		public bool CanClose
		{
			get { return (bool) GetValue(CanCloseProperty); }
			set { SetValue(CanCloseProperty, value); }
		}
	}
}
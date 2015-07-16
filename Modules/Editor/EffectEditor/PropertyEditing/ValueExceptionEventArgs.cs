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

using System;

namespace VixenModules.Editor.EffectEditor.PropertyEditing
{
	/// <summary>
	/// Contains event data related to value exceptions.
	/// </summary>
	public sealed class ValueExceptionEventArgs : EventArgs
	{
		private readonly Exception _exception;
		private readonly string _message;
		private readonly ValueExceptionSource _source;
		private readonly PropertyItemValue _value;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueExceptionEventArgs"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="value">The value.</param>
		/// <param name="source">The source.</param>
		/// <param name="exception">The exception.</param>
		public ValueExceptionEventArgs(string message, PropertyItemValue value, ValueExceptionSource source,
			Exception exception)
		{
			if (message == null) throw new ArgumentNullException("message");
			if (value == null) throw new ArgumentNullException("value");
			if (exception == null) throw new ArgumentNullException("exception");
			_message = message;
			_value = value;
			_source = source;
			_exception = exception;
		}

		/// <summary>
		/// Gets the exception.
		/// </summary>
		/// <value>The exception.</value>
		public Exception Exception
		{
			get { return _exception; }
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>The message.</value>
		public string Message
		{
			get { return _message; }
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <value>The property value.</value>
		public PropertyItemValue PropertyValue
		{
			get { return _value; }
		}

		/// <summary>
		/// Gets the source.
		/// </summary>
		/// <value>The source.</value>
		public ValueExceptionSource Source
		{
			get { return _source; }
		}
	}
}
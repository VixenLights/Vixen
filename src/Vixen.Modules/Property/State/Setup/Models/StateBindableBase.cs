using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VixenModules.Property.State.Setup.Models
{
	/// <summary>
	/// Provides property change notifications for State property setup models.
	/// </summary>
	public abstract class StateBindableBase : INotifyPropertyChanged
	{
		/// <inheritdoc />
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// Updates a backing field and raises <see cref="PropertyChanged"/> when its value changes.
		/// </summary>
		/// <typeparam name="T">The type of value stored by the backing field.</typeparam>
		/// <param name="field">The backing field to update.</param>
		/// <param name="value">The value to store in the backing field.</param>
		/// <param name="propertyName">The name of the property that changed.</param>
		/// <returns><see langword="true" /> if the value changed; otherwise, <see langword="false" />.</returns>
		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
			{
				return false;
			}

			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event.
		/// </summary>
		/// <param name="propertyName">The name of the property that changed.</param>
		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

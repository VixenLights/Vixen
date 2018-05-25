using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Vixen.Annotations;

namespace VixenModules.App.Marks
{
	// It is not ideal having multiple versions of this similar logic, but because the entire app is not using WPF, 
	// it is not great to have WPFCommon as a dependency in pure winforms projects. Sp for now we will use a copy of this here.
	[Serializable]
	public class BindableBase : INotifyPropertyChanged
	{
		protected virtual void SetProperty<T>(ref T member, T val,
			[CallerMemberName] string propertyName = null)
		{
			if (object.Equals(member, val)) return;

			member = val;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}


		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged = delegate { };
	}
}

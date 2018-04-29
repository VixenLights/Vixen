using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Vixen.Annotations;

namespace Common.WPFCommon.ViewModel
{
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
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

	    [field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}

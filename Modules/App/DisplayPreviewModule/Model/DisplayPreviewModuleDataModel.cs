namespace Vixen.Modules.DisplayPreviewModule.Model
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Media.Imaging;
    using Vixen.Module;

    [DataContract]
    [KnownType(typeof(BitmapImage))]
    public class DisplayPreviewModuleDataModel : ModuleDataModelBase
    {
        private ObservableCollection<DisplayItem> _displayItems;

        [DataMember]
        public string BackgroundImage { get; set; }

        [DataMember]
        public int DisplayHeight { get; set; }

        [DataMember]
        public ObservableCollection<DisplayItem> DisplayItems
        {
            get
            {
                return _displayItems ?? (_displayItems = new ObservableCollection<DisplayItem>());
            }

            set
            {
                _displayItems = value ?? new ObservableCollection<DisplayItem>();
            }
        }

        [DataMember]
        public int DisplayWidth { get; set; }

        [DataMember]
        public double Opactity { get; set; }

        public override IModuleDataModel Clone()
        {
            return new DisplayPreviewModuleDataModel
                   {
                       BackgroundImage = BackgroundImage, 
                       DisplayItems = new ObservableCollection<DisplayItem>(DisplayItems.Select(displayItem => displayItem.Clone()).ToList()), 
                       DisplayHeight = DisplayHeight, 
                       DisplayWidth = DisplayWidth, 
                       Opactity = Opactity
                   };
        }
    }
}

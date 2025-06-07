using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using NLog;
using Vixen.Sys.Props.Model;

namespace VixenApplication.SetupDisplay.ViewModels
{
	public class PropPreviewViewModel : ViewModelBase
	{
        private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		public PropPreviewViewModel(/* dependency injection here */)
		{
		}

		public override string Title { get { return "Prop Preview"; } }

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		protected override async Task InitializeAsync()
		{
			await base.InitializeAsync();

			// TODO: subscribe to events here
		}

		protected override async Task CloseAsync()
		{
			// TODO: unsubscribe from events here

			await base.CloseAsync();
		}

        

        /// <summary>
        /// Gets or sets the PropModel value.
        /// </summary>;
        [Model]
        public ILightPropModel PropModel
        {
            get { return GetValue<ILightPropModel>(PropModelProperty); }
            private set { SetValue(PropModelProperty, value); }
        }

        /// <summary>;
        /// PropModel property data.
        /// </summary>;
        public static readonly IPropertyData PropModelProperty = RegisterProperty<ILightPropModel>(nameof(PropModel));

        #region NodePoints property

        /// <summary>
        /// Gets or sets the NodePoints value.
        /// </summary>
        [ViewModelToModel("PropModel")]
        public ObservableCollection<NodePoint> NodePoints
        {
            get { return GetValue<ObservableCollection<NodePoint>>(NodePointsProperty); }
            set { SetValue(NodePointsProperty, value); }
        }

        /// <summary>
        /// NodePoints property data.
        /// </summary>
        public static readonly IPropertyData NodePointsProperty = RegisterProperty<ObservableCollection<NodePoint>>(nameof(NodePoints));

        #endregion
	}
}

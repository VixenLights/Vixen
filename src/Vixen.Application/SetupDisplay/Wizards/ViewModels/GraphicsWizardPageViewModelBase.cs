using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Vixen.Sys.Props.Model;
using VixenApplication.SetupDisplay.OpenGL;
using VixenApplication.SetupDisplay.ViewModels;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
    /// <summary>
    /// Base class for a wizard page that also displays 3-D prop graphics.
    /// </summary>
    /// <typeparam name="TWizardPage">Type of wizard page</typeparam>
    /// <typeparam name="TPropModel">Type of prop model</typeparam>
    public class GraphicsWizardPageViewModelBase<TWizardPage, TPropModel> : WizardPageViewModelBase<TWizardPage>
        where TWizardPage : class, IWizardPage
        where TPropModel : class, ILightPropModel, new()
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wizardPage">Wizard page model</param>
        protected GraphicsWizardPageViewModelBase(TWizardPage wizardPage) : base(wizardPage)
        {
            LightPropModel = new TPropModel();
            List<IPropModel> propModels = new List<IPropModel>();
            propModels.Add(LightPropModel);

			// Create the prop drawing engine
			DrawingEngine = new OpenGLPropDrawingEngine(propModels, 1, 100.0f);

			AttachRotationHandlers(Rotations);
        }
        #endregion

        #region Protected Properties
        public TPropModel LightPropModel
        {
            get { return GetValue<TPropModel>(LightPropModelProperty); }
            set { SetValue(LightPropModelProperty, value); }
        }
        private static readonly IPropertyData LightPropModelProperty = RegisterProperty<TPropModel>(nameof(LightPropModel));
        #endregion

        #region Public Properties
        /// <summary>
        /// Collection of rotations to support rotating the props around the x,y, and z axis.
        /// </summary>
        [ViewModelToModel]
        public ObservableCollection<AxisRotationViewModel> Rotations
        {
            get { return GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty); }
            set
            {
				// If the current and new collection are the same reference, do nothing. Otherwise, detach handlers from the current collection,
				// set the new collection, and attach handlers to the new collection.
				var _currentRotations = GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty);
	            if (!ReferenceEquals(_currentRotations, value))
	            {
		            DetachRotationHandlers(_currentRotations);
		            SetValue(RotationsProperty, value);
		            AttachRotationHandlers(value);
	            }

	            var _rotations = GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty);
	            if (_rotations != null)
	            {
		            for (int index = 0; index < _rotations.Count; index++)
		            {
			            _rotations[index].RotationAngleDefault = _rotations[index].RotationAngle;
		            }
	            }
            }
        }
		private static readonly IPropertyData RotationsProperty = RegisterProperty<ObservableCollection<AxisRotationViewModel>>(nameof(Rotations));

		/// <summary>
		/// OpenGL prop drawing engine.
		/// </summary>
		public OpenGLPropDrawingEngine DrawingEngine { get; set; } = new OpenGLPropDrawingEngine(new List<IPropModel>(), 1, 100f);

		#endregion

		#region Private Methods
		/// <summary>
		/// Add Rotation Handlers to the collection and each item in the collection. This ensures that when a rotation is changed, 
		/// the prop nodes will update accordingly. It also ensures that when the collection changes (e.g. a new rotation is added), 
		/// the new item will have handlers attached.
		/// </summary>
		/// <param name="rotationCollection">Specifies the collection of rotation view models to attach handlers.</param>
		private void AttachRotationHandlers(ObservableCollection<AxisRotationViewModel>? rotationCollection)
		{
			if (rotationCollection == null)
			{
				return;
			}

			// Ensure collection changed handler is attached only once
			rotationCollection.CollectionChanged -= OnRotationsCollectionChanged;
			rotationCollection.CollectionChanged += OnRotationsCollectionChanged;

			foreach (var rotation in rotationCollection)
			{
				// Ensure rotation changed handler is attached only once
				rotation.RotationChanged -= OnRotationChanged;
				rotation.RotationChanged += OnRotationChanged;
			}
		}

		/// <summary>
		/// Detaches event handlers related to rotation changes from the specified collection and its items.
		/// </summary>
		/// <param name="rotationCollection">Specifies the collection of rotation view models from which to remove event handlers.</param>
		private void DetachRotationHandlers(ObservableCollection<AxisRotationViewModel>? rotationCollection)
		{
			if (rotationCollection == null)
			{
				return;
			}

			rotationCollection.CollectionChanged -= OnRotationsCollectionChanged;

			foreach (var rotation in rotationCollection)
			{
				rotation.RotationChanged -= OnRotationChanged;
			}
		}

		/// <summary>
		/// Event handler for when a collection of rotation view models changed.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OnRotationsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e == null)
			{
				return;
			}

			if (e.OldItems != null)
			{
				foreach (AxisRotationViewModel oldItem in e.OldItems)
				{
					oldItem.RotationChanged -= OnRotationChanged;
				}
			}

			if (e.NewItems != null)
			{
				foreach (AxisRotationViewModel newItem in e.NewItems)
				{
					// Ensure rotation changed handler is attached only once
					newItem.RotationChanged -= OnRotationChanged;
					newItem.RotationChanged += OnRotationChanged;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Reset && sender is ObservableCollection<AxisRotationViewModel> rotationCollection)
			{
				// Reattach for the new state of the collection
				DetachRotationHandlers(rotationCollection);
				AttachRotationHandlers(rotationCollection);
			}
		}

		/// <summary>
		/// Event handler for when a prop rotation changed.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OnRotationChanged(object? sender, EventArgs e)
        {
            // Is there a changed axis?
            var newRotation = sender as AxisRotationViewModel;
            if (newRotation == null)
            {
                return;
            }

            // Then try to find the axis it now duplicates
            var duplicateRotation = Rotations.FirstOrDefault(x => x != newRotation && x.Axis == newRotation.Axis);

            // If there is an axis duplication (i.e. two axis have the same plane), then...
            if (duplicateRotation != null)
            {
                // Find the Axis that is no longer specified
                var otherRotation = Rotations.FirstOrDefault(x => x != newRotation && x != duplicateRotation);
                if (otherRotation == null)
                {
                    return;
                }

                var missingAxis = newRotation.Axes.FirstOrDefault(x => x != duplicateRotation.Axis && x != otherRotation.Axis);

                // Finally assign the missing axis to the duplicated plane and swap the rotations between the new and duplicated axes
                duplicateRotation.Axis = missingAxis;
                (duplicateRotation.RotationAngle, newRotation.RotationAngle) = (newRotation.RotationAngle, duplicateRotation.RotationAngle);
            }

            // Set the updated parameters
            LightPropModel.AxisRotations = AxisRotationViewModel.ConvertToModel(Rotations);           
        }
        #endregion

        protected override async Task InitializeAsync()
        {
            // Set the updated parameters
            LightPropModel.AxisRotations = AxisRotationViewModel.ConvertToModel(Rotations);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catel.Collections;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Common.WPFCommon.Command;
using Common.WPFCommon.Services;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class DrawingPanelViewModel : ViewModelBase
	{
		private readonly ElementTreeViewModel _elementTreeViewModel;
		private readonly Dictionary<Guid, List<LightViewModel>> _elementModelMap;
		
		public DrawingPanelViewModel(ElementTreeViewModel elementTreeViewModel)
		{
			_elementTreeViewModel = elementTreeViewModel;
			_elementModelMap = new Dictionary<Guid, List<LightViewModel>>();
			LightNodes = new ObservableCollection<LightViewModel>();

			TransformCommand = new RelayCommand<Transform>(Transform);

			AlignTopsCommand = new RelayCommand(AlignTops, CanExecuteAlignmentMethod);
			AlignBottomsCommand = new RelayCommand(AlignBottoms, CanExecuteAlignmentMethod);
			AlignLeftCommand = new RelayCommand(AlignLeft, CanExecuteAlignmentMethod);
			AlignRightCommand = new RelayCommand(AlignRight, CanExecuteAlignmentMethod);
			DistributeHorizontallyCommand = new RelayCommand(DistributeHorizontally, CanExecuteAlignmentMethod);
			DistributeVerticallyCommand = new RelayCommand(DistributeVertically, CanExecuteAlignmentMethod);
			FlipHorizontalCommand = new RelayCommand(FlipHorizontal, CanExecuteAlignmentMethod);
			FlipVerticalCommand = new RelayCommand(FlipVertical, CanExecuteAlignmentMethod);

			DeleteSelectedLightsCommand = new RelayCommand(DeleteSelectedLights);
			Configuration = ConfigurationService.Instance().Config;
			SelectedItems = new FastObservableCollection<LightViewModel>();
			SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
			IsDrawing = true;
			Prop = elementTreeViewModel.Prop;
		}

		private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			AlignLeftCommand.RaiseCanExecuteChanged();
			AlignRightCommand.RaiseCanExecuteChanged();
			AlignBottomsCommand.RaiseCanExecuteChanged();
			AlignTopsCommand.RaiseCanExecuteChanged();
			DistributeVerticallyCommand.RaiseCanExecuteChanged();
			DistributeHorizontallyCommand.RaiseCanExecuteChanged();
			FlipHorizontalCommand.RaiseCanExecuteChanged();
			FlipVerticalCommand.RaiseCanExecuteChanged();

			DecreaseLightSizeCommand.RaiseCanExecuteChanged();
			IncreaseLightSizeCommand.RaiseCanExecuteChanged();
			MatchLightSizeCommand.RaiseCanExecuteChanged();

			DeleteSelectedLightsCommand.RaiseCanExecuteChanged();
		}

		private bool CanExecuteAlignmentMethod()
		{
			return SelectedItems.Any();
		}


		#region Properties

		#region Prop model property

		/// <summary>
		/// Gets or sets the Prop value.
		/// </summary>
		[Model]
		public Prop Prop
		{
			get { return GetValue<Prop>(PropProperty); }
			private set
			{
				SetValue(PropProperty, value);

				RefreshLightViewModels();
			}
		}

		/// <summary>
		/// Prop property data.
		/// </summary>
		public static readonly PropertyData PropProperty = RegisterProperty("Prop", typeof(Prop));

		#endregion

		public FastObservableCollection<LightViewModel> SelectedItems { get; set; }

		#region LightNodes property

		/// <summary>
		/// Gets or sets the LightNodes value.
		/// </summary>
		public ObservableCollection<LightViewModel> LightNodes
		{
			get { return GetValue<ObservableCollection<LightViewModel>>(LightNodesProperty); }
			set { SetValue(LightNodesProperty, value); }
		}

		/// <summary>
		/// LightNodes property data.
		/// </summary>
		public static readonly PropertyData LightNodesProperty = RegisterProperty("LightNodes", typeof(ObservableCollection<LightViewModel>));

		#endregion

		#region Width property

		/// <summary>
		/// Gets or sets the Width value.
		/// </summary>
		[ViewModelToModel("Prop")]
		public double Width
		{
			get { return GetValue<double>(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		/// <summary>
		/// Width property data.
		/// </summary>
		public static readonly PropertyData WidthProperty = RegisterProperty("Width", typeof(double), null);

		#endregion

		#region Height property

		/// <summary>
		/// Gets or sets the Height value.
		/// </summary>
		[ViewModelToModel("Prop")]
		public double Height
		{
			get { return GetValue<double>(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}

		/// <summary>
		/// Height property data.
		/// </summary>
		public static readonly PropertyData HeightProperty = RegisterProperty("Height", typeof(double), null);

		#endregion

		#region Image property

		/// <summary>
		/// Gets or sets the Image value.
		/// </summary>
		[ViewModelToModel("Prop")]
		public BitmapSource Image
		{
			get { return GetValue<BitmapSource>(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		/// <summary>
		/// Image property data.
		/// </summary>
		public static readonly PropertyData ImageProperty = RegisterProperty("Image", typeof(BitmapSource), null);

		#endregion

		#region Opacity property

		/// <summary>
		/// Gets or sets the Opacity value.
		/// </summary>
		[ViewModelToModel("Prop")]
		public double Opacity
		{
			get { return GetValue<double>(OpacityProperty); }
			set { SetValue(OpacityProperty, value); }
		}

		/// <summary>
		/// Opacity property data.
		/// </summary>
		public static readonly PropertyData OpacityProperty = RegisterProperty("Opacity", typeof(double), null);

		#endregion

		#region IsDrawing property

		/// <summary>
		/// Gets or sets the IsDrawing value.
		/// </summary>
		public bool IsDrawing
		{
			get { return GetValue<bool>(IsDrawingProperty); }
			set { SetValue(IsDrawingProperty, value); }
		}

		/// <summary>
		/// IsDrawing property data.
		/// </summary>
		public static readonly PropertyData IsDrawingProperty = RegisterProperty("IsDrawing", typeof(bool));

		#endregion

		#region Confguration model property

		/// <summary>
		/// Gets or sets the Confguration value.
		/// </summary>
		[Model]
		public Configuration Configuration
		{
			get { return GetValue<Configuration>(ConfgurationProperty); }
			private set { SetValue(ConfgurationProperty, value); }
		}

		/// <summary>
		/// Confguration property data.
		/// </summary>
		public static readonly PropertyData ConfgurationProperty = RegisterProperty("Configuration", typeof(Configuration));

		#endregion

		#region LightColor property

		/// <summary>
		/// Gets or sets the LightColor value.
		/// </summary>
		[ViewModelToModel("Configuration")]
		public Color LightColor
		{
			get { return GetValue<Color>(LightColorProperty); }
			//set { SetValue(LightColorProperty, value); }
		}

		/// <summary>
		/// LightColor property data.
		/// </summary>
		public static readonly PropertyData LightColorProperty = RegisterProperty("LightColor", typeof(Color), null);

		#endregion

		#region SelectedLightColor property

		/// <summary>
		/// Gets or sets the SelectedLightColor value.
		/// </summary>
		[ViewModelToModel("Configuration")]
		public Color SelectedLightColor
		{
			get { return GetValue<Color>(SelectedLightColorProperty); }
			//set { SetValue(SelectedLightColorProperty, value); }
		}

		/// <summary>
		/// SelectedLightColor property data.
		/// </summary>
		public static readonly PropertyData SelectedLightColorProperty = RegisterProperty("SelectedLightColor", typeof(Color), null);

		#endregion

		#endregion Properties

		public bool IsLightsDirty
		{
			get
			{
				return LightNodes.Any(x => x.IsDirty);
			}
		}

		public void ClearIsDirty()
		{
			this.ClearIsDirtyOnAllChildren();
			//RootNodesViewModels.ForEach(x => x.ClearIsDirtyOnAllChilds());
		}
		
		internal void RefreshLightViewModels()
		{
			_elementModelMap.Clear();
			LightNodes.Clear();
			SelectedItems.Clear();
			foreach (var elementModel in PropModelServices.Instance().GetLeafNodes())
			{
				//See if we already have a lvm list for this element.
				if (!_elementModelMap.ContainsKey(elementModel.Id))
				{
					List<LightViewModel> lightViewModels = new List<LightViewModel>();
					//Build the lvm for these lights
					foreach (var elementModelLight in elementModel.Lights)
					{
						var lvm = new LightViewModel(elementModelLight);
						lightViewModels.Add(lvm);
					}
					_elementModelMap.Add(elementModel.Id, lightViewModels);
				}
			}

			LightNodes.AddRange(_elementModelMap.Values.SelectMany(x => x.ToArray()));
		}

		public void DeleteSelectedLights()
		{
			var lightstoDelete = SelectedItems.Select(l => l.Light).ToList();
			if (lightstoDelete.Any())
			{
				var dependencyResolver = this.GetDependencyResolver();
				var mbs = dependencyResolver.Resolve<IMessageBoxService>();
				var response = mbs.GetUserConfirmation(
					"Deleting lights will remove them from all any/all groups they are part of. Are you sure?",
					"Delete lights");

				if (response.Result == MessageResult.OK)
				{
					DeselectAll();
					PropModelServices.Instance().RemoveLights(lightstoDelete);
					RefreshLightViewModels();
					OnLightModelsChanged();
				}
				
			}
		   
		}

		public void DeselectAll()
		{
			LightNodes.ForEach(l => l.IsSelected = false);
			SelectedItems.Clear();
		}

		public void Deselect(IEnumerable<ElementModelViewModel> elementModels)
		{
			List<LightViewModel> selectedModels = new List<LightViewModel>();
			foreach (var elementModel in elementModels)
			{
				List<LightViewModel> lvmList;
				if (_elementModelMap.TryGetValue(elementModel.ElementModel.Id, out lvmList))
				{
					foreach (var l in lvmList)
					{
						l.IsSelected = false;
						selectedModels.Add(l);
					}
				}
			}
			SelectedItems.RemoveItems(selectedModels);
		}

		public void Select(IEnumerable<ElementModelViewModel> elementModels)
		{
			List<LightViewModel> selectedModels = new List<LightViewModel>();
			foreach (var elementModel in elementModels)
			{
				List<LightViewModel> lvmList;
				if (_elementModelMap.TryGetValue(elementModel.ElementModel.Id, out lvmList))
				{
					foreach (var lightViewModel in lvmList)
					{
						if (!lightViewModel.IsSelected)
						{
							lightViewModel.IsSelected = true;
							selectedModels.Add(lightViewModel);
						}
					}
				}

			}
			SelectedItems.AddItems(selectedModels);
		}

		public void Transform(Transform t)
		{
			SelectedItems.AsParallel().ForEach(x => x.Center = t.Transform(x.Center));
		}

		public void AlignTops()
		{
			var ln = SelectedItems.First();
			foreach (var lightViewModel in SelectedItems)
			{
				lightViewModel.Light.Y = ln.Y;
			}
		}

		public void AlignBottoms()
		{
			var ln = SelectedItems.First();
			foreach (var lightViewModel in SelectedItems)
			{
				lightViewModel.Light.Y = ln.Y;
			}
		}

		public void AlignLeft()
		{
			var ln = SelectedItems.First();
			foreach (var lightViewModel in SelectedItems)
			{
				lightViewModel.Light.X = ln.X;
			}
		}

		public void AlignRight()
		{
			var ln = SelectedItems.First();
			foreach (var lightViewModel in SelectedItems)
			{
				lightViewModel.Light.X = ln.X;
			}
		}

		public void FlipHorizontal()
		{
			// Find min and max X position
			var max = SelectedItems.Max(element => Math.Abs(element.X));
			var min = SelectedItems.Min(element => Math.Abs(element.X));
			foreach (var lightViewModel in SelectedItems)
			{
				lightViewModel.Light.X = max - (lightViewModel.Light.X - min);
			}
		}

		public void FlipVertical()
		{
			// Find min and max Y position
			var max = SelectedItems.Max(element => Math.Abs(element.Y));
			var min = SelectedItems.Min(element => Math.Abs(element.Y));
			foreach (var lightViewModel in SelectedItems)
			{
				lightViewModel.Light.Y = max - (lightViewModel.Light.Y - min);
			}
		}

		public void DistributeHorizontally()
		{
			if (SelectedItems.Count > 2)
			{
				var minX = SelectedItems.Min(x => x.Light.X);
				var maxX = SelectedItems.Max(x => x.Light.X);
				var count = SelectedItems.Count - 1;

				var dist = (maxX - minX) / count;

				int y = 0;
				double holdValue = minX;
				foreach (var lightViewModel in SelectedItems.OrderBy(x => x.X))
				{
					if (y != 0)
					{
						holdValue += dist;
						lightViewModel.X = holdValue;
					}

					y++;
				}

			}

		}

		public void DistributeVertically()
		{
			if (SelectedItems.Count > 2)
			{
				var minY = SelectedItems.Min(x => x.Light.Y);
				var maxY = SelectedItems.Max(x => x.Light.Y);
				var count = SelectedItems.Count - 1;

				var dist = (maxY - minY) / count;

				int y = 0;
				double holdValue = minY;
				foreach (var lightViewModel in SelectedItems.OrderBy(x => x.Y))
				{
					if (y != 0)
					{
						holdValue += dist;
						lightViewModel.Y = holdValue;
					}

					y++;
				}

			}
		}


		#region Commands



		public RelayCommand<Transform> TransformCommand { get; private set; }

		#region Alignment Commands

		public RelayCommand AlignLeftCommand { get; private set; }
		public RelayCommand AlignRightCommand { get; private set; }
		public RelayCommand AlignTopsCommand { get; private set; }
		public RelayCommand AlignBottomsCommand { get; private set; }
		public RelayCommand DistributeHorizontallyCommand { get; private set; }
		public RelayCommand DistributeVerticallyCommand { get; private set; }
		public RelayCommand FlipHorizontalCommand { get; private set; }
		public RelayCommand FlipVerticalCommand { get; private set; }


		#endregion

		#region Delete Command

		public RelayCommand DeleteSelectedLightsCommand { get; private set; }

		#endregion

		#region IncreaseLightSize command

		private Command _increaseLightSizeCommand;

		/// <summary>
		/// Gets the IncreaseLightSize command.
		/// </summary>
		public Command IncreaseLightSizeCommand
		{
			get { return _increaseLightSizeCommand ?? (_increaseLightSizeCommand = new Command(IncreaseLightSize, CanIncreaseLightSize)); }
		}

		/// <summary>
		/// Method to invoke when the IncreaseLightSize command is executed.
		/// </summary>
		private void IncreaseLightSize()
		{
			foreach (var lightViewModel in SelectedItems)
			{
				var em = PropModelServices.Instance().GetElementModel(lightViewModel.Light.ParentModelId);
				if (em != null)
				{
					em.LightSize++;
				}

			}
		}

		/// <summary>
		/// Method to check whether the IncreaseLightSize command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanIncreaseLightSize()
		{
			return SelectedItems.Any();
		}

		#endregion

		#region DecreaseLightSize command

		private Command _decreaseLightSizeCommand;

		/// <summary>
		/// Gets the DecreaseLightSize command.
		/// </summary>
		public Command DecreaseLightSizeCommand
		{
			get { return _decreaseLightSizeCommand ?? (_decreaseLightSizeCommand = new Command(DecreaseLightSize, CanDecreaseLightSize)); }
		}

		/// <summary>
		/// Method to invoke when the DecreaseLightSize command is executed.
		/// </summary>
		private void DecreaseLightSize()
		{
			foreach (var lightViewModel in SelectedItems)
			{
				var em = PropModelServices.Instance().GetElementModel(lightViewModel.Light.ParentModelId);
				if (em != null)
				{
					em.LightSize--;
				}
			}
		}

		/// <summary>
		/// Method to check whether the DecreaseLightSize command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanDecreaseLightSize()
		{
			return SelectedItems.Any();
		}

		#endregion

		#region MatchLightSize command

		private Command _matchLightSizeCommand;

		/// <summary>
		/// Gets the MatchLightSize command.
		/// </summary>
		public Command MatchLightSizeCommand
		{
			get { return _matchLightSizeCommand ?? (_matchLightSizeCommand = new Command(MatchLightSize, CanMatchLightSize)); }
		}

		/// <summary>
		/// Method to invoke when the MatchLightSize command is executed.
		/// </summary>
		private void MatchLightSize()
		{
			if (SelectedItems.Count < 2) return;
			var lvmReference = SelectedItems.First();
			var emReference = PropModelServices.Instance().GetElementModel(lvmReference.Light.ParentModelId);

			foreach (var lightViewModel in SelectedItems.Skip(1))
			{
				var em = PropModelServices.Instance().GetElementModel(lightViewModel.Light.ParentModelId);
				if (em != null)
				{
					em.LightSize = emReference.LightSize;
				}

			}
		}

		/// <summary>
		/// Method to check whether the MatchLightSize command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanMatchLightSize()
		{
			return SelectedItems.Count>1;
		}

		#endregion

		#endregion
		
		public event EventHandler LightModelsChanged;

		private void OnLightModelsChanged()
		{
			if (LightModelsChanged != null)
				LightModelsChanged(this, EventArgs.Empty);
		}

	}
}


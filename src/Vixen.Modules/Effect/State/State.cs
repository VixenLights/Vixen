using System.ComponentModel;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.State
{
	public sealed class State: BaseEffect
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private StateData _data;
		private EffectIntents _elementData = new();

		public State()
		{
			_data = new StateData();
		}

		#region Overrides of EffectModuleInstanceBase

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		protected override void _PreRender(CancellationTokenSource? cancellationToken = null)
		{
			_elementData = new EffectIntents();
			RenderNodes();
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		#endregion

		#region Overrides of BaseEffect

		protected override EffectTypeModuleData EffectModuleData => _data;

		#endregion

		#region Properties

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"MarkCollection")]
		[ProviderDescription(@"MarkCollection")]
		[TypeConverter(typeof(IMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
		public string MarkCollectionId
		{
			get
			{
				var c = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
				return c?.Name ?? String.Empty;
			}
			set
			{
				var newMarkCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(value));
				var id = newMarkCollection?.Id ?? Guid.Empty;
				if (!id.Equals(_data.MarkCollectionId))
				{
					var oldMarkCollection = MarkCollections.FirstOrDefault(x => x.Id.Equals(_data.MarkCollectionId));
					RemoveMarkCollectionListeners(oldMarkCollection);
					_data.MarkCollectionId = id;
					AddMarkCollectionListeners(newMarkCollection);
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}
		
		[Value]
		[ProviderCategory("Config", 2)]
		[ProviderDisplayName(@"TimingMode")]
		[ProviderDescription(@"TimingMode")]
		[PropertyOrder(1)]
		public TimingMode TimingMode
		{
			get
			{
				return _data.TimingMode;

			}
			set
			{
				if (_data.TimingMode != value)
				{
					_data.TimingMode = value;
					SetTimingModeBrowsables();
					IsDirty = true;
					OnPropertyChanged();
					if (_data.TimingMode == TimingMode.MarkCollection && _data.MarkCollectionId == Guid.Empty)
					{
						if (MarkCollections.Any())
						{
							var mc = MarkCollections.FirstOrDefault(x => x.CollectionType == MarkCollectionType.Phoneme);
							if (mc != null)
							{
								MarkCollectionId = mc.Name;
							}
						}
					}
				}
			}
		}
		
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"AllowMarkGaps")]
		[ProviderDescription(@"AllowMarkGaps")]
		[PropertyOrder(3)]
		public bool AllowMarkGaps
		{
			get { return _data.AllowMarkGaps; }
			set
			{
				_data.AllowMarkGaps = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion
		
		private void CheckForInvalidColorData()
		{
			// initialize the color handling
			GetValidColors();
		}

		private void RenderNodes()
		{
			//Find the state property in the TargetNodes
			//Gather the elements for the State(s) we are rendering. 
			//Do we have discrete elements? If so check target color vs supported colors.
			//Render Set Level effects on the elements we are rendering in the required colors.
		}
		
		#region Overrides of EffectModuleInstanceBase

		/// <inheritdoc />
		protected override void MarkCollectionsChanged()
		{
			if (TimingMode == TimingMode.MarkCollection)
			{
				var markCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(MarkCollectionId));
				InitializeMarkCollectionListeners(markCollection);
			}
		}

		/// <inheritdoc />
		protected override void MarkCollectionsRemoved(IList<IMarkCollection> addedCollections)
		{
			var mc = addedCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			if(mc != null)
			{
				//Our collection is gone!!!!
				RemoveMarkCollectionListeners(mc);
				MarkCollectionId = String.Empty;
			}
		}

		#endregion

		#region Browsables

		private void SetTimingModeBrowsables()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{nameof(MarkCollectionId), TimingMode == TimingMode.MarkCollection},
				{nameof(AllowMarkGaps), TimingMode == TimingMode.MarkCollection}
			};

			SetBrowsable(propertyStates);
			TypeDescriptor.Refresh(this);
		}

		#endregion
		
		public override IModuleDataModel? ModuleData
		{
			get { return _data; }
			set
			{
				if (value != null)
				{
					_data = (StateData)value;
					CheckForInvalidColorData();
					SetTimingModeBrowsables();
					IsDirty = true;
				}
				
			}
		}
	}
}
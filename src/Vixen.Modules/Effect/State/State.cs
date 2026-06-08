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
		
		/// <summary>
		/// Gets or sets the source used to determine active State items.
		/// </summary>
		/// <value>One of the enumeration values that specifies the State item render source.</value>
		[Value]
		[ProviderCategory("Config", 2)]
		[ProviderDisplayName(@"Render Source")]
		[ProviderDescription(@"Selects how active State items are determined.")]
		[PropertyOrder(1)]
		public StateRenderSource RenderSource
		{
			get
			{
				return _data.RenderSource;

			}
			set
			{
				if (_data.RenderSource != value)
				{
					_data.RenderSource = value;
					SetRenderSourceBrowsables();
					IsDirty = true;
					OnPropertyChanged();
					if (_data.RenderSource == StateRenderSource.MarkCollection && _data.MarkCollectionId == Guid.Empty)
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
			if (RenderSource == StateRenderSource.MarkCollection)
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

		private void SetRenderSourceBrowsables()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{nameof(MarkCollectionId), RenderSource == StateRenderSource.MarkCollection}
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
					SetRenderSourceBrowsables();
					IsDirty = true;
				}
				
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using Common.Resources.Properties;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.Curves;
using VixenModules.App.LipSyncApp;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Effect.Effect;
using VixenModules.Property.Face;
using ZedGraph;

namespace VixenModules.Effect.LipSync
{

	public class LipSync : BaseEffect
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private LipSyncData _data;
		private EffectIntents _elementData = null;
		private static Dictionary<PhonemeType, Bitmap> _phonemeBitmaps = null;
		private readonly LipSyncMapLibrary _library = null;
		private List<IMark> _marks = null;
		private FastPictureEffect _thePic;
		
		static LipSync()
		{
			LoadResourceBitmaps();
		}

		public LipSync()
		{
			_data = new LipSyncData();
			_library = ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary;
		}

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
			
			SetOrientation();
		}

		private void CheckForInvalidColorData()
		{
			// initialize the color handling
			GetValidColors();
		}

		protected void SetOrientation()
		{
			var orientation = GetOrientation();
			if (orientation.Item1)
			{
				Orientation = orientation.Item2;
			}
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_elementData = new EffectIntents();
			RenderNodes();
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNodes()
		{
			var renderNodes = TargetNodes.SelectMany(x => x.GetNodeEnumerator());
			if (LipSyncMode == LipSyncMode.MarkCollection)
			{
				SetupMarks();
			}

			PhonemeType phoneme = _data.StaticPhoneme;

			if (MappingType == MappingType.Map)
			{

				if (!_library.Library.ContainsKey(_data.PhonemeMapping))
				{
					_data.PhonemeMapping = _library.DefaultMappingName;
				}

				LipSyncMapData mapData;
				if (_library.Library.TryGetValue(_data.PhonemeMapping, out mapData))
				{
					if (mapData.IsMatrix)
					{
						RenderMapMatrix(mapData, phoneme);
					}
					else
					{
						//We should never get here becasue we no longer have string maps
						Logging.Error("Trying to render as deprecated string maps!");
						foreach (var element in renderNodes)
						{
							RenderMapElements(mapData, element, phoneme);
						}
					}
				}
			}
			else
			{
				foreach (var element in renderNodes)
				{
					RenderPropertyMapElements(element, phoneme);
				}
			}
		}

		private void RenderPropertyMapElements(IElementNode element, PhonemeType phoneme)
		{
			var fm = element.Properties.Get(FaceDescriptor.ModuleId) as FaceModule;
			if (fm == null) return;

			if (fm.IsFaceComponentType(FaceComponent.Outlines) && ShowOutline)
			{
				var colorVal = fm.ConfiguredColorAndIntensity();
				var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, TimeSpan);
				_elementData.Add(result);
			}
			else if (fm.IsFaceComponentType(FaceComponent.EyesOpen) && EyeMode == EyeMode.Open)
			{
				var colorVal = fm.ConfiguredColorAndIntensity();
				var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, TimeSpan);
				_elementData.Add(result);
			}
			if (fm.IsFaceComponentType(FaceComponent.EyesClosed) && EyeMode == EyeMode.Closed)
			{
				var colorVal = fm.ConfiguredColorAndIntensity();
				var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, TimeSpan);
				_elementData.Add(result);
			}
			else
			{
				if (LipSyncMode == LipSyncMode.MarkCollection && _marks != null)
				{
					TimeSpan lastMarkTime = StartTime;
					foreach (var mark in _marks)
					{
						if (!AllowMarkGaps)
						{
							var gapDuration = mark.StartTime - lastMarkTime;
							if (gapDuration.TotalMilliseconds > 10 && fm.PhonemeState("REST"))
							{
								//Fill the gap with a rest
								var colorVal = fm.ConfiguredColorAndIntensity();
								var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, gapDuration);
								result.OffsetAllCommandsByTime(lastMarkTime - StartTime);
								_elementData.Add(result);
							}

							lastMarkTime = mark.EndTime;
						}
						if (fm.PhonemeState(mark.Text))
						{
							var colorVal = fm.ConfiguredColorAndIntensity();
							var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, mark.Duration);
							result.OffsetAllCommandsByTime(mark.StartTime - StartTime);
							_elementData.Add(result);
						}
					}

					if (!AllowMarkGaps)
					{
						var gapDuration = StartTime + TimeSpan - lastMarkTime;
						if (gapDuration.TotalMilliseconds > 10 && fm.PhonemeState("REST"))
						{
							//Fill the gap with a rest
							var colorVal = fm.ConfiguredColorAndIntensity();
							var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, gapDuration);
							result.OffsetAllCommandsByTime(lastMarkTime - StartTime);
							_elementData.Add(result);
						}
					}
				}
				else
				{
					if (fm.PhonemeState(phoneme.ToString()))
					{
						var colorVal = fm.ConfiguredColorAndIntensity();
						var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, TimeSpan);
						_elementData.Add(result);
					}
				}
			}
		}

		private void RenderMapElements(LipSyncMapData mapData, IElementNode element, PhonemeType phoneme)
		{
			LipSyncMapItem item = mapData.FindMapItem(element.Id);
			if (item == null) return;

			if (mapData.IsFaceComponentType(FaceComponent.Outlines, item))
			{
				var colorVal = mapData.ConfiguredColorAndIntensity(item);
				var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, TimeSpan);
				_elementData.Add(result);
			}
			else if (mapData.IsFaceComponentType(FaceComponent.EyesOpen, item))
			{
				var colorVal = mapData.ConfiguredColorAndIntensity(item);
				var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, TimeSpan);
				_elementData.Add(result);
			}
			else
			{
				if (LipSyncMode == LipSyncMode.MarkCollection && _marks != null)
				{
					TimeSpan lastMarkTime = StartTime;
					foreach (var mark in _marks)
					{
						if (!AllowMarkGaps)
						{
							var gapDuration = mark.StartTime - lastMarkTime;
							if (gapDuration.TotalMilliseconds > 10 && mapData.PhonemeState("REST", item))
							{
								//Fill the gap with a rest
								var colorVal = mapData.ConfiguredColorAndIntensity(item);
								var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, gapDuration);
								result.OffsetAllCommandsByTime(lastMarkTime - StartTime);
								_elementData.Add(result);
							}

							lastMarkTime = mark.EndTime;
						}
						if (mapData.PhonemeState(mark.Text, item))
						{
							var colorVal = mapData.ConfiguredColorAndIntensity(item);
							var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, mark.Duration);
							result.OffsetAllCommandsByTime(mark.StartTime - StartTime);
							_elementData.Add(result);
						}
					}

					if (!AllowMarkGaps)
					{
						var gapDuration = StartTime + TimeSpan - lastMarkTime;
						if (gapDuration.TotalMilliseconds > 10 && mapData.PhonemeState("REST", item))
						{
							//Fill the gap with a rest
							var colorVal = mapData.ConfiguredColorAndIntensity(item);
							var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, gapDuration);
							result.OffsetAllCommandsByTime(lastMarkTime - StartTime);
							_elementData.Add(result);
						}
					}
				}
				else
				{
					if (mapData.PhonemeState(phoneme.ToString(), item))
					{
						var colorVal = mapData.ConfiguredColorAndIntensity(item);
						var result = CreateIntentsForElement(element, colorVal.Item1, colorVal.Item2, TimeSpan);
						_elementData.Add(result);
					}
				}
			}
		}

		private void RenderMapMatrix(LipSyncMapData mapData, PhonemeType phoneme)
		{
			SetupPictureEffect();
			if (LipSyncMode == LipSyncMode.MarkCollection)
			{
				TimeSpan lastMarkTime = StartTime;
				foreach (var mark in _marks)
				{
					if (!AllowMarkGaps)
					{
						var gapDuration = mark.StartTime - lastMarkTime;
						if (gapDuration.TotalMilliseconds > 10)
						{
							//Fill the gap with a rest
							var restImage = mapData.ImageForPhoneme("REST");
							if (restImage != null)
							{
								_thePic.StartTime = mark.StartTime - StartTime - gapDuration;
								_thePic.Image = restImage;
								_thePic.TimeSpan = gapDuration;
								_thePic.MarkDirty();
								var result = _thePic.Render();
								result.OffsetAllCommandsByTime(lastMarkTime - StartTime);
								_elementData.Add(result);
							}
						}

						lastMarkTime = mark.EndTime;
					}

					var image = mapData.ImageForPhoneme(mark.Text);
					if (image != null)
					{
						_thePic.StartTime = mark.StartTime - StartTime;
						_thePic.Image = image;
						_thePic.TimeSpan = mark.Duration;
						_thePic.MarkDirty();
						var result = _thePic.Render();
						result.OffsetAllCommandsByTime(mark.StartTime - StartTime);
						_elementData.Add(result);
					}
				}

				if (!AllowMarkGaps)
				{
					_thePic.StartTime = lastMarkTime - StartTime;
					var gapDuration = StartTime + TimeSpan - lastMarkTime;
					if (gapDuration.TotalMilliseconds > 10)
					{
						//Fill the gap with a rest
						var restImage = mapData.ImageForPhoneme("REST");
						if (restImage != null)
						{
							_thePic.Image = restImage;
							_thePic.TimeSpan = gapDuration;
							_thePic.MarkDirty();
							var result = _thePic.Render();
							result.OffsetAllCommandsByTime(lastMarkTime - StartTime);
							_elementData.Add(result);
						}
					}
				}
			}
			else
			{
				var image = mapData.ImageForPhoneme(phoneme);
				if (image!=null)
				{
					_thePic.Image = image;
					_thePic.TimeSpan = TimeSpan;
					_thePic.MarkDirty();
					var result = _thePic.Render();
					_elementData.Add(result);
				}
			}

			TearDownPictureEffect();
		}

		private void SetupPictureEffect()
		{
			if (_thePic == null)
			{
				_thePic = new FastPictureEffect();
			}
			//_thePic.Source = PictureSource.File;
			_thePic.TargetNodes = TargetNodes;
			_thePic.CacheElementEnumerator();
			_thePic.StringOrientation = Orientation;
			_thePic.ScaleToGrid = ScaleToGrid;
			_thePic.ScalePercent = ScalePercent;
			_thePic.YOffsetCurve = YOffsetCurve;
			_thePic.XOffsetCurve = XOffsetCurve;
			_thePic.TotalFrames = (int)TimeSpan.TotalMilliseconds / 50;
			_thePic.EffectEndTime = TimeSpan;
			_thePic.Level = IntensityLevel;
		}

		private void TearDownPictureEffect()
		{
			if (_thePic != null)
			{
				_thePic.UloadElementCache();
				_thePic = null;
			}
		}

		private void SetupMarks()
		{
			if (MarkCollections == null || MarkCollections.All(x => x.CollectionType == MarkCollectionType.Generic))
			{
				LipSyncMode = LipSyncMode.Phoneme;
				return;
			}

			if (string.IsNullOrEmpty(MarkCollectionId))
			{
				var dmc = MarkCollections.FirstOrDefault(x => x.CollectionType == MarkCollectionType.Phoneme);
				if (dmc != null && string.IsNullOrEmpty(MarkCollectionId))
				{
					MarkCollectionId = dmc.Name;
				}
			}
			
			
			IMarkCollection mc = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			if (mc != null)
			{
				_marks = mc.MarksInclusiveOfTime(StartTime, StartTime + TimeSpan);
			}
			else
			{
				_marks = new List<IMark>();
			}
		}

		#region Overrides of EffectModuleInstanceBase

		/// <inheritdoc />
		protected override void MarkCollectionsChanged()
		{
			if (LipSyncMode == LipSyncMode.MarkCollection)
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

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		protected void SetMappingType()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{"PhonemeMapping", MappingType == MappingType.Map},
				{"EyeMode", MappingType == MappingType.FaceDefinition},
				{"ShowOutline", MappingType == MappingType.FaceDefinition}
			};

			SetBrowsable(propertyStates);
			TypeDescriptor.Refresh(this);
		}

		protected void SetMatrixBrowesables()
		{
			bool isMap = MappingType == MappingType.Map;
			
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{"Orientation", isMap},
				{"ScaleToGrid", isMap},
				{"ScalePercent", isMap && !ScaleToGrid },
				{"IntensityLevel", isMap },
				{"ShowOutline", !isMap },
				{"EyeMode", !isMap },
				{"YOffsetCurve", isMap },
				{"XOffsetCurve", isMap }
			};
		
			SetBrowsable(propertyStates);
			TypeDescriptor.Refresh(this);
		}

		protected void SetLipsyncModeBrowsables()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{nameof(StaticPhoneme), LipSyncMode == LipSyncMode.Phoneme},
				{nameof(LyricData), LipSyncMode == LipSyncMode.Phoneme},
				{nameof(MarkCollectionId), LipSyncMode == LipSyncMode.MarkCollection},
				{nameof(AllowMarkGaps), LipSyncMode == LipSyncMode.MarkCollection}
			};

			SetBrowsable(propertyStates);
			TypeDescriptor.Refresh(this);
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as LipSyncData;
				CheckForInvalidColorData();
				SetMatrixBrowesables();
				SetLipsyncModeBrowsables();
				SetMappingType();
				IsDirty = true;
			}
		}

		[Value]
		[ProviderCategory(@"Setup",0)]
		[ProviderDisplayName(@"Orientation")]
		[ProviderDescription(@"Orientation")]
		public StringOrientation Orientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				SetMatrixBrowesables();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory("Config", 2)]
		[ProviderDisplayName(@"LipSyncMode")]
		[ProviderDescription(@"LipSyncMode")]
		[PropertyOrder(1)]
		public LipSyncMode LipSyncMode
		{
			get
			{
				return _data.LipSyncMode;

			}
			set
			{
				if (_data.LipSyncMode != value)
				{
					_data.LipSyncMode = value;
					SetLipsyncModeBrowsables();
					IsDirty = true;
					OnPropertyChanged();
					if (_data.LipSyncMode == LipSyncMode.MarkCollection && _data.MarkCollectionId == Guid.Empty)
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
		[ProviderDisplayName(@"MarkCollection")]
		[ProviderDescription(@"MarkCollection")]
		[TypeConverter(typeof(IMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
		public string MarkCollectionId
		{
			get
			{
				return MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId)?.Name;
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
		[ProviderDisplayName(@"MappingType")]
		[ProviderDescription(@"MappingType")]
		[PropertyOrder(3)]
		public MappingType MappingType
		{
			get { return _data.MappingType; }
			set
			{
				_data.MappingType = value;
				IsDirty = true;
				SetMappingType();
				SetMatrixBrowesables();
				OnPropertyChanged();
				if (value == MappingType.Map)
				{
					EnsureDefaultMap();
				}
			}
		}

		private void EnsureDefaultMap()
		{
			if (PhonemeMapping == String.Empty && _library.Any())
			{
				PhonemeMapping = _library.DefaultMappingName;
			}
		}

		[Value]
		[ProviderCategory("Config",2)]
		[ProviderDisplayName(@"PhonemeMapping")]
		[ProviderDescription(@"PhonemeMapping")]
		[PropertyEditor("SelectionEditor")]
		[TypeConverter(typeof(PhonemeMappingConverter))]
		[PropertyOrder(4)]
		public String PhonemeMapping
		{
			get { return _data.PhonemeMapping;  }
			set
			{
				_data.PhonemeMapping = value;
				IsDirty = true;
				SetMatrixBrowesables();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory("Config", 2)]
		[ProviderDisplayName(@"LyricData")]
		[ProviderDescription(@"LyricData")]
		[PropertyOrder(5)]
		public String LyricData
		{
			get { return _data.LyricData; }
			set
			{
				_data.LyricData = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory("Config", 2)]
		[ProviderDisplayName(@"StaticPhoneme")]
		[ProviderDescription(@"StaticPhoneme")]
		[PropertyOrder(6)]
		public PhonemeType StaticPhoneme
		{
			get { return _data.StaticPhoneme; }
			set
			{
				_data.StaticPhoneme = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"XOffsetCurve")]
		[ProviderDescription(@"XOffsetCurve")]
		[PropertyOrder(7)]
		public Curve XOffsetCurve
		{
			get { return _data.XOffsetCurve; }
			set
			{
				_data.XOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"YOffsetCurve")]
		[ProviderDescription(@"YOffsetCurve")]
		[PropertyOrder(8)]
		public Curve YOffsetCurve
		{
			get { return _data.YOffsetCurve; }
			set
			{
				_data.YOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"ScaleToGrid")]
		[ProviderDescription(@"ScaleToGrid")]
		[PropertyOrder(9)]
		public bool ScaleToGrid
		{
			get { return _data.ScaleToGrid; }
			set
			{
				_data.ScaleToGrid = value;
				IsDirty = true;
				SetMatrixBrowesables();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"ScalePercent")]
		[ProviderDescription(@"ScalePercent")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(10)]
		public int ScalePercent
		{
			get { return _data.ScalePercent; }
			set
			{
				_data.ScalePercent = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"EyeMode")]
		[ProviderDescription(@"EyeMode")]
		[PropertyOrder(11)]
		public EyeMode EyeMode
		{
			get { return _data.EyeMode; }
			set
			{
				_data.EyeMode = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"ShowOutline")]
		[ProviderDescription(@"ShowOutline")]
		[PropertyOrder(12)]
		public bool ShowOutline
		{
			get { return _data.ShowOutline; }
			set
			{
				_data.ShowOutline = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"AllowMarkGaps")]
		[ProviderDescription(@"AllowMarkGaps")]
		[PropertyOrder(13)]
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

		[Value]
		[ProviderCategory(@"Brightness",3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(1)]
		public int IntensityLevel
		{
			get { return _data.Level; }
			set
			{
				_data.Level = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		private static void LoadResourceBitmaps()
		{
			if (_phonemeBitmaps == null)
			{
				_phonemeBitmaps = new Dictionary<PhonemeType, Bitmap>();
				_phonemeBitmaps.Add(PhonemeType.AI, Resources.AI);
				_phonemeBitmaps.Add(PhonemeType.E, Resources.E);
				_phonemeBitmaps.Add(PhonemeType.ETC, Resources.etc);
				_phonemeBitmaps.Add(PhonemeType.FV, Resources.FV);
				_phonemeBitmaps.Add(PhonemeType.L, Resources.L);
				_phonemeBitmaps.Add(PhonemeType.MBP, Resources.MBP);
				_phonemeBitmaps.Add(PhonemeType.O, Resources.O);
				_phonemeBitmaps.Add(PhonemeType.REST, Resources.rest);
				_phonemeBitmaps.Add(PhonemeType.U, Resources.U);
				_phonemeBitmaps.Add(PhonemeType.WQ, Resources.WQ);
			}
		}

		public override bool ForceGenerateVisualRepresentation { get { return true; } }

		public override void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle)
		{
			try
			{
				Bitmap displayImage = null;
				Bitmap scaledImage = null;
				
				if (LipSyncMode == LipSyncMode.MarkCollection)
				{
					if (_marks.Any())
					{
						bool first = true;
						int endX = 0;

						TimeSpan lastMarkTime = StartTime;
						foreach (var mark in _marks)
						{
							if (!AllowMarkGaps)
							{
								var gapDuration = mark.StartTime - lastMarkTime;
								if (gapDuration.TotalMilliseconds > 10)
								{
									if (_phonemeBitmaps.TryGetValue(PhonemeType.REST, out displayImage))
									{
										endX = (int)((mark.StartTime.Ticks - StartTime.Ticks) / (double)TimeSpan.Ticks * clipRectangle.Width);
										var startX = (int)((lastMarkTime.Ticks - StartTime.Ticks) / (double)TimeSpan.Ticks * clipRectangle.Width);
										lock (displayImage)
										{
											scaledImage = new Bitmap(displayImage,
												Math.Min(clipRectangle.Width, endX - startX),
												clipRectangle.Height);
										}
										g.DrawImage(scaledImage, clipRectangle.X + startX, clipRectangle.Y);
										if (first)
										{
											first = false;
										}
										else
										{
											g.DrawLine(Pens.Black, new Point(clipRectangle.X + startX, 0), new Point(clipRectangle.X + startX, clipRectangle.Height));
										}
									}
								}

								lastMarkTime = mark.EndTime;
							}

							if (Enum.TryParse(mark.Text.ToUpper(CultureInfo.InvariantCulture), out PhonemeType phoneme))
							{
								if (_phonemeBitmaps.TryGetValue(phoneme, out displayImage))
								{
									endX = (int)((mark.EndTime.Ticks - StartTime.Ticks) / (double)TimeSpan.Ticks * clipRectangle.Width);
									if (endX <= 0) continue;
									var startX = (int)(Math.Max(0,mark.StartTime.Ticks - StartTime.Ticks) / (double)TimeSpan.Ticks * clipRectangle.Width);
									lock(displayImage)
									{
										scaledImage = new Bitmap(displayImage,
										Math.Min(clipRectangle.Width, endX - startX),
										clipRectangle.Height);
									}
									g.DrawImage(scaledImage, clipRectangle.X + startX, clipRectangle.Y);
									if (first && mark.StartTime <= StartTime)
									{
										first = false;
										continue;
									}

									g.DrawLine(Pens.Black, new Point(clipRectangle.X + startX, 0), new Point(clipRectangle.X + startX, clipRectangle.Height));
								}
							}

							first = false;
						}

						if (!AllowMarkGaps)
						{
							var gapDuration = StartTime + TimeSpan - lastMarkTime;
							if (gapDuration.TotalMilliseconds > 10)
							{
								if (_phonemeBitmaps.TryGetValue(PhonemeType.REST, out displayImage))
								{
									endX = clipRectangle.Width;
									var startX = (int)((lastMarkTime.Ticks - StartTime.Ticks) / (double)TimeSpan.Ticks * clipRectangle.Width);
									lock (displayImage)
									{
										scaledImage = new Bitmap(displayImage,
											Math.Min(clipRectangle.Width, endX - startX),
											clipRectangle.Height);
									}
									g.DrawImage(scaledImage, clipRectangle.X + startX, clipRectangle.Y);

									g.DrawLine(Pens.Black, new Point(clipRectangle.X + startX, 0), new Point(clipRectangle.X + startX, clipRectangle.Height));
								}
							}
						}
						else
						{
							if (_marks.Last().EndTime < StartTime + TimeSpan)
							{
								//draw a closing line on the image
								g.DrawLine(Pens.Black, new Point(clipRectangle.X + endX, 0), new Point(clipRectangle.X + endX, clipRectangle.Height));
							}
						}
					}
				}
				else
				{
					var displayValue = string.IsNullOrWhiteSpace(LyricData) ? "-" : LyricData;

					using (var backgroundBrush = new SolidBrush(Color.Green))
					{
						g.FillRectangle(backgroundBrush, clipRectangle);
					}

					if (_phonemeBitmaps.TryGetValue(StaticPhoneme, out displayImage))
					{
						scaledImage = new Bitmap(displayImage,
							Math.Min(clipRectangle.Height, clipRectangle.Width),
							clipRectangle.Height);
						g.DrawImage(scaledImage, clipRectangle.X, clipRectangle.Y);
					}

					if ((scaledImage != null) && (scaledImage.Width < clipRectangle.Width))
					{
						var textStart = clipRectangle.X + scaledImage.Width;
						var textWidth = clipRectangle.Width - scaledImage.Width;
						Font adjustedFont = Vixen.Common.Graphics.GetAdjustedFont(g, displayValue, new Rectangle(textStart, clipRectangle.Y, textWidth, clipRectangle.Height), "Vixen.Fonts.DigitalDream.ttf", 48);
						using (var stringBrush = new SolidBrush(Color.Yellow))
						{
							g.DrawString(displayValue, adjustedFont, stringBrush, 2 + textStart, 2);
						}
					}
				}
				
				
				
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		#region Overrides of BaseEffect

		/// <inheritdoc />
		protected override EffectTypeModuleData EffectModuleData => _data;

		#endregion
	}
}
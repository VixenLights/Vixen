using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Tree
{
	public class Tree : PixelEffectBase
	{
		private TreeData _data;
		private int _treeWidth;
		private int _xLimit;
		private int _branchColor;
		private int _branch;
		private int _m;
		private int _colorIdx;
		private int _row;

		public Tree()
		{
			_data = new TreeData();
		}

		public override bool IsDirty
		{
			get
			{
				if (Colors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		#region Setup

		[Value]
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 30, 1)]
		[PropertyOrder(0)]
		public int Speed
		{
			get { return _data.Speed; }
			set
			{
				_data.Speed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Overall Intensity")]
		[ProviderDescription(@"Overall Intensity")]
		[PropertyOrder(1)]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Tree", 2)]
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(0)]
		public ColorGradient BackgroundColor
		{
			get { return _data.BackgroundColor; }
			set
			{
				_data.BackgroundColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Tree", 2)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		[PropertyOrder(1)]
		public Curve BackgroundLevelCurve
		{
			get { return _data.BackgroundLevelCurve; }
			set
			{
				_data.BackgroundLevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Branches", 3)]
		[ProviderDisplayName(@"Branch Direction")]
		[ProviderDescription(@"Branch Direction")]
		[PropertyOrder(0)]
		public TreeBranchDirection BranchDirection
		{
			get { return _data.BranchDirection; }
			set
			{
				_data.BranchDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Branches", 3)]
		[ProviderDisplayName(@"Branches")]
		[ProviderDescription(@"Branches")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(1)]
		public int Branches
		{
			get { return _data.Branches; }
			set
			{
				_data.Branches = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Branches", 3)]
		[ProviderDisplayName(@"Color Type")]
		[ProviderDescription(@"Color Type")]
		[PropertyOrder(2)]
		public TreeColorType ColorType
		{
			get { return _data.ColorType; }
			set
			{
				_data.ColorType = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Branches", 3)]
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(5)]
		public List<ColorGradient> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Branches properties

		[Value]
		[ProviderCategory(@"Blending", 4)]
		[ProviderDisplayName(@"Blend")]
		[ProviderDescription(@"Blends between Branches")]
		[PropertyOrder(0)]
		public Curve BlendCurve
		{
			get { return _data.BlendCurve; }
			set
			{
				_data.BlendCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Blending", 4)]
		[ProviderDisplayName(@"Blend Direction")]
		[ProviderDescription(@"Toggles Blend Direction Up/Down")]
		[PropertyOrder(1)]
		public bool ToggleBlend
		{
			get { return _data.ToggleBlend; }
			set
			{
				_data.ToggleBlend = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/tree/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as TreeData;
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			_branchColor = 0;
			_treeWidth = 0;
			_xLimit = BufferWi;
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		private int Branch
		{
			get { return _branch; }
			set
			{
				if (_branch != value)
					_branchColor++;
				_branch = value;
			}
			
		}

		private int M
		{
			get { return _m; }
			set
			{
				if (_m != value && value == 3)
				{
					_colorIdx++;
					if (_colorIdx == Colors.Count)
					{
						_colorIdx = 0;
					}
				}
				_m = value;
			}
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			double pos = GetEffectTimeIntervalPosition(frame);
			double intervalPosFactor = pos * 100;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			double blendLevel = BlendCurve.GetValue(intervalPosFactor) / 100;
			double backgroundLevelCurve = BackgroundLevelCurve.GetValue(intervalPosFactor) / 100;
			HSV backgroundhsv = HSV.FromRGB(BackgroundColor.GetColorAt(pos));
			int x, y, mod, b;
			float V;
			int cycleLen = (int)(frame * Speed * (FrameTimespan.Milliseconds / 50d));
			int pixelsPerBranch = (int)(0.5 + (double)BufferHt / Branches);
			if (pixelsPerBranch == 0)
			{
				pixelsPerBranch = 1;
			}

			if (frame == _xLimit)
			{
				_treeWidth = 0;
				_xLimit += BufferWi;
			}
			_treeWidth++;

			for (y = 0; y < BufferHt; y++)
			{
				mod = y % pixelsPerBranch;
				if (mod == 0) mod = pixelsPerBranch;
				V = ToggleBlend //Fade between branches
					? (float)(1 - (1.0 * mod / pixelsPerBranch) * (1 - blendLevel))
					: (float)((1.0 * mod / pixelsPerBranch) * (blendLevel));

				backgroundhsv.V = backgroundLevelCurve * V; // we have now set the color for the background tree
				Branch = (int)((y - 1) / pixelsPerBranch);
				if (_branchColor >= Colors.Count || Branch == 0)
					_branchColor = 0;
				_row = pixelsPerBranch - mod; // now row=0 is bottom of branch, row=1 is one above bottom
				//  mod = which pixel we are in the branch
				//	mod=1,row=pixels_per_branch-1   top picrl in branch
				//	mod=2, second pixel down into branch
				//	mod=pixels_per_branch,row=0  last pixel in branch
				//
				//	row = 0, the $p is in the bottom row of tree
				//	row =1, the $p is in second row from bottom
				b = (int)((cycleLen) / BufferWi) % Branches; // what branch we are on based on frame #
				//
				//	b = 0, we are on bottomow row of tree during frames 1 to BufferWi
				//	b = 1, we are on second row from bottom, frames = BufferWi+1 to 2*BufferWi
				//	b = 2, we are on third row from bottome, frames - 2*BufferWi+1 to 3*BufferWi

				for (x = 0; x < BufferWi; x++)
				{
					Color col = backgroundhsv.ToRGB();
					M = (x % 6);
					// m=0, 1sr strand
					// m=1, 2nd strand
					// m=5, last strand in 6 strand pattern

					switch (ColorType)
					{
						case TreeColorType.Twinkle:
							_colorIdx = Rand(0, Colors.Count);
							break;
						case TreeColorType.AlternatePixel:
							_colorIdx = (x % Colors.Count);
							break;
						case TreeColorType.Static:
							_colorIdx = _branchColor;
						break;
						case TreeColorType.Alternate:
						break;
					}

					switch (BranchDirection)
					{
							case TreeBranchDirection.UpRight:
							case TreeBranchDirection.DownRight:
							case TreeBranchDirection.UpLeft:
							case TreeBranchDirection.DownLeft:
								if (Branch <= b && x <= _treeWidth && (((_row == 3 && (M == 0 || M == 5)) || ((_row == 2 && (M == 1 || M == 4)) || ((_row == 1 && (M == 2 || M == 3)))))))
									col = Colors[_colorIdx].GetColorAt(pos);
							break;

							case TreeBranchDirection.Up:
							case TreeBranchDirection.Down:
								if (Branch <= b && 
								(((_row == 3 && (M == 0 || M == 5)) || ((_row == 2 && (M == 1 || M == 4)) || ((_row == 1 && (M == 2 || M == 3)))))))
									col = Colors[_colorIdx].GetColorAt(pos);
							break;

							case TreeBranchDirection.Left:
								if ((BufferWi - x) <= _treeWidth && (((_row == 3 && (M == 0 || M == 5)) || ((_row == 2 && (M == 1 || M == 4)) || ((_row == 1 && (M == 2 || M == 3)))))))
									col = Colors[_colorIdx].GetColorAt(pos);
							break;

							case TreeBranchDirection.Alternate:
								if (Branch%2 != 0)
								{
									if ((BufferWi - x) <= _treeWidth &&  (((_row == 3 && (M == 0 || M == 5)) || ((_row == 2 && (M == 1 || M == 4)) || ((_row == 1 && (M == 2 || M == 3)))))))
										col = Colors[_colorIdx].GetColorAt(pos);
								}
								else
								{
									if (x <= _treeWidth && (((_row == 3 && (M == 0 || M == 5)) || ((_row == 2 && (M == 1 || M == 4)) || ((_row == 1 && (M == 2 || M == 3)))))))
										col = Colors[_colorIdx].GetColorAt(pos);
								}
							break;

							case TreeBranchDirection.Right:
							if (x <= _treeWidth && (((_row == 3 && (M == 0 || M == 5)) || ((_row == 2 && (M == 1 || M == 4)) || ((_row == 1 && (M == 2 || M == 3)))))))
								col = Colors[_colorIdx].GetColorAt(pos);
							break;

							case TreeBranchDirection.None:
								if (((((_row == 3 && (M == 0 || M == 5)) || ((_row == 2 && (M == 1 || M == 4)) || ((_row == 1 && (M == 2 || M == 3))))))))
									col = Colors[_colorIdx].GetColorAt(pos);
							break;
					}

					if (level < 1)
					{
						HSV hsv = HSV.FromRGB(col);
						hsv.V = hsv.V*level; //adjusts overall intensity
						col = hsv.ToRGB();
					}
					
					switch (BranchDirection)
					{
							case TreeBranchDirection.Down:
							case TreeBranchDirection.DownRight:
								frameBuffer.SetPixel(x, BufferHt - y, col);
							break;
							case TreeBranchDirection.UpLeft:
								frameBuffer.SetPixel(BufferWi - x, y, col);
							break;
							case TreeBranchDirection.DownLeft:
								frameBuffer.SetPixel(BufferWi - x, BufferHt - y, col);
							break;
							default:
								frameBuffer.SetPixel(x, y, col);
							break;
					}
				}
				_colorIdx = (Branch % Colors.Count);
			}
		}
	}
}

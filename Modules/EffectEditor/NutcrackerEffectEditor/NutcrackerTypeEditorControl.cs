using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.IO;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;
using VixenModules.Effect.Nutcracker;
using Common.Controls;

namespace VixenModules.EffectEditor.NutcrackerEffectEditor
{
	public partial class NutcrackerTypeEditorControl : UserControl, IEffectEditorControl
	{
		private NutcrackerData _data = null;
		public NutcrackerEffects effect = new NutcrackerEffects();
		private DisplayItem displayItem = null;

		public NutcrackerTypeEditorControl()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			buttonHelp.Image = new Bitmap(Common.Resources.Properties.Resources.help, new Size(16, 16));

			NutcrackerDataValue = new NutcrackerData();
		}

		public IEffect TargetEffect { get; set; }

		public NutcrackerData Data
		{
			get { return _data; }
			set { _data = value; }
		}

		public NutcrackerData NutcrackerDataValue
		{
			get { return _data; }
			set { _data = value; }
		}

		public object[] EffectParameterValues
		{
			get
			{
				object[] o = new object[] {NutcrackerDataValue};
				Data = o[0] as NutcrackerData;
				return o;
			}
			set { Data = value[0] as NutcrackerData; }
		}

		private void timerRender_Tick(object sender, EventArgs e)
		{
			effect.RenderNextEffect(Data.CurrentEffect);
			int stringCount = StringCount;
			if (displayItem != null && displayItem.Shape != null) {
				if (displayItem.Shape is PreviewMegaTree) {
					PreviewMegaTree tree = displayItem.Shape as PreviewMegaTree;
					for (int stringNum = 0; stringNum < stringCount; stringNum++) {
						PreviewBaseShape treeString = tree._strings[stringNum];
						for (int pixelNum = 0; pixelNum < treeString.Pixels.Count; pixelNum++)
						{
							treeString.Pixels[pixelNum].PixelColor = Data.StringOrienation == NutcrackerEffects.StringOrientations.Horizontal ? effect.Pixels[pixelNum][stringNum] : effect.Pixels[stringNum][pixelNum];
						}
					}
				}
				if (displayItem.Shape is PreviewPixelGrid) {
					PreviewPixelGrid grid = displayItem.Shape as PreviewPixelGrid;
					for (int stringNum = 0; stringNum < stringCount; stringNum++) {
						PreviewBaseShape gridString = grid._strings[stringNum];
						for (int pixelNum = 0; pixelNum < gridString.Pixels.Count; pixelNum++)
						{
							gridString.Pixels[pixelNum].PixelColor = grid.StringOrientation == PreviewPixelGrid.StringOrientations.Horizontal ? effect.Pixels[pixelNum][stringNum] : effect.Pixels[stringNum][pixelNum];
							
						}
					}
				}
				else if (displayItem.Shape is PreviewArch) {
					PreviewArch arch = displayItem.Shape as PreviewArch;
					for (int pixelNum = 0; pixelNum < arch.PixelCount; pixelNum++) {
						arch.Pixels[pixelNum].PixelColor = Data.StringOrienation==NutcrackerEffects.StringOrientations.Vertical?effect.Pixels[0][pixelNum]:effect.Pixels[pixelNum][0];
					}
				}
				else if (displayItem.Shape is PreviewLine) {
					PreviewLine line = displayItem.Shape as PreviewLine;
					for (int pixelNum = 0; pixelNum < line.PixelCount; pixelNum++) {
						line.Pixels[pixelNum].PixelColor = Data.StringOrienation == NutcrackerEffects.StringOrientations.Vertical ? effect.Pixels[0][pixelNum] : effect.Pixels[pixelNum][0];
					}
				}
				else if (displayItem.Shape is PreviewCane)
				{
					PreviewCane cane = displayItem.Shape as PreviewCane;
					for (int pixelNum = 0; pixelNum < cane.LinePixelCount + cane.ArchPixelCount; pixelNum++)
					{
						cane.Pixels[pixelNum].PixelColor = Data.StringOrienation == NutcrackerEffects.StringOrientations.Vertical ? effect.Pixels[0][pixelNum] : effect.Pixels[pixelNum][0];
					}
				}

				preview.RenderInForeground();
			}
			
		}

		private void PopulateEffectComboBox()
		{
			foreach (NutcrackerEffects.Effects nutcrackerEffect in Enum.GetValues(typeof(NutcrackerEffects.Effects)))
			{
				comboBoxEffect.Items.Add(nutcrackerEffect.ToString());
			}
		}

		private bool loading = true;
		private void NutcrackerTypeEditorControl_Load(object sender, EventArgs e)
		{
			PopulateEffectComboBox();

			effect.Data = Data;
			effect.Duration = TargetEffect.TimeSpan;
			effect.TimeInterval = 50;
			// Load item from Data
			SetCurrentEffect(Data.CurrentEffect);
			comboBoxEffect.SelectedItem = Data.CurrentEffect.ToString();
			trackBarSpeed.Value = Data.Speed;
			chkFitToTime.Checked = Data.FitToTime;
			radioButtonHorizontal.Checked = (Data.StringOrienation == NutcrackerEffects.StringOrientations.Horizontal);
			radioButtonVertical.Checked = (Data.StringOrienation == NutcrackerEffects.StringOrientations.Vertical);

			LoadBarsData();
			LoadButterflyData();
			LoadColorWashData();
			LoadGarlandData();
			LoadFire();
			LoadLife();
			LoadMeteor();
			LoadFireworks();
			LoadSnowflakes();
			LoadSnowstorm();
			LoadSpirals();
			LoadTwinkles();
			LoadText();
			LoadPicture();
			LoadSpirograph();
			LoadTree();
			LoadMovie();
			LoadPictureTile();
			LoadCurtain();
			LoadGlediatorData();
			LoadColors();
			LoadPreview();

			scrollPixelSize.Value = Data.PixelSize;

			timerRender.Start();

			loading = false;
		}

		private void LoadColors()
		{
			for (int colorNum = 0; colorNum < effect.Palette.Colors.Count(); colorNum++) {
				Color color = effect.Palette.Colors[colorNum];
				//Console.WriteLine("cnum:" + colorNum + " clr:" + color);
				CheckBox checkBox =
					this.Controls.Find("checkBoxColor" + (colorNum + 1).ToString(), true).FirstOrDefault() as CheckBox;
				Panel colorPanel = this.Controls.Find("panelColor" + (colorNum + 1).ToString(), true).FirstOrDefault() as Panel;
				checkBox.Checked = effect.Palette.ColorsActive[colorNum];
				colorPanel.BackColor = color;
			}
		}

		private void SetCurrentEffect(NutcrackerEffects.Effects selectedEffect)
		{
			Data.CurrentEffect = selectedEffect;
			ConfigureMainEditorOptions(selectedEffect);
			effect.SetNextState(true);
			SetCurrentTab(selectedEffect.ToString());
		}

		private void ConfigureMainEditorOptions(NutcrackerEffects.Effects selectedEffect)
		{
			//This logic placement is crappy, but it prevents an error for now.
			//Will fix better when each one gets factored out into it's own effect.
			if (selectedEffect == NutcrackerEffects.Effects.Snowflakes)
			{
				if (StringCount == 1)
				{
					radioButtonHorizontal.Enabled = false;
				}
			}
			else
			{
				radioButtonHorizontal.Enabled = true;
			}

			chkFitToTime.Enabled = selectedEffect == NutcrackerEffects.Effects.Curtain;
		}

		private void SetCurrentEffect(string effectName)
		{
			foreach (NutcrackerEffects.Effects nutcrackerEffect in Enum.GetValues(typeof (NutcrackerEffects.Effects))) {
				if (nutcrackerEffect.ToString() == effectName) {
					SetCurrentEffect(nutcrackerEffect);
				}
			}
		}

		private void SetCurrentTab(string tabName)
		{
			foreach (TabPage tab in tabEffectProperties.TabPages) {
				if (tab.Text == tabName) {
					tabEffectProperties.SelectedTab = tab;
					if (tab.Text.Equals("Fire"))
					{
						groupBoxColors.Enabled = false;
					}
					else
					{
						groupBoxColors.Enabled = true;
					}
					break;
				}
			}
		}

		private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
		{
			Data.Speed = trackBarSpeed.Value;
		}

		private void chkFitToTime_CheckedChanged(object sender, EventArgs e)
		{
			Data.FitToTime = chkFitToTime.Checked;
		}


		private void comboBoxEffect_SelectedIndexChanged(object sender, EventArgs e)
		{
			effect.SetNextState(true);
			SetCurrentEffect(comboBoxEffect.SelectedItem.ToString());
		}

		private void DeletePreviewDisplayItem()
		{
			if (preview.DisplayItems != null && preview.DisplayItems.Count > 0) {
				preview.DisplayItems.RemoveAt(0);
			}
		}

		private int StringCount
		{
			get
			{
				int childCount = 0;
				foreach (IElementNode node in TargetEffect.TargetNodes.FirstOrDefault().Children) {
					if (!node.IsLeaf) {
						childCount++;
					}
				}
				if (childCount == 0 && TargetEffect.TargetNodes.FirstOrDefault().Children.Any() ) {
					childCount = 1;
				}
				if (childCount == 0)
					childCount = 1;
				return childCount;
			}
		}

		private int PixelsPerString()
		{
			int pps = PixelsPerString(TargetEffect.TargetNodes.FirstOrDefault());
			return pps;
		}

		private int PixelsPerString(IElementNode parentNode)
		{
			//TODO: what would we do if parentNode is null?
			int pps = 0;
			int leafCount = 0;
			int groupCount = 0;
			// if no groups are children, then return nChildren
			// otherwise return the size of the first group
			IElementNode firstGroup = null;
			foreach (IElementNode node in parentNode.Children) {
				if (node.IsLeaf) {
					leafCount++;
				}
				else {
					groupCount++;
					if (firstGroup == null)
						firstGroup = node;
				}
			}
			if (groupCount == 0) {
				pps = leafCount;
			}
			else {
				// this needs to be called on a group, first might be an element
				//pps = PixelsPerStringx(parentNode.Children.FirstOrDefault());
				// this is marginally better but its not clear what to do about further nesting
				pps = PixelsPerString(firstGroup);
			}
			if (pps == 0)
				pps = 1;
			return pps;
		}

		#region Preview

		private void SetupMegaTree(int degrees)
		{
			int stringCount = StringCount;
			if (stringCount < 2) return;
			preview.Data = new VixenPreviewData();
			preview.LoadBackground();
			preview.BackgroundAlpha = 0;
			displayItem = new DisplayItem();
			PreviewMegaTree tree = new PreviewMegaTree(new PreviewPoint(10, 10), null, 1);
			tree.BaseHeight = 25;
			tree.TopHeight = 1;
			tree.TopWidth = 1;
			tree.StringType = PreviewBaseShape.StringTypes.Pixel;
			tree.Degrees = degrees;

			tree.StringCount = stringCount;

			tree.PixelCount = PixelsPerString();
			tree.PixelSize = Data.PixelSize;
			tree.PixelColor = Color.White;
			tree.Top = 10;
			tree.Left = 10;
			tree.BottomRight = new Point(preview.Width - 10, preview.Height - 10);
			tree.Layout();
			displayItem.Shape = tree;

			preview.AddDisplayItem(displayItem);
		}

		private void SetupArch()
		{
			preview.Data = new VixenPreviewData();
			preview.LoadBackground();
			preview.BackgroundAlpha = 0;
			displayItem = new DisplayItem();
			PreviewArch arch = new PreviewArch(new PreviewPoint(10, 10), null, 1);
			arch.PixelCount = PixelsPerString();
			arch.PixelSize = Data.PixelSize;
			arch.PixelColor = Color.White;
			arch.TopLeft = new Point(10, preview.Height/2);
			arch.BottomRight = new Point((int) (preview.Width - 10), (int) (preview.Height - 10));
			arch.Layout();
			displayItem.Shape = arch;

			preview.AddDisplayItem(displayItem);
		}

		private void SetupLine(bool horizontal)
		{
			preview.Data = new VixenPreviewData();
			preview.LoadBackground();
			preview.BackgroundAlpha = 0;
			displayItem = new DisplayItem();
			PreviewPoint p1, p2;
			if (horizontal) {
				p1 = new PreviewPoint(10, preview.Height/2);
				p2 = new PreviewPoint(preview.Width - 10, preview.Height/2);
			}
			else {
				p1 = new PreviewPoint(preview.Width/2, preview.Height - 10);
				p2 = new PreviewPoint(preview.Width/2, 10);
			}
			PreviewLine line = new PreviewLine(p1, p2, PixelsPerString(), null, 1);

			line.PixelCount = PixelsPerString();
			line.PixelSize = Data.PixelSize;
			line.PixelColor = Color.White;
			line.Layout();
			displayItem.Shape = line;

			preview.AddDisplayItem(displayItem);
		}

		private void SetupPixelGrid()
		{
			if (StringCount < 2) return;
			preview.Data = new VixenPreviewData();
			preview.LoadBackground();
			preview.BackgroundAlpha = 0;
			displayItem = new DisplayItem();

			PreviewPixelGrid grid = new PreviewPixelGrid(new PreviewPoint(10, 10), null, 1);
			
			grid.StringType = PreviewBaseShape.StringTypes.Pixel;
			grid.StringCount = StringCount;
			grid.LightsPerString = PixelsPerString();
			grid.PixelSize = Data.PixelSize;
			grid.PixelColor = Color.White;
			grid.Top = 10;
			grid.Left = 10;
			if (Data.StringOrienation == NutcrackerEffects.StringOrientations.Horizontal)
			{
				grid.StringOrientation = PreviewPixelGrid.StringOrientations.Horizontal;
				grid.BottomRight = new Point(Math.Min(StringCount * Data.PixelSize * 2, preview.Width - 10), preview.Width - 10); ;
				grid.BottomLeft.Y = Math.Min(StringCount * Data.PixelSize * 2, preview.Width - 10);
				grid.BottomRight = new Point(Math.Min(grid.LightsPerString * Data.PixelSize * 2, preview.Width - 10), preview.Width - 10);
				grid.Left = Math.Max( (preview.Width - 10 - (grid.LightsPerString * Data.PixelSize * 2))/2 , 10);
			}
			else
			{
				grid.BottomRight = new Point(preview.Width-10,preview.Height-10);
			}
			
			grid.Layout();
			displayItem.Shape = grid;

			preview.AddDisplayItem(displayItem);
		}

		private void SetupCane()
		{
			preview.Data = new VixenPreviewData();
			preview.LoadBackground();
			preview.BackgroundAlpha = 0;
			displayItem = new DisplayItem();

			PreviewCane cane = new PreviewCane(new PreviewPoint(10, 10), null, 1);
			cane.StringType = PreviewBaseShape.StringTypes.Pixel;

			cane.LinePixelCount = PixelsPerString() / 2;
			cane.ArchPixelCount = PixelsPerString() / 2;
			cane.PixelSize = Data.PixelSize;
			cane.PixelColor = Color.White;

			cane.ArchLeft = new Point(0, preview.Height / 4);
			cane.TopLeft = new Point(preview.Width / 4, preview.Height / 8);
			cane.BottomRight = new Point((preview.Width / 4) + (preview.Width / 2), (preview.Height / 4) + (preview.Height / 2));

			cane.Layout();
			displayItem.Shape = cane;

			preview.AddDisplayItem(displayItem);
		}

		public void SetupPreview()
		{
			DeletePreviewDisplayItem();
			int wid;
			int ht;

			if (Data.StringOrienation == NutcrackerEffects.StringOrientations.Horizontal) 
			{
				wid = PixelsPerString();
				ht = StringCount;
			}
			else
			{
				wid = StringCount;
				ht = PixelsPerString();
			}
			effect.InitBuffer(wid, ht);

			switch (Data.PreviewType) {
				case NutcrackerEffects.PreviewType.Tree90:
					SetupMegaTree(90);
					break;
				case NutcrackerEffects.PreviewType.Tree180:
					SetupMegaTree(180);
					break;
				case NutcrackerEffects.PreviewType.Tree270:
					SetupMegaTree(270);
					break;
				case NutcrackerEffects.PreviewType.Tree360:
					SetupMegaTree(360);
					break;
				case NutcrackerEffects.PreviewType.Grid:
					SetupPixelGrid();
					break;
				case NutcrackerEffects.PreviewType.Arch:
					SetupArch();
					break;
				case NutcrackerEffects.PreviewType.HorizontalLine:
					SetupLine(true);
					break;
				case NutcrackerEffects.PreviewType.VerticalLine:
					SetupLine(false);
					break;
				case NutcrackerEffects.PreviewType.Cane:
					SetupCane();
					break;
				default:
					SetupMegaTree(180);
					break;
			}
		}

		private void comboBoxDisplayType_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (NutcrackerEffects.PreviewType previewType in Enum.GetValues(typeof (NutcrackerEffects.PreviewType))) {
				if (previewType.ToString() == comboBoxDisplayType.SelectedItem.ToString()) {
					Data.PreviewType = previewType;
					break;
				}
			}
			SetupPreview();
		}

		private void LoadPreview()
		{
			foreach (NutcrackerEffects.PreviewType previewType in Enum.GetValues(typeof (NutcrackerEffects.PreviewType))) {
				comboBoxDisplayType.Items.Add(previewType.ToString());
			}

			comboBoxDisplayType.SelectedItem = Data.PreviewType.ToString();

			SetupPreview();
		}

		#endregion // Preview

		#region Colors

		private void checkBoxColor_CheckedChanged(object sender, EventArgs e)
		{
			if (loading) return;
			CheckBox checkBox = sender as CheckBox;
			string colorNum = checkBox.Name.Substring(checkBox.Name.Length - 1);
			if (checkBox.Checked) {
				string panelName = "panelColor" + colorNum;
				Panel colorPanel = this.Controls.Find(panelName, true).FirstOrDefault() as Panel;
				effect.Palette.SetColor(Convert.ToInt32(colorNum), colorPanel.BackColor);
			}
			else {
				effect.Palette.ColorsActive[Convert.ToInt32(colorNum) - 1] = false;
			}
			effect.SetNextState(true);
		}

		private void panelColor_Click(object sender, EventArgs e)
		{
			Panel colorPanel = sender as Panel;
			colorDialog.Color = colorPanel.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK) {
				colorPanel.BackColor = colorDialog.Color;
				string colorNum = colorPanel.Name.Substring(colorPanel.Name.Length - 1);
				effect.Palette.SetColor(Convert.ToInt32(colorNum), colorPanel.BackColor, false);
			}
		}

		#endregion

		#region Bars

		private void LoadBarsData()
		{
			trackBarPaletteRepeat.Value = Data.Bars_PaletteRepeat;
			checkBoxBars3D.Checked = Data.Bars_3D;
			checkBoxBarsHighlight.Checked = Data.Bars_Highlight;
			comboBoxBarsDirection.SelectedIndex = Data.Bars_Direction;
		}

		private void Bars_ParametersChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Bars_PaletteRepeat = trackBarPaletteRepeat.Value;
			Data.Bars_Highlight = checkBoxBarsHighlight.Checked;
			Data.Bars_3D = checkBoxBars3D.Checked;
			if (comboBoxBarsDirection.SelectedItem != null)
			{
				Data.Bars_Direction = comboBoxBarsDirection.SelectedIndex;
			}
		}

		private void trackBarPaletteRepeat_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                                Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			Bars_ParametersChanged(sender, EventArgs.Empty);
		}

		#endregion

		#region Butterfly

		private void LoadButterflyData()
		{
			trackButterflyStyle.Value = Data.Butterfly_Style;
			trackButterflyBkgrdChunks.Value = Data.Butterfly_BkgrdChunks;
			trackButterflyBkgrdSkip.Value = Data.Butterfly_Style;
			comboBoxButterflyColors.SelectedIndex = Data.Butterfly_Colors;
			comboButterflyDirection.SelectedIndex = Data.Butterfly_Direction;
		}

		private void Butterfly_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                    Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Butterfly_Style = trackButterflyStyle.Value;
			Data.Butterfly_BkgrdChunks = trackButterflyBkgrdChunks.Value;
			Data.Butterfly_BkgrdSkip = trackButterflyBkgrdSkip.Value;
		}

		private void comboBoxButterflyColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Butterfly_Colors = comboBoxButterflyColors.SelectedIndex;
		}

		private void comboButterflyDirection_SelectedIndexChanged(object sender, EventArgs e)
		{
			Data.Butterfly_Direction = comboButterflyDirection.SelectedIndex;
		}



		#endregion

		#region ColorWash

		private void LoadColorWashData()
		{
			trackColorWashCount.Value = Data.ColorWash_Count;
			checkBoxColorWashHorizontalFade.Checked = Data.ColorWash_FadeHorizontal;
			checkBoxColorWashVerticalFade.Checked = Data.ColorWash_FadeVertical;
		}

		private void trackColorWashCount_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                              Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			Data.ColorWash_Count = trackColorWashCount.Value;
			effect.SetNextState(true);
		}

		private void checkBoxColorWashHorizontalFade_CheckedChanged(object sender, EventArgs e)
		{
			Data.ColorWash_FadeHorizontal = checkBoxColorWashHorizontalFade.Checked;
			effect.SetNextState(true);
		}

		private void checkBoxColorWashVerticalFade_CheckedChanged(object sender, EventArgs e)
		{
			Data.ColorWash_FadeVertical = checkBoxColorWashVerticalFade.Checked;
			effect.SetNextState(true);
		}

		#endregion

		#region Fire

		private void LoadFire()
		{
			trackFireHeight.Value = Data.Fire_Height;
			trackFireHueShift.Value = Data.Fire_Hue;
		}

		private void trackFireHueShift_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			Data.Fire_Hue = trackFireHueShift.Value;
		}

		private void trackFireHeight_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                          Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			Data.Fire_Height = trackFireHeight.Value;
		}

		#endregion

		#region Garlands

		private void LoadGarlandData()
		{
			trackBarGarlandSpacing.Value = Data.Garland_Spacing;
			trackBarGarlandType.Value = Data.Garland_Type;
		}

		private void Garlands_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                   Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Garland_Type = trackBarGarlandType.Value;
			Data.Garland_Spacing = trackBarGarlandSpacing.Value;
			effect.SetNextState(true);
		}

		#endregion

		#region Life

		private void LoadLife()
		{
			trackLifeType.Value = Data.Life_Type;
			trackLifeCellsToStart.Value = Data.Life_CellsToStart;
		}

		private void Life_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                               Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Life_CellsToStart = trackLifeCellsToStart.Value;
			Data.Life_Type = trackLifeType.Value;
			effect.SetNextState(true);
		}

		#endregion // Life

		#region Meteor

		private void LoadMeteor()
		{
			comboBoxMeteorColors.SelectedIndex = Data.Meteor_Colors;
			trackMeteorCount.Value = Data.Meteor_Count;
			trackMeteorTrailLength.Value = Data.Meteor_TrailLength;
		}

		private void comboBoxMeteorColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (loading) return;
			switch (comboBoxMeteorColors.SelectedItem.ToString()) {
				case "Rainbow":
					Data.Meteor_Colors = 0;
					break;
				case "Range":
					Data.Meteor_Colors = 1;
					break;
				case "Palette":
					Data.Meteor_Colors = 2;
					break;
			}
		}

		private void Meteor_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                 Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Meteor_Count = trackMeteorCount.Value;
			Data.Meteor_TrailLength = trackMeteorTrailLength.Value;
		}

		#endregion // Meteor

		#region Fireworks

		private void LoadFireworks()
		{
			trackFireworkNumberOfExplosions.Value = Data.Fireworks_Explosions;
			trackFireworkFade.Value = Data.Fireworks_Fade;
			trackFireworkParticles.Value = Data.Fireworks_Particles;
			trackerFireworkVelocity.Value = Data.Fireworks_Velocity;
		}

		private void Fireworks_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                    Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Fireworks_Explosions = trackFireworkNumberOfExplosions.Value;
			Data.Fireworks_Fade = trackFireworkFade.Value;
			Data.Fireworks_Particles = trackFireworkParticles.Value;
			Data.Fireworks_Velocity = trackerFireworkVelocity.Value;
		}

		#endregion

		#region Snowflakes

		private void LoadSnowflakes()
		{
			trackSnowflakeMax.Value = Data.Snowflakes_Max;
			trackSnowflakeType.Value = Data.Snowflakes_Type;
		}

		private void Snowflake_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                    Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Snowflakes_Max = trackSnowflakeMax.Value;
			Data.Snowflakes_Type = trackSnowflakeType.Value;
		}

		#endregion // Snowflakes

		#region Snowstorm

		private void LoadSnowstorm()
		{
			trackSnowstormMaxFlakes.Value = Data.Snowstorm_MaxFlakes;
			trackSnowstormTrailLength.Value = Data.Snowstorm_TrailLength;
		}

		private void Snowstorm_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                    Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Snowstorm_TrailLength = trackSnowstormTrailLength.Value;
			Data.Snowstorm_MaxFlakes = trackSnowstormMaxFlakes.Value;
		}

		#endregion // Snowstorm

		#region Spirals

		private void LoadSpirals()
		{
			trackSpiralsDirection.Value = Data.Spirals_Direction;
			trackSpiralsRepeat.Value = Data.Spirals_PaletteRepeat;
			trackSpiralsRotations.Value = Data.Spirals_Rotation;
			trackSpiralsThickness.Value = Data.Spirals_Thickness;
			checkSpirals3D.Checked = Data.Spirals_3D;
			checkSpiralsBlend.Checked = Data.Spirals_Blend;
		}

		private void Spirals_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                  Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Spirals_Direction = trackSpiralsDirection.Value;
			Data.Spirals_PaletteRepeat = trackSpiralsRepeat.Value;
			Data.Spirals_Rotation = trackSpiralsRotations.Value;
			Data.Spirals_Thickness = trackSpiralsThickness.Value;
		}

		private void Spirals_CheckedChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Spirals_3D = checkSpirals3D.Checked;
			Data.Spirals_Blend = checkSpiralsBlend.Checked;
		}

		#endregion // Twinkles

		#region Twinkles

		private void LoadTwinkles()
		{
			trackTwinkleCount.Value = Data.Twinkles_Count;
			trackTwinkleSteps.Value = Data.Twinkles_Steps;
			chkTwinkleStrobe.Checked = Data.Twinkles_Strobe;
		}

		private void trackTwinkleCount_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                            Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Twinkles_Count = trackTwinkleCount.Value;
		}

		private void trackTwinkleSteps_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Twinkles_Steps = trackTwinkleSteps.Value;
		}


		private void chkTwinkleStrobe_CheckedChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Twinkles_Strobe = chkTwinkleStrobe.Checked;
		}


		#endregion // Twinkles

		#region Text

		private void LoadText()
		{
			textTextLine1.Text = Data.Text_Line1;
			textTextLine2.Text = Data.Text_Line2;
			textTextLine3.Text = Data.Text_Line3;
			textTextLine4.Text = Data.Text_Line4;
			comboBoxTextDirection.SelectedIndex = Data.Text_Direction;
			trackTextTop.Value = Data.Text_Top;
			chkCenterStop.Checked = Data.Text_CenterStop;
			textBoxTextFont.Text = String.Format("{0} {1} pt", Data.Text_Font.FontValue.Name, Data.Text_Font.FontValue.SizeInPoints);
		}

		private void Text_TextChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Text_Line1 = textTextLine1.Text;
			Data.Text_Line2 = textTextLine2.Text;
			Data.Text_Line3 = textTextLine3.Text;
			Data.Text_Line4 = textTextLine4.Text;
		}

		private void trackTextTop_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                       Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Text_Top = trackTextTop.Value;
		}

		private void buttonTextFont_Click(object sender, EventArgs e)
		{
			fontDialog.Font = Data.Text_Font;
			if (fontDialog.ShowDialog() == DialogResult.OK) {
				Data.Text_Font = fontDialog.Font;
				textBoxTextFont.Text = String.Format("{0} {1} pt", fontDialog.Font.Name, fontDialog.Font.SizeInPoints);
			}
		}

		private void chkCenterStop_CheckedChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Text_CenterStop = chkCenterStop.Checked;
		}


		private void comboBoxTextDirection_SelectedIndexChanged(object sender, EventArgs e)
		{
			Data.Text_Direction = comboBoxTextDirection.SelectedIndex;
		}
		
		#endregion // Text

		#region Curtain

		private void LoadCurtain()
		{
			comboCurtainEffect.SelectedIndex = Data.Curtain_Effect;
			comboCurtainEdge.SelectedIndex = Data.Curtain_Edge;
			chkCurtainRepeat.Checked = Data.Curtain_Repeat;
			trackCurtainSwagWidth.Value = Data.Curtain_SwagWidth;
		}

		private void comboCurtainEffect_SelectedIndexChanged(object sender, EventArgs e)
		{
			Data.Curtain_Effect = comboCurtainEffect.SelectedIndex;
			effect.SetNextState(true);
		}

		private void comboCurtainEdge_SelectedIndexChanged(object sender, EventArgs e)
		{
			Data.Curtain_Edge = comboCurtainEdge.SelectedIndex;
			effect.SetNextState(true);
		}

		private void chkCurtainRepeat_CheckedChanged(object sender, EventArgs e)
		{
			Data.Curtain_Repeat = chkCurtainRepeat.Checked;
		}

		private void trackCurtainSwagWidth_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			Data.Curtain_SwagWidth = trackCurtainSwagWidth.Value;
			effect.SetNextState(true);
		}
		


		#endregion

		#region Picture

		private void LoadPicture()
		{
			textPictureFileName.Text = Path.Combine(NutcrackerDescriptor.ModulePath, Data.Picture_FileName); 
			comboBoxPictureDirection.SelectedIndex = Data.Picture_Direction;
			trackPictureGifSpeed.Value = Data.Picture_GifSpeed;
			trackPictureGifSpeed.Enabled = true;
			trackPictureScaleToGrid.Checked = Data.Picture_ScaleToGrid;
			trackPictureScalePercent.Enabled = !Data.Picture_ScaleToGrid;
			trackPictureScalePercent.Value = Data.Picture_ScalePercent;

		}

		private void buttonPictureSelect_Click(object sender, EventArgs e)
		{
			fileDialog.Filter = "All Files|*.*|jpg|*.jpg|jpeg|*.jpeg|gif|.gif|png|*.png|bmp|*.bmp";
			if (fileDialog.ShowDialog() == DialogResult.OK) {
				// Copy the file to the Vixen folder
				var imageFile = new FileInfo(fileDialog.FileName);
				var destFileName = Path.Combine(NutcrackerDescriptor.ModulePath, imageFile.Name);
				var sourceFileName = imageFile.FullName;
				if (sourceFileName != destFileName) {
					File.Copy(sourceFileName, destFileName, true);
				}

				textPictureFileName.Text = destFileName;
				Data.Picture_FileName = imageFile.Name;
			}
		}

		private void comboBoxPictureDirection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Picture_Direction = comboBoxPictureDirection.SelectedIndex;
			effect.SetNextState(true);
		}

		private void trackPictureGifSpeed_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                               Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Picture_GifSpeed = trackPictureGifSpeed.Value;
		}

		private void trackPictureScaleToGrid_CheckedChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Picture_ScaleToGrid = trackPictureScaleToGrid.Checked;
			trackPictureScalePercent.Enabled = !Data.Picture_ScaleToGrid;
		}

		private void trackPictureScalePercent_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			Data.Picture_ScalePercent = trackPictureScalePercent.Value;
		}

		

		#endregion // Picture

		#region Spirograph

		private void LoadSpirograph()
		{
			trackSpirographROuter.Value = Data.Spirograph_ROuter;
			trackSpirographRInner.Value = Data.Spirograph_RInner;
			trackSpirographDistance.Value = Data.Spirograph_Distance;
			checkBoxSpirographAnimate.Checked = Data.Spirograph_Animate;
		}

		private void checkBoxSpirographAnimate_CheckedChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Spirograph_Animate = checkBoxSpirographAnimate.Checked;
		}

		private void Spirograph_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                     Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Spirograph_Distance = trackSpirographDistance.Value;
			Data.Spirograph_ROuter = trackSpirographROuter.Value;
			Data.Spirograph_RInner = trackSpirographRInner.Value;
		}

		#endregion

		#region Tree

		private void LoadTree()
		{
			trackTreeBranches.Value = Data.Tree_Branches;
		}

		private void trackTreeBranches_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                            Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Tree_Branches = trackTreeBranches.Value;
		}

		#endregion //Tree

		#region Movie

		private void LoadMovie()
		{
			trackMoviePlaybackSpeed.Value = Data.Movie_PlaybackSpeed;
			comboBoxMovieMovementDirection.SelectedIndex = Data.Movie_MovementDirection;
		}

		private void DeleteExistingMovieFiles(string folder)
		{
			System.IO.DirectoryInfo folderInfo = new System.IO.DirectoryInfo(folder);

			foreach (System.IO.FileInfo file in folderInfo.GetFiles()) {
				file.Delete();
			}
			foreach (System.IO.DirectoryInfo dir in folderInfo.GetDirectories()) {
				dir.Delete(true);
			}
		}

		private void ProcessMovie(string movieFileName, string destinationFolder)
		{
			try {
				NutcrackerProcessingMovie f = new NutcrackerProcessingMovie();
				f.Show();
				ffmpeg.ffmpeg converter = new ffmpeg.ffmpeg(movieFileName);
				converter.MakeThumbnails(50, 50, destinationFolder, 1000 / VixenSystem.DefaultUpdateInterval);
				f.Close();
			}
			catch (Exception ex) {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("There was a problem converting " + movieFileName + ": " + ex.Message, "Error Converting Movie", false, true);
				messageBox.ShowDialog();
			}
		}

		private void buttonMovieSelectFile_Click(object sender, EventArgs e)
		{
			fileDialog.Filter = "All Files|*.*";
			if (fileDialog.ShowDialog() == DialogResult.OK) {
				// If this effect doesn't have working folder make one.
				// TODO: delete the folder if the effect is removed from the timeline?
				if (Data.Movie_DataPath.Length == 0) {
					Data.Movie_DataPath = Guid.NewGuid().ToString();
				}
				var destFolder = System.IO.Path.Combine(NutcrackerDescriptor.ModulePath, Data.Movie_DataPath);
				if (!System.IO.Directory.Exists(destFolder)) {
					System.IO.Directory.CreateDirectory(destFolder);
				}
				DeleteExistingMovieFiles(destFolder);
				ProcessMovie(fileDialog.FileName, destFolder);
				effect.SetNextState(true);
			}
		}

		private void Movie_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.Movie_PlaybackSpeed = trackMoviePlaybackSpeed.Value;
		}

		private void comboBoxMovieMovementDirection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.Movie_MovementDirection = comboBoxMovieMovementDirection.SelectedIndex;
		}

		#endregion // Movies

		#region PictureTile

		//private void LoadPictureTile()
		//{
		//    string folder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules\\Effect\\PictureTiles");
		//    //System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Modules\\Effect\\PictureTiles";
		//    Console.WriteLine(folder);
		//    System.IO.DirectoryInfo folderInfo = new System.IO.DirectoryInfo(folder);

		//    foreach (System.IO.FileInfo file in folderInfo.GetFiles()) {
		//        // TODO: check for valid image formats
		//        if (file.Extension.ToLower() != ".db") {
		//            string title = file.Name;
		//            PictureComboBoxItem item = new PictureComboBoxItem(title, file, comboBoxPictureTileFileName.ItemHeight,
		//                                                               comboBoxPictureTileFileName.ItemHeight);
		//            comboBoxPictureTileFileName.Items.Add(item);

		//            if (item.File.FullName == Data.PictureTile_FileName) {
		//                comboBoxPictureTileFileName.SelectedIndex = comboBoxPictureTileFileName.Items.Count - 1;
		//            }
		//        }
		//    }

		//    if (comboBoxPictureTileFileName.Items.Count > 0 && comboBoxPictureTileFileName.SelectedIndex < 0)
		//        comboBoxPictureTileFileName.SelectedIndex = 0;

		//    trackPictureTileMovementDirection.Value = Data.PictureTile_Direction;
		//    numericPictureTileScale.Value = Convert.ToDecimal(Data.PictureTile_Scaling);
		//    checkPictureTileReplaceColor.Checked = Data.PictureTile_ReplaceColor;
		//    checkPictureTileCopySaturation.Checked = Data.PictureTile_UseSaturation;
		//}

		private const string IMAGE_RESX_SOURCE = "VixenModules.Effect.Nutcracker.PictureTiles";
		private void LoadPictureTile()
		{
			string[] resourceNames = typeof(Nutcracker).Assembly.GetManifestResourceNames();
			foreach (var res in resourceNames)
			{
				string title = res.Replace(IMAGE_RESX_SOURCE + ".", string.Empty); ;
				PictureComboBoxItem item = new PictureComboBoxItem(title, res, comboBoxPictureTileFileName.ItemHeight,
																									comboBoxPictureTileFileName.ItemHeight, typeof(Nutcracker));
				comboBoxPictureTileFileName.Items.Add(item);
				//if (!Data.PictureFile_Custom && item.ResourceName == Data.PictureTile_FileName)
				if (item.ResourceName == Data.PictureTile_FileName)
				{
					comboBoxPictureTileFileName.SelectedIndex = comboBoxPictureTileFileName.Items.Count - 1;
				}

			}
			if (comboBoxPictureTileFileName.Items.Count > 0 && comboBoxPictureTileFileName.SelectedIndex < 0)
				comboBoxPictureTileFileName.SelectedIndex = 0;
			trackPictureTileMovementDirection.Value = Data.PictureTile_Direction;
			numericPictureTileScale.Value = Convert.ToDecimal(Data.PictureTile_Scaling);
			checkPictureTileReplaceColor.Checked = Data.PictureTile_ReplaceColor;
			checkPictureTileCopySaturation.Checked = Data.PictureTile_UseSaturation;
		}

		private void comboBoxPictureTileFileName_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (loading) return;
			PictureComboBoxItem item = comboBoxPictureTileFileName.SelectedItem as PictureComboBoxItem;
			if (item != null) {
				//FileInfo file = item.File;
				//Data.PictureTile_FileName = file.FullName;
				Data.PictureTile_FileName = item.ResourceName;
			}
			effect.SetNextState(true);
		}

		private void PictureTile_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                      Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			if (loading) return;
			Data.PictureTile_Direction = trackPictureTileMovementDirection.Value;
		}

		private void numericPictureTileScale_ValueChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.PictureTile_Scaling = Convert.ToDouble(numericPictureTileScale.Value);
			effect.SetNextState(true);
		}

		private void PictureTile_CheckedChanged(object sender, EventArgs e)
		{
			if (loading) return;
			Data.PictureTile_ReplaceColor = checkPictureTileReplaceColor.Checked;
			Data.PictureTile_UseSaturation = checkPictureTileCopySaturation.Checked;
			effect.SetNextState(true);
		}

		private void buttonPictureTileSelect_Click(object sender, EventArgs e)
		{
			fileDialog.Filter = "All Files|*.*|jpg|*.jpg|jpeg|*.jpeg|gif|.gif|png|*.png|bmp|*.bmp";
			if (fileDialog.ShowDialog() == DialogResult.OK) {
				// Copy the file to the Vixen folder
				var imageFile = new FileInfo(fileDialog.FileName);
				var destFileName = Path.Combine(NutcrackerDescriptor.ModulePath, imageFile.Name);
				var sourceFileName = imageFile.FullName;
				if (sourceFileName != destFileName) {
					File.Copy(sourceFileName, destFileName, true);
				}

				textPictureTileFileName.Text = destFileName;
				Data.PictureTile_FileName = imageFile.Name;
			}
		}

		#endregion // PictureTile

		#region Glediator

		private void LoadGlediatorData()
		{
			textGlediatorFileName.Text = Data.Glediator_FileName;
		}

		private void buttonGlediatorFile_Click(object sender, EventArgs e)
		{
			fileDialog.Filter = @"Glediator Files|*.gled";
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				// Copy the file to the Vixen folder
				var gledFile = new FileInfo(fileDialog.FileName);
				var destFileName = Path.Combine(NutcrackerDescriptor.ModulePath, gledFile.Name);
				var sourceFileName = gledFile.FullName;
				if (sourceFileName != destFileName)
				{
					File.Copy(sourceFileName, destFileName, true);
				}

				textGlediatorFileName.Text = destFileName;
				Data.Glediator_FileName = gledFile.Name;
			}
		}

		#endregion

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Effect_Nutcracker);	
		}

		private void scrollPixelSize_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender,
		                                          Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			Data.PixelSize = scrollPixelSize.Value;
			// the 2D preview types, when string cnt < 2, can return without setting this
			if (displayItem == null)
				return;
			//displayItem.Shape.PixelSize = Data.PixelSize;
			SetupPreview();
		}

		private void radioButtonVertical_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonVertical.Checked)
			{
				Data.StringOrienation = NutcrackerEffects.StringOrientations.Vertical;
				if (checkBoxColorWashHorizontalFade.Checked || StringCount == 1)
				{
					checkBoxColorWashHorizontalFade.Checked = false;
					checkBoxColorWashHorizontalFade.Enabled = false;
				}
				
				checkBoxColorWashVerticalFade.Enabled = true;
				SetupPreview();
			}
		}

		private void radioButtonHorizontal_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonHorizontal.Checked)
			{
				Data.StringOrienation = NutcrackerEffects.StringOrientations.Horizontal;
				if (checkBoxColorWashVerticalFade.Checked || StringCount == 1)
				{
					checkBoxColorWashVerticalFade.Checked = false;
					checkBoxColorWashVerticalFade.Enabled = false;
				}
				
				checkBoxColorWashHorizontalFade.Enabled = true;
				SetupPreview();
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			if (effect != null)
			{
				effect.Dispose();
			}
			base.Dispose(disposing);
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}
	}
}
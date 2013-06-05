using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;
using VixenModules.Effect.Nutcracker;

namespace VixenModules.EffectEditor.NutcrackerEffectEditor
{
    public partial class NutcrackerTypeEditorControl : UserControl, IEffectEditorControl
	{
        private NutcrackerData _data = null;
        public NutcrackerEffects effect = new NutcrackerEffects();
        private DisplayItem displayItem = null;
        private PreviewMegaTree tree = null;

        public NutcrackerTypeEditorControl()
		{
			InitializeComponent();
            NutcrackerDataValue = new NutcrackerData();
		}

		public IEffect TargetEffect { get; set; }

        public NutcrackerData Data
        {
            get 
            { 
                return _data; 
            }
            set 
            {
                _data = value; 
            }
        }

        public NutcrackerData NutcrackerDataValue
        {
            get { return _data; }
            set
            {
                _data = value;
            }
        }

		public object[] EffectParameterValues
		{
			get 
            {
                object[] o = new object[] { NutcrackerDataValue };
                Data = o[0] as NutcrackerData;
                return o;
            }
			set 
            { 
                Data = value[0] as NutcrackerData;
            }
		}

        private void timerRender_Tick(object sender, EventArgs e)
        {
            effect.RenderNextEffect(Data.CurrentEffect);

            PreviewMegaTree tree = displayItem.Shape as PreviewMegaTree;
            for (int stringNum = 0; stringNum < StringCount; stringNum++)
            {
                int currentString = StringCount - stringNum - 1;
                //Console.WriteLine("sc:" + StringCount + " sn:" + Convert.ToInt32(stringNum+1).ToString() + " cs:" + currentString);
                PreviewBaseShape treeString = tree._strings[currentString];
                for (int pixelNum = 0; pixelNum < treeString.Pixels.Count; pixelNum++)
                {
                    treeString.Pixels[pixelNum].PixelColor = effect.Pixels[stringNum][pixelNum];
                }
            }

            preview.RenderInForeground();
        }

        bool loading = true;
        private void NutcrackerTypeEditorControl_Load(object sender, EventArgs e)
        {
            foreach (NutcrackerEffects.Effects nutcrackerEffect in Enum.GetValues(typeof(NutcrackerEffects.Effects)))
            {
                comboBoxEffect.Items.Add(nutcrackerEffect.ToString());
            }

            foreach (ElementNode node in Data.TargetNodes)
            {
                if (node != null)
                {
                    Console.WriteLine(node.Name);
                    //RenderNode(node);
                }
            }


            effect.Data = Data;

            SetupPreview();

            // Load item from Data
            SetCurrentEffect(Data.CurrentEffect);
            comboBoxEffect.SelectedItem = Data.CurrentEffect.ToString();
            trackBarSpeed.Value = Data.Speed;

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
            LoadColors();

            timerRender.Start();

            loading = false;
        }

        private void LoadColors()
        {
            for (int colorNum = 0; colorNum < effect.Palette.Count(); colorNum++) 
            {
                Color color = effect.Palette.Colors[colorNum];
                CheckBox checkBox = this.Controls.Find("checkBoxColor" + (colorNum+1).ToString(), true).FirstOrDefault() as CheckBox;
                Panel colorPanel = this.Controls.Find("panelColor" + (colorNum+1).ToString(), true).FirstOrDefault() as Panel;
                checkBox.Checked = true;
                colorPanel.BackColor = color;
            }
        }

        private void SetCurrentEffect(NutcrackerEffects.Effects selectedEffect)
        {
            Data.CurrentEffect = selectedEffect;
            effect.SetNextState(true);
            SetCurrentTab(selectedEffect.ToString());
        }

        private void SetCurrentEffect(string effectName)
        {
            foreach (NutcrackerEffects.Effects nutcrackerEffect in Enum.GetValues(typeof(NutcrackerEffects.Effects)))
            {
                if (nutcrackerEffect.ToString() == effectName)
                {
                    SetCurrentEffect(nutcrackerEffect);
                }
            }
        }

        private void SetCurrentTab(string tabName)
        {
            foreach (TabPage tab in tabEffectProperties.TabPages)
            {
                if (tab.Name == tabName)
                {
                    tabEffectProperties.SelectedTab = tab;
                    break;
                }
            }
        }

        private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
        {
            Data.Speed = trackBarSpeed.Value;
        }

        private void comboBoxEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (loading) return;
            effect.SetNextState(true);
            SetCurrentEffect(comboBoxEffect.SelectedItem.ToString());
        }

        private void DeletePreviewDisplayItem()
        {
            if (preview.DisplayItems != null && preview.DisplayItems.Count > 0)
            {
                preview.DisplayItems.RemoveAt(0);
            }
        }

        private int StringCount
        {
            get
            {
                int childCount = TargetEffect.TargetNodes.FirstOrDefault().Children.Count();
                //Console.WriteLine("StringCount:" + childCount);
                //return TargetEffect.TargetNodes.Count();
                return childCount;
            }
        }

        private void SetupMegaTree(int degrees)
        {
            preview.Data = new VixenPreviewData();
            preview.LoadBackground();
            preview.BackgroundAlpha = 0;
            displayItem = new DisplayItem();
            tree = new PreviewMegaTree(new PreviewPoint(10, 10), null);
            tree.BaseHeight = 25;
            tree.TopHeight = 1;
            tree.TopWidth = 1;
            tree.StringType = PreviewBaseShape.StringTypes.Pixel;
            tree.Degrees = degrees;

            if (degrees == 90)
                tree.StringCount = StringCount * 4;
            if (degrees == 180)
                tree.StringCount = StringCount * 2;
            if (degrees == 270)
                tree.StringCount = (int)(StringCount * 1.25);
            if (degrees == 360)
                tree.StringCount = StringCount;

            Console.WriteLine("degrees:" + degrees + " StringCount:" + StringCount + " tree.StringCount:" + tree.StringCount);

            tree.PixelCount = 50;
            tree.PixelSize = 3;
            tree.PixelColor = Color.White;
            tree.Top = 10;
            tree.Left = 10;
            tree.BottomRight.X = preview.Width - 10;
            tree.BottomRight.Y = preview.Height - 10;
            tree.Layout();
            displayItem.Shape = tree;

            preview.AddDisplayItem(displayItem);
        }

        private void SetupArch()
        {
        }

        public void SetupPreview()
        {
            DeletePreviewDisplayItem();
            Console.WriteLine("SetupPreview:" + Data.PreviewType.ToString());

            effect.InitBuffer(StringCount, 50);

            switch (Data.PreviewType)
            {
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
                case NutcrackerEffects.PreviewType.Arch:
                    SetupArch();
                    break;
                default:
                    SetupMegaTree(180);
                    break;
            }
        }

        private void comboBoxDisplayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxDisplayType.SelectedIndex)
            {
                case 0: Data.PreviewType = NutcrackerEffects.PreviewType.Tree90; break;
                case 1: Data.PreviewType = NutcrackerEffects.PreviewType.Tree180; break;
                case 2: Data.PreviewType = NutcrackerEffects.PreviewType.Tree270; break;
                case 3: Data.PreviewType = NutcrackerEffects.PreviewType.Tree360; break;
                case 4: Data.PreviewType = NutcrackerEffects.PreviewType.Arch; break;
                default: Data.PreviewType = NutcrackerEffects.PreviewType.Tree180; break;
            }
            SetupPreview();
        }

        #region Colors

        private void checkBoxColor_CheckedChanged(object sender, EventArgs e)
        {
            if (loading) return;
            CheckBox checkBox = sender as CheckBox;
            string colorNum = checkBox.Name.Substring(checkBox.Name.Length - 1);
            if (checkBox.Checked)
            {
                string panelName = "panelColor" + colorNum;
                Panel colorPanel = this.Controls.Find(panelName, true).FirstOrDefault() as Panel;
                effect.Palette.SetColor(Convert.ToInt32(colorNum), colorPanel.BackColor);
            }
            else
            {
                effect.Palette.ColorsActive[Convert.ToInt32(colorNum) - 1] = false;
            }
            effect.SetNextState(true);
        }

        private void panelColor_Click(object sender, EventArgs e)
        {
            Panel colorPanel = sender as Panel;
            colorDialog.Color = colorPanel.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
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
            switch (Data.Bars_Direction)
            {
                case 0:
                    comboBoxBarsDirection.SelectedItem = "Up";
                    break;
                case 1:
                    comboBoxBarsDirection.SelectedItem = "Down";
                    break;
                case 2:
                    comboBoxBarsDirection.SelectedItem = "Expand";
                    break;
                case 3:
                    comboBoxBarsDirection.SelectedItem = "Compress";
                    break;
            }
        }

        private void Bars_ParametersChanged(object sender, EventArgs e)
        {
            if (loading) return;
            Data.Bars_PaletteRepeat = trackBarPaletteRepeat.Value;
            Data.Bars_Highlight = checkBoxBarsHighlight.Checked;
            Data.Bars_3D = checkBoxBars3D.Checked;
            if (comboBoxBarsDirection.SelectedItem != null)
            {
                switch (comboBoxBarsDirection.SelectedItem.ToString())
                {
                    case "Up":
                        Data.Bars_Direction = 0;
                        break;
                    case "Down":
                        Data.Bars_Direction = 1;
                        break;
                    case "Expand":
                        Data.Bars_Direction = 2;
                        break;
                    case "Compress":
                        Data.Bars_Direction = 3;
                        break;
                }
            }
        }

        private void trackBarPaletteRepeat_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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
            switch (Data.Butterfly_Colors)
            {
                case 0:
                    comboBoxButterflyColors.SelectedItem = "Rainbow";
                    break;
                case 1:
                    comboBoxButterflyColors.SelectedItem = "Palette";
                    break;
            }
        }

        private void Butterfly_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            if (loading) return;
            Data.Butterfly_Style = trackButterflyStyle.Value;
            Data.Butterfly_BkgrdChunks = trackButterflyBkgrdChunks.Value;
            Data.Butterfly_BkgrdSkip = trackButterflyBkgrdSkip.Value;
        }

        private void comboBoxButterflyColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) return;
            switch (comboBoxButterflyColors.SelectedItem.ToString())
            {
                case "Rainbow":
                    Data.Butterfly_Colors = 0;
                    break;
                case "Palette":
                    Data.Butterfly_Colors = 1;
                    break;
            }
        }

        #endregion

        #region ColorWash

        private void LoadColorWashData()
        {
            trackColorWashCount.Value = Data.ColorWash_Count;
            checkBoxColorWashHorizontalFade.Checked = Data.ColorWash_FadeHorizontal;
            checkBoxColorWashVerticalFade.Checked = Data.ColorWash_FadeVertical;
        }

        private void trackColorWashCount_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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
        }

        private void trackFireHeight_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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

        private void Garlands_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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

        private void Life_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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
            switch (comboBoxMeteorColors.SelectedItem.ToString())
            {
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

        private void Meteor_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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

        private void Fireworks_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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

        private void Snowflake_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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

        private void Snowstorm_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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

        private void Spirals_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
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
        }

        private void trackTwinkleCount_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            if (loading) return;
            Data.Twinkles_Count = trackTwinkleCount.Value;
        }

        #endregion // Twinkles

        #region Text

        private void LoadText()
        {
            textTextLine1.Text = Data.Text_Line1;
            textTextLine2.Text = Data.Text_Line2;
            comboBoxTextDirection.SelectedIndex = Data.Text_Direction;
            trackTextTop.Value = Data.Text_Top;
        }

        private void Text_TextChanged(object sender, EventArgs e)
        {
            if (loading) return;
            Data.Text_Line1 = textTextLine1.Text;
            Data.Text_Line2 = textTextLine2.Text;
            Data.Text_Direction = comboBoxTextDirection.SelectedIndex;
            //Data.Text_TextRotation = 
            //Data.Text_Left = 
        }

        private void trackTextTop_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            if (loading) return;
            Data.Text_Top = trackTextTop.Value;
        }

        private void buttonTextFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = Data.Text_Font;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                Data.Text_Font = fontDialog.Font;
            }
        }

        #endregion // Text

        #region Picture

        private void LoadPicture()
        {
            textPictureFileName.Text = Data.Picture_FileName;
            comboBoxPictureDirection.SelectedIndex = Data.Picture_Direction;
            trackPictureGifSpeed.Value = Data.Picture_GifSpeed;
        }

        #endregion // Picture

        private void buttonPictureSelect_Click(object sender, EventArgs e)
        {
            fileDialog.Filter = "jpg|*.jpg|jpeg|*.jpeg|gif|.gif|png|*.png|bmp|*.bmp|All Files|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                // Copy the file to the Vixen folder
                var imageFile = new System.IO.FileInfo(fileDialog.FileName);
                var destFileName = System.IO.Path.Combine(NutcrackerDescriptor.ModulePath, imageFile.Name);
                var sourceFileName = imageFile.FullName;
                if (sourceFileName != destFileName)
                {
                    System.IO.File.Copy(sourceFileName, destFileName, true);
                }

                textPictureFileName.Text = destFileName;
                Data.Picture_FileName = destFileName;
            }
        }

        private void comboBoxPictureDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) return;
            Data.Picture_Direction = comboBoxPictureDirection.SelectedIndex;
        }

        private void trackPictureGifSpeed_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            if (loading) return;
            Data.Picture_GifSpeed = trackPictureGifSpeed.Value;
        }


    }
}

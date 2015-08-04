using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using VixenModules.Effect.VerticalMeter;
using Common.ValueTypes;
using VixenModules.App.ColorGradients;
using System.Drawing.Drawing2D;
using VixenModules.Property.Color;
using VixenModules.Effect.AudioHelp;


namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    public partial class AudioMeterEffectEditorControlBase : UserControl, IEffectEditorControl
    {

        public AudioMeterEffectEditorControlBase()
        {
            InitializeComponent();
        }

        private bool _discreteColors;
        private IEnumerable<Color> _validDiscreteColors;
        private IEffect _targetEffect;

        private MeterColorTypes _MeterColorStyle;
        private ColorGradient _ColorGradientValue;
        private bool _playing;
        private System.Threading.Timer _volumeTimer;

        public IEffect TargetEffect
        {
            get { return _targetEffect; }
            set
            {
                _targetEffect = value;
                _discreteColors = false;
                if (_targetEffect == null) return;

                HashSet<Color> validColors = new HashSet<Color>();

                // look for the color property of the target effect element, and restrict the gradient.
                // If it's a group, iterate through all children (and their children, etc.), finding as many color
                // properties as possible; then we can decide what to do based on that.
                validColors.AddRange(_targetEffect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
                _discreteColors = validColors.Any();
                _validDiscreteColors = validColors;

                UpdateVolumeGradientImage();
            }
        }

        private int _gain;
        private int _range;

        public int DecayTime { get { return (int)(DecayBar.Value); } set { DecayBar.Value = value; } }
        public int AttackTime { get { return (int)(AttackBar.Value); } set { AttackBar.Value = value; } }
        public bool Normalize { get { return (bool)(NormalizeCheckbox.Checked); } set { NormalizeCheckbox.Checked = value; } }
        public int Gain { get { return _gain; } set { _gain = value; GainBar.Value = 50 - value; } }
        public int Range { get { return _range; } set { _range = value; RangeBar.Value = 50-value-_gain; } }
        public int GreenPos { get { return GreenBar.Value; } set { GreenBar.Value = value; } }
        public int RedPos { get { return RedBar.Value; } set { RedBar.Value = value; } }
        public ColorGradient ColorGradientValue { get { return _ColorGradientValue; } set { _ColorGradientValue = value; UpdateVolumeGradientImage(); } }
        #region MeterColorStyle
        public MeterColorTypes MeterColorStyle
        {
            get { return _MeterColorStyle; }
            set
            {
                _MeterColorStyle = value;
                switch (value)
                {
                    case MeterColorTypes.Custom:
                        CustomRadio.Checked = true;
                        updateVolumeMeterColorGradient();
                        break;
                    case MeterColorTypes.Linear:
                        LinearRadio.Checked = true;
                        updateVolumeMeterColorGradient();
                        break;
                    case MeterColorTypes.Discrete:
                        DiscreteRadio.Checked = true;
                        updateVolumeMeterColorGradient();
                        break;
                }

            }
        }
    #endregion

        public virtual object[] EffectParameterValues
        {
            get;
            set;
        }

        private void VerticalMeterEffectEditorControl_Load(object sender, EventArgs e)
        {
            _playing = false;
            if (!this.DesignMode)
            {
                AudioHelp = new AudioHelper((EffectModuleInstanceBase)TargetEffect);
            }
        }

        private void VolumeColorCurveEditor_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }



        private void UpdateVolumeGradientImage()
        {
            if (_ColorGradientValue == null)
            {
                _ColorGradientValue = new ColorGradient();
            }

            Size rotatedSize = new Size(VolumeGradientPanel.Size.Height, VolumeGradientPanel.Size.Width);
            Bitmap rotatedGradient = _ColorGradientValue.GenerateColorGradientImage(rotatedSize, _discreteColors);
            rotatedGradient.RotateFlip(RotateFlipType.Rotate270FlipNone);
            VolumeGradientPanel.BackgroundImage = rotatedGradient;
        }

        private void updateVolumeMeterColorGradient()
        {
            if (MeterColorStyle == MeterColorTypes.Discrete)
            {
                Color[] myColors = { Color.Lime, Color.Lime, Color.Yellow, Color.Yellow, Color.Red, Color.Red };
                float greenPos = (float)GreenBar.Value/100;
                float yellowPos = (float)RedBar.Value/100;
                float[] myPositions = { .000001F, greenPos, greenPos + .000001F, yellowPos, yellowPos + .000001F, 1 };
                ColorBlend linearBlend = new ColorBlend();
                linearBlend.Colors = myColors;
                linearBlend.Positions = myPositions;
                ColorGradient linearGradient = new ColorGradient(linearBlend);
                _ColorGradientValue = linearGradient;
                UpdateVolumeGradientImage();
            }
            else if (MeterColorStyle == MeterColorTypes.Linear)
            {
                Color[] myColors = { Color.Lime, Color.Yellow, Color.Red };
                float[] myPositions = { (float)0.00000000000001, (float)GreenBar.Value / 100, (float)RedBar.Value / 100 };
                ColorBlend linearBlend = new ColorBlend();
                linearBlend.Colors = myColors;
                linearBlend.Positions = myPositions;

                ColorGradient linearGradient = new ColorGradient(linearBlend);
                _ColorGradientValue = linearGradient;
                UpdateVolumeGradientImage();
            }
        }

        private void DiscreteRadio_MouseClick(object sender, MouseEventArgs e)
        {
            _MeterColorStyle = MeterColorTypes.Discrete;
            updateVolumeMeterColorGradient();
        }

        private void LinearRadio_MouseClick(object sender, MouseEventArgs e)
        {
            _MeterColorStyle = MeterColorTypes.Linear;
            updateVolumeMeterColorGradient();
        }

        private void CustomRadio_Click(object sender, EventArgs e)
        {
            _MeterColorStyle = MeterColorTypes.Custom;
            VolumeGradientPanel_Click(null, null);
        }

        private void RedBar_Scroll(object sender, EventArgs e)
        {
            updateVolumeMeterColorGradient();
        }

        private void GreenBar_Scroll(object sender, EventArgs e)
        {
            updateVolumeMeterColorGradient();
        }

        private void VolumeGradientPanel_Click(object sender, EventArgs e)
        {
            MeterColorStyle = MeterColorTypes.Custom;
            using (ColorGradientEditor cge = new ColorGradientEditor(_ColorGradientValue, _discreteColors, _validDiscreteColors))
            {
                foreach (Control control in cge.Controls)
                    if (control is GradientEditPanel)
                    {
                        ((GradientEditPanel)control).LockColorEditorHSV_Value = false;
                        break;
                    }
                
                DialogResult result = cge.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _ColorGradientValue = cge.Gradient;
                    UpdateVolumeGradientImage();
                }
            }
        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void _StopPlayback()
        {
            previewButton.Text = "Play";
            _volumeTimer.Dispose();
            AudioHelp.StopPlayback();
            AudioHelp.freeMem();
            _playing = false;
        }

        private void VolumeMeterUpdate(object o)
        {
            //33000 - Buffer size of the fmod wrapper (int)(33000.0 / AudioHelp.AudioSampleRate * 1000)
            int time = AudioHelp.GetCurrentPlaybackTime() - (int)AudioHelp.MediaStartTime.TotalMilliseconds;

            if (time > (AudioHelp.EffectDuration))
            {
                //_StopPlayback modifies the UI
                this.Invoke((MethodInvoker)delegate
                {
                    _StopPlayback();
                });
                return;
            }


            double volume;
            if (time < 0)
                volume = -50;
            else
                volume = AudioHelp.VolumeAtTime(time);

            if (volume < -50) volume = -50.0;
            if (volume > 0) volume = 0;
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    VolumeMeter.Value = 100 - (int)(volume / -50 * 100);
                });
            }
            catch { }
        }

        private AudioHelper AudioHelp;
        private System.Threading.TimerCallback TimerDelegate;
        private System.Threading.Timer TimerItem;

        private void previewButton_Click(object sender, EventArgs e)
        {
            
            if (_playing)
            {
                _StopPlayback();
            }
            else
            {
                previewButton.Text = "Loading";
                previewButton.Refresh();
                AudioHelp.AttackTime = AttackTime;
                AudioHelp.DecayTime = DecayTime;
                AudioHelp.Gain = 0;
                AudioHelp.Normalize = Normalize;

                AudioHelp.ReloadAudio();
                AudioHelp.StartPlayback();
                VolumeMeter.Value = 0;
                TimerDelegate = new System.Threading.TimerCallback(VolumeMeterUpdate);
                TimerItem = new System.Threading.Timer(TimerDelegate, null, 0, 30);
                _volumeTimer = TimerItem;
                previewButton.Text = "Stop";
                _playing = true;
            }
        }

        private void GainBar_Scroll(object sender, EventArgs e) //Max
        {
            _gain = 50 - GainBar.Value;
            _range = (50 - RangeBar.Value) - _gain;
        }

        private void RangeBar_Scroll(object sender, EventArgs e) //Min
        {
            _range = (50 - RangeBar.Value) - _gain;
        }

        private void VerticalMeterEffectEditorControl_Validated(object sender, EventArgs e)
        {
            if(_volumeTimer != null)
                _volumeTimer.Dispose();
            AudioHelp.StopPlayback();
            AudioHelp.freeMem();
        }

        private void VerticalMeterEffectEditorControl_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GainBar.Value <= RangeBar.Value)
            {
                MessageBox.Show("Maximum volume slider must be greater than minimum volume slider.");
                e.Cancel = true;
            }
            if (RedBar.Value <= GreenBar.Value)
            {
                MessageBox.Show("Invalid coloring, left color must be higher than right color, else the universe will implode.");
                e.Cancel = true;
            }
        }

        private void AttackBar_Scroll(object sender, EventArgs e)
        {

        }





    }
}

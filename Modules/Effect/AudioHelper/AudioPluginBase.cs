using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Property.Color;
using System.Windows.Forms;
using Vixen.Module.Media;
using Vixen.Services;
using VixenModules.Media.Audio;
using Vixen.Execution;
using Vixen.Execution.Context;
using VixenModules.Effect.AudioHelp;
using System.Drawing.Drawing2D;

namespace VixenModules.Effect.AudioHelp
{
    public enum MeterColorTypes { Linear, Discrete, Custom };

    public abstract class AudioPluginBase : EffectModuleInstanceBase
    {
        protected IAudioPluginData _data;
        protected EffectIntents _elementData = null;
        protected AudioHelper _audioHelper;
        protected Color[] _colors;
        public AudioHelper AudioHelper { get { return _audioHelper; } }

        private ColorGradient _workingGradient;

        #region Attribute Accessors


        [Value]
        public int DecayTime
        {
            get { return _data.DecayTime; }
            set
            {
                _data.DecayTime = value;
                _audioHelper.DecayTime = value;
                IsDirty = true;
            }
        }

        [Value]
        public int AttackTime
        {
            get { return _data.AttackTime; }
            set
            {
                _data.AttackTime = value;
                _audioHelper.AttackTime = value;
                IsDirty = true;
            }
        }

        [Value]
        public bool Normalize
        {
            get { return _data.Normalize; }
            set
            {
                _audioHelper.Normalize = value;
                _data.Normalize = value;
                IsDirty = true;
            }
        }

        [Value]
        public int Gain
        {
            get { return _data.Gain; }
            set
            {
                _data.Gain = value;
                _audioHelper.Gain = value;
                IsDirty = true;
            }
        }

        [Value]
        public int Range
        {
            get { return _data.Range; }
            set
            {
                _data.Range = value;
                IsDirty = true;
            }
        }

        [Value]
        public int GreenColorPosition
        {
            get { return _data.GreenColorPosition; }
            set
            {
                _data.GreenColorPosition = value;
                updateColorGradient();
                IsDirty = true;
            }
        }

        [Value]
        public int RedColorPosition
        {
            get { return _data.RedColorPosition; }
            set
            {
                _data.RedColorPosition = value;
                updateColorGradient();
                IsDirty = true;
            }
        }

        [Value]
        public ColorGradient MeterColorGradient
        {
            get { return _data.MeterColorGradient; }
            set
            {
                _data.MeterColorGradient = value;
                updateColorGradient();
                IsDirty = true;
            }
        }

        [Value]
        public MeterColorTypes MeterColorStyle
        {
            get { return _data.MeterColorStyle; }
            set
            {
                _data.MeterColorStyle = value;
                updateColorGradient();
                IsDirty = true;
            }
        }

        #endregion


        public AudioPluginBase()
        {
        }

        public Color GetColorAt(double pos)
        {
            if (_workingGradient == null) return Color.Black;
            return _workingGradient.GetColorAt(pos);
        }

        protected void updateColorGradient(){
            if (_data == null)
                return;

            switch (_data.MeterColorStyle)
            {
                case MeterColorTypes.Discrete:
                    Color[] discreteColors = { Color.Lime, Color.Lime, Color.Yellow, Color.Yellow, Color.Red, Color.Red };
                    float greenPos = (float)_data.GreenColorPosition / 100;
                    float yellowPos = (float)_data.RedColorPosition / 100;
                    float[] DiscretePositions = { .000001F, greenPos, greenPos + .000001F, yellowPos, yellowPos + .000001F, 1 };
                    ColorBlend discreteBlend = new ColorBlend();
                    discreteBlend.Colors = discreteColors;
                    discreteBlend.Positions = DiscretePositions;
                    ColorGradient discreteGradient = new ColorGradient(discreteBlend);
                    _workingGradient = discreteGradient;
                    break;
                case MeterColorTypes.Linear:
                    Color[] linearColors = { Color.Lime, Color.Yellow, Color.Red };
                    float[] myPositions = { (float)0.00000000000001, (float)_data.GreenColorPosition / 100, (float)_data.RedColorPosition / 100 };
                    ColorBlend linearBlend = new ColorBlend();
                    linearBlend.Colors = linearColors;
                    linearBlend.Positions = myPositions;

                    ColorGradient linearGradient = new ColorGradient(linearBlend);
                    _workingGradient = linearGradient;
                    break;
                case MeterColorTypes.Custom: _workingGradient = _data.MeterColorGradient; break;
            }
        }

        protected override void _PreRender(CancellationTokenSource tokenSource = null)
        {
            _elementData = new EffectIntents();

            _audioHelper.ReloadAudio();

            foreach (ElementNode node in TargetNodes)
            {
                if (tokenSource != null && tokenSource.IsCancellationRequested)
                    return;

                if (node != null)
                    RenderNode(node);
            }
            _audioHelper.freeMem();
        }

        protected override EffectIntents _Render()
		{
			return _elementData;
		}

        public override IModuleDataModel ModuleData
		{
            get { return _data; }
			set {
                _data = value as IAudioPluginData;
                updateColorGradient();
                _audioHelper.DecayTime = _data.DecayTime;
                _audioHelper.AttackTime = _data.AttackTime;
                _audioHelper.Gain = _data.Gain;
                _audioHelper.Normalize = _data.Normalize;
                _audioHelper.ReloadAudio();
            }
		}

        public override bool IsDirty
		{
			get
			{
				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

        protected abstract void RenderNode(ElementNode node);
    }
}

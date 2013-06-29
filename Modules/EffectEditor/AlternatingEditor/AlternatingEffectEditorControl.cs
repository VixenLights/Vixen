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
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Sys;
using VixenModules.App.ColorGradients;

namespace VixenModules.EffectEditor.AlternatingEditor
{
	public partial class AlternatingEffectEditorControl : UserControl, IEffectEditorControl
	{
		public AlternatingEffectEditorControl()
		{
			InitializeComponent();
			numChangeInterval.Maximum = trackBarInterval.Maximum;
			numChangeInterval.Minimum = trackBarInterval.Minimum;
		}

		public object[] EffectParameterValues
		{
			get
			{
				return new object[]
				       	{
				       		Level1,
				       		Color1,
				       		Level2,
				       		Color2,
				       		Interval,
				       		DepthOfEffect,
				       		GroupInterval,
				       		Enabled,
				       		StaticColor1,
				       		StaticColor2,
				       		ColorGradient1,
				       		ColorGradient2
				       	};
			}
			set
			{
				if (value.Length != 12) {
					VixenSystem.Logging.Warning("Alternating effect parameters set with " + value.Length + " parameters");
					return;
				}
				var val = value;
				Level1 = (double) value[0];
				Color1 = (Color) value[1];
				Level2 = (double) value[2];
				Color2 = (Color) value[3];
				Interval = (int) value[4];
				DepthOfEffect = (int) value[5];
				GroupInterval = (int) value[6];
				Enabled = (bool) value[7];
				StaticColor1 = (bool) value[8];
				StaticColor2 = (bool) value[9];
				ColorGradient1 = (ColorGradient) value[10];
				ColorGradient2 = (ColorGradient) value[11];
			}
		}


		private IEffect _targetEffect;

		public IEffect TargetEffect
		{
			get { return _targetEffect; }
			set { _targetEffect = value; }
		}

		public int Interval
		{
			get { return trackBarInterval.Value; }
			set { trackBarInterval.Value = value > trackBarInterval.Maximum ? trackBarInterval.Maximum : value; }
		}

		public Color Color1
		{
			get { return colorTypeEditorControl1.ColorValue; }
			set { colorTypeEditorControl1.ColorValue = value; }
		}

		public Color Color2
		{
			get { return colorTypeEditorControl2.ColorValue; }
			set { colorTypeEditorControl2.ColorValue = value; }
		}

		public double Level1
		{
			get { return levelTypeEditorControl1.LevelValue; }
			set { levelTypeEditorControl1.LevelValue = value; }
		}

		public double Level2
		{
			get { return levelTypeEditorControl2.LevelValue; }
			set { levelTypeEditorControl2.LevelValue = value; }
		}

		public bool Enabled
		{
			get { return !this.chkEnabled.Checked; }
			set { this.chkEnabled.Checked = !value; }
		}

		public ColorGradient ColorGradient1
		{
			get { return this.gradient1.ColorGradientValue; }
			set { this.gradient1.ColorGradientValue = value; }
		}

		public ColorGradient ColorGradient2
		{
			get { return this.gradient2.ColorGradientValue; }
			set { this.gradient2.ColorGradientValue = value; }
		}

		public bool StaticColor1
		{
			get
			{
				return this.radioStatic1.Checked;
				;
			}
			set
			{
				this.radioStatic1.Checked = value;
				this.radioGradient1.Checked = !value;
			}
		}

		public bool StaticColor2
		{
			get { return this.radioStatic2.Checked; }
			set
			{
				this.radioStatic2.Checked = value;
				this.radioGradient2.Checked = !value;
			}
		}

		public int GroupInterval
		{
			get { return (int) numGroup.Value; }
			set { numGroup.Value = value; }
		}

		public int DepthOfEffect { get; set; }

		private void trackBarInterval_ValueChanged(object sender, EventArgs e)
		{
			numChangeInterval.Value = trackBarInterval.Value;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			trackBarInterval.Value = (int) numChangeInterval.Value;
		}
	}
}
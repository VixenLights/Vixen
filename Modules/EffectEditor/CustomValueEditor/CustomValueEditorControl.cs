using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;
using Vixen.Sys;
using VixenModules.Effect.CustomValue;

namespace VixenModules.EffectEditor.CustomValueEditor
{
	public partial class CustomValueEditorControl : UserControl, IEffectEditorControl
	{
		public CustomValueEditorControl()
		{
			InitializeComponent();
		}

		public object[] EffectParameterValues
		{
			get
			{
				return new object[]
				       	{
				       		ValueType,
				       		Value8Bit,
				       		Value16Bit,
				       		Value32Bit,
				       		Value64Bit,
				       		ColorValue,
				       		StringValue
				       	};
			}
			set
			{
				if (value.Length != 7) {
					VixenSystem.Logging.Warning("Custom Value effect parameters set with " + value.Length + " parameters");
					return;
				}

				ValueType = (CustomValueType)value[0];
				Value8Bit = (byte)value[1];
				Value16Bit = (ushort)value[2];
				Value32Bit = (uint)value[3];
				Value64Bit = (ulong)value[4];
				ColorValue = (Color)value[5];
				StringValue = (string)value[6];
			}
		}

		public IEffect TargetEffect { get; set; }

		private void radioButtonTypes_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDown8bit.Enabled = radioButton8bit.Checked;
			numericUpDown16bit.Enabled = radioButton16bit.Checked;
			numericUpDown32bit.Enabled = radioButton32bit.Checked;
			numericUpDown64bit.Enabled = radioButton64bit.Checked;
			colorTypeEditorControlColor.Enabled = radioButtonColor.Checked;
			textBoxStringValue.Enabled = radioButtonString.Checked;
		}

		public CustomValueType ValueType
		{
			get
			{
				if (radioButton8bit.Checked)
					return CustomValueType._8Bit;
				if (radioButton16bit.Checked)
					return CustomValueType._16Bit;
				if (radioButton32bit.Checked)
					return CustomValueType._32Bit;
				if (radioButton64bit.Checked)
					return CustomValueType._64Bit;
				if (radioButtonColor.Checked)
					return CustomValueType.Color;
				if (radioButtonString.Checked)
					return CustomValueType.String;

				throw new Exception("Unknown ValueType selected..?");
			}
			set
			{
				switch (value) {
					case CustomValueType._8Bit:
						radioButton8bit.Checked = true;
						break;

					case CustomValueType._16Bit:
						radioButton16bit.Checked = true;
						break;

					case CustomValueType._32Bit:
						radioButton32bit.Checked = true;
						break;

					case CustomValueType._64Bit:
						radioButton64bit.Checked = true;
						break;

					case CustomValueType.Color:
						radioButtonColor.Checked = true;
						break;

					case CustomValueType.String:
						radioButtonString.Checked = true;
						break;
				}
			}
		}

		public byte Value8Bit
		{
			get { return (byte)numericUpDown8bit.Value; }
			set { numericUpDown8bit.Value = value; }
		}

		public ushort Value16Bit
		{
			get { return (ushort)numericUpDown16bit.Value; }
			set { numericUpDown16bit.Value = value; }
		}

		public uint Value32Bit
		{
			get { return (uint)numericUpDown32bit.Value; }
			set { numericUpDown32bit.Value = value; }
		}

		public ulong Value64Bit
		{
			get { return (ulong)numericUpDown64bit.Value; }
			set { numericUpDown64bit.Value = value; }
		}

		public Color ColorValue
		{
			get { return colorTypeEditorControlColor.ColorValue; }
			set { colorTypeEditorControlColor.ColorValue = value; }
		}

		public string StringValue
		{
			get { return textBoxStringValue.Text; }
			set { textBoxStringValue.Text = value; }
		}
	}
}

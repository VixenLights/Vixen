using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.CustomValue
{
	public class CustomValueDescriptor : EffectModuleDescriptorBase
	{
		public static Guid TypeGuid = new Guid("0c4095bf-a9b4-425b-97b8-c0251dd5ad47");

		public override string TypeName
		{
			get { return "Custom Value"; }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Basic; }
		}

		public override Guid TypeId
		{
			get { return TypeGuid; }
		}

		public override Type ModuleClass
		{
			get { return typeof(CustomValueModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (CustomValueData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "An effect that can output a specific command value to be passed to controllers."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Custom Value"; }
		}

		public override ParameterSignature Parameters
		{
			get
			{
				return new ParameterSignature(
					new ParameterSpecification("Value Type", typeof (CustomValueType)),
					new ParameterSpecification("8-bit value", typeof (byte)),
					new ParameterSpecification("16-bit value", typeof (ushort)),
					new ParameterSpecification("32-bit value", typeof (uint)),
					new ParameterSpecification("64-bit value", typeof (ulong)),
					new ParameterSpecification("color value", typeof (Color)),
					new ParameterSpecification("string value", typeof (string))
					);
			}
		}
	}




	[DataContract]
	public class CustomValueData : ModuleDataModelBase
	{
		[DataMember]
		public CustomValueType ValueType { get; set; }

		[DataMember]
		public byte Value8Bit { get; set; }

		[DataMember]
		public ushort Value16Bit { get; set; }

		[DataMember]
		public uint Value32Bit { get; set; }

		[DataMember]
		public ulong Value64Bit { get; set; }

		[DataMember]
		public Color ColorValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		public CustomValueData()
		{
			ValueType = CustomValueType._8Bit;
			ColorValue = Color.White;
			// presumably the rest will default to something sane (0's, nulls, etc.)
		}

		public override IModuleDataModel Clone()
		{
			CustomValueData result = new CustomValueData();
			result.ValueType = ValueType;
			result.Value8Bit = Value8Bit;
			result.Value16Bit = Value16Bit;
			result.Value32Bit = Value32Bit;
			result.Value64Bit = Value64Bit;
			result.ColorValue = ColorValue;
			result.StringValue = StringValue;
			return result;
		}
	}




	public class CustomValueModule : EffectModuleInstanceBase
	{
		private EffectIntents _elementData = null;

		private CustomValueData _data;

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as CustomValueData; }
		}

		public CustomValueModule()
		{
			//_data = new CustomValueData();
		}

		protected override void TargetNodesChanged()
		{
			//Nothing to do
		}

		protected override void _PreRender()
		{
			_elementData = new EffectIntents();

			if (_data == null)
				return;

			ICommand command = null;

			switch (ValueType) {
				case CustomValueType._8Bit:
					command = new _8BitCommand(Value8Bit);
					break;
				case CustomValueType._16Bit:
					command = new _16BitCommand(Value16Bit);
					break;
				case CustomValueType._32Bit:
					command = new _32BitCommand(Value32Bit);
					break;
				case CustomValueType._64Bit:
					command = new _64BitCommand(Value64Bit);
					break;
				case CustomValueType.Color:
					command = new ColorCommand(ColorValue);
					break;
				case CustomValueType.String:
					command = new StringCommand(StringValue);
					break;
			}

			CommandValue value = new CommandValue(command);

			foreach (ElementNode node in TargetNodes) {
				IIntent intent = new CommandIntent(value, TimeSpan);
				_elementData.AddIntentForElement(node.Element.Id, intent, TimeSpan.Zero);
			}
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		[Value]
		public CustomValueType ValueType
		{
			get { return _data.ValueType; }
			set
			{
				_data.ValueType = value;
				IsDirty = true;
			}
		}

		[Value]
		public byte Value8Bit
		{
			get { return _data.Value8Bit; }
			set
			{
				_data.Value8Bit = value;
				IsDirty = true;
			}
		}

		[Value]
		public ushort Value16Bit
		{
			get { return _data.Value16Bit; }
			set
			{
				_data.Value16Bit = value;
				IsDirty = true;
			}
		}

		[Value]
		public uint Value32Bit
		{
			get { return _data.Value32Bit; }
			set
			{
				_data.Value32Bit = value;
				IsDirty = true;
			}
		}

		[Value]
		public ulong Value64Bit
		{
			get { return _data.Value64Bit; }
			set
			{
				_data.Value64Bit = value;
				IsDirty = true;
			}
		}

		[Value]
		public Color ColorValue
		{
			get { return _data.ColorValue; }
			set
			{
				_data.ColorValue = value;
				IsDirty = true;
			}
		}

		[Value]
		public string StringValue
		{
			get { return _data.StringValue; }
			set
			{
				_data.StringValue = value;
				IsDirty = true;
			}
		}

	}



	public enum CustomValueType
	{
		_8Bit,
		_16Bit,
		_32Bit,
		_64Bit,
		Color,
		String,
	};
}

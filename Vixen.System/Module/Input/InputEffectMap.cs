using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
//using Vixen.Commands.KnownDataTypes;
using Vixen.Module.Effect;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.Module.Input
{
	[DataContract]
	//// Not excited about these.
		//[KnownType(typeof(Level))]
		//[KnownType(typeof(Position))]
		//[KnownType(typeof(System.Drawing.Color))]
	public class InputEffectMap : IInputEffectMap, IEquatable<InputEffectMap>, IEquatable<IInputEffectMap>
	{
		public InputEffectMap(IInputModuleInstance inputModule, IEffectModuleInstance effectModule, IInputInput input,
		                      int parameterIndex, IEnumerable<Guid> nodes)
		{
			InputModuleId = inputModule.InstanceId;
			InputId = input.Name;
			InputValueParameterIndex = parameterIndex;
			Nodes = nodes.ToArray();
			EffectModuleId = effectModule.Descriptor.TypeId;
			EffectParameterValues = effectModule.ParameterValues;
		}

		[DataMember]
		public Guid InputModuleId { get; set; }

		[DataMember]
		public string InputId { get; set; }

		[DataMember]
		public Guid EffectModuleId { get; set; }

		[DataMember]
		public object[] EffectParameterValues { get; set; }

		[DataMember]
		public int InputValueParameterIndex { get; set; }

		[DataMember]
		public Guid[] Nodes { get; set; }

		public bool IsMappedTo(IInputModuleInstance inputModule, IInputInput input = null)
		{
			return inputModule.InstanceId == InputModuleId && (input == null || input.Name == InputId);
		}

		public EffectNode GenerateEffect(IInputInput input, TimeSpan effectTimeSpan)
		{
			IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(EffectModuleId);
			EffectParameterValues[InputValueParameterIndex] = input.Value;
			effect.ParameterValues = EffectParameterValues;
			effect.TimeSpan = effectTimeSpan;
			effect.StartTime = TimeSpan.Zero;
			effect.TargetNodes = Nodes.Select(x => VixenSystem.Nodes.FirstOrDefault(y => y.Id == x)).ToArray();
			EffectNode effectNode = new EffectNode(effect, TimeSpan.Zero);

			return effectNode;
		}

		public bool Equals(InputEffectMap other)
		{
			return Equals(other as IInputEffectMap);
		}

		public bool Equals(IInputEffectMap other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return
				other.InputModuleId.Equals(InputModuleId) &&
				Equals(other.InputId, InputId) &&
				other.EffectModuleId.Equals(EffectModuleId) &&
				other.InputValueParameterIndex == InputValueParameterIndex;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (InputEffectMap)) return false;
			return Equals((InputEffectMap) obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				int result = InputModuleId.GetHashCode();
				result = (result*397) ^ (InputId != null ? InputId.GetHashCode() : 0);
				result = (result*397) ^ EffectModuleId.GetHashCode();
				result = (result*397) ^ InputValueParameterIndex;
				return result;
			}
		}
	}
}
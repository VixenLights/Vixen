namespace Vixen.Sys {
	public interface IHasOutputs {
		int OutputCount { get; }
		void AddOutput(Output.Output output);
		void RemoveOutput(Output.Output output);
		Output.Output[] Outputs { get; }
	}

	public interface IHasOutputs<T> : IHasOutputs
		where T : Vixen.Sys.Output.Output {
		void AddOutput(T output);
		void RemoveOutput(T output);
		T[] Outputs { get; }
	}
}

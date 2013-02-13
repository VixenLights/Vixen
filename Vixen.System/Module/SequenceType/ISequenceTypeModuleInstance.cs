namespace Vixen.Module.SequenceType {
	public interface ISequenceTypeModuleInstance : ISequenceType, IModuleInstance {
		bool IsCustomSequenceLoader
		{
			get;
		}

		Vixen.Sys.ISequence LoadSequenceFromFile(string filePath);
	}
}

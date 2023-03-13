namespace Vixen.Module.Editor
{
	public interface IEditorModuleDescriptor : IModuleDescriptor
	{
		Type SequenceType { get; }
		Type EditorUserInterfaceClass { get; }
	}
}
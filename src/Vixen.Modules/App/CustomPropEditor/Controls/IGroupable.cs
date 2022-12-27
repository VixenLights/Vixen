namespace VixenModules.App.CustomPropEditor.Controls
{
	public interface IGroupable
	{
		Guid ID { get; }
		Guid ParentID { get; set; }
		bool IsGroup { get; set; }
	}
}

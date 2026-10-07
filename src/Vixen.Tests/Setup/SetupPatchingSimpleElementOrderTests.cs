using System.Reflection;
using System.Windows.Forms;
using Common.Controls;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Tests.Core;
using VixenApplication.Setup;
using Xunit;

namespace Vixen.Tests.Setup;

[Collection(OutputControllerOutputIndexTestCollection.Name)]
public sealed class SetupPatchingSimpleElementOrderTests : IDisposable
{
	private readonly NodeManager _previousNodes = VixenSystem.Nodes;

	public SetupPatchingSimpleElementOrderTests()
	{
		SetVixenSystemProperty(nameof(VixenSystem.Nodes), new NodeManager());
	}

	[StaFact]
	public void RangeOrderFlowsThroughSetupSelectionEventAndPatchingCache()
	{
		using var setupElements = new SetupElementsTree([], []);
		using var patching = new SetupPatchingSimple();
		var elementTree = (ElementTree)typeof(SetupElementsTree)
			.GetField("elementTree", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(setupElements)!;
		var treeView = (MultiSelectTreeview)typeof(ElementTree)
			.GetField("treeview", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(elementTree)!;
		var elements = new[] {
			new ElementNode("A", null, Array.Empty<ElementNode>()),
			new ElementNode("B", null, Array.Empty<ElementNode>()),
			new ElementNode("C", null, Array.Empty<ElementNode>())
		};
		foreach (ElementNode element in elements) {
			treeView.Nodes.Add(new TreeNode(element.Name) { Tag = element });
		}

		List<ElementNode>? eventOrder = null;
		setupElements.ElementSelectionChanged += (_, args) => eventOrder = args.ElementNodes;
		treeView.SelectedNode = treeView.Nodes[2];
		SelectWithShift(treeView, treeView.Nodes[0]);

		Assert.Equal([elements[2], elements[1], elements[0]], eventOrder);
		Assert.Equal([elements[2], elements[1], elements[0]], setupElements.SelectedElements);
		patching.UpdateElementSelection(setupElements.SelectedElements);
		Assert.Equal([elements[2], elements[1], elements[0]], CachedElementNodes(patching));
	}

	public void Dispose()
	{
		SetVixenSystemProperty(nameof(VixenSystem.Nodes), _previousNodes);
	}

	private static void SetVixenSystemProperty(string propertyName, object value)
	{
		var property = typeof(VixenSystem).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
		property!.GetSetMethod(true)!.Invoke(null, [value]);
	}

	private static List<ElementNode> CachedElementNodes(SetupPatchingSimple control)
	{
		var field = typeof(SetupPatchingSimple).GetField("_cachedElementNodes", BindingFlags.Instance | BindingFlags.NonPublic);
		return Assert.IsType<List<ElementNode>>(field!.GetValue(control));
	}

	private static void SelectWithShift(MultiSelectTreeview treeView, TreeNode endpoint)
	{
		MethodInfo selectNode = typeof(MultiSelectTreeview).GetMethod(
			"SelectNode",
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[typeof(TreeNode), typeof(Keys)],
			null)!;
		selectNode.Invoke(treeView, [endpoint, Keys.Shift]);
	}
}

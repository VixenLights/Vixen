using Vixen.Extensions;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.ViewModels;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Views
{
	public partial class ElementMapperView
	{
		public ElementMapperView(ElementMapperViewModel vm)
		{
			InitializeComponent();
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
			DataContext = vm;
		}
	}
}

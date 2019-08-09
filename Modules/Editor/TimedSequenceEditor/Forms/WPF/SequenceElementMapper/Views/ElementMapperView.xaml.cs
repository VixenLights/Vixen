using Catel.IoC;
using Catel.Runtime.Serialization.Json;
using Vixen.Extensions;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Models;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Services;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.ViewModels;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Views
{
	public partial class ElementMapperView
	{
		public ElementMapperView(ElementMapperViewModel viewModel)
		{
			var serviceLocator = ServiceLocator.Default;
			if(!serviceLocator.IsTypeRegistered(typeof(IJsonSerializer)))
			{
				serviceLocator.RegisterType<IJsonSerializer, JsonSerializer>();
			}
			if(!serviceLocator.IsTypeRegistered(typeof(ModelPersistenceService<ElementMap>)))
			{
				serviceLocator.RegisterType<IModelPersistenceService<ElementMap>, ModelPersistenceService<ElementMap>>();
			}
			
			InitializeComponent();
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
			DataContext = viewModel;
		}
	}
}

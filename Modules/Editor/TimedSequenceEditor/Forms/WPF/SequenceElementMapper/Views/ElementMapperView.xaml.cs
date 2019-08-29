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
			if(!serviceLocator.IsTypeRegistered(typeof(IModelPersistenceService<ElementMap>)))
			{
				serviceLocator.RegisterType<IModelPersistenceService<ElementMap>, ModelPersistenceService<ElementMap>>();
			}
			if (!serviceLocator.IsTypeRegistered(typeof(IElementMapService)))
			{
				serviceLocator.RegisterType<IElementMapService, ElementMapService>();
			}

			InitializeComponent();
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
			DataContext = viewModel;
		}
	}
}

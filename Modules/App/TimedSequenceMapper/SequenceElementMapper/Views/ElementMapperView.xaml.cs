using Catel.IoC;
using Vixen.Extensions;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Models;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Services;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.ViewModels;

namespace VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Views
{
	public partial class ElementMapperView
	{
		public ElementMapperView(ElementMapperViewModel viewModel)
		{
			var serviceLocator = ServiceLocator.Default;
		
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

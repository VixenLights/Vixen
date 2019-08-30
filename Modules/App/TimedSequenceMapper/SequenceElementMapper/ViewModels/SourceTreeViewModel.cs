using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Catel.Data;
using Catel.MVVM;
using GongSolutions.Wpf.DragDrop;
using Vixen.Sys;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Models;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Services;

namespace VixenModules.App.TimedSequenceMapper.SequenceElementMapper.ViewModels
{
	public class SourceTreeViewModel:ViewModelBase, IDropTarget
	{
		private readonly IElementMapService _elementMapService;

		public SourceTreeViewModel(ObservableCollection<ElementNodeProxy> sourceNodes, IElementMapService elementMapService)
		{
			_elementMapService = elementMapService;
			SourceTreeNodes = sourceNodes;
		}

		#region SourceTreeNodes model property

		/// <summary>
		/// Gets or sets the SourceTreeNodes value.
		/// </summary>
		[Model]
		public ObservableCollection<ElementNodeProxy> SourceTreeNodes
		{
			get { return GetValue<ObservableCollection<ElementNodeProxy>>(SourceTreeNodesProperty); }
			private set { SetValue(SourceTreeNodesProperty, value); }
		}

		/// <summary>
		/// SourceTreeNodes property data.
		/// </summary>
		public static readonly PropertyData SourceTreeNodesProperty = RegisterProperty("SourceTreeNodes", typeof(ObservableCollection<ElementNodeProxy>));

		#endregion

		#region Implementation of IDropTarget

		/// <inheritdoc />
		public void DragOver(IDropInfo dropInfo)
		{
			var data = ExtractData(dropInfo.Data).OfType<object>().ToList();
			bool canDrop = data.First() is IElementNode;

			if (canDrop)
			{
				dropInfo.Effects = DragDropEffects.Copy;
				dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
			}
			else
			{
				dropInfo.Effects = DragDropEffects.None;
			}
		}

		/// <inheritdoc />
		public void Drop(IDropInfo dropInfo)
		{
			if (dropInfo?.DragInfo == null || dropInfo.TargetItem == null)
			{
				return;
			}

			var data = ExtractData(dropInfo.Data).OfType<object>().ToList();

			if (data.Any())
			{
				if (data.First() is IElementNode en)
				{
					if (dropInfo.TargetItem is ElementNodeProxy envm)
					{
						var map = _elementMapService.ElementMap.GetBySourceId(envm.Id);
						if (map!=null)
						{
							map.TargetId = en.Id;
							map.TargetName = en.Name;
						}
						else
						{
							ElementMapping em = new ElementMapping(envm.Id, envm.Name);
							em.TargetId = en.Id;
							em.TargetName = en.Name;
							_elementMapService.ElementMap.Add(em);
						}

						if (ParentViewModel is ElementMapperViewModel emvm)
						{
							emvm.MapModified = true;
						}
					}
				}
			}
		}

		public static IEnumerable ExtractData(object data)
		{
			if (data is IEnumerable && !(data is string))
			{
				return (IEnumerable)data;
			}
			else
			{
				return Enumerable.Repeat(data, 1);
			}
		}
		
		#endregion
	}
}

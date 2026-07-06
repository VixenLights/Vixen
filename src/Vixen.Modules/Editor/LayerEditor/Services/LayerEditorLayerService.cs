using Vixen.Module.MixingFilter;
using Vixen.Services;
using Vixen.Sys.LayerMixing;

namespace VixenModules.Editor.LayerEditor.Services
{
	/// <summary>
	/// Provides the default implementation of Layer Editor layer operations.
	/// </summary>
	public sealed class LayerEditorLayerService : ILayerEditorLayerService
	{
		/// <inheritdoc />
		public ILayer AddLayer(SequenceLayers layers, Guid filterTypeId)
		{
			ArgumentNullException.ThrowIfNull(layers);

			return AddLayer(layers, LayerMixingFilterService.Instance.GetInstance(filterTypeId));
		}

		/// <inheritdoc />
		public ILayer AddLayer(SequenceLayers layers, ILayerMixingFilterInstance layerMixingFilter)
		{
			ArgumentNullException.ThrowIfNull(layers);
			ArgumentNullException.ThrowIfNull(layerMixingFilter);

			layers.AddLayer(layerMixingFilter);
			return layers.Layers[0];
		}

		/// <inheritdoc />
		public bool RemoveLayer(SequenceLayers layers, ILayer layer)
		{
			if (layers == null || layer == null || layers.IsDefaultLayer(layer))
			{
				return false;
			}

			var index = layers.IndexOfLayer(layer);
			if (index < 0)
			{
				return false;
			}

			layers.RemoveLayerAt(index);
			return true;
		}

		/// <inheritdoc />
		public bool MoveLayer(SequenceLayers layers, ILayer layer, int newIndex)
		{
			if (layers == null || layer == null || layers.IsDefaultLayer(layer))
			{
				return false;
			}

			var oldIndex = layers.IndexOfLayer(layer);
			if (oldIndex < 0 || newIndex < 0 || newIndex >= layers.Count - 1 || oldIndex == newIndex)
			{
				return false;
			}

			layers.MoveLayer(oldIndex, newIndex);
			return true;
		}

		/// <inheritdoc />
		public bool ConfigureLayer(ILayerMixingFilterInstance layerMixingFilter)
		{
			return layerMixingFilter != null && layerMixingFilter.HasSetup && layerMixingFilter.Setup();
		}

		/// <inheritdoc />
		public bool QuickRenameLayer(SequenceLayers layers, ILayer layer)
		{
			if (layers == null || layer == null || layers.IsDefaultLayer(layer) || layer.LayerMixingFilter == null)
			{
				return false;
			}

			var desiredName = layer.LayerMixingFilter.Descriptor?.TypeName;
			if (string.IsNullOrWhiteSpace(desiredName))
			{
				return false;
			}

			layer.LayerName = CreateUniqueLayerName(layers.Layers.Where(existingLayer => !ReferenceEquals(existingLayer, layer)), desiredName);
			return true;
		}

		/// <inheritdoc />
		public string CreateUniqueLayerName(IEnumerable<ILayer> layers, string desiredName)
		{
			ArgumentNullException.ThrowIfNull(layers);

			if (string.IsNullOrWhiteSpace(desiredName))
			{
				return "Layer";
			}

			var existingNames = layers.Select(layer => layer.LayerName).ToHashSet(StringComparer.Ordinal);
			if (!existingNames.Contains(desiredName))
			{
				return desiredName;
			}

			var counter = 2;
			string candidate;
			do
			{
				candidate = $"{desiredName} - {counter++}";
			}
			while (existingNames.Contains(candidate));

			return candidate;
		}

		/// <inheritdoc />
		public bool HasExportableLayers(SequenceLayers layers)
		{
			return layers != null && layers.Layers.Any(layer => !layers.IsDefaultLayer(layer));
		}
	}
}

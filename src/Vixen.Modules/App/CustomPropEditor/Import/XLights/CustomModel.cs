using System.Collections.Generic;
using System.Threading.Tasks;
using VixenModules.App.CustomPropEditor.Import.XLights.Faces;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal class CustomModel
	{
		private int _y;
		private int _x;

		public CustomModel(string name)
		{
			SubModels = new List<SubModel>();
			FaceInfos = new List<FaceInfo>();
			StateInfos = new List<StateInfo>();
			Name = name;
			PixelSize = 1;
		}

		public string Name { get; private set; }

		public int PixelSize { get; set; }

		public string StrandNames { get; set; }

		public string NodeNames { get; set; }

		public string StringType { get; set; }

		/// <summary>
		/// Model size in the X dimension
		/// </summary>
		public int X
		{
			get => _x;
			set
			{
				_x = value;
				CalculateScale(X, Y);
			}
		}

		/// <summary>
		/// Model size in the Y direction
		/// </summary>
		public int Y
		{
			get => _y;
			set
			{
				_y = value; 
				CalculateScale(X, Y);
			}
		}

		internal int Scale { get; private set; }

		public string ModelDefinition { get; set; }

		public List<SubModel> SubModels { get; set; }

		public List<FaceInfo> FaceInfos { get; set; }

		public List<StateInfo> StateInfos { get; set; }

		private void CalculateScale(int x, int y)
		{
			if (x < 100 && y < 100)
			{
				Scale = 4;
			}
			else if (x < 200 && y < 200)
			{
				Scale = 2;
			}
			else
			{
				Scale = 1;
			}
		}

		internal async Task<Dictionary<int, ModelNode>> CreateModelNodesAsync()
		{
			var elementCandidates = new Dictionary<int, ModelNode>();
			await Task.Factory.StartNew(() =>
			{

				string[] rows = ModelDefinition.Split(';');
				int y = 1;
				foreach (var row in rows)
				{
					string[] nodes = row.Split(',');
					int x = 1;
					foreach (var node in nodes)
					{
						if (!string.IsNullOrEmpty(node))
						{
							int order;
							int.TryParse(node, out order);
							var modelNode = new ModelNode()
							{
								Order = order,
								X = x,
								Y = y
							};

							elementCandidates[order] = modelNode;
						}

						x += Scale;
					}

					y += Scale;
				}

			});

			return elementCandidates;

		}
	}
}

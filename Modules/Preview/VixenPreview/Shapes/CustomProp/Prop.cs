using Polenter.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	public class Prop
	{
	
		private int _width;
		private int _height;
		/// <summary>
		/// 
		/// </summary>
//public Square[,] Squares { get; set; }

		public DataTable Data { get; set; }
		private DataGridView _dataGrid;
		public Prop() { }

		public Prop(DataGridView dataGrid, int width, int height)
		{

			_dataGrid = dataGrid;
			_width = width;
			_height = height;
			GenerateGrid();
		}

		public Prop(Prop obj)
		{
			 
			this.Name = obj.Name;
			Channels = obj.Channels;
			this.Height = obj.Height;
			this.Width = obj.Width;
			GenerateGrid( );

		}

		public void UpdateGrid(int height, int width)
		{
			_width = width;
			_height = height;
			GenerateGrid( );
		}

		private void GenerateGrid( )
		{
			if (Data == null) Data = new DataTable();
			while (Data.Rows.Count < _height)
			{
				Data.Rows.Add(Data.NewRow());
			}
			while (Data.Rows.Count > _height)
			{
				Data.Rows.RemoveAt(Data.Rows.Count -1);
			}
			while (Data.Columns.Count < _width)
			{
				DataColumn column = new DataColumn();
		 
				Data.Columns.Add();
			}
			while (Data.Columns.Count > _width)
			{
				Data.Columns.RemoveAt(Data.Columns.Count - 1);
			}

		}
		
		//private void GenerateGrid(Square[,] squares = null)
		//{
		//	Panel.Enabled = true;

		//	if (Squares != null)
		//		foreach (Square s in Squares)
		//		{
		//			s.RemoveEvents();
		//		}

		//	Panel.Controls.Clear();
		//	Panel.SuspendLayout();
		//	Panel.Parent.SuspendLayout();
		//	Squares = new Square[Width, Height];

		//	for (int x = 0; x < Width; x++)
		//	{
		//		for (int y = 0; y < Height; y++)
		//		{
		//			Square s;
		//			if (squares != null)
		//			{
		//				try  //For now brute force..eventually check to ensure the matrix has the requested value...
		//				{
		//					s = new Square(this, squares[x, y].X, squares[x, y].Y, squares[x, y].ChannelID);
		//				}
		//				catch (Exception)
		//				{
		//					s = new Square(this, x, y);
		//				}

		//			}
		//			else
		//				s = new Square(this, x, y);

		//			Squares[x, y] = s;
		//		}
		//	}

		//	Panel.ResumeLayout();
		//	Panel.Parent.ResumeLayout();
		//}
		
		
		public static Prop FromFile(string fileName)
		{
			
			var serializer = new SharpSerializer();
			try
			{
				var prop = serializer.Deserialize(fileName) as Prop;
			
				return prop;
			}
			catch
			{
			}
			return null;

		}


		public DisplayItem ToDisplayItem()
		{
			//Theres got to be a better way to do this... LOL
			return (DisplayItem)PreviewTools.DeSerializeToDisplayItem(PreviewTools.SerializeToString(new PreviewCustomProp(new PreviewPoint(10, 10), null, 1, this)), typeof(DisplayItem));
		}


	 
		public int Height
		{
			set { _height = value; }
			get { return (this._height); }
		}


		public int Width
		{
			set { this._width = value; }
			get { return (this._width); }
		}

		public string Name { get; set; }

		public List<PropChannel> Channels { get; set; }

	 
		[ExcludeFromSerialization]
		public int SelectedChannelId { get; set; }

		[ExcludeFromSerialization]
		public string SelectedChannelName { get; set; }


	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Ink;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Collections;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;


namespace VixenModules.Preview.DisplayPreview.Model.Shapes
{
	[DataContract]
	internal class UserDefinedShape:Shape
	{
		private double _strokeThickness;
		private StrokeCollection _strokes;
		private string _strokeCollection;

		public UserDefinedShape()
		{
			if (_strokes == null)
			{
				_strokes = new StrokeCollection();
			}
			(_strokes as INotifyCollectionChanged).CollectionChanged += delegate
			{
				//changed
			};

		}
		[DataMember]
		protected string StringStrokeCollection
		{
			get
			{
				return _strokeCollection;
			}
			set
			{
				_strokeCollection = value;
			}
		}

		public double[] StrokeThicknesses
		{
			get
			{
				return new double[]{1,2,3,4,5,6,7,8,9};
			}
		}

		public override string Name
		{
			get
			{
				return "Line";
			}
		}

		public override ShapeType ShapeType
		{
			get
			{
				return ShapeType.UserDefinedShape;
			}
		}

		public StrokeCollection Strokes
		{
			get
			{
				return _strokes;
			}

			set
			{
				if (_strokes != null)
				{
					(_strokes as INotifyCollectionChanged).CollectionChanged -= delegate
					{
						//changed
						NotifyPropertyChanged("Strokes");
					};
				}
				
				_strokes = value;
				(_strokes as INotifyCollectionChanged).CollectionChanged += delegate
				{
					//changed
					NotifyPropertyChanged("Strokes");
				};
				NotifyPropertyChanged("Strokes");
			}
		}

		[DataMember]
		public double StrokeThickness
		{
			get
			{
				if (_strokeThickness <= 0)
				{
					_strokeThickness = 5;
				}

				return _strokeThickness;
			}

			set
			{
				_strokeThickness = value;
				NotifyPropertyChanged("StrokeThickness");
			}
		}

		public override IShape Clone()
		{
			return new UserDefinedShape { StrokeThickness = StrokeThickness,
					Strokes= Strokes.Clone()};
		}

		private void Initialize()
		{
			_strokeThickness = 0;	
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			Initialize();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			Strokes = StringToStroke(_strokeCollection);
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			_strokeCollection = StrokeToString(Strokes);
		}

		private static StrokeCollection StringToStroke(string stroke)
		{
			StrokeCollectionConverter converter = new StrokeCollectionConverter();
			return (StrokeCollection)converter.ConvertFromString(stroke);
		}

		private static string StrokeToString(StrokeCollection sc)
		{
			StrokeCollectionConverter converter = new StrokeCollectionConverter();
			string stringStroke = (string)converter.ConvertToString(sc);
			return stringStroke;
		}
		
	}

	


}

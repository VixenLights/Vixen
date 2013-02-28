using System;
using System.Runtime.Serialization;
using System.Windows.Ink;
using System.Collections.Specialized;
using System.Windows;
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
			(_strokes as INotifyCollectionChanged).CollectionChanged += StrokesChangedEventHandler;

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
				return new double[]{1,2,3,4,6,8,10,12};
			}
		}

		public override string Name
		{
			get
			{
				return "User Defined";
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
					(_strokes as INotifyCollectionChanged).CollectionChanged -= StrokesChangedEventHandler;
					
				}
				
				_strokes = value;
				(_strokes as INotifyCollectionChanged).CollectionChanged += StrokesChangedEventHandler;
				
				NotifyPropertyChanged("Strokes");
			}
		}

		private void StrokesChangedEventHandler(object sender, EventArgs e)
		{
			NotifyPropertyChanged("Strokes");	
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
				UpdateStrokeThickness();
				NotifyPropertyChanged("StrokeThickness");
			}
		}
		public override Brush Brush{
			set
			{
				base.Brush = value;
				UpdateStrokesBrush();
			}
		}
		private void UpdateStrokeThickness()
		{
			if (Strokes == null) return;
			foreach (Stroke stroke in Strokes)
			{
				stroke.DrawingAttributes.Width=StrokeThickness;
				stroke.DrawingAttributes.Height = StrokeThickness;
			}
			NotifyPropertyChanged("Strokes");
		}
		private void UpdateStrokesBrush()
		{
			if (Strokes == null) return;
			SolidColorBrush newBrush = (SolidColorBrush)Brush;
			
			foreach (Stroke stroke in Strokes)
			{
				stroke.DrawingAttributes.Color=newBrush.Color;	
			}
			NotifyPropertyChanged("Strokes");
		}
		public override IShape Clone()
		{
			return new UserDefinedShape { StrokeThickness = StrokeThickness,
					Strokes= Strokes.Clone()};
		}

		private void Initialize()
		{
			
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

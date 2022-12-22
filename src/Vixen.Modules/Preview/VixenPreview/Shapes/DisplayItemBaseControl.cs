﻿namespace VixenModules.Preview.VixenPreview.Shapes
{
	public partial class DisplayItemBaseControl : UserControl
	{
		public PreviewBaseShape _shape;
		private string _title;

		public DisplayItemBaseControl()
		{
			InitializeComponent();
		}

		public DisplayItemBaseControl(PreviewBaseShape shape)
		{
			_shape = shape;
			InitializeComponent();
		}

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public PreviewBaseShape Shape
		{
			get { return _shape; }
			set { _shape = value; }
		}

		protected void OnPropertyEdited()
		{
			PropertyEdited?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler<EventArgs> PropertyEdited;
	}
}
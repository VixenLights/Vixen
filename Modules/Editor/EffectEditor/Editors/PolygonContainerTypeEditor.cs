using Common.Controls;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using VixenModules.App.Polygon;
using VixenModules.Editor.PolygonEditor.Views;

namespace VixenModules.Editor.EffectEditor.Editors
{
	/// <summary>
	/// Displays the polygon editor.
	/// </summary>
	public class PolygonContainerTypeEditor : TypeEditor
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PolygonContainerTypeEditor() : base(typeof(PolygonContainer), EditorKeys.PolygonContainerEditorKey)
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Display the polygon editor.
		/// </summary>
		/// <param name="propertyItem">Property from effect</param>
		/// <param name="value">Polygon container that is being edited</param>
		/// <param name="commandSource">?</param>
		/// <returns>Edited polygon container</returns>
		public override object ShowDialog(PropertyItem propertyItem, object value, IInputElement commandSource)
		{
			try
			{				
				Debug.Assert(value != null);
				
				// Clone the polygon container
				PolygonContainer polygonContainer = ((PolygonContainer)value).Clone();					
			
				// Give the polygon container to the view
				PolygonEditorView newWindow = new PolygonEditorView(polygonContainer); 
				
				// Display the modal polygon editor
				bool? okButton = newWindow.ShowDialog();

				// If the OK button was pressed then...
				if (okButton.HasValue && okButton.Value)
				{
					// Create a new polygon container to the effect property change logic is triggered
					PolygonContainer polygonContainerUpdated = new PolygonContainer();
					polygonContainerUpdated.Polygons.AddRange(newWindow.Polygons);
					polygonContainerUpdated.PolygonTimes.AddRange(newWindow.PolygonTimes);
					polygonContainerUpdated.Lines.AddRange(newWindow.Lines);
					polygonContainerUpdated.LineTimes.AddRange(newWindow.LineTimes);
					polygonContainerUpdated.Ellipses.AddRange(newWindow.Ellipses);
					polygonContainerUpdated.EllipseTimes.AddRange(newWindow.EllipseTimes);

					polygonContainerUpdated.Width = polygonContainer.Width;
					polygonContainerUpdated.Height = polygonContainer.Height;

					// Set the return value
					value = polygonContainerUpdated;
				}					
			}
			catch (Exception e)
			{
				var messageBox = new MessageBoxForm("An error occurred loading the polygon editor.", "Polygon Error", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();				
			}

			// Return the updated polygon container
			return value;
		}

		#endregion
	}
}

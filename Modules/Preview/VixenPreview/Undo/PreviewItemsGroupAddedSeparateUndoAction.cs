using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class PreviewItemsGroupAddedSeparateUndoAction : Common.Controls.UndoAction
	{
		private VixenPreviewControl m_form;
		private DisplayItem m_newDisplay;

		public PreviewItemsGroupAddedSeparateUndoAction(VixenPreviewControl form, DisplayItem newDisplayItem)
		{
			m_form = form;
			m_newDisplay = newDisplayItem;
		}

		protected void removeEffects()
		{
			m_form.SelectedDisplayItems.Clear();

			if (m_newDisplay.IsLightShape())
			{
				PreviewPoint translatedPoint = new PreviewPoint(m_newDisplay.LightShape.Pixels[0].X, m_newDisplay.LightShape.Pixels[0].Y);
				m_form.SelectItemUnderPoint(translatedPoint, false);
			}
			
			m_form.SeparateTemplateItems(m_newDisplay);
		}

		protected void addGroupEffects()
		{
			DisplayItem selectDisplayItem;
			var nextShape = false;
			m_form.SelectedDisplayItems.Clear();

			if (m_newDisplay.IsLightShape())
			{
				foreach (var shape in m_newDisplay.LightShape.Strings)
				{
					PreviewPoint translatedPoint = new PreviewPoint(shape.Pixels[0].X, shape.Pixels[0].Y);
					m_form.SelectItemUnderPoint(translatedPoint, nextShape);

					nextShape = true;
				}
			}
			m_form.AddNewGroup(out selectDisplayItem, m_form.SelectedDisplayItems);
		}
	}
}
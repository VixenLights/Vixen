using Common.Controls.Scaling;
using Common.Resources.Properties;
using Vixen.Services;
using Vixen.Sys;

namespace VixenModules.Editor.TimedSequenceEditor
{
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	internal class TimedSequenceRowLabel : Common.Controls.Timeline.RowLabel
	{
		private const int TagDotDiameter = 8;
		private const int TagDotSpacing = 3;

		private static readonly Bitmap IconOpen = Resources.bullet_toggle_minus_16px;
		private static readonly Bitmap IconClosed = Resources.bullet_toggle_plus_16px;
		private static readonly Bitmap IconActiveOpen = Resources.bullet_toggle_minus_active_16px;
		private static readonly Bitmap IconActiveClosed = Resources.bullet_toggle_plus_active_16px;
		private static readonly double ScaleFactor = ScalingTools.GetScaleFactor();

		static TimedSequenceRowLabel()
		{
			ToggleTreeButtonWidth = (int)(IconOpen.Width * ScaleFactor + 8);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (ParentRow.HasExpandableChildRows)
			{

				Bitmap icon;
				var showChildActive = ShowActiveIndicators && ChildActiveIndicator();
				if (showChildActive)
				{
					icon = ParentRow.TreeOpen ? IconActiveOpen : IconActiveClosed;
				}
				else
				{
					icon = ParentRow.TreeOpen ? IconOpen : IconClosed;
				}


				int x = (int)(IconArea.Width - icon.Width*ScaleFactor)/2;
				int y = (int)(IconArea.Height - icon.Height*ScaleFactor)/2;
				if (showChildActive)
				{
					e.Graphics.FillRectangle(Brushes.Yellow, x, y, icon.Width, icon.Height);
				}
				e.Graphics.DrawImage(icon, x, y);
			}

			DrawTagColorDots(e);
		}

		private void DrawTagColorDots(PaintEventArgs e)
		{
			if (ParentRow.Tag is not ElementNode elementNode)
			{
				return;
			}

			List<ElementTagDefinition> coloredTags = GetColoredTags(elementNode);
			if (coloredTags.Count == 0)
			{
				return;
			}

			using Font font = GetLabelFont();
			Size textSize = TextRenderer.MeasureText(Name, font);
			int x = LabelArea.X + textSize.Width + TagDotSpacing * 2;
			int y = LabelArea.Y + (LabelArea.Height - TagDotDiameter) / 2;

			foreach (ElementTagDefinition tag in coloredTags)
			{
				using var brush = new SolidBrush(ColorTranslator.FromHtml(tag.DisplayColor));
				e.Graphics.FillEllipse(brush, x, y, TagDotDiameter, TagDotDiameter);
				x += TagDotDiameter + TagDotSpacing;
			}
		}

		private static List<ElementTagDefinition> GetColoredTags(ElementNode elementNode)
		{
			var tags = new List<ElementTagDefinition>();
			foreach (Guid tagId in elementNode.Tags)
			{
				ElementTagDefinition tag = ElementTagService.Instance.GetById(tagId);
				if (tag != null && !string.IsNullOrEmpty(tag.DisplayColor))
				{
					tags.Add(tag);
				}
			}
			tags.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));
			return tags;
		}
	}
}
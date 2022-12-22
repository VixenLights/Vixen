﻿using System.Windows;

namespace VixenModules.Editor.EffectEditor.Input
{
	public interface IDropTargetAdvisor
	{
		UIElement TargetUI { get; set; }

		bool ApplyMouseOffset { get; }
		bool IsValidDataObject(IDataObject obj);
		void OnDropCompleted(IDataObject obj, Point dropPoint);
		UIElement GetVisualFeedback(IDataObject obj);
		UIElement GetTopContainer();
	}
}
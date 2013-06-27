/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.WinFormsUI
{
	/// <summary>
	/// Helper class implementing methods for converting NShape classes and types to WinForms specific classes and types.
	/// </summary>
	internal static class WinFormHelpers
	{
		#region [Public] Convert WinForms MouseEventArgs to NShape MouseEventArgsDg

		/// <summary>
		/// Extracts and returns NShapeMouseEventArgs from <see cref="T:Windows.Forms.MouseEventArgs" />.
		/// </summary>
		public static MouseEventArgsDg GetMouseEventArgs(MouseEventType eventType, MouseEventArgs e)
		{
			return GetMouseEventArgs(eventType, e.Button, e.Clicks, e.X, e.Y, e.Delta);
		}


		/// <summary>
		/// Extracts and returns NShapeMouseEventArgs from <see cref="T:Windows.Forms.MouseEventArgs" />.
		/// </summary>
		public static MouseEventArgsDg GetMouseEventArgs(MouseEventType eventType, MouseEventArgs e, Rectangle controlBounds)
		{
			return GetMouseEventArgs(eventType, e.Button, e.Clicks, e.X, e.Y, e.Delta, controlBounds);
		}


		/// <summary>
		/// Returns NShapeMouseEventArgs extracted from information provided by the <see cref="T:System.Windows.Forms.Control" /> class.
		/// </summary>
		public static MouseEventArgsDg GetMouseEventArgs(Control control, MouseEventType eventType, int clicks, int delta)
		{
			Point mousePos = control.PointToClient(Control.MousePosition);
			return GetMouseEventArgs(eventType, Control.MouseButtons, clicks, mousePos.X, mousePos.Y, delta, control.Bounds);
		}


		/// <summary>
		/// Returns NShapeMouseEventArgs buildt with the provided parameters
		/// </summary>
		public static MouseEventArgsDg GetMouseEventArgs(MouseEventType eventType, MouseButtons mouseButtons, int clicks,
		                                                 int x, int y, int delta)
		{
			return GetMouseEventArgs(eventType, mouseButtons, clicks, x, y, delta, Geometry.InvalidRectangle);
		}


		/// <summary>
		/// Returns NShapeMouseEventArgs buildt with the provided parameters
		/// </summary>
		public static MouseEventArgsDg GetMouseEventArgs(MouseEventType eventType, MouseButtons mouseButtons, int clicks,
		                                                 int x, int y, int delta, Rectangle controlBounds)
		{
			mouseEventArgs.SetButtons(mouseButtons);
			mouseEventArgs.SetClicks(clicks);
			mouseEventArgs.SetEventType(eventType);
			if (Geometry.IsValid(controlBounds))
				mouseEventArgs.SetPosition(
					Math.Min(Math.Max(controlBounds.Left, x), controlBounds.Right),
					Math.Min(Math.Max(controlBounds.Top, y), controlBounds.Bottom)
					);
			else mouseEventArgs.SetPosition(x, y);
			mouseEventArgs.SetWheelDelta(delta);
			mouseEventArgs.SetModifiers(GetModifiers());
			return mouseEventArgs;
		}

		#endregion

		#region [Public] Convert WinForms KeyEventArgs to NShape KeyEventArgsDg

		/// <summary>
		/// Extracts and returns KeyEventArgs from Windows.Forms.KeyEventArgs
		/// </summary>
		public static KeyEventArgsDg GetKeyEventArgs(KeyEventType eventType, KeyEventArgs e)
		{
			return GetKeyEventArgs(eventType, '\0', (int) e.KeyData, e.Handled, e.SuppressKeyPress);
		}


		/// <summary>
		/// Extracts and returns KeyEventArgs from Windows.Forms.PreviewKeyDownEventArgs
		/// </summary>
		public static KeyEventArgsDg GetKeyEventArgs(PreviewKeyDownEventArgs e)
		{
			return GetKeyEventArgs(KeyEventType.PreviewKeyDown, '\0', (int) e.KeyData, false, false);
		}


		/// <summary>
		/// Extracts and returns KeyEventArgs from Windows.Forms.KeyPressEventArgs
		/// </summary>
		public static KeyEventArgsDg GetKeyEventArgs(KeyPressEventArgs e)
		{
			int keyData = (int) char.ToUpper(e.KeyChar) | (int) Control.ModifierKeys;
			return GetKeyEventArgs(KeyEventType.KeyPress, e.KeyChar, keyData, e.Handled, false);
		}


		/// <summary>
		/// Returns KeyEventArgs built with the provided parameters
		/// </summary>
		public static KeyEventArgsDg GetKeyEventArgs(KeyEventType eventType, char keyChar, int keyData, bool handled,
		                                             bool suppressKeyPress)
		{
			keyEventArgs.SetEventType(eventType);
			keyEventArgs.SetKeyChar(keyChar);
			keyEventArgs.SetKeyData(keyData);
			keyEventArgs.Handled = handled;
			keyEventArgs.SuppressKeyPress = suppressKeyPress;
			return keyEventArgs;
		}

		#endregion

		#region [Public] Build WinForms ContextMenus from NShape MenuItemDefs

		/// <summary>
		/// Creates a collection of ToolStripMenuItems from a collection of MenuItemDefs. Actions that are not allowed will be skipped.
		/// </summary>
		public static IEnumerable<ToolStripItem> GetContextMenuItemsFromAllowedActions(IEnumerable<MenuItemDef> actions,
		                                                                               Project project)
		{
			if (actions == null) throw new ArgumentNullException("actions");
			if (project == null) throw new ArgumentNullException("project");
			MenuItemDef lastMenuItemDef = null;
			// Attention!!
			// We have to iterate manually instead of unsing foreach here because otherwise always the least 
			// processed action's Execute method will be called.
			IEnumerator<MenuItemDef> enumerator = actions.GetEnumerator();
			while (enumerator.MoveNext()) {
				// Skip actions that are not allowed
				if (!enumerator.Current.IsGranted(project.SecurityManager)) continue;
				// If the item is a separator and no 'real' item was created before, skip the separator 
				// as we do not want a context  menu beginning with a menu seperator
				if (enumerator.Current is SeparatorMenuItemDef
				    && (lastMenuItemDef == null || lastMenuItemDef is SeparatorMenuItemDef)) continue;
				// Build and return menu item
				yield return CreateMenuItemFromAction(enumerator.Current, project);
				lastMenuItemDef = enumerator.Current;
			}
		}


		/// <summary>
		/// Creates a collection of ToolStripMenuItems from a collection of MenuItemDefs. 
		/// Actions that are not allowed will disabled and their tool tip displays the reason.
		/// </summary>
		public static IEnumerable<ToolStripItem> GetContextMenuItemsFromActions(IEnumerable<MenuItemDef> actions,
		                                                                        Project project)
		{
			if (actions == null) throw new ArgumentNullException("actions");
			if (project == null) throw new ArgumentNullException("project");
			// Attention!!
			// We have to iterate manually instead of unsing foreach here because otherwise always the least 
			// processed action's Execute method will be called.
			IEnumerator<MenuItemDef> enumerator = actions.GetEnumerator();
			while (enumerator.MoveNext()) {
				// Build and return menu item
				yield return CreateMenuItemFromAction(enumerator.Current, project);
			}
		}


		/// <summary>
		/// Fills the given ContextMenuStrip with ToolStripItems created from the given collection of MenuItemDefs.
		/// </summary>
		public static void BuildContextMenu(ContextMenuStrip contextMenuStrip, IEnumerable<MenuItemDef> actions,
		                                    Project project, bool skipIfNotGranted)
		{
			if (contextMenuStrip == null) throw new ArgumentNullException("contextMenuStrip");
			if (actions == null) throw new ArgumentNullException("actions");
			if (project == null) throw new ArgumentNullException("project");
			int itemCnt = contextMenuStrip.Items.Count;
			// Add a ToolStripSeparator if the last item is not a ToolStripSeparator
			if (itemCnt > 0 && !(contextMenuStrip.Items[itemCnt - 1] is ToolStripSeparator))
				contextMenuStrip.Items.Add(CreateMenuItemSeparator());
			IEnumerable<ToolStripItem> items =
				skipIfNotGranted
					? GetContextMenuItemsFromAllowedActions(actions, project)
					: GetContextMenuItemsFromActions(actions, project);
			foreach (ToolStripItem item in items) contextMenuStrip.Items.Add(item);
		}


		/// <summary>
		/// Fills the given ContextMenuStrip with ToolStripItems created from the given collection of MenuItemDefs.
		/// </summary>
		public static void BuildContextMenu(ContextMenuStrip contextMenu, IEnumerable<ToolStripItem> menuItems)
		{
			if (contextMenu == null) throw new ArgumentNullException("contextMenu");
			if (menuItems == null) throw new ArgumentNullException("menuItems");
			int itemCnt = contextMenu.Items.Count;
			if (itemCnt > 0 && !(contextMenu.Items[itemCnt - 1] is ToolStripSeparator))
				contextMenu.Items.Add(CreateMenuItemSeparator());
			foreach (ToolStripItem item in menuItems)
				contextMenu.Items.Add(item);
		}


		/// <summary>
		/// Removes all ToolStripItems formerly created from MenuItemDefs.
		/// </summary>
		public static void CleanUpContextMenu(ContextMenuStrip contextMenuStrip)
		{
			if (contextMenuStrip == null) throw new ArgumentNullException("contextMenuStrip");
			// Do not dispose the items here because the execute method of the attached action will be called later!
			for (int i = contextMenuStrip.Items.Count - 1; i >= 0; --i) {
				if (contextMenuStrip.Items[i].Tag is MenuItemDef)
					contextMenuStrip.Items.RemoveAt(i);
			}
		}

		#endregion

		#region [Private] Methods

		private static KeysDg GetModifiers()
		{
			// get Modifier Keys
			KeysDg result = KeysDg.None;
			if ((Control.ModifierKeys & Keys.Control) != 0)
				result |= KeysDg.Control;
			if ((Control.ModifierKeys & Keys.Shift) != 0)
				result |= KeysDg.Shift;
			if ((Control.ModifierKeys & Keys.Alt) != 0)
				result |= KeysDg.Alt;
			return result;
		}


		private static ToolStripItem CreateMenuItemFromAction(MenuItemDef action, Project project)
		{
			if (action is SeparatorMenuItemDef) return CreateMenuItemSeparator();
			else {
				// Build ContextMenu item
				ToolStripMenuItem menuItem = new ToolStripMenuItem(action.Title, null, (s, e) => action.Execute(action, project));
				menuItem.Tag = action;
				menuItem.Name = action.Name;
				menuItem.Text = action.Title;
				menuItem.Checked = action.Checked;
				//menuItem.CheckOnClick = false;
				menuItem.Enabled = (action.IsFeasible && action.IsGranted(project.SecurityManager));
				if (action.IsFeasible && !action.IsGranted(project.SecurityManager)) {
					if (action is DelegateMenuItemDef)
						menuItem.ToolTipText = string.Format("Action is deactivated because you don't have the permission for '{0}'.",
						                                     ((DelegateMenuItemDef) action).RequiredPermission);
					else menuItem.ToolTipText = "Action is deactivated because you don't have the required permissions.";
				}
				else menuItem.ToolTipText = action.Description;
				menuItem.Image = action.Image;
				menuItem.ImageTransparentColor = action.ImageTransparentColor;

				menuItem.DropDownItems.Clear();
				// Add sub menu items (do not skip any sub items: if parent is granted, subitems are granted too)
				if (action.SubItems != null) {
					for (int i = 0; i < action.SubItems.Length; ++i)
						menuItem.DropDownItems.Add(CreateMenuItemFromAction(action.SubItems[i], project));
				}
				return menuItem;
			}
		}


		private static ToolStripSeparator CreateMenuItemSeparator()
		{
			ToolStripSeparator result = new ToolStripSeparator();
			result.Tag = new SeparatorMenuItemDef();
			return result;
		}

		#endregion

		#region [Private] Types

		private class HelperMouseEventArgs : MouseEventArgsDg
		{
			public HelperMouseEventArgs()
				: base(MouseEventType.MouseMove, MouseButtonsDg.None, 0, 0, Point.Empty, KeysDg.None)
			{
			}


			public void SetButtons(MouseButtons mouseButtons)
			{
				MouseButtonsDg btns = MouseButtonsDg.None;
				if ((mouseButtons & MouseButtons.Left) > 0) btns |= MouseButtonsDg.Left;
				if ((mouseButtons & MouseButtons.Middle) > 0) btns |= MouseButtonsDg.Middle;
				if ((mouseButtons & MouseButtons.Right) > 0) btns |= MouseButtonsDg.Right;
				if ((mouseButtons & MouseButtons.XButton1) > 0) btns |= MouseButtonsDg.ExtraButton1;
				if ((mouseButtons & MouseButtons.XButton2) > 0) btns |= MouseButtonsDg.ExtraButton2;
				this.buttons = btns;
			}


			public void SetClicks(int clicks)
			{
				this.clicks = clicks;
			}


			public void SetEventType(MouseEventType eventType)
			{
				this.eventType = eventType;
			}


			public void SetPosition(int x, int y)
			{
				this.position.X = x;
				this.position.Y = y;
			}


			public void SetWheelDelta(int delta)
			{
				this.wheelDelta = delta;
			}


			public void SetModifiers(KeysDg modifiers)
			{
				this.modifiers = modifiers;
			}
		}


		private class HelperKeyEventArgs : KeyEventArgsDg
		{
			public HelperKeyEventArgs()
				: base(KeyEventType.PreviewKeyDown, 0, '\0', false, false)
			{
			}


			public void SetKeyData(int keyData)
			{
				this.keyData = keyData;
			}


			public void SetKeyChar(char keyChar)
			{
				this.keyChar = keyChar;
			}


			public void SetEventType(KeyEventType eventType)
			{
				this.eventType = eventType;
			}
		}

		#endregion

		#region [Private] Fields

		private static HelperMouseEventArgs mouseEventArgs = new HelperMouseEventArgs();
		private static HelperKeyEventArgs keyEventArgs = new HelperKeyEventArgs();

		#endregion
	}
}
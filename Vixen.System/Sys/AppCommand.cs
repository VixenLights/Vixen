using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace Vixen.Sys
{
	// The application will have an array of these.
	// Each one represents a top-level menu, a context menu, or a toolbar.
	public class AppCommand
	{
		// Could be a menu item, context menu item, toolstrip item.
		private ToolStripItem _toolStripItem;
		private readonly Control _rootControl;

		public const char PathDelimiter = '\\';

		private delegate void SafeCallDelegate<in T>(T cmd);

		public AppCommand(Control rootControl)
		{
			if (MainWindow == null && rootControl != null)
			{
				MainWindow = rootControl;
			}
			// Default to a menu command.
			Style = AppCommandStyle.Menu;
			Visible = true;
			Enabled = true;

			_rootControl = rootControl;
			foreach (AppCommand child in _items) {
				_AddToRoot(child);
			}
		}

		internal static Control MainWindow { get; set; }

		private void _AddToRoot(AppCommand appCommand)
		{
			if (MainWindow.InvokeRequired)
			{
				var d = new SafeCallDelegate<AppCommand>(_AddToRoot);
				MainWindow.Invoke(d, appCommand);
			}
			else
			{
				if (_rootControl != null) {
					switch (appCommand.Style) {
						case AppCommandStyle.Context:
							// Control, as-is
							// Don't do anything.  Not going to create a context menu and parent it.
							break;
						case AppCommandStyle.Menu:
							// MenuStrip child control
							if (_rootControl is Form) {
								MenuStrip menuStrip = (_rootControl as Form).MainMenuStrip;
								if (menuStrip != null) {
									menuStrip.Items.Add(appCommand.Item);
								}
							}
							else if (_rootControl is MenuStrip) {
								(_rootControl as MenuStrip).Items.Add(appCommand.Item);
							}
							break;
					}
				}
			}
			
		}

		private void _RemoveFromRoot(AppCommand appCommand)
		{
			if (MainWindow.InvokeRequired)
			{
				var d = new SafeCallDelegate<AppCommand>(_RemoveFromRoot);
				MainWindow.Invoke(d, appCommand);
			}
			else
			{
				if (_rootControl != null) {
					switch (appCommand.Style) {
						case AppCommandStyle.Context:
							// Control, as-is
							// Don't do anything.
							break;
						case AppCommandStyle.Menu:
							// MenuStrip child control
							if (_rootControl is Form) {
								MenuStrip menuStrip = (_rootControl as Form).MainMenuStrip;
								if (menuStrip != null) {
									menuStrip.Items.Remove(appCommand.Item);
								}
							}
							else if (_rootControl is MenuStrip) {
								(_rootControl as MenuStrip).Items.Remove(appCommand.Item);
							}
							break;
					}
				}
			}
			
		}

		public AppCommand()
			: this((Control) null)
		{
		}

		public AppCommand(string name)
			: this()
		{
			Name = name;
		}

		public AppCommand(string name, string text)
			: this(name)
		{
			if (text != "-") {
				Text = text;
			}
			else {
				Style = AppCommandStyle.Separator;
				Name = null;
			}
		}

		public AppCommand(string name, string text, Image image)
			: this(name, text)
		{
			this.Image = image;
		}

		public ToolStripItem Item
		{
			get { return _toolStripItem; }
		}

		private string _name;

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				_toolStripItem.Name = value;
			}
		}

		private string _text;

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				_toolStripItem.Text = value;
			}
		}

		private Image _image;

		public Image Image
		{
			get { return _image; }
			set
			{
				_image = value;
				_toolStripItem.Image = value;
			}
		}

		private bool _enabled;

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				_toolStripItem.Enabled = value;
			}
		}

		private bool _visible;

		public bool Visible
		{
			get { return _visible; }
			set
			{
				_visible = value;
				_toolStripItem.Visible = value;
			}
		}

		public void Add(AppCommand appCommand)
		{
			if (MainWindow.InvokeRequired)
			{
				var d = new SafeCallDelegate<AppCommand>(Add);
				MainWindow.Invoke(d, appCommand);
			}
			else
			{
				appCommand.Parent = this;
				_items.Add(appCommand);
				(_toolStripItem as ToolStripMenuItem).DropDownItems.Add(appCommand.Item);
				if (_rootControl != null) {
					_AddToRoot(appCommand);
				}
			}
			
		}

		public void Remove(string appCommandName)
		{
			if (MainWindow.InvokeRequired)
			{
				var d = new SafeCallDelegate<string>(Remove);
				MainWindow.Invoke(d, appCommandName);
			}
			else
			{
				AppCommand appCommand = _items.FirstOrDefault(x => x.Name == appCommandName);
				if (appCommand != null) {
					appCommand.Parent = null;
					_items.Remove(appCommand);
					(_toolStripItem as ToolStripMenuItem).DropDownItems.Remove(appCommand.Item);
					if (_rootControl != null) {
						_RemoveFromRoot(appCommand);
					}
				}
			}
			
		}

		private List<AppCommand> _items = new List<AppCommand>();

		public AppCommand[] Items
		{
			get { return _items.ToArray(); }
		}

		public enum AppCommandStyle
		{
			Context,
			Menu,
			Separator
		};

		private AppCommandStyle _style;

		protected AppCommandStyle Style
		{
			get { return _style; }
			set
			{
				if (_style != value) {
					if (_toolStripItem != null) {
						_toolStripItem.Click -= ItemClick;
					}
					_style = value;
					switch (Style) {
						case AppCommandStyle.Context:
							_toolStripItem = new ToolStripMenuItem();
							break;
						case AppCommandStyle.Menu:
							_toolStripItem = new ToolStripMenuItem();
							break;
						case AppCommandStyle.Separator:
							_toolStripItem = new ToolStripSeparator();
							break;
					}
					this.Enabled = this.Enabled;
					this.Image = this.Image;
					this.MergeIndex = this.MergeIndex;
					_toolStripItem.MergeAction = MergeAction.Insert;
					AppCommand[] items = Items;
					_items.Clear();
					foreach (AppCommand item in items) {
						Add(item);
					}
					this.Name = this.Name;
					this.Parent = this.Parent;
					this.Text = this.Text;
					this.Visible = this.Visible;
					_toolStripItem.Click += ItemClick;
				}
			}
		}

		//Going to have a local handler to decouple the consumer from the control.
		public event EventHandler Click;

		protected virtual void ItemClick(object sender, EventArgs e)
		{
			if (Click != null) {
				Click(sender, e);
			}
		}

		public AppCommand Find(string relativePath)
		{
			// If there is nothing left in the path, we've arrived at the destination.
			if (string.IsNullOrEmpty(relativePath)) return this;

			// Split the remaining path into two parts -- next node and remainder.
			string childName;
			string pathRemaining;
			int index = relativePath.IndexOf(PathDelimiter);
			if (index != -1) {
				childName = relativePath.Substring(0, index);
				pathRemaining = relativePath.Substring(index + 1);
			}
			else {
				childName = relativePath;
				pathRemaining = string.Empty;
			}

			AppCommand appCommand = _items.Find(x => x.Name == childName);
			if (appCommand != null) {
				// Found the child, continue the traversal.
				return appCommand.Find(pathRemaining);
			}
			else {
				// Child not found, invalid path.
				return null;
			}
		}

		private int _mergeIndex;

		public int MergeIndex
		{
			get { return _mergeIndex; }
			set
			{
				_mergeIndex = value;
				_toolStripItem.MergeIndex = value;
			}
		}

		public AppCommand Parent { get; private set; }

	}
}
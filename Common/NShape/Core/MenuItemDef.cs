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
using System.Diagnostics;
using Dataweb.NShape.Commands;


namespace Dataweb.NShape.Advanced
{
	/// <summary>
	/// Represents a possible action. Used to define a button, toolbar button or menu item that is linked to diagramming actions.
	/// </summary>
	public abstract class MenuItemDef
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" />.
		/// </summary>
		protected MenuItemDef()
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" />.
		/// </summary>
		protected MenuItemDef(string title)
			: this()
		{
			this.title = title;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" />.
		/// </summary>
		protected MenuItemDef(string title, Bitmap image, Color imageTransparentColor)
			: this(title)
		{
			this.image = image;
			this.transparentColor = imageTransparentColor;
			if (this.image != null) {
				if (this.transparentColor.IsEmpty)
					this.image.MakeTransparent();
				else this.image.MakeTransparent(this.transparentColor);
			}
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" />.
		/// </summary>
		protected MenuItemDef(string title, Bitmap image, string description, bool isFeasible)
			: this(title)
		{
			this.image = image;
			this.description = description;
			this.isFeasible = isFeasible;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" />.
		/// </summary>
		protected MenuItemDef(string title, Bitmap image, Color transparentColor,
		                      string name, string description, bool isChecked, bool isFeasible)
			: this(title, image, transparentColor)
		{
			this.name = name;
			this.description = description;
			this.isChecked = isChecked;
			this.isFeasible = isFeasible;
		}


		/// <summary>
		/// Gets or sets an object that provides additional data.
		/// </summary>
		public object Tag
		{
			get { return tag; }
			set { tag = value; }
		}


		/// <summary>
		/// Culture invariant name that can be used as key for the presenting widget.
		/// </summary>
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}


		/// <summary>
		/// Culture-depending title to display as caption of the presenting widget.
		/// </summary>
		public virtual string Name
		{
			get
			{
				if (string.IsNullOrEmpty(name))
					return this.GetType().Name;
				else return name;
			}
			set { name = value; }
		}


		/// <summary>
		/// This text is displayed as tool tip by the presenting widget.
		/// Describes the performed action if active, the reason why it is disabled if the requirement for the action 
		/// is not met (e.g. Unselecting shapes requires selected shapes) or the reason why the action is not allowed.
		/// </summary>
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}


		/// <summary>
		/// Subitems of the action.
		/// </summary>
		public virtual MenuItemDef[] SubItems
		{
			get { return subItems; }
		}


		/// <summary>
		/// True if all requirements for performing the action are met. If false, the presenting widget should appear disabled.
		/// </summary>
		public virtual bool IsFeasible
		{
			get { return isFeasible; }
			set { isFeasible = value; }
		}


		/// <summary>
		/// Specifies if the action may or may not be executed due to security restrictions.
		/// </summary>
		public abstract bool IsGranted(ISecurityManager securityManager);


		/// <summary>
		/// True if the presenting item should appear as checked item.
		/// </summary>
		public virtual bool Checked
		{
			get { return isChecked; }
			set { isChecked = value; }
		}


		/// <summary>
		/// An image for the presenting widget's icon.
		/// </summary>
		public virtual Bitmap Image
		{
			get { return image; }
			set
			{
				image = value;
				if (image != null && transparentColor != Color.Empty)
					image.MakeTransparent(transparentColor);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public virtual Color ImageTransparentColor
		{
			get { return transparentColor; }
			set
			{
				transparentColor = value;
				if (image != null && transparentColor != Color.Empty)
					image.MakeTransparent(transparentColor);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public abstract void Execute(MenuItemDef action, Project project);

		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected MenuItemDef[] subItems = null;

		private object tag = null;
		private string title = string.Empty;
		private string name = null;
		private string description = null;
		private bool isFeasible = true;
		private bool isChecked = false;
		private Bitmap image = null;
		private Color transparentColor = Color.Empty;

		#endregion
	}


	/// <summary>
	/// Dummy action for creating MenuSeperators
	/// </summary>
	public class SeparatorMenuItemDef : MenuItemDef
	{
		/// <ToBeCompleted></ToBeCompleted>
		public SeparatorMenuItemDef() : base()
		{
		}


		/// <override></override>
		public override void Execute(MenuItemDef action, Project project)
		{
			if (action == null) throw new ArgumentNullException("action");
			if (project == null) throw new ArgumentNullException("project");
			// nothing to do
		}


		/// <override></override>
		public override string Name
		{
			get { return name; }
			set
			{
				/* nothing to do */
			}
		}


		/// <override></override>
		public override string Title
		{
			get { return title; }
			set
			{
				/* nothing to do */
			}
		}


		/// <override></override>
		public override string Description
		{
			get { return string.Empty; }
			set
			{
				/* nothing to do */
			}
		}


		/// <override></override>
		public override bool IsGranted(ISecurityManager securityManager)
		{
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			return true;
		}


		/// <override></override>
		public override bool IsFeasible
		{
			get { return true; }
			set
			{
				/* nothing to do */
			}
		}


		/// <override></override>
		public override bool Checked
		{
			get { return false; }
			set
			{
				/* nothing to do */
			}
		}


		/// <override></override>
		public override Bitmap Image
		{
			get { return null; }
			set
			{
				/* nothing to do */
			}
		}


		/// <override></override>
		public override Color ImageTransparentColor
		{
			get { return Color.Empty; }
			set
			{
				/* nothing to do */
			}
		}


		private const string name = "SeparatorAction";
		private const string title = "----------";
	}


	/// <summary>
	/// Throws a NotImplementedException. 
	/// This class is meant as a placeholder and should never be used in a productive environment.
	/// </summary>
	public class NotImplementedMenuItemDef : MenuItemDef
	{
		/// <ToBeCompleted></ToBeCompleted>
		public NotImplementedMenuItemDef(string title)
			: base(title)
		{
		}

		/// <override></override>
		public override void Execute(MenuItemDef action, Project project)
		{
			if (action == null) throw new ArgumentNullException("action");
			if (project == null) throw new ArgumentNullException("project");
			throw new NotImplementedException();
		}


		/// <override></override>
		public override bool IsGranted(ISecurityManager securityManager)
		{
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			return true;
		}


		/// <override></override>
		public override bool IsFeasible
		{
			get { return false; }
			set
			{
				/* nothing to do */
			}
		}


		/// <override></override>
		public override bool Checked
		{
			get { return false; }
			set
			{
				/* nothing to do */
			}
		}


		/// <override></override>
		public override Bitmap Image
		{
			get { return null; }
			set
			{
				/* nothing to do */
			}
		}


		/// <override></override>
		public override Color ImageTransparentColor
		{
			get { return Color.Empty; }
			set
			{
				/* nothing to do */
			}
		}


		private const string notImplementedText = "This action is not yet implemented.";
	}


	/// <summary>
	/// Defines a group of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" />.
	/// </summary>
	public class GroupMenuItemDef : MenuItemDef
	{
		/// <ToBeCompleted></ToBeCompleted>
		public GroupMenuItemDef()
			: base()
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public GroupMenuItemDef(string title)
			: base(title)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public GroupMenuItemDef(string title, Bitmap image, Color imageTransparentColor)
			: base(title, image, imageTransparentColor)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public GroupMenuItemDef(string title, Bitmap image, string description, bool isFeasible)
			: base(title, image, description, isFeasible)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public GroupMenuItemDef(string title, Bitmap image, Color transparentColor, string name, string description,
		                        bool isChecked, bool isFeasible)
			: base(title, image, transparentColor, name, description, isChecked, isFeasible)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public GroupMenuItemDef(string title, Bitmap image, string description, bool isFeasible, MenuItemDef[] actions,
		                        int defaultActionIndex)
			: base(title, image, description, isFeasible)
		{
			this.subItems = actions;
			this.defaultActionIdx = defaultActionIndex;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public GroupMenuItemDef(string title, Bitmap image, Color transparentColor, string name, string description,
		                        bool isChecked, bool isFeasible, MenuItemDef[] actions, int defaultActionIndex)
			: base(title, image, transparentColor, name, description, isChecked, isFeasible)
		{
			this.subItems = actions;
			this.defaultActionIdx = defaultActionIndex;
		}


		/// <override></override>
		public override bool IsGranted(ISecurityManager securityManager)
		{
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			return true;
		}


		/// <override></override>
		public override void Execute(MenuItemDef action, Project project)
		{
			//if (action == null) throw new ArgumentNullException("action");
			//if (project == null) throw new ArgumentNullException("project");
			//if (DefaultAction != null) DefaultAction.Execute(DefaultAction, project);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MenuItemDef DefaultAction
		{
			get
			{
				return (subItems == null || defaultActionIdx < 0 || defaultActionIdx >= subItems.Length)
				       	? null
				       	: subItems[defaultActionIdx];
			}
		}


		private int defaultActionIdx = -1;
	}


	/// <summary>
	/// Executes a given delegate.
	/// </summary>
	public class DelegateMenuItemDef : MenuItemDef
	{
		/// <ToBeCompleted></ToBeCompleted>
		public delegate void ActionExecuteDelegate(MenuItemDef action, Project project);


		/// <ToBeCompleted></ToBeCompleted>
		public DelegateMenuItemDef(string text)
			: base(text, null, Color.Empty)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DelegateMenuItemDef(string text, Bitmap image, Color imageTransparentColor)
			: base(text, image, imageTransparentColor)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DelegateMenuItemDef(string title, Bitmap image, string description, bool isFeasible,
		                           Permission requiredPermission, ActionExecuteDelegate executeDelegate)
			: this(
				title, image, Color.Empty, string.Format("{0} Action", title), description, false, isFeasible, requiredPermission,
				null, executeDelegate)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DelegateMenuItemDef(string title, Bitmap image, string description, bool isFeasible,
		                           Permission requiredPermission, IEnumerable<Shape> shapes,
		                           ActionExecuteDelegate executeDelegate)
			: this(
				title, image, Color.Empty, string.Format("{0} Action", title), description, false, isFeasible, requiredPermission,
				shapes, executeDelegate)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DelegateMenuItemDef(string title, Bitmap image, string description, bool isFeasible,
		                           Permission requiredPermission, char securityDomainName,
		                           ActionExecuteDelegate executeDelegate)
			: this(
				title, image, Color.Empty, string.Format("{0} Action", title), description, false, isFeasible, requiredPermission,
				securityDomainName, executeDelegate)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DelegateMenuItemDef(string title, Bitmap image, string name, string description, bool isChecked,
		                           bool isFeasible, Permission requiredPermission, ActionExecuteDelegate executeDelegate)
			: this(title, image, Color.Empty, name, description, isChecked, isFeasible, requiredPermission, null, executeDelegate
				)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DelegateMenuItemDef(string title, Bitmap image, Color transparentColor, string name, string description,
		                           bool isChecked, bool isFeasible, Permission requiredPermission,
		                           ActionExecuteDelegate executeDelegate)
			: this(
				title, image, transparentColor, name, description, isChecked, isFeasible, requiredPermission, null, executeDelegate)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected DelegateMenuItemDef(string title, Bitmap image, Color transparentColor, string name, string description,
		                              bool isChecked, bool isFeasible, Permission requiredPermission,
		                              IEnumerable<Shape> shapes, ActionExecuteDelegate executeDelegate)
			: base(title, image, transparentColor, name, description, isChecked, isFeasible)
		{
			this.executeDelegate = executeDelegate;
			this.requiredPermission = requiredPermission;
			this.securityDomainObjects = shapes;
			Debug.Assert(PermissionsAreValid());
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected DelegateMenuItemDef(string title, Bitmap image, Color transparentColor, string name, string description,
		                              bool isChecked, bool isFeasible, Permission requiredPermission, char securityDomainName,
		                              ActionExecuteDelegate executeDelegate)
			: base(title, image, transparentColor, name, description, isChecked, isFeasible)
		{
			this.executeDelegate = executeDelegate;
			this.requiredPermission = requiredPermission;
			this.securityDomainName = securityDomainName;
			Debug.Assert(PermissionsAreValid());
		}


		/// <override></override>
		public override void Execute(MenuItemDef action, Project project)
		{
			if (action == null) throw new ArgumentNullException("action");
			if (project == null) throw new ArgumentNullException("project");
			executeDelegate(action, project);
		}


		/// <override></override>
		public override bool IsGranted(ISecurityManager securityManager)
		{
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			if (securityDomainObjects != null)
				return securityManager.IsGranted(requiredPermission, securityDomainObjects);
			else if (securityDomainName != '\0')
				return securityManager.IsGranted(requiredPermission, securityDomainName);
			else return securityManager.IsGranted(requiredPermission);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Permission RequiredPermission
		{
			get { return requiredPermission; }
			set { requiredPermission = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ActionExecuteDelegate Delegate
		{
			get { return executeDelegate; }
			set { executeDelegate = value; }
		}


		private bool PermissionsAreValid()
		{
			switch (requiredPermission) {
				case Permission.All:
					return false;
				case Permission.None:
					return true;
				case Permission.Connect:
				case Permission.Data:
				case Permission.Delete:
				case Permission.Insert:
				case Permission.Layout:
				case Permission.Present:
					return (securityDomainObjects != null || securityDomainName != NoSecurityDomain);
				case Permission.Designs:
				case Permission.Security:
				case Permission.Templates:
					return (securityDomainObjects == null && securityDomainName == NoSecurityDomain);
				default:
					return false;
			}
		}


		// Fields
		private const char NoSecurityDomain = '\0';
		private Permission requiredPermission = Permission.None;
		private char securityDomainName = NoSecurityDomain;
		private IEnumerable<Shape> securityDomainObjects = null;
		private ActionExecuteDelegate executeDelegate;
	}


	/// <summary>
	/// Adds a Command to the History and executes it.
	/// </summary>
	public class CommandMenuItemDef : MenuItemDef
	{
		/// <ToBeCompleted></ToBeCompleted>
		public CommandMenuItemDef()
			: base()
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CommandMenuItemDef(string title)
			: base(title)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CommandMenuItemDef(string title, Bitmap image, Color transparentColor)
			: base(title, image, transparentColor)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CommandMenuItemDef(string title, Bitmap image, string notFeasibleDescription, bool isFeasible, ICommand command)
			: base(title, image, notFeasibleDescription, isFeasible)
		{
			this.command = command;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CommandMenuItemDef(string title, Bitmap image, Color transparentColor, string name,
		                          string notFeasibleDescription, bool isChecked, bool isFeasible, ICommand command)
			: base(title, image, transparentColor, name, notFeasibleDescription, isChecked, isFeasible)
		{
			this.command = command;
		}


		/// <override></override>
		public override string Description
		{
			get
			{
				if (IsFeasible) return command.Description;
				else return base.Description;
			}
			set { base.Description = value; }
		}


		/// <override></override>
		public override bool IsGranted(ISecurityManager securityManager)
		{
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			if (command != null) {
				Exception exc = command.CheckAllowed(securityManager);
				if (exc != null) {
					Description = exc.Message;
					return false;
				}
				else return true;
			}
			else return false;
		}


		/// <override></override>
		public override void Execute(MenuItemDef action, Project project)
		{
			if (action == null) throw new ArgumentNullException("action");
			if (project == null) throw new ArgumentNullException("project");
			if (command != null) project.ExecuteCommand(command);
		}


		/// <summary>
		/// Specifies the command executed by the <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" />.
		/// </summary>
		public ICommand Command
		{
			get { return command; }
		}

		#region Fields

		private ICommand command = null;

		#endregion
	}
}
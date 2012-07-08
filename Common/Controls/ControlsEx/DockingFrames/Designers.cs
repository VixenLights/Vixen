using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ControlsEx.DockingFrames
{
	/// <summary>
	/// Designer for the TabControl
	/// </summary>
	public class TabControlDesigner:ControlDesigner
	{
		#region variables
		private TabControl _owner;
		#endregion
		public override void Initialize(IComponent component)
		{
			this._owner=(TabControl)component;
			base.Initialize (component);
			//subscribe the selection changed event
			ISelectionService selserv=(ISelectionService)this.GetService(typeof(ISelectionService));
			selserv.SelectionChanged+=new EventHandler(selserv_SelectionChanged);
		}
		protected override void Dispose(bool disposing)
		{
			//unsubscribe the selection changed event
			ISelectionService selserv=(ISelectionService)this.GetService(typeof(ISelectionService));
			selserv.SelectionChanged-=new EventHandler(selserv_SelectionChanged);
			base.Dispose (disposing);
		}
		#region helpers
		/// <summary>
		/// gets or sets the current selection
		/// </summary>
		private object SelectedComponent
		{
			get
			{
				ISelectionService selserv=(ISelectionService)this.GetService(typeof(ISelectionService));
				return selserv.PrimarySelection;
			}
			set
			{
				ISelectionService selserv=(ISelectionService)this.GetService(typeof(ISelectionService));
				ArrayList lst=new ArrayList();
				lst.Add(value);
				selserv.SetSelectedComponents(lst,SelectionTypes.Replace);
			}
		}
		private TabPage GetContainingPage(object obj)
		{
			Control parent=obj as Control;
			if(parent==null) return null;
			while(parent!=null && !(parent is TabPage))
			{
				parent=parent.Parent;
			}
			return (TabPage)parent;
		}
		#endregion
		#region controller
		//select new selectedpage
		private void selserv_SelectionChanged(object sender, EventArgs e)
		{
			ISelectionService selserv=(ISelectionService)
				this.GetService(typeof(ISelectionService));
			foreach(object obj in selserv.GetSelectedComponents())
			{
				TabPage page=GetContainingPage(obj);
				if(page==null) continue;
				if(page.Owner==_owner)
				{
					_owner.SelectedPage=page;
					return;
				}
			}
		}
		//make sure the pages are correctly removed / copied
		public override ICollection AssociatedComponents
		{
			get{return this._owner.Controls;}
		}
		//check wether the control should receive the
		//mouse messages or should be selected
		protected override bool GetHitTest(Point point)
		{
			Point pt=base.Control.PointToClient(point);
			if (this._owner.ClientRectangle.Contains(pt))
			{
				if(SelectedComponent==this.Control)//click header
					return _owner.TabBounds.Contains(point);
				else
					return false;//select control
			}
			return false;
		}
		//deny dragdrops
		protected override void OnDragOver(DragEventArgs de)
		{
			de.Effect=DragDropEffects.None;
		}
		//deny dragdrops
		protected override void OnDragDrop(DragEventArgs de)
		{
			de.Effect=DragDropEffects.None;
		}
		#endregion
		#region verbs
		//add a new tab
		private void OnAddTab(object sender, EventArgs e)
		{
			MemberDescriptor controls=TypeDescriptor.GetProperties(base.Component)["Controls"];
			IDesignerHost host=(IDesignerHost)this.GetService(typeof(IDesignerHost));
			using (DesignerTransaction tabadd=host.CreateTransaction("TabAdded"))
			{
				base.RaiseComponentChanging(controls);
				TabPage page=(TabPage)host.CreateComponent(typeof(TabPage));
				page.Caption="TabPage"+(this._owner.Controls.Count+1).ToString();
				this._owner.Controls.Add(page);
				base.RaiseComponentChanged(controls,null,null);
				tabadd.Commit();
			}
		}
		public override DesignerVerbCollection Verbs
		{
			get
			{
				return new DesignerVerbCollection(new DesignerVerb[]
					{new DesignerVerb("Add Tab",new EventHandler(OnAddTab))});
			}
		}
		#endregion
	}
	/// <summary>
	/// Designer for the TabPage
	/// </summary>
	public class TabPageDesigner:ScrollableControlDesigner
	{
		public override void Initialize(IComponent component)
		{
			base.Initialize (component);
			//subscribe the selection changed event
			ISelectionService selserv=(ISelectionService)this.GetService(typeof(ISelectionService));
			selserv.SelectionChanged+=new EventHandler(selserv_SelectionChanged);
		}
		protected override void Dispose(bool disposing)
		{
			//unsubscribe the selection changed event
			ISelectionService selserv=(ISelectionService)this.GetService(typeof(ISelectionService));
			selserv.SelectionChanged-=new EventHandler(selserv_SelectionChanged);
			base.Dispose (disposing);
		}
		#region helpers
		/// <summary>
		/// gets the currently selected control
		/// </summary>
		private object SelectedComponent
		{
			get
			{
				ISelectionService selserv=(ISelectionService)this.GetService(typeof(ISelectionService));
				return selserv.PrimarySelection;
			}
		}
		#endregion
		#region controller
		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			base.OnPaintAdornments (pe);
			if (SelectedComponent==this.Control)//paint selection frame
				ControlPaint.DrawSelectionFrame(pe.Graphics,false,
					this.Control.ClientRectangle,
					Rectangle.Inflate(this.Control.ClientRectangle,-6,-6),
					this.Control.BackColor);
		}
		//refresh the control to make sure the selection frame is painted correctly
		private void selserv_SelectionChanged(object sender, EventArgs e)
		{
			this.Control.Refresh();
		}
		#endregion
	}
}

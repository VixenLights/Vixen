using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ControlsEx.SideBar
{
	/// <summary>
	/// Zusammenfassung für SideBarDesigner.
	/// </summary>
	public class SideBarDesigner:ScrollableControlDesigner
	{
		#region variables
		private SideBar _owner;
		#endregion
		public override void Initialize(IComponent component)
		{
			this._owner=(SideBar)component;
			base.Initialize (component);
		}
		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			return;
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
		#endregion
		#region controller
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
		private void OnAddItem(object sender, EventArgs e)
		{
			MemberDescriptor controls=TypeDescriptor.GetProperties(base.Component)["Controls"];
			IDesignerHost host=(IDesignerHost)this.GetService(typeof(IDesignerHost));
			using (DesignerTransaction tabadd=host.CreateTransaction("SideBar Item added"))
			{
				base.RaiseComponentChanging(controls);
				SideBarItem item=(SideBarItem)host.CreateComponent(typeof(SideBarItem));
				item.Caption=item.Name;
				this._owner.Controls.Add(item);
				base.RaiseComponentChanged(controls,null,null);
				tabadd.Commit();
			}
		}
		public override DesignerVerbCollection Verbs
		{
			get
			{
				return new DesignerVerbCollection(new DesignerVerb[]
					{new DesignerVerb("Add Item",new EventHandler(OnAddItem))});
			}
		}
		#endregion
	}
	public class SideBarItemDesigner:ParentControlDesigner
	{
		private SideBarItem _owner;
		public override void Initialize(IComponent component)
		{
			base.Initialize (component);
			this._owner=component as SideBarItem;
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
		public override SelectionRules SelectionRules
		{
			get
			{
				if(this._owner!=null && this._owner.Collapsed)
					return SelectionRules.Visible;
				else
					return SelectionRules.BottomSizeable |
						SelectionRules.Visible;
			}
		}

	}
}

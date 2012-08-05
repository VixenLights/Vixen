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
using System.Reflection;
using System.Windows.Forms;

using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.Designer {

	public partial class EventMonitorForm : Form {

		public EventMonitorForm() {
			InitializeComponent();
			Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}


		#region [Public] Delegate Types

		public delegate void EventMonitorDelegate<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;
		
		public delegate void EventMonitorLogDelegate(object sender, string eventName);

		#endregion


		#region [Public] Methods

		public void AddEventSources<T>(IEnumerable<T> eventSenders) {
			foreach (T eventSender in eventSenders)
				AddEventSource(eventSender);
		}


		public void AddEventSource<T>(T eventSender) {
			if (eventSender == null) throw new ArgumentNullException("eventSender");

			string name = eventSender.GetType().Name;
			PropertyInfo pi = eventSender.GetType().GetProperty("Title");
			if (pi != null) name += string.Format(" ({0})", pi.GetValue(eventSender, null).ToString());
			else {
				pi = eventSender.GetType().GetProperty("Name");
				if (pi != null) name += string.Format(" ({0})", pi.GetValue(eventSender, null).ToString());
			}

			eventSources.Add(eventSender);
			eventSourcesListBox.Items.Add(name);
		}


		public bool RemoveEventSource<T>(T eventSource) {
			int idx = eventSources.IndexOf(eventSource);
			if (idx < 0) return false;

			UnregisterEvents(eventSource);
			eventSources.RemoveAt(idx);
			eventSourcesListBox.Items.RemoveAt(idx);
			return true;
		}

		#endregion


		#region [Private] Registering event handlers

		private void RegisterAllEvents() {
			foreach (object eventSender in eventSourcesListBox.Items)
				DoRegisterEvents(eventSender, false);
		}


		private void RegisterEvents<T>(T eventSource) {
			if (eventSource is IDiagramPresenter)
				RegisterEvents((IDiagramPresenter)eventSource);
			else if (eventSource is IRepository)
				RegisterEvents((IRepository)eventSource);
			else DoRegisterEvents(eventSource, false);
		}


		private void RegisterEvents(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException();
			diagramPresenter.DiagramChanged += new EventHandler(diagramPresenter_DiagramChanged);
			diagramPresenter.DiagramChanging += new EventHandler(diagramPresenter_DiagramChanging);
			diagramPresenter.LayerVisibilityChanged += new EventHandler<LayersEventArgs>(diagramPresenter_LayerVisibilityChanged);
			diagramPresenter.ShapeClick += new EventHandler<DiagramPresenterShapeClickEventArgs>(diagramPresenter_ShapeClick);
			diagramPresenter.ShapeDoubleClick += new EventHandler<DiagramPresenterShapeClickEventArgs>(diagramPresenter_ShapeDoubleClick);
			diagramPresenter.ShapesInserted += new EventHandler<DiagramPresenterShapesEventArgs>(diagramPresenter_ShapesInserted);
			diagramPresenter.ShapesRemoved += new EventHandler<DiagramPresenterShapesEventArgs>(diagramPresenter_ShapesRemoved);
			diagramPresenter.ShapesSelected += new EventHandler(diagramPresenter_ShapesSelected);
		}


		private void RegisterEvents(IRepository repository) {
			if (repository == null) throw new ArgumentNullException("repository");
			DoRegisterEvent(repository, "DesignDeleted");
			DoRegisterEvent(repository, "DesignInserted");
			DoRegisterEvent(repository, "DesignUpdated");
			DoRegisterEvent(repository, "DiagramDeleted");
			DoRegisterEvent(repository, "DiagramDeleted");
			DoRegisterEvent(repository, "DiagramInserted");
			DoRegisterEvent(repository, "DiagramUpdated");
			DoRegisterEvent(repository, "ModelDeleted");
			DoRegisterEvent(repository, "ModelInserted");
			DoRegisterEvent(repository, "ModelUpdated");
			DoRegisterEvent(repository, "ModelMappingsDeleted");
			DoRegisterEvent(repository, "ModelMappingsInserted");
			DoRegisterEvent(repository, "ModelMappingsUpdated");
			DoRegisterEvent(repository, "ModelObjectsDeleted");
			DoRegisterEvent(repository, "ModelObjectsInserted");
			DoRegisterEvent(repository, "ModelObjectsUpdated");
			DoRegisterEvent(repository, "ProjectUpdated");
			DoRegisterEvent(repository, "ShapesDeleted");
			DoRegisterEvent(repository, "ShapesInserted");
			DoRegisterEvent(repository, "ShapesUpdated");
			DoRegisterEvent(repository, "StyleDeleted");
			DoRegisterEvent(repository, "StyleInserted");
			DoRegisterEvent(repository, "StyleUpdated");
			DoRegisterEvent(repository, "TemplateDeleted");
			DoRegisterEvent(repository, "TemplateInserted");
			DoRegisterEvent(repository, "TemplateShapeReplaced");
			DoRegisterEvent(repository, "TemplateUpdated");
		}

		#endregion


		#region [Private] Unregistering event handlers

		private void UnregisterAllEvents() {
			foreach (object eventSender in eventSourcesListBox.Items)
				DoUnregisterEvents(eventSender);
		}


		private void UnregisterEvents<T>(T eventSource) {
			if (eventSource is IDiagramPresenter)
				UnregisterEvents((IDiagramPresenter)eventSource);
			else if (eventSource is IRepository)
				UnregisterEvents((IRepository)eventSource);
			else DoUnregisterEvents(eventSource);
		}


		private void UnregisterEvents(IDiagramPresenter diagramPresenter) {
			if (diagramPresenter == null) throw new ArgumentNullException();
			diagramPresenter.DiagramChanged += new EventHandler(diagramPresenter_DiagramChanged);
			diagramPresenter.DiagramChanging += new EventHandler(diagramPresenter_DiagramChanging);
			diagramPresenter.LayerVisibilityChanged += new EventHandler<LayersEventArgs>(diagramPresenter_LayerVisibilityChanged);
			diagramPresenter.ShapeClick += new EventHandler<DiagramPresenterShapeClickEventArgs>(diagramPresenter_ShapeClick);
			diagramPresenter.ShapeDoubleClick += new EventHandler<DiagramPresenterShapeClickEventArgs>(diagramPresenter_ShapeDoubleClick);
			diagramPresenter.ShapesInserted += new EventHandler<DiagramPresenterShapesEventArgs>(diagramPresenter_ShapesInserted);
			diagramPresenter.ShapesRemoved += new EventHandler<DiagramPresenterShapesEventArgs>(diagramPresenter_ShapesRemoved);
			diagramPresenter.ShapesSelected += new EventHandler(diagramPresenter_ShapesSelected);
		}


		private void UnregisterEvents(IRepository repository) {
			if (repository == null) throw new ArgumentNullException("repository");
			DoUnregisterEvents(repository);
		}

		#endregion


		#region [Private] IDiagramPresenter Event Handlers

		void diagramPresenter_ShapesSelected(object sender, EventArgs e) {
			LogRaisedEvent(sender, e, "ShapesSelected");
		}

		void diagramPresenter_ShapesRemoved(object sender, DiagramPresenterShapesEventArgs e) {
			LogRaisedEvent(sender, e, "ShapesRemoved");
		}

		void diagramPresenter_ShapesInserted(object sender, DiagramPresenterShapesEventArgs e) {
			LogRaisedEvent(sender, e, "ShapesInserted");
		}

		void diagramPresenter_ShapeDoubleClick(object sender, DiagramPresenterShapeClickEventArgs e) {
			LogRaisedEvent(sender, e, "ShapeDoubleClick");
		}

		void diagramPresenter_ShapeClick(object sender, DiagramPresenterShapeClickEventArgs e) {
			LogRaisedEvent(sender, e, "ShapeClick");
		}

		void diagramPresenter_LayerVisibilityChanged(object sender, LayersEventArgs e) {
			LogRaisedEvent(sender, e, "LayerVisibilityChanged");
		}

		void diagramPresenter_DiagramChanging(object sender, EventArgs e) {
			LogRaisedEvent(sender, e, "DiagramChanging");
		}

		void diagramPresenter_DiagramChanged(object sender, EventArgs e) {
			LogRaisedEvent(sender, e, "DiagramChanged");
		}

		#endregion


		#region [Private] (Un)Registering Generic Event Handlers

		private void DoRegisterEvents(object eventSender, bool declaredTypeOnly) {
			if (eventSender == null) throw new ArgumentNullException("eventSender");
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			if (declaredTypeOnly) bindingFlags |= BindingFlags.DeclaredOnly;

			Type eventSenderType = eventSender.GetType();
			foreach (EventInfo eventInfo in eventSenderType.GetEvents(bindingFlags))
				DoRegisterEvent(eventSender, eventInfo);
		}


		private void DoRegisterEvent(object eventSender, string eventName) {
			EventInfo eventInfo = eventSender.GetType().GetEvent(eventName);
			DoRegisterEvent(eventSender, eventInfo);
		}


		private void DoRegisterEvent(object eventSender, EventInfo eventInfo) {
			string eventName = eventInfo.Name;
			Delegate handler = new EventHandler<EventArgs>((s, e) => LogRaisedEvent(s, e, eventName));

			Delegate eventHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType, handler.Target, handler.Method);
			eventInfo.AddEventHandler(eventSender, eventHandler);

			if (!eventHandlerDictionary.ContainsKey(eventSender))
				eventHandlerDictionary.Add(eventSender, new List<HandlerInfo>());
			eventHandlerDictionary[eventSender].Add(HandlerInfo.Create(eventSender, eventInfo, eventHandler));
		}


		private void DoUnregisterEvents(object eventSender) {
			if (eventHandlerDictionary.ContainsKey(eventSender)) {
				for (int i = eventHandlerDictionary[eventSender].Count - 1; i >= 0; --i) {
					HandlerInfo handlerInfo = eventHandlerDictionary[eventSender][i];
					handlerInfo.EventInfo.RemoveEventHandler(handlerInfo.EventSender, handlerInfo.Handler);
				}
			} else {
				System.Diagnostics.Debug.Print("Event source '{0}' not registered.", eventSender);
			}
		}

		#endregion


		internal void LogRaisedEvent<TEventArgs>(object sender, TEventArgs e, string eventName) where TEventArgs : EventArgs {
			string senderName;
			if (sender is Control)
				senderName = ((Control)sender).Name;
			else senderName = sender.GetType().Name;
			eventListBox.Items.Add(string.Format("{4} {0}.{1}({2}, {3})", sender, eventName, senderName, e, DateTime.Now.ToString("HH:mm:ss.ffff  ")));
			eventListBox.SelectedIndex = eventListBox.Items.Count - 1;
		}


		#region [Private] UI implementation

		private void CheckAllItems(bool check) {
			int cnt = eventSourcesListBox.Items.Count;
			for (int i = 0; i < cnt; ++i) {
				if (eventSourcesListBox.GetItemChecked(i) != check)
					eventSourcesListBox.SetItemChecked(i, check);
			}
		}
		
		
		private void componentsListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
			if (e.NewValue == CheckState.Checked) {
				RegisterEvents(eventSources[e.Index]);
			} else {
				UnregisterEvents(eventSources[e.Index]);
			}
		}


		private void checkAllMenuItem_Click(object sender, EventArgs e) {
			CheckAllItems(true);
		}


		private void uncheckAllMenuItem_Click(object sender, EventArgs e) {
			CheckAllItems(false);
		}
		
		
		private void EventMonitorForm_FormClosed(object sender, FormClosedEventArgs e) {
			UnregisterAllEvents();
		}

		#endregion


		#region [Private] Types

		private struct HandlerInfo : IEquatable<HandlerInfo> {

			public static readonly HandlerInfo Empty;

			public static HandlerInfo Create(object eventSender, EventInfo eventInfo, Delegate handler) {
				HandlerInfo result = HandlerInfo.Empty;
				result.EventSender = eventSender;
				result.EventInfo = eventInfo;
				result.Handler = handler;
				return result;
			}

			public HandlerInfo(object eventSender, EventInfo eventInfo, Delegate handler) {
				this.EventSender = eventSender;
				this.EventInfo = eventInfo;
				this.Handler = handler;
			}

			public object EventSender;

			public EventInfo EventInfo;
			
			public Delegate Handler;

			public bool Equals(HandlerInfo other) {
				return (other.EventInfo == this.EventInfo
					&& other.EventSender == this.EventSender
					&& other.Handler == this.Handler);
			}

			static HandlerInfo() {
				Empty.EventSender = null;
				Empty.EventInfo = null;
				Empty.Handler = null;
			}
		}

		#endregion


		#region [Private] Fields

		Dictionary<object, List<HandlerInfo>> eventHandlerDictionary = new Dictionary<object, List<HandlerInfo>>();
		List<object> eventSources = new List<object>();

		#endregion
	}
}

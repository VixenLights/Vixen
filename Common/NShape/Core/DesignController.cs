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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;


namespace Dataweb.NShape.Controllers {

	/// <ToBeCompleted></ToBeCompleted>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(DesignController), "DesignController.bmp")]
	public partial class DesignController : Component {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.DesignController" />.
		/// </summary>
		public DesignController() { }


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.DesignController" />.
		/// </summary>
		public DesignController(Project project)
			: this() {
			// Set property in order to register event handlers
			if (project == null) throw new ArgumentNullException("project");
			this.Project = project;
		}


		#region [Public] Events

		/// <summary>
		/// Raised when the <see cref="T:Dataweb.NShape.Controllers.DesignController" /> was initialized.
		/// </summary>
		public event EventHandler Initialized;

		/// <summary>
		/// Raised when the <see cref="T:Dataweb.NShape.Controllers.DesignController" /> was uninitialized.
		/// </summary>
		public event EventHandler Uninitialized;

		/// <summary>
		/// Raised when a new design was created.
		/// </summary>
		public event EventHandler<DesignEventArgs> DesignCreated;

		/// <summary>
		/// Raised when a design has changed
		/// </summary>
		public event EventHandler<DesignEventArgs> DesignChanged;

		/// <summary>
		/// Raised when a design has been deleted.
		/// </summary>
		public event EventHandler<DesignEventArgs> DesignDeleted;

		/// <summary>
		/// Raised when a new style has been created
		/// </summary>
		public event EventHandler<StyleEventArgs> StyleCreated;

		/// <summary>
		/// Raised when a style has been modified
		/// </summary>
		public event EventHandler<StyleEventArgs> StyleChanged;

		/// <summary>
		/// Raisd when a style has been deleted.
		/// </summary>
		public event EventHandler<StyleEventArgs> StyleDeleted;

		#endregion


		#region [Public] Properties

		/// <ToBeCompleted></ToBeCompleted>
		[Browsable(false)]
		public IEnumerable<Design> Designs {
			get {
				if (project == null) return EmptyEnumerator<Design>.Empty;
				else return project.Repository.GetDesigns();
			}
		}


		/// <summary>
		/// Provides access to a <see cref="T:Dataweb.NShape.Project" />.
		/// </summary>
		[Category("NShape")]
		public Project Project {
			get { return project; }
			set {
				if (project != null) UnregisterProjectEventHandlers();
				project = value;
				if (project != null) RegisterProjectEventHandlers();
			}
		}


		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		public string ProductVersion {
			get { return this.GetType().Assembly.GetName().Version.ToString(); }
		}

		#endregion


		#region [Public] Methods

		/// <ToBeCompleted></ToBeCompleted>
		public bool CanDelete(Design design) {
			return (design != null && design != project.Design);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool CanDelete(Design selectedDesign, Style style) {
			if (style == null) return false;
			// if the style is owned by an other design (should not happen)
			Design design = GetOwnerDesign(style);
			if (design != selectedDesign) return false;
			// check if style is a standard style
			if (design.IsStandardStyle(style)) return false;
			// check if the deleted style is used by other styles 
			// only ColorStyles are used by other styles
			foreach (IStyle s in GetOwnerStyles(selectedDesign, style))
				return false;
			return true;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerable<IStyle> GetOwnerStyles(Design design, IStyle style) {
			if (design == null) throw new ArgumentNullException("design");
			if (style == null) throw new ArgumentNullException("style");
			// check all Styles of the parent design if the style is used by (not yet deleted) styles
			if (style is ColorStyle) {
				foreach (IStyle s in design.Styles) {
					if (s is ICapStyle && ((ICapStyle)s).ColorStyle == style)
						yield return s;
					else if (s is ICharacterStyle && ((ICharacterStyle)s).ColorStyle == style)
						yield return s;
					else if (s is IFillStyle && (((IFillStyle)s).AdditionalColorStyle == style || ((IFillStyle)s).BaseColorStyle == style))
						yield return s;
					else if (s is ILineStyle && ((ILineStyle)s).ColorStyle == style)
						yield return s;
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void CreateDesign() {
			int designCnt = 1;
			foreach (Design d in Designs)
				++designCnt;
			//
			Design design = new Design(string.Format("Design {0}", designCnt));
			ICommand cmd = new CreateDesignCommand(design);
			project.ExecuteCommand(cmd);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void CreateStyle(Design design, StyleCategory category) {
			if (design == null) throw new ArgumentNullException("design");
			Style style;
			switch (category) {
				case StyleCategory.CapStyle:
					style = new CapStyle(GetNewStyleName(design.CapStyles));
					((CapStyle)style).CapShape = CapShape.None;
					((CapStyle)style).ColorStyle = design.ColorStyles.Black;
					break;
				case StyleCategory.CharacterStyle: 
					style = new CharacterStyle(GetNewStyleName(design.CharacterStyles));
					((CharacterStyle)style).ColorStyle = design.ColorStyles.Black;
					break;
				case StyleCategory.ColorStyle: 
					style = new ColorStyle(GetNewStyleName(design.ColorStyles));
					((ColorStyle)style).Color = Color.Black;
					((ColorStyle)style).Transparency = 0;
					break;
				case StyleCategory.FillStyle: 
					style = new FillStyle(GetNewStyleName(design.FillStyles));
					((FillStyle)style).AdditionalColorStyle = design.ColorStyles.White;
					((FillStyle)style).BaseColorStyle = design.ColorStyles.Black;
					((FillStyle)style).FillMode = FillMode.Gradient;
					((FillStyle)style).ImageLayout = ImageLayoutMode.Fit;
					break;
				case StyleCategory.LineStyle: 
					style = new LineStyle(GetNewStyleName(design.LineStyles));
					((LineStyle)style).ColorStyle = design.ColorStyles.Black;
					((LineStyle)style).DashCap = System.Drawing.Drawing2D.DashCap.Round;
					((LineStyle)style).DashType = DashType.Solid;
					((LineStyle)style).LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
					((LineStyle)style).LineWidth = 1;
					break;
				case StyleCategory.ParagraphStyle: 
					style = new ParagraphStyle(GetNewStyleName(design.ParagraphStyles));
					((ParagraphStyle)style).Alignment = ContentAlignment.MiddleCenter;
					((ParagraphStyle)style).Padding = new TextPadding(3);
					((ParagraphStyle)style).Trimming = StringTrimming.EllipsisCharacter;
					((ParagraphStyle)style).WordWrap = false;
					break;
				default: throw new NShapeUnsupportedValueException(typeof(StyleCategory), category);
			}
			ICommand cmd = new CreateStyleCommand(design, style);
			project.ExecuteCommand(cmd);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void ReplaceStyle(Design design, Style style, string propertyName, object oldValue, object newValue) {
			if (design == null) throw new ArgumentNullException("design");
			if (style == null) throw new ArgumentNullException("style");
			PropertyInfo propertyInfo = style.GetType().GetProperty(propertyName);
			if (propertyInfo == null) throw new NShapeException("Property {0} not found in Type {1}.", propertyName, style.GetType().Name);

			bool performPropertyChange = true;
			if (string.Compare(propertyName, "Name", StringComparison.InvariantCultureIgnoreCase) == 0)
				performPropertyChange = StyleNameExists(design, style, (string)newValue);
			
			if (performPropertyChange) {
				ICommand cmd = new StylePropertySetCommand(design, style, propertyInfo, oldValue, newValue);
				project.ExecuteCommand(cmd);
				if (StyleChanged != null) StyleChanged(this, GetStyleEventArgs(design, style));
			} else propertyInfo.SetValue(style, oldValue, null);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void DeleteDesign(Design design) {
			if (design == null) throw new ArgumentNullException("design");
			ICommand cmd = new DeleteDesignCommand(design);
			project.ExecuteCommand(cmd);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void DeleteStyle(Design design, Style style) {
			if (design == null) throw new ArgumentNullException("design");
			if (style == null) throw new ArgumentNullException("style");
			ICommand cmd = new DeleteStyleCommand(design, style);
			project.ExecuteCommand(cmd);
		}

		#endregion
		

		#region [Private] Methods

		private void Initialize() {
			RegisterRepositoryEventHandlers();
			if (Initialized != null) Initialized(this, EventArgs.Empty);
		}


		private void Uninitialize() {
			UnregisterRepositoryEventHandlers();
			if (Uninitialized != null) Uninitialized(this, EventArgs.Empty);
		}


		private DesignEventArgs GetDesignEventArgs(Design design) {
			designEventArgs.Design = design;
			return designEventArgs;
		}


		private StyleEventArgs GetStyleEventArgs(Design design, IStyle style) {
			styleEventArgs.Design = design;
			styleEventArgs.Style = style;
			return styleEventArgs;
		}


		private Design GetOwnerDesign(IStyle style) {
			if (project.Design.ContainsStyle(style))
				return project.Design;
			foreach (Design design in project.Repository.GetDesigns()) {
				if (design.ContainsStyle(style))
					return design;
			}
			return null;
		}


		private string GetNewStyleName<T>(StyleCollection<T> styleCollection) 
			where T : class, IStyle {
			string newName;
			string typeName = typeof(T).Name;
			int cnt = styleCollection.Count;
			do
				newName = string.Format("{0} {1}", typeName, ++cnt);
			while (styleCollection.Contains(newName));
			return newName;
		}


		private bool StyleNameExists(Design design, IStyle style, string newName) {
			if (design == null) throw new ArgumentNullException("design");
			if (style == null) throw new ArgumentNullException("style");
			return (design.FindStyleByName(newName, style.GetType()) != null);
		}

		#endregion


		#region [Private] Methods: (Un)Registering events

		private void RegisterProjectEventHandlers() {
			Debug.Assert(project != null);
			project.Opened += project_Opened;
			project.Closing += project_Closing;
			if (project.IsOpen) Initialize();
		}


		private void UnregisterProjectEventHandlers() {
			Uninitialize();
			if (project != null) {
				project.Opened -= project_Opened;
				project.Closing -= project_Closing;
			}
		}


		private void RegisterRepositoryEventHandlers() {
			Debug.Assert(project != null && project.Repository != null);
			if (project != null && project.Repository != null) {
				project.Repository.DesignInserted += Repository_DesignInserted;
				project.Repository.DesignUpdated += Repository_DesignUpdated;
				project.Repository.DesignDeleted += Repository_DesignDeleted;
				project.Repository.StyleInserted += Repository_StyleInserted;
				project.Repository.StyleUpdated += Repository_StyleUpdated;
				project.Repository.StyleDeleted += Repository_StyleDeleted;
			}
		}


		private void UnregisterRepositoryEventHandlers() {
			Debug.Assert(project != null && project.Repository != null);
			if (project != null && project.Repository != null) {
				project.Repository.DesignInserted -= Repository_DesignInserted;
				project.Repository.DesignUpdated -= Repository_DesignUpdated;
				project.Repository.DesignDeleted -= Repository_DesignDeleted;
				project.Repository.StyleInserted -= Repository_StyleInserted;
				project.Repository.StyleUpdated -= Repository_StyleUpdated;
				project.Repository.StyleDeleted -= Repository_StyleDeleted;
			}
		}

		#endregion


		#region [Private] Methods: Event handler implementations

		private void project_Opened(object sender, EventArgs e) {
			Initialize();
		}


		private void project_Closing(object sender, EventArgs e) {
			Uninitialize();
		}


		private void Repository_DesignInserted(object sender, RepositoryDesignEventArgs e) {
			if (DesignCreated != null) DesignCreated(this, GetDesignEventArgs(e.Design));
		}


		private void Repository_DesignUpdated(object sender, RepositoryDesignEventArgs e) {
			if (DesignChanged != null) DesignChanged(this, GetDesignEventArgs(e.Design));
		}


		private void Repository_DesignDeleted(object sender, RepositoryDesignEventArgs e) {
			if (DesignDeleted != null) DesignDeleted(this, GetDesignEventArgs(e.Design));
		}


		private void Repository_StyleInserted(object sender, RepositoryStyleEventArgs e) {
			if (StyleCreated != null) StyleCreated(this, GetStyleEventArgs(GetOwnerDesign(e.Style), e.Style));
		}


		private void Repository_StyleDeleted(object sender, RepositoryStyleEventArgs e) {
			if (StyleDeleted != null) StyleDeleted(this, GetStyleEventArgs(GetOwnerDesign(e.Style), e.Style));
		}


		private void Repository_StyleUpdated(object sender, RepositoryStyleEventArgs e) {
			if (StyleChanged != null) StyleChanged(this, GetStyleEventArgs(GetOwnerDesign(e.Style), e.Style));
		}

		#endregion


		#region Fields

		private Project project;
		private DesignEventArgs designEventArgs = new DesignEventArgs();
		private StyleEventArgs styleEventArgs = new StyleEventArgs();

		#endregion
	}


	#region EventArgs

	/// <ToBeCompleted></ToBeCompleted>
	public class DesignEventArgs : EventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public DesignEventArgs(Design design) {
			if (design == null) throw new ArgumentNullException("design");
			this.design = design;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public Design Design {
			get { return design; }
			internal set { design = value; }
		}

		internal DesignEventArgs() { }

		private Design design;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class StyleEventArgs : DesignEventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public StyleEventArgs(Design design, IStyle style)
			: base(design) {
			if (style == null) throw new ArgumentNullException("style");
			this.style = style;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public IStyle Style {
			get { return style; }
			internal set { style = value; }
		}

		internal StyleEventArgs() { }

		private IStyle style;
	}

	#endregion

}
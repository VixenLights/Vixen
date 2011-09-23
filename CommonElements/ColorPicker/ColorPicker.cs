using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.Serialization;
using CommonElements.ColorManagement.ColorModels;

namespace CommonElements.ColorPicker
{
	/// <summary>
	/// Zusammenfassung für Form1.
	/// </summary>
	public sealed class ColorPicker : System.Windows.Forms.Form
	{
		private ColorSelectionPlane colorSelectionPlane1;
		private ColorSelectionFader colorSelectionFader1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.RadioButton rdHSV_H;
		private System.Windows.Forms.RadioButton rdHSV_S;
		private System.Windows.Forms.RadioButton rdHSV_V;
		private System.Windows.Forms.RadioButton rdSecond_1;
		private System.Windows.Forms.RadioButton rdSecond_2;
		private System.Windows.Forms.RadioButton rdSecond_3;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem ctxHSV_RGB;
		private System.Windows.Forms.MenuItem ctxHSV_LAB;
		private System.Windows.Forms.TextBox tbHSV_H;
		private System.Windows.Forms.TextBox tbHSV_S;
		private System.Windows.Forms.TextBox tbHSV_V;
		private System.Windows.Forms.TextBox tbSecond_1;
		private System.Windows.Forms.TextBox tbSecond_2;
		private System.Windows.Forms.TextBox tbSecond_3;
		private System.Windows.Forms.Label lblHSV_H;
		private System.Windows.Forms.Label lblHSV_S;
		private System.Windows.Forms.Label lblHSV_V;
		private System.Windows.Forms.Label lblSecond_1;
		private System.Windows.Forms.Label lblSecond_2;
		private System.Windows.Forms.Label lblSecond_3;
		private ColorLabel lblColorOut;
		private System.Windows.Forms.MenuItem separator1;
		private System.Windows.Forms.MenuItem ctxPrevColor;
		private System.Windows.Forms.MenuItem ctxCopy;
		private ToolTip toolTip;
		private IContainer components;

		public ColorPicker():this(Mode.HSV_RGB,Fader.HSV_H){}
		public ColorPicker(Mode mode, Fader fader)
		{
			_mode=mode;
			_fader=fader;

			InitializeComponent();

			UpdateUI();
			filter=new ShiftKeyFilter();
			filter.ShiftStateChanged+=new EventHandler(filter_ShiftStateChanged);
			Application.AddMessageFilter(filter);
		}
		/// <summary>
		/// Die verwendeten Ressourcen bereinigen.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			if(filter!=null)
			{
				Application.RemoveMessageFilter(filter);
				filter=null;
			}
			base.Dispose( disposing );
		}
		#region Vom Windows Form-Designer generierter Code
		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPicker));
			this.label1 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.ctxHSV_RGB = new System.Windows.Forms.MenuItem();
			this.ctxHSV_LAB = new System.Windows.Forms.MenuItem();
			this.separator1 = new System.Windows.Forms.MenuItem();
			this.ctxPrevColor = new System.Windows.Forms.MenuItem();
			this.ctxCopy = new System.Windows.Forms.MenuItem();
			this.rdHSV_H = new System.Windows.Forms.RadioButton();
			this.rdHSV_S = new System.Windows.Forms.RadioButton();
			this.rdHSV_V = new System.Windows.Forms.RadioButton();
			this.rdSecond_1 = new System.Windows.Forms.RadioButton();
			this.rdSecond_2 = new System.Windows.Forms.RadioButton();
			this.rdSecond_3 = new System.Windows.Forms.RadioButton();
			this.tbHSV_H = new System.Windows.Forms.TextBox();
			this.tbHSV_S = new System.Windows.Forms.TextBox();
			this.tbHSV_V = new System.Windows.Forms.TextBox();
			this.tbSecond_1 = new System.Windows.Forms.TextBox();
			this.tbSecond_2 = new System.Windows.Forms.TextBox();
			this.tbSecond_3 = new System.Windows.Forms.TextBox();
			this.lblHSV_H = new System.Windows.Forms.Label();
			this.lblHSV_S = new System.Windows.Forms.Label();
			this.lblHSV_V = new System.Windows.Forms.Label();
			this.lblSecond_1 = new System.Windows.Forms.Label();
			this.lblSecond_2 = new System.Windows.Forms.Label();
			this.lblSecond_3 = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.lblColorOut = new CommonElements.ColorPicker.ColorLabel();
			this.colorSelectionFader1 = new ColorSelectionFader();
			this.colorSelectionPlane1 = new ColorSelectionPlane();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AccessibleDescription = null;
			this.label1.AccessibleName = null;
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.Color.Silver;
			this.label1.Font = null;
			this.label1.Name = "label1";
			this.toolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleDescription = null;
			this.btnCancel.AccessibleName = null;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.BackgroundImage = null;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = null;
			this.btnCancel.Name = "btnCancel";
			this.toolTip.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
			// 
			// btnOK
			// 
			this.btnOK.AccessibleDescription = null;
			this.btnOK.AccessibleName = null;
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.BackgroundImage = null;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Font = null;
			this.btnOK.Name = "btnOK";
			this.toolTip.SetToolTip(this.btnOK, resources.GetString("btnOK.ToolTip"));
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ctxHSV_RGB,
            this.ctxHSV_LAB,
            this.separator1,
            this.ctxPrevColor,
            this.ctxCopy});
			resources.ApplyResources(this.contextMenu, "contextMenu");
			// 
			// ctxHSV_RGB
			// 
			this.ctxHSV_RGB.Checked = true;
			resources.ApplyResources(this.ctxHSV_RGB, "ctxHSV_RGB");
			this.ctxHSV_RGB.Index = 0;
			this.ctxHSV_RGB.RadioCheck = true;
			this.ctxHSV_RGB.Click += new System.EventHandler(this.ctxOptions_Click);
			// 
			// ctxHSV_LAB
			// 
			resources.ApplyResources(this.ctxHSV_LAB, "ctxHSV_LAB");
			this.ctxHSV_LAB.Index = 1;
			this.ctxHSV_LAB.RadioCheck = true;
			this.ctxHSV_LAB.Click += new System.EventHandler(this.ctxOptions_Click);
			// 
			// separator1
			// 
			resources.ApplyResources(this.separator1, "separator1");
			this.separator1.Index = 2;
			// 
			// ctxPrevColor
			// 
			resources.ApplyResources(this.ctxPrevColor, "ctxPrevColor");
			this.ctxPrevColor.Index = 3;
			this.ctxPrevColor.Click += new System.EventHandler(this.ctxOptions_Click);
			// 
			// ctxCopy
			// 
			resources.ApplyResources(this.ctxCopy, "ctxCopy");
			this.ctxCopy.Index = 4;
			this.ctxCopy.Click += new System.EventHandler(this.ctxOptions_Click);
			// 
			// rdHSV_H
			// 
			this.rdHSV_H.AccessibleDescription = null;
			this.rdHSV_H.AccessibleName = null;
			resources.ApplyResources(this.rdHSV_H, "rdHSV_H");
			this.rdHSV_H.BackgroundImage = null;
			this.rdHSV_H.Checked = true;
			this.rdHSV_H.Font = null;
			this.rdHSV_H.Name = "rdHSV_H";
			this.rdHSV_H.TabStop = true;
			this.toolTip.SetToolTip(this.rdHSV_H, resources.GetString("rdHSV_H.ToolTip"));
			this.rdHSV_H.CheckedChanged += new System.EventHandler(this.UpdaterdFaderedChanged);
			// 
			// rdHSV_S
			// 
			this.rdHSV_S.AccessibleDescription = null;
			this.rdHSV_S.AccessibleName = null;
			resources.ApplyResources(this.rdHSV_S, "rdHSV_S");
			this.rdHSV_S.BackgroundImage = null;
			this.rdHSV_S.Font = null;
			this.rdHSV_S.Name = "rdHSV_S";
			this.toolTip.SetToolTip(this.rdHSV_S, resources.GetString("rdHSV_S.ToolTip"));
			this.rdHSV_S.CheckedChanged += new System.EventHandler(this.UpdaterdFaderedChanged);
			// 
			// rdHSV_V
			// 
			this.rdHSV_V.AccessibleDescription = null;
			this.rdHSV_V.AccessibleName = null;
			resources.ApplyResources(this.rdHSV_V, "rdHSV_V");
			this.rdHSV_V.BackgroundImage = null;
			this.rdHSV_V.Font = null;
			this.rdHSV_V.Name = "rdHSV_V";
			this.toolTip.SetToolTip(this.rdHSV_V, resources.GetString("rdHSV_V.ToolTip"));
			this.rdHSV_V.CheckedChanged += new System.EventHandler(this.UpdaterdFaderedChanged);
			// 
			// rdSecond_1
			// 
			this.rdSecond_1.AccessibleDescription = null;
			this.rdSecond_1.AccessibleName = null;
			resources.ApplyResources(this.rdSecond_1, "rdSecond_1");
			this.rdSecond_1.BackgroundImage = null;
			this.rdSecond_1.Font = null;
			this.rdSecond_1.Name = "rdSecond_1";
			this.toolTip.SetToolTip(this.rdSecond_1, resources.GetString("rdSecond_1.ToolTip"));
			this.rdSecond_1.CheckedChanged += new System.EventHandler(this.UpdaterdFaderedChanged);
			// 
			// rdSecond_2
			// 
			this.rdSecond_2.AccessibleDescription = null;
			this.rdSecond_2.AccessibleName = null;
			resources.ApplyResources(this.rdSecond_2, "rdSecond_2");
			this.rdSecond_2.BackgroundImage = null;
			this.rdSecond_2.Font = null;
			this.rdSecond_2.Name = "rdSecond_2";
			this.toolTip.SetToolTip(this.rdSecond_2, resources.GetString("rdSecond_2.ToolTip"));
			this.rdSecond_2.CheckedChanged += new System.EventHandler(this.UpdaterdFaderedChanged);
			// 
			// rdSecond_3
			// 
			this.rdSecond_3.AccessibleDescription = null;
			this.rdSecond_3.AccessibleName = null;
			resources.ApplyResources(this.rdSecond_3, "rdSecond_3");
			this.rdSecond_3.BackgroundImage = null;
			this.rdSecond_3.Font = null;
			this.rdSecond_3.Name = "rdSecond_3";
			this.toolTip.SetToolTip(this.rdSecond_3, resources.GetString("rdSecond_3.ToolTip"));
			this.rdSecond_3.CheckedChanged += new System.EventHandler(this.UpdaterdFaderedChanged);
			// 
			// tbHSV_H
			// 
			this.tbHSV_H.AccessibleDescription = null;
			this.tbHSV_H.AccessibleName = null;
			resources.ApplyResources(this.tbHSV_H, "tbHSV_H");
			this.tbHSV_H.BackgroundImage = null;
			this.tbHSV_H.Font = null;
			this.tbHSV_H.Name = "tbHSV_H";
			this.toolTip.SetToolTip(this.tbHSV_H, resources.GetString("tbHSV_H.ToolTip"));
			this.tbHSV_H.Leave += new System.EventHandler(this.tbValue_Leave);
			this.tbHSV_H.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbValue_KeyUp);
			// 
			// tbHSV_S
			// 
			this.tbHSV_S.AccessibleDescription = null;
			this.tbHSV_S.AccessibleName = null;
			resources.ApplyResources(this.tbHSV_S, "tbHSV_S");
			this.tbHSV_S.BackgroundImage = null;
			this.tbHSV_S.Font = null;
			this.tbHSV_S.Name = "tbHSV_S";
			this.toolTip.SetToolTip(this.tbHSV_S, resources.GetString("tbHSV_S.ToolTip"));
			this.tbHSV_S.Leave += new System.EventHandler(this.tbValue_Leave);
			this.tbHSV_S.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbValue_KeyUp);
			// 
			// tbHSV_V
			// 
			this.tbHSV_V.AccessibleDescription = null;
			this.tbHSV_V.AccessibleName = null;
			resources.ApplyResources(this.tbHSV_V, "tbHSV_V");
			this.tbHSV_V.BackgroundImage = null;
			this.tbHSV_V.Font = null;
			this.tbHSV_V.Name = "tbHSV_V";
			this.toolTip.SetToolTip(this.tbHSV_V, resources.GetString("tbHSV_V.ToolTip"));
			this.tbHSV_V.Leave += new System.EventHandler(this.tbValue_Leave);
			this.tbHSV_V.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbValue_KeyUp);
			// 
			// tbSecond_1
			// 
			this.tbSecond_1.AccessibleDescription = null;
			this.tbSecond_1.AccessibleName = null;
			resources.ApplyResources(this.tbSecond_1, "tbSecond_1");
			this.tbSecond_1.BackgroundImage = null;
			this.tbSecond_1.Font = null;
			this.tbSecond_1.Name = "tbSecond_1";
			this.toolTip.SetToolTip(this.tbSecond_1, resources.GetString("tbSecond_1.ToolTip"));
			this.tbSecond_1.Leave += new System.EventHandler(this.tbValue_Leave);
			this.tbSecond_1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbValue_KeyUp);
			// 
			// tbSecond_2
			// 
			this.tbSecond_2.AccessibleDescription = null;
			this.tbSecond_2.AccessibleName = null;
			resources.ApplyResources(this.tbSecond_2, "tbSecond_2");
			this.tbSecond_2.BackgroundImage = null;
			this.tbSecond_2.Font = null;
			this.tbSecond_2.Name = "tbSecond_2";
			this.toolTip.SetToolTip(this.tbSecond_2, resources.GetString("tbSecond_2.ToolTip"));
			this.tbSecond_2.Leave += new System.EventHandler(this.tbValue_Leave);
			this.tbSecond_2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbValue_KeyUp);
			// 
			// tbSecond_3
			// 
			this.tbSecond_3.AccessibleDescription = null;
			this.tbSecond_3.AccessibleName = null;
			resources.ApplyResources(this.tbSecond_3, "tbSecond_3");
			this.tbSecond_3.BackgroundImage = null;
			this.tbSecond_3.Font = null;
			this.tbSecond_3.Name = "tbSecond_3";
			this.toolTip.SetToolTip(this.tbSecond_3, resources.GetString("tbSecond_3.ToolTip"));
			this.tbSecond_3.Leave += new System.EventHandler(this.tbValue_Leave);
			this.tbSecond_3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbValue_KeyUp);
			// 
			// lblHSV_H
			// 
			this.lblHSV_H.AccessibleDescription = null;
			this.lblHSV_H.AccessibleName = null;
			resources.ApplyResources(this.lblHSV_H, "lblHSV_H");
			this.lblHSV_H.Font = null;
			this.lblHSV_H.Name = "lblHSV_H";
			this.toolTip.SetToolTip(this.lblHSV_H, resources.GetString("lblHSV_H.ToolTip"));
			// 
			// lblHSV_S
			// 
			this.lblHSV_S.AccessibleDescription = null;
			this.lblHSV_S.AccessibleName = null;
			resources.ApplyResources(this.lblHSV_S, "lblHSV_S");
			this.lblHSV_S.Font = null;
			this.lblHSV_S.Name = "lblHSV_S";
			this.toolTip.SetToolTip(this.lblHSV_S, resources.GetString("lblHSV_S.ToolTip"));
			// 
			// lblHSV_V
			// 
			this.lblHSV_V.AccessibleDescription = null;
			this.lblHSV_V.AccessibleName = null;
			resources.ApplyResources(this.lblHSV_V, "lblHSV_V");
			this.lblHSV_V.Font = null;
			this.lblHSV_V.Name = "lblHSV_V";
			this.toolTip.SetToolTip(this.lblHSV_V, resources.GetString("lblHSV_V.ToolTip"));
			// 
			// lblSecond_1
			// 
			this.lblSecond_1.AccessibleDescription = null;
			this.lblSecond_1.AccessibleName = null;
			resources.ApplyResources(this.lblSecond_1, "lblSecond_1");
			this.lblSecond_1.Font = null;
			this.lblSecond_1.Name = "lblSecond_1";
			this.toolTip.SetToolTip(this.lblSecond_1, resources.GetString("lblSecond_1.ToolTip"));
			// 
			// lblSecond_2
			// 
			this.lblSecond_2.AccessibleDescription = null;
			this.lblSecond_2.AccessibleName = null;
			resources.ApplyResources(this.lblSecond_2, "lblSecond_2");
			this.lblSecond_2.Font = null;
			this.lblSecond_2.Name = "lblSecond_2";
			this.toolTip.SetToolTip(this.lblSecond_2, resources.GetString("lblSecond_2.ToolTip"));
			// 
			// lblSecond_3
			// 
			this.lblSecond_3.AccessibleDescription = null;
			this.lblSecond_3.AccessibleName = null;
			resources.ApplyResources(this.lblSecond_3, "lblSecond_3");
			this.lblSecond_3.Font = null;
			this.lblSecond_3.Name = "lblSecond_3";
			this.toolTip.SetToolTip(this.lblSecond_3, resources.GetString("lblSecond_3.ToolTip"));
			// 
			// toolTip
			// 
			this.toolTip.AutomaticDelay = 1000;
			this.toolTip.AutoPopDelay = 5000;
			this.toolTip.InitialDelay = 1000;
			this.toolTip.ReshowDelay = 200;
			// 
			// lblColorOut
			// 
			this.lblColorOut.AccessibleDescription = null;
			this.lblColorOut.AccessibleName = null;
			resources.ApplyResources(this.lblColorOut, "lblColorOut");
			this.lblColorOut.BackgroundImage = null;
			this.lblColorOut.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.lblColorOut.ContextMenu = this.contextMenu;
			this.lblColorOut.Name = "lblColorOut";
			this.lblColorOut.OldColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.toolTip.SetToolTip(this.lblColorOut, resources.GetString("lblColorOut.ToolTip"));
			this.lblColorOut.ColorChanged += new System.EventHandler(this.lblColorOut_ColorChanged);
			// 
			// colorSelectionFader1
			// 
			this.colorSelectionFader1.AccessibleDescription = null;
			this.colorSelectionFader1.AccessibleName = null;
			resources.ApplyResources(this.colorSelectionFader1, "colorSelectionFader1");
			this.colorSelectionFader1.BackgroundImage = null;
			this.colorSelectionFader1.Font = null;
			this.colorSelectionFader1.Name = "colorSelectionFader1";
			this.colorSelectionFader1.TabStop = false;
			this.toolTip.SetToolTip(this.colorSelectionFader1, resources.GetString("colorSelectionFader1.ToolTip"));
			// 
			// colorSelectionPlane1
			// 
			this.colorSelectionPlane1.AccessibleDescription = null;
			this.colorSelectionPlane1.AccessibleName = null;
			resources.ApplyResources(this.colorSelectionPlane1, "colorSelectionPlane1");
			this.colorSelectionPlane1.BackgroundImage = null;
			this.colorSelectionPlane1.Font = null;
			this.colorSelectionPlane1.Name = "colorSelectionPlane1";
			this.colorSelectionPlane1.TabStop = false;
			this.toolTip.SetToolTip(this.colorSelectionPlane1, resources.GetString("colorSelectionPlane1.ToolTip"));
			// 
			// ColorPicker
			// 
			this.AcceptButton = this.btnOK;
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.BackgroundImage = null;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.lblColorOut);
			this.Controls.Add(this.lblHSV_H);
			this.Controls.Add(this.tbSecond_3);
			this.Controls.Add(this.tbSecond_2);
			this.Controls.Add(this.tbSecond_1);
			this.Controls.Add(this.tbHSV_V);
			this.Controls.Add(this.tbHSV_S);
			this.Controls.Add(this.tbHSV_H);
			this.Controls.Add(this.rdHSV_H);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.colorSelectionFader1);
			this.Controls.Add(this.colorSelectionPlane1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.rdHSV_S);
			this.Controls.Add(this.rdHSV_V);
			this.Controls.Add(this.rdSecond_1);
			this.Controls.Add(this.rdSecond_2);
			this.Controls.Add(this.rdSecond_3);
			this.Controls.Add(this.lblHSV_S);
			this.Controls.Add(this.lblHSV_V);
			this.Controls.Add(this.lblSecond_1);
			this.Controls.Add(this.lblSecond_2);
			this.Controls.Add(this.lblSecond_3);
			this.Font = null;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = null;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColorPicker";
			this.ShowInTaskbar = false;
			this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region types
		private class ShiftKeyFilter:IMessageFilter
		{
			private const int WM_KEYDOWN = 0x100;
			private const int WM_KEYUP = 0x101;

			public bool PreFilterMessage(ref Message m)
			{
				switch(m.Msg)
				{
					case WM_KEYDOWN:
						if(m.WParam.ToInt32()==(int)Keys.ShiftKey)
						{
							RaiseShiftStateChanged();
							return true;
						}break;
					case WM_KEYUP:
						if(m.WParam.ToInt32()==(int)Keys.ShiftKey)
						{
							RaiseShiftStateChanged();
							return true;
						}break;
				}
				return false;
			}
			private void RaiseShiftStateChanged()
			{
				if(ShiftStateChanged!=null)
							ShiftStateChanged(this,EventArgs.Empty);
			}
			public event EventHandler ShiftStateChanged;
		}

		public enum Fader
		{
			HSV_H=0,
			HSV_S=1,
			HSV_V=2,

			Second_1=3,
			Second_2=4,
			Second_3=5
		}
		public enum Mode
		{
			HSV_RGB=0,
			HSV_LAB=1
		}
		#endregion
		#region variables
		private ShiftKeyFilter filter;
		private ColorSelectionModule _module;
		private XYZ _color=XYZ.White;
		private Mode _mode=Mode.HSV_RGB;
		private Fader _fader=Fader.HSV_H;
		#endregion
		#region ui updating
		public void UpdateUI()
		{
			ChangeModule();
			ChangeDescriptions();
			UpdaterdFader();
			UpdatectxOptions();
			UpdatetbValue(null);
			
			_module.XYZ=_color;
			lblColorOut.Color=
				lblColorOut.OldColor=_color.ToRGB();
		}
		#region module
		private void ChangeModule(ColorSelectionModule value)
		{
			if(value==_module) return;
			if(_module!=null)
			{
				_module.ColorChanged-=new EventHandler(_module_ColorChanged);
				_module.ColorSelectionFader=null;
				_module.ColorSelectionPlane=null;
			}
			_module=value;
			if(_module!=null)
			{
				_module.ColorChanged+=new EventHandler(_module_ColorChanged);
				_module.XYZ=_color;
				_module.ColorSelectionFader=colorSelectionFader1;
				_module.ColorSelectionPlane=colorSelectionPlane1;
			}

		}
		private void ChangeModule()
		{
			switch(_fader)
			{
				case Fader.HSV_H: ChangeModule(new ColorSelectionModuleHSV_H()); break;
				case Fader.HSV_S: ChangeModule(new ColorSelectionModuleHSV_S()); break;
				case Fader.HSV_V: ChangeModule(new ColorSelectionModuleHSV_V()); break;
				case Fader.Second_1:
					if(_mode==Mode.HSV_RGB)
						ChangeModule(new ColorSelectionModuleRGB_R());
					else
						ChangeModule(new ColorSelectionModuleLAB_L());
					break;
				case Fader.Second_2:
					if(_mode==Mode.HSV_RGB)
						ChangeModule(new ColorSelectionModuleRGB_G());
					else
						ChangeModule(new ColorSelectionModuleLAB_a());
					break;
				default:
					if(_mode==Mode.HSV_RGB)
						ChangeModule(new ColorSelectionModuleRGB_B());
					else
						ChangeModule(new ColorSelectionModuleLAB_b()); break;
			}
		}
		private void ChangeDescriptions()
		{
			switch(_mode)
			{
				case Mode.HSV_RGB:
					rdSecond_1.Text="R";
					rdSecond_2.Text="G";
					rdSecond_3.Text="B";
					break;
				default:
					rdSecond_1.Text="L";
					rdSecond_2.Text="a*";
					rdSecond_3.Text="b*";
					break;
			}
		}
		#endregion
		#region contextmenu
		private void ctxOptions_Click(object sender, System.EventArgs e)
		{
			Mode newmode=_mode;
			if(sender==ctxPrevColor)
			{
				Color=XYZ.FromRGB(lblColorOut.OldColor);
				return;
			}
			else if(sender==ctxCopy)
			{
				string str=ColorLabel.ColorToHexString(lblColorOut.Color);
				try
				{
					Clipboard.SetDataObject(str,true);
				}
				catch{}
				return;
			}
			//read checkbox
			else if(sender==ctxHSV_RGB)
				newmode=Mode.HSV_RGB;
			else if(sender==ctxHSV_LAB)
				newmode=Mode.HSV_LAB;
			//compare to old
			if(newmode==_mode) return;
			//update ui
			_mode=newmode;
			UpdatectxOptions();
			ChangeDescriptions();
			ChangeModule();
			UpdatetbValue(null);
		}
		private void UpdatectxOptions()
		{
			ctxHSV_RGB.Checked=_mode==Mode.HSV_RGB;
			ctxHSV_LAB.Checked=_mode==Mode.HSV_LAB;
		}
		#endregion
		#region rdFader
		private void UpdaterdFaderedChanged(object sender, System.EventArgs e)
		{
			if(sender==rdHSV_H)
				_fader=Fader.HSV_H;
			else if(sender==rdHSV_S)
				_fader=Fader.HSV_S;
			else if(sender==rdHSV_V)
				_fader=Fader.HSV_V;
				//secondary faders
			else if(sender==rdSecond_1)
				_fader=Fader.Second_1;
			else if(sender==rdSecond_2)
				_fader=Fader.Second_2;
			else//(sender==rdSecond_3)
				_fader=Fader.Second_3;

            ChangeModule();
		}
		private void UpdaterdFader()
		{
			if(_fader==Fader.HSV_H)
				rdHSV_H.Checked=true;
			else if(_fader==Fader.HSV_S)
				rdHSV_S.Checked=true;
			else if(_fader==Fader.HSV_V)
				rdHSV_V.Checked=true;
			else if(_fader==Fader.Second_1)
				rdSecond_1.Checked=true;
			else if(_fader==Fader.Second_2)
				rdSecond_2.Checked=true;
			else if(_fader==Fader.Second_3)
				rdSecond_3.Checked=true;
		}
		#endregion
		#region tbValue
		private void tbValue_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(!(sender is TextBox)) return;
			if(e.KeyCode==Keys.Return)
			{
				UpdatetbValue(null);
				e.Handled=true;
				return;
			}
			double value;
			if(!double.TryParse(((TextBox)sender).Text,
				System.Globalization.NumberStyles.Integer,
				null,out value)) return;
			#region hsv  textboxes
			if(sender==tbHSV_H)
			{
				HSV chsv=HSV.FromRGB(_color.ToRGB());
				chsv.H=value/360.0;
				_color=XYZ.FromRGB(chsv.ToRGB());
			}
			else if(sender==tbHSV_S)
			{
				HSV chsv=HSV.FromRGB(_color.ToRGB());
				chsv.S=value/100.0;
				_color=XYZ.FromRGB(chsv.ToRGB());
			}
			else if(sender==tbHSV_V)
			{
				HSV chsv=HSV.FromRGB(_color.ToRGB());
				chsv.V=value/100.0;
				_color=XYZ.FromRGB(chsv.ToRGB());
			}
			#endregion
			#region secondary textboxes
			else if(_mode==Mode.HSV_RGB)
			{
				RGB crgb=_color.ToRGB();
				if(sender==tbSecond_1)
				{
					crgb.R=value/255.0;
				}
				else if(sender==tbSecond_2)
				{
					crgb.G=value/255.0;
				}
				else //sender==tbSecond_3
				{
					crgb.B=value/255.0;
				}
				_color=XYZ.FromRGB(crgb);
			}
			else if(_mode==Mode.HSV_LAB)
			{
				LAB clab=LAB.FromXYZ(_color);
				if(sender==tbSecond_1)
				{
					clab.L=value;
				}
				else if(sender==tbSecond_2)
				{
					clab.a=value;
				}
				else //sender==tbSecond_3
				{
					clab.b=value;
				}
				_color=clab.ToXYZ();
			}
			#endregion
			//update ui
			_module.XYZ=_color;
			lblColorOut.Color=_color.ToRGB();
			UpdatetbValue((TextBox)sender);
		}		
		private void tbValue_Leave(object sender, System.EventArgs e)
		{
			UpdatetbValue(null);
		}
		private void UpdatetbValue(TextBox skipupdate)
		{
			#region hsv textboxes
			HSV chsv=HSV.FromRGB(_color.ToRGB());
			if(skipupdate!=tbHSV_H)
				tbHSV_H.Text=(chsv.H*360.0).ToString("0");
			if(skipupdate!=tbHSV_S)
				tbHSV_S.Text=(chsv.S*100.0).ToString("0");
			if(skipupdate!=tbHSV_V)
				tbHSV_V.Text=(chsv.V*100.0).ToString("0");
			#endregion
			#region secondary textboxes
			if(_mode==Mode.HSV_RGB)
			{
				RGB crgb=_color.ToRGB();
				if(skipupdate!=tbSecond_1)
					tbSecond_1.Text=(crgb.R*255.0).ToString("0");
				if(skipupdate!=tbSecond_2)
					tbSecond_2.Text=(crgb.G*255.0).ToString("0");
				if(skipupdate!=tbSecond_3)
					tbSecond_3.Text=(crgb.B*255.0).ToString("0");
			}
			else//(_mode==Mode.HSV_LAB)
			{
				LAB clab=LAB.FromXYZ(_color);
				if(skipupdate!=tbSecond_1)
					tbSecond_1.Text=clab.L.ToString("0");
				if(skipupdate!=tbSecond_2)
					tbSecond_2.Text=clab.a.ToString("0");
				if(skipupdate!=tbSecond_3)
					tbSecond_3.Text=clab.b.ToString("0");
			}
			#endregion
		}
		#endregion
		#region module & lbl
		private void _module_ColorChanged(object sender, EventArgs e)
		{
			if(_module==null) return;
			_color=_module.XYZ;
			lblColorOut.Color=_color.ToRGB();
			UpdatetbValue(null);
		}

		private void lblColorOut_ColorChanged(object sender, System.EventArgs e)
		{
			_color=XYZ.FromRGB(lblColorOut.Color);
			_module.XYZ=_color;
			UpdatetbValue(null);
		}
		#endregion
		#endregion
		#region properties
		/// <summary>
		/// gets or sets the color as device-independent CIE-XYZ color
		/// </summary>
		[Description("gets or sets the color as device-independent CIE-XYZ color")]
		public XYZ Color
		{
			get{return _color;}
			set
			{
				if(value==_color) return;
				_color=_module.XYZ=value;
				lblColorOut.Color=
					lblColorOut.OldColor=value.ToRGB();
				UpdatetbValue(null);
			}
		}
		[Browsable(false)]
		public Fader PrimaryFader
		{
			get{return _fader;}
//			set
//			{
//				if(value==_fader) return;
//				_fader=value;
//				UpdaterdFader();
//				ChangeModule();
//			}
		}
		[Browsable(false)]
		public Mode SecondaryMode
		{
			get{return _mode;}
//			set
//			{
//				if(value==_mode) return;
//				_mode=value;
//				UpdatectxOptions();
//				ChangeModule();
//				UpdatetbValue(null);
//			}
		}
		#endregion
		private void filter_ShiftStateChanged(object sender, EventArgs e)
		{
			colorSelectionPlane1.Refresh();
			colorSelectionFader1.Refresh();
		}
	}
}

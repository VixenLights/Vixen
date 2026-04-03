using System.ComponentModel;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.Theme;
using Message = System.Windows.Forms.Message;

namespace Common.Controls.ColorManagement.ColorPicker
{
	/// <summary>
	/// Zusammenfassung f�r Form1.
	/// </summary>
	public sealed class ColorPicker : Form
	{
		private ColorSelectionPlane colorSelectionPlane1;
		private ColorSelectionFader colorSelectionFader1;
		private Label label1;
		private Button btnCancel;
		private Button btnOK;
		private RadioButton rdHSV_H;
		private RadioButton rdHSV_S;
		private RadioButton rdHSV_V;
		private RadioButton rdSecond_1;
		private RadioButton rdSecond_2;
		private RadioButton rdSecond_3;
		private ContextMenuStrip contextMenu;
		private ToolStripMenuItem ctxHSV_RGB;
		private ToolStripMenuItem ctxHSV_LAB;
		private TextBox tbHSV_H;
		private TextBox tbHSV_S;
		private TextBox tbHSV_V;
		private TextBox tbSecond_1;
		private TextBox tbSecond_2;
		private TextBox tbSecond_3;
		private Label lblHSV_H;
		private Label lblHSV_S;
		private Label lblHSV_V;
		private Label lblSecond_1;
		private Label lblSecond_2;
		private Label lblSecond_3;
		private ColorLabel lblColorOut;
		private ToolStripMenuItem separator1;
		private ToolStripMenuItem ctxPrevColor;
		private ToolStripMenuItem ctxCopy;
		private ToolTip toolTip;
		private GroupBox quickPickBox;
		private Button blueButton;
		private Button greenButton;
		private Button redButton;
		private Button whiteButton;
		private IContainer components;

		public ColorPicker() : this(Mode.HSV_RGB, Fader.HSV_H)
		{
		}

		public ColorPicker(Mode mode, Fader fader)
		{
			_mode = mode;
			_fader = fader;

			InitializeComponent();
			List<Control> ignoreControls = new List<Control>() {whiteButton, redButton, greenButton, blueButton };
			ThemeUpdateControls.UpdateControls(this, ignoreControls);
			whiteButton.BackColor = System.Drawing.Color.White;
			whiteButton.BackgroundImage = null;
			redButton.BackColor = System.Drawing.Color.Red;
			redButton.BackgroundImage = null;
			greenButton.BackColor = System.Drawing.Color.Lime;
			greenButton.BackgroundImage = null;
			blueButton.BackColor = System.Drawing.Color.Blue;
			blueButton.BackgroundImage = null;

			UpdateUI();
			filter = new ShiftKeyFilter();
			filter.ShiftStateChanged += new EventHandler(filter_ShiftStateChanged);
			Application.AddMessageFilter(filter);
		}

		/// <summary>
		/// Die verwendeten Ressourcen bereinigen.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			if (filter != null) {
				Application.RemoveMessageFilter(filter);
				filter = null;
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			components = new Container();
			ComponentResourceManager resources = new ComponentResourceManager(typeof(ColorPicker));
			label1 = new Label();
			btnCancel = new Button();
			btnOK = new Button();
			contextMenu = new ContextMenuStrip();
			ctxHSV_RGB = new ToolStripMenuItem();
			ctxHSV_LAB = new ToolStripMenuItem();
			separator1 = new ToolStripMenuItem();
			ctxPrevColor = new ToolStripMenuItem();
			ctxCopy = new ToolStripMenuItem();
			rdHSV_H = new RadioButton();
			rdHSV_S = new RadioButton();
			rdHSV_V = new RadioButton();
			rdSecond_1 = new RadioButton();
			rdSecond_2 = new RadioButton();
			rdSecond_3 = new RadioButton();
			tbHSV_H = new TextBox();
			tbHSV_S = new TextBox();
			tbHSV_V = new TextBox();
			tbSecond_1 = new TextBox();
			tbSecond_2 = new TextBox();
			tbSecond_3 = new TextBox();
			lblHSV_H = new Label();
			lblHSV_S = new Label();
			lblHSV_V = new Label();
			lblSecond_1 = new Label();
			lblSecond_2 = new Label();
			lblSecond_3 = new Label();
			toolTip = new ToolTip(components);
			quickPickBox = new GroupBox();
			whiteButton = new Button();
			blueButton = new Button();
			greenButton = new Button();
			redButton = new Button();
			lblColorOut = new ColorLabel();
			colorSelectionFader1 = new ColorSelectionFader();
			colorSelectionPlane1 = new ColorSelectionPlane();
			quickPickBox.SuspendLayout();
			SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			// 
			// btnCancel
			// 
			resources.ApplyResources(btnCancel, "btnCancel");
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Name = "btnCancel";
			btnCancel.UseVisualStyleBackColor = false;
			// 
			// btnOK
			// 
			resources.ApplyResources(btnOK, "btnOK");
			btnOK.DialogResult = DialogResult.OK;
			btnOK.Name = "btnOK";
			btnOK.UseVisualStyleBackColor = false;
			// 
			// contextMenu
			// 
			contextMenu.Items.AddRange(new ToolStripMenuItem[] {
            ctxHSV_RGB,
            ctxHSV_LAB,
            separator1,
            ctxPrevColor,
            ctxCopy});
			// 
			// ctxHSV_RGB
			// 
			ctxHSV_RGB.Checked = true;
			//this.ctxHSV_RGB.Index = 0;
			ctxHSV_RGB.Checked = true;
			resources.ApplyResources(ctxHSV_RGB, "ctxHSV_RGB");
			ctxHSV_RGB.Click += new EventHandler(ctxOptions_Click);
			// 
			// ctxHSV_LAB
			// 
			//this.ctxHSV_LAB.Index = 1;
			ctxHSV_LAB.Checked = true;
			resources.ApplyResources(ctxHSV_LAB, "ctxHSV_LAB");
			ctxHSV_LAB.Click += new EventHandler(ctxOptions_Click);
			// 
			// separator1
			// 
			//this.separator1.Index = 2;
			resources.ApplyResources(separator1, "separator1");
			// 
			// ctxPrevColor
			// 
			//this.ctxPrevColor.Index = 3;
			resources.ApplyResources(ctxPrevColor, "ctxPrevColor");
			ctxPrevColor.Click += new EventHandler(ctxOptions_Click);
			// 
			// ctxCopy
			// 
			//this.ctxCopy.Index = 4;
			resources.ApplyResources(ctxCopy, "ctxCopy");
			ctxCopy.Click += new EventHandler(ctxOptions_Click);
			// 
			// rdHSV_H
			// 
			rdHSV_H.Checked = true;
			resources.ApplyResources(rdHSV_H, "rdHSV_H");
			rdHSV_H.Name = "rdHSV_H";
			rdHSV_H.TabStop = true;
			rdHSV_H.CheckedChanged += new EventHandler(UpdaterdFaderedChanged);
			// 
			// rdHSV_S
			// 
			resources.ApplyResources(rdHSV_S, "rdHSV_S");
			rdHSV_S.Name = "rdHSV_S";
			rdHSV_S.CheckedChanged += new EventHandler(UpdaterdFaderedChanged);
			// 
			// rdHSV_V
			// 
			resources.ApplyResources(rdHSV_V, "rdHSV_V");
			rdHSV_V.Name = "rdHSV_V";
			rdHSV_V.CheckedChanged += new EventHandler(UpdaterdFaderedChanged);
			// 
			// rdSecond_1
			// 
			resources.ApplyResources(rdSecond_1, "rdSecond_1");
			rdSecond_1.Name = "rdSecond_1";
			rdSecond_1.CheckedChanged += new EventHandler(UpdaterdFaderedChanged);
			// 
			// rdSecond_2
			// 
			resources.ApplyResources(rdSecond_2, "rdSecond_2");
			rdSecond_2.Name = "rdSecond_2";
			rdSecond_2.CheckedChanged += new EventHandler(UpdaterdFaderedChanged);
			// 
			// rdSecond_3
			// 
			resources.ApplyResources(rdSecond_3, "rdSecond_3");
			rdSecond_3.Name = "rdSecond_3";
			rdSecond_3.CheckedChanged += new EventHandler(UpdaterdFaderedChanged);
			// 
			// tbHSV_H
			// 
			resources.ApplyResources(tbHSV_H, "tbHSV_H");
			tbHSV_H.Name = "tbHSV_H";
			tbHSV_H.KeyUp += new KeyEventHandler(tbValue_KeyUp);
			tbHSV_H.Leave += new EventHandler(tbValue_Leave);
			// 
			// tbHSV_S
			// 
			resources.ApplyResources(tbHSV_S, "tbHSV_S");
			tbHSV_S.Name = "tbHSV_S";
			tbHSV_S.KeyUp += new KeyEventHandler(tbValue_KeyUp);
			tbHSV_S.Leave += new EventHandler(tbValue_Leave);
			// 
			// tbHSV_V
			// 
			resources.ApplyResources(tbHSV_V, "tbHSV_V");
			tbHSV_V.Name = "tbHSV_V";
			tbHSV_V.KeyUp += new KeyEventHandler(tbValue_KeyUp);
			tbHSV_V.Leave += new EventHandler(tbValue_Leave);
			// 
			// tbSecond_1
			// 
			resources.ApplyResources(tbSecond_1, "tbSecond_1");
			tbSecond_1.Name = "tbSecond_1";
			tbSecond_1.KeyUp += new KeyEventHandler(tbValue_KeyUp);
			tbSecond_1.Leave += new EventHandler(tbValue_Leave);
			// 
			// tbSecond_2
			// 
			resources.ApplyResources(tbSecond_2, "tbSecond_2");
			tbSecond_2.Name = "tbSecond_2";
			tbSecond_2.KeyUp += new KeyEventHandler(tbValue_KeyUp);
			tbSecond_2.Leave += new EventHandler(tbValue_Leave);
			// 
			// tbSecond_3
			// 
			resources.ApplyResources(tbSecond_3, "tbSecond_3");
			tbSecond_3.Name = "tbSecond_3";
			tbSecond_3.KeyUp += new KeyEventHandler(tbValue_KeyUp);
			tbSecond_3.Leave += new EventHandler(tbValue_Leave);
			// 
			// lblHSV_H
			// 
			resources.ApplyResources(lblHSV_H, "lblHSV_H");
			lblHSV_H.Name = "lblHSV_H";
			// 
			// lblHSV_S
			// 
			resources.ApplyResources(lblHSV_S, "lblHSV_S");
			lblHSV_S.Name = "lblHSV_S";
			// 
			// lblHSV_V
			// 
			resources.ApplyResources(lblHSV_V, "lblHSV_V");
			lblHSV_V.Name = "lblHSV_V";
			// 
			// lblSecond_1
			// 
			resources.ApplyResources(lblSecond_1, "lblSecond_1");
			lblSecond_1.Name = "lblSecond_1";
			// 
			// lblSecond_2
			// 
			resources.ApplyResources(lblSecond_2, "lblSecond_2");
			lblSecond_2.Name = "lblSecond_2";
			// 
			// lblSecond_3
			// 
			resources.ApplyResources(lblSecond_3, "lblSecond_3");
			lblSecond_3.Name = "lblSecond_3";
			// 
			// toolTip
			// 
			toolTip.AutomaticDelay = 1000;
			toolTip.AutoPopDelay = 5000;
			toolTip.InitialDelay = 1000;
			toolTip.ReshowDelay = 200;
			// 
			// quickPickBox
			// 
			quickPickBox.Controls.Add(whiteButton);
			quickPickBox.Controls.Add(blueButton);
			quickPickBox.Controls.Add(greenButton);
			quickPickBox.Controls.Add(redButton);
			resources.ApplyResources(quickPickBox, "quickPickBox");
			quickPickBox.Name = "quickPickBox";
			quickPickBox.TabStop = false;
			quickPickBox.Paint += new PaintEventHandler(groupBoxes_Paint);
			// 
			// whiteButton
			// 
			whiteButton.BackColor = System.Drawing.Color.White;
			whiteButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			resources.ApplyResources(whiteButton, "whiteButton");
			whiteButton.Name = "whiteButton";
			whiteButton.UseVisualStyleBackColor = false;
			whiteButton.Click += new EventHandler(whiteButton_Click);
			// 
			// blueButton
			// 
			blueButton.BackColor = System.Drawing.Color.Blue;
			blueButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			resources.ApplyResources(blueButton, "blueButton");
			blueButton.Name = "blueButton";
			blueButton.UseVisualStyleBackColor = false;
			blueButton.Click += new EventHandler(blueButton_Click);
			// 
			// greenButton
			// 
			greenButton.BackColor = System.Drawing.Color.Green;
			greenButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			resources.ApplyResources(greenButton, "greenButton");
			greenButton.Name = "greenButton";
			greenButton.UseVisualStyleBackColor = false;
			greenButton.Click += new EventHandler(greenButton_Click);
			// 
			// redButton
			// 
			redButton.BackColor = System.Drawing.Color.Red;
			redButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			resources.ApplyResources(redButton, "redButton");
			redButton.Name = "redButton";
			redButton.UseVisualStyleBackColor = false;
			redButton.Click += new EventHandler(redButton_Click);
			// 
			// lblColorOut
			// 
			resources.ApplyResources(lblColorOut, "lblColorOut");
			lblColorOut.BackColor = System.Drawing.Color.WhiteSmoke;
			lblColorOut.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			lblColorOut.ContextMenuStrip = contextMenu;
			lblColorOut.Name = "lblColorOut";
			lblColorOut.OldColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			toolTip.SetToolTip(lblColorOut, resources.GetString("lblColorOut.ToolTip"));
			lblColorOut.ColorChanged += new EventHandler(lblColorOut_ColorChanged);
			// 
			// colorSelectionFader1
			// 
			resources.ApplyResources(colorSelectionFader1, "colorSelectionFader1");
			colorSelectionFader1.Name = "colorSelectionFader1";
			colorSelectionFader1.TabStop = false;
			toolTip.SetToolTip(colorSelectionFader1, resources.GetString("colorSelectionFader1.ToolTip"));
			// 
			// colorSelectionPlane1
			// 
			resources.ApplyResources(colorSelectionPlane1, "colorSelectionPlane1");
			colorSelectionPlane1.Name = "colorSelectionPlane1";
			colorSelectionPlane1.TabStop = false;
			toolTip.SetToolTip(colorSelectionPlane1, resources.GetString("colorSelectionPlane1.ToolTip"));
			// 
			// ColorPicker
			// 
			AcceptButton = btnOK;
			resources.ApplyResources(this, "$this");
			BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			CancelButton = btnCancel;
			Controls.Add(quickPickBox);
			Controls.Add(lblColorOut);
			Controls.Add(lblHSV_H);
			Controls.Add(tbSecond_3);
			Controls.Add(tbSecond_2);
			Controls.Add(tbSecond_1);
			Controls.Add(tbHSV_V);
			Controls.Add(tbHSV_S);
			Controls.Add(tbHSV_H);
			Controls.Add(rdHSV_H);
			Controls.Add(btnCancel);
			Controls.Add(label1);
			Controls.Add(colorSelectionFader1);
			Controls.Add(colorSelectionPlane1);
			Controls.Add(btnOK);
			Controls.Add(rdHSV_S);
			Controls.Add(rdHSV_V);
			Controls.Add(rdSecond_1);
			Controls.Add(rdSecond_2);
			Controls.Add(rdSecond_3);
			Controls.Add(lblHSV_S);
			Controls.Add(lblHSV_V);
			Controls.Add(lblSecond_1);
			Controls.Add(lblSecond_2);
			Controls.Add(lblSecond_3);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ColorPicker";
			ShowInTaskbar = false;
			quickPickBox.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		#region types

		private class ShiftKeyFilter : IMessageFilter
		{
			private const int WM_KEYDOWN = 0x100;
			private const int WM_KEYUP = 0x101;

			public bool PreFilterMessage(ref Message m)
			{
				switch (m.Msg) {
					case WM_KEYDOWN:
						if (m.WParam.ToInt32() == (int) Keys.ShiftKey) {
							RaiseShiftStateChanged();
							return true;
						}
						break;
					case WM_KEYUP:
						if (m.WParam.ToInt32() == (int) Keys.ShiftKey) {
							RaiseShiftStateChanged();
							return true;
						}
						break;
				}
				return false;
			}

			private void RaiseShiftStateChanged()
			{
				if (ShiftStateChanged != null)
					ShiftStateChanged(this, EventArgs.Empty);
			}

			public event EventHandler ShiftStateChanged;
		}

		public enum Fader
		{
			HSV_H = 0,
			HSV_S = 1,
			HSV_V = 2,

			Second_1 = 3,
			Second_2 = 4,
			Second_3 = 5
		}

		public enum Mode
		{
			HSV_RGB = 0,
			HSV_LAB = 1
		}

		#endregion

		#region variables

		private ShiftKeyFilter filter;
		private ColorSelectionModule _module;
		private XYZ _color = XYZ.White;
		private Mode _mode = Mode.HSV_RGB;
		private Fader _fader = Fader.HSV_H;
		private bool lockValueV = false;

		#endregion

		#region ui updating

		public void UpdateUI()
		{
			ChangeModule();
			ChangeDescriptions();
			UpdaterdFader();
			UpdatectxOptions();
			UpdatetbValue(null);

			_module.XYZ = InternalColor;
			lblColorOut.Color = lblColorOut.OldColor = InternalColor.ToRGB();
		}

		#region module

		private void ChangeModule(ColorSelectionModule value)
		{
			if (value == _module) return;
			if (_module != null) {
				_module.ColorChanged -= new EventHandler(_module_ColorChanged);
				_module.ColorSelectionFader = null;
				_module.ColorSelectionPlane = null;
			}
			_module = value;
			if (_module != null) {
				_module.ColorChanged += new EventHandler(_module_ColorChanged);
				_module.XYZ = InternalColor;
				_module.ColorSelectionFader = colorSelectionFader1;
				_module.ColorSelectionPlane = colorSelectionPlane1;
				
			}
		}

		private void ChangeModule()
		{
			switch (_fader) {
				case Fader.HSV_H:
					ChangeModule(new ColorSelectionModuleHSV_H());
					break;
				case Fader.HSV_S:
					ChangeModule(new ColorSelectionModuleHSV_S());
					break;
				case Fader.HSV_V:
					ChangeModule(new ColorSelectionModuleHSV_V());
					break;
				case Fader.Second_1:
					if (_mode == Mode.HSV_RGB)
						ChangeModule(new ColorSelectionModuleRGB_R());
					else
						ChangeModule(new ColorSelectionModuleLAB_L());
					break;
				case Fader.Second_2:
					if (_mode == Mode.HSV_RGB)
						ChangeModule(new ColorSelectionModuleRGB_G());
					else
						ChangeModule(new ColorSelectionModuleLAB_a());
					break;
				default:
					if (_mode == Mode.HSV_RGB)
						ChangeModule(new ColorSelectionModuleRGB_B());
					else
						ChangeModule(new ColorSelectionModuleLAB_b());
					break;
			}
		}

		private void ChangeDescriptions()
		{
			switch (_mode) {
				case Mode.HSV_RGB:
					rdSecond_1.Text = "R";
					rdSecond_2.Text = "G";
					rdSecond_3.Text = "B";
					break;
				default:
					rdSecond_1.Text = "L";
					rdSecond_2.Text = "a*";
					rdSecond_3.Text = "b*";
					break;
			}
		}

		#endregion

		#region contextmenu

		private void ctxOptions_Click(object sender, EventArgs e)
		{
			Mode newmode = _mode;
			if (sender == ctxPrevColor) {
				Color = XYZ.FromRGB(lblColorOut.OldColor);
				return;
			}
			else if (sender == ctxCopy) {
				string str = ColorLabel.ColorToHexString(lblColorOut.Color);
				try {
					Clipboard.SetDataObject(str, true);
				}
				catch {
				}
				return;
			}
				//read checkbox
			else if (sender == ctxHSV_RGB)
				newmode = Mode.HSV_RGB;
			else if (sender == ctxHSV_LAB)
				newmode = Mode.HSV_LAB;
			//compare to old
			if (newmode == _mode) return;
			//update ui
			_mode = newmode;
			UpdatectxOptions();
			ChangeDescriptions();
			ChangeModule();
			UpdatetbValue(null);
		}

		private void UpdatectxOptions()
		{
			ctxHSV_RGB.Checked = _mode == Mode.HSV_RGB;
			ctxHSV_LAB.Checked = _mode == Mode.HSV_LAB;
		}

		#endregion

		#region rdFader

		private void UpdaterdFaderedChanged(object sender, EventArgs e)
		{
			if (sender == rdHSV_H)
				_fader = Fader.HSV_H;
			else if (sender == rdHSV_S)
				_fader = Fader.HSV_S;
			else if (sender == rdHSV_V)
				_fader = Fader.HSV_V;
				//secondary faders
			else if (sender == rdSecond_1)
				_fader = Fader.Second_1;
			else if (sender == rdSecond_2)
				_fader = Fader.Second_2;
			else //(sender==rdSecond_3)
				_fader = Fader.Second_3;

			ChangeModule();
		}

		private void UpdaterdFader()
		{
			if (_fader == Fader.HSV_H)
				rdHSV_H.Checked = true;
			else if (_fader == Fader.HSV_S)
				rdHSV_S.Checked = true;
			else if (_fader == Fader.HSV_V)
				rdHSV_V.Checked = true;
			else if (_fader == Fader.Second_1)
				rdSecond_1.Checked = true;
			else if (_fader == Fader.Second_2)
				rdSecond_2.Checked = true;
			else if (_fader == Fader.Second_3)
				rdSecond_3.Checked = true;
		}

		#endregion

		#region tbValue

		private void tbValue_KeyUp(object sender, KeyEventArgs e)
		{
			if (!(sender is TextBox)) return;
			if (e.KeyCode == Keys.Return) {
				UpdatetbValue(null);
				e.Handled = true;
				return;
			}
			double value;
			if (!double.TryParse(((TextBox) sender).Text,
			                     System.Globalization.NumberStyles.Integer,
			                     null, out value)) return;

			#region hsv  textboxes

			if (sender == tbHSV_H) {
				HSV chsv = HSV.FromRGB(InternalColor.ToRGB());
				chsv.H = value/360.0;
				InternalColor = XYZ.FromRGB(chsv.ToRGB());
			}
			else if (sender == tbHSV_S) {
				HSV chsv = HSV.FromRGB(InternalColor.ToRGB());
				chsv.S = value/100.0;
				InternalColor = XYZ.FromRGB(chsv.ToRGB());
			}
			else if (sender == tbHSV_V) {
				HSV chsv = HSV.FromRGB(InternalColor.ToRGB());
				chsv.V = value/100.0;
				InternalColor = XYZ.FromRGB(chsv.ToRGB());
			}
				#endregion
				#region secondary textboxes

			else if (_mode == Mode.HSV_RGB) {
				RGB crgb = InternalColor.ToRGB();
				if (sender == tbSecond_1) {
					crgb.R = value/255.0;
				}
				else if (sender == tbSecond_2) {
					crgb.G = value/255.0;
				}
				else //sender==tbSecond_3
				{
					crgb.B = value/255.0;
				}
				InternalColor = XYZ.FromRGB(crgb);
			}
			else if (_mode == Mode.HSV_LAB) {
				LAB clab = LAB.FromXYZ(InternalColor);
				if (sender == tbSecond_1) {
					clab.L = value;
				}
				else if (sender == tbSecond_2) {
					clab.a = value;
				}
				else //sender==tbSecond_3
				{
					clab.b = value;
				}
				InternalColor = clab.ToXYZ();
			}

			#endregion

			//update ui
			_module.XYZ = InternalColor;
			lblColorOut.Color = InternalColor.ToRGB();
			UpdatetbValue((TextBox) sender);
		}

		private void tbValue_Leave(object sender, EventArgs e)
		{
			UpdatetbValue(null);
		}

		private void UpdatetbValue(TextBox skipupdate)
		{
			#region hsv textboxes

			HSV chsv = HSV.FromRGB(InternalColor.ToRGB());
			if (skipupdate != tbHSV_H)
				tbHSV_H.Text = (chsv.H*360.0).ToString("0");
			if (skipupdate != tbHSV_S)
				tbHSV_S.Text = (chsv.S*100.0).ToString("0");
			if (skipupdate != tbHSV_V)
				tbHSV_V.Text = (chsv.V*100.0).ToString("0");

			#endregion

			#region secondary textboxes

			if (_mode == Mode.HSV_RGB) {
				RGB crgb = InternalColor.ToRGB();
				if (skipupdate != tbSecond_1)
					tbSecond_1.Text = (crgb.R*255.0).ToString("0");
				if (skipupdate != tbSecond_2)
					tbSecond_2.Text = (crgb.G*255.0).ToString("0");
				if (skipupdate != tbSecond_3)
					tbSecond_3.Text = (crgb.B*255.0).ToString("0");
			}
			else //(_mode==Mode.HSV_LAB)
			{
				LAB clab = LAB.FromXYZ(InternalColor);
				if (skipupdate != tbSecond_1)
					tbSecond_1.Text = clab.L.ToString("0");
				if (skipupdate != tbSecond_2)
					tbSecond_2.Text = clab.a.ToString("0");
				if (skipupdate != tbSecond_3)
					tbSecond_3.Text = clab.b.ToString("0");
			}

			#endregion
		}

		#endregion

		#region module & lbl

		private void _module_ColorChanged(object sender, EventArgs e)
		{
			if (_module == null) return;
			InternalColor = _module.XYZ;
			lblColorOut.Color = InternalColor.ToRGB();
			UpdatetbValue(null);
		}

		private void lblColorOut_ColorChanged(object sender, EventArgs e)
		{
			InternalColor = XYZ.FromRGB(lblColorOut.Color);
			_module.XYZ = InternalColor;
			UpdatetbValue(null);
		}

		#endregion

		#endregion

		#region properties

		/// <summary>
		/// gets or sets the color as device-independent CIE-XYZ color
		/// </summary>
		[Description("gets or sets the color as device-independent CIE-XYZ color")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XYZ Color
		{
			get { return InternalColor; }
			set
			{
				if (value == InternalColor) return;
				InternalColor = value;
				_module.XYZ = InternalColor;
				lblColorOut.Color = lblColorOut.OldColor = value.ToRGB();
				UpdatetbValue(null);
			}
		}

		/// <summary>
		/// An internal call for setting the color, so it can be filtered if needed (locked to ranges, etc.)
		/// </summary>
		private XYZ InternalColor
		{
			get { return _color; }
			set
			{
				if (LockValue_V) {
					HSV temp = HSV.FromRGB(value.ToRGB());
					temp.V = 1.0;
					value = XYZ.FromRGB(temp.ToRGB());
				}
				_color = value;
			}
		}

		/// <summary>
		/// if true, will lock the value for V to 100%.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool LockValue_V
		{
			get { return lockValueV; }
			set
			{
				lockValueV = value;
				rdHSV_V.AutoCheck = !value;
				tbHSV_V.Enabled = !value;
			}
		}

		[Browsable(false)]
		public Fader PrimaryFader
		{
			get { return _fader; }
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
			get { return _mode; }
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

		private void redButton_Click(object sender, EventArgs e)
		{
			InternalColor = XYZ.FromRGB(new RGB(255, 0, 0));
			lblColorOut.Color = InternalColor.ToRGB();
			UpdatetbValue(null);
		}

		private void greenButton_Click(object sender, EventArgs e)
		{
			InternalColor = XYZ.FromRGB(new RGB(0, 255, 0));
			lblColorOut.Color = InternalColor.ToRGB();
			UpdatetbValue(null);
		}

		private void blueButton_Click(object sender, EventArgs e)
		{
			InternalColor = XYZ.FromRGB(new RGB(0, 0, 255));
			lblColorOut.Color = InternalColor.ToRGB();
			UpdatetbValue(null);
		}

		private void whiteButton_Click(object sender, EventArgs e)
		{
			InternalColor = XYZ.FromRGB(new RGB(255, 255, 255));
			lblColorOut.Color = InternalColor.ToRGB();
			UpdatetbValue(null);
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
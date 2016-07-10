using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.NameGeneration;
using Common.Controls.Theme;
using Common.Resources;
using Vixen.Rule;
using Vixen.Rule.Name;
using System.Drawing;
using Common.Controls.Scaling;

namespace Common.Controls
{
	public partial class NameGenerator : BaseForm
	{
		private List<string> OldNames { get; set; }

		private int _fixedCount;

		private string SelectedGroupName;

		private int FixedCount
		{
			get { return _fixedCount; }
			set
			{
				_fixedCount = value;
				if (_fixedCount > 0) {
					numericUpDownItemCount.Value = _fixedCount;
					numericUpDownItemCount.Enabled = false;
				}
				else {
					numericUpDownItemCount.Value = 1;
					numericUpDownItemCount.Enabled = true;
				}
			}
		}

		private List<INamingGenerator> Generators;

		public NameGenerator()
		{
			InitializeComponent();
			Icon = Resources.Properties.Resources.Icon_Vixen3;
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			buttonMoveRuleUp.Image = Tools.GetIcon(Resources.Properties.Resources.arrow_up, iconSize);
			buttonMoveRuleUp.Text = "";
			buttonMoveRuleDown.Image = Tools.GetIcon(Resources.Properties.Resources.arrow_down, iconSize);
			buttonMoveRuleDown.Text = "";
			buttonAddNewRule.Image = Tools.GetIcon(Resources.Properties.Resources.add, iconSize);
			buttonAddNewRule.Text = "";
			buttonDeleteRule.Image = Tools.GetIcon(Resources.Properties.Resources.delete, iconSize);
			buttonDeleteRule.Text = "";
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			Generators = new List<INamingGenerator>();

			listViewNames.Columns.Clear();
			listViewNames.Columns.Add(new ColumnHeader {Text = "Name"});
			labelColumnHeader1.Text = "Name";
			labelColumnHeader2.Text = "";
		}

		public NameGenerator(IEnumerable<string> oldNames)
			: this()
		{
			SelectedGroupName = "NewName";
			OldNames = new List<string>(oldNames);
			FixedCount = OldNames.Count();
			listViewNames.Columns.Clear();
			listViewNames.Columns.Add(new ColumnHeader {Text = "Old Name"});
			listViewNames.Columns.Add(new ColumnHeader { Text = "New Name" });
			labelColumnHeader1.Text = "Old Name";
			labelColumnHeader2.Text = "New Name";
		}

		public NameGenerator(int fixedCount)
			: this()
		{
			FixedCount = fixedCount;
		}

		public NameGenerator(string selectedGroupName)
			: this()
		{
			SelectedGroupName = selectedGroupName;
		}

		private void BulkRename_Load(object sender, EventArgs e)
		{
			INamingGenerator[] namingGenerators = Vixen.Services.ApplicationServices.GetAllNamingGenerators();
			comboBoxRuleTypes.DisplayMember = "Name";
			comboBoxRuleTypes.ValueMember = string.Empty;
			comboBoxRuleTypes.DataSource = namingGenerators;

			INamingTemplate[] namingTemplates = Vixen.Services.ApplicationServices.GetAllNamingTemplates();
			comboBoxTemplates.DisplayMember = "Name";
			comboBoxTemplates.ValueMember = string.Empty;
			comboBoxTemplates.DataSource = namingTemplates;

			ResizeListviewColumns();
			PopulateNames();
			listViewGenerators.Items[0].Selected = true;
		}

		public List<string> Names { get; set; }


		private void ResizeListviewColumns()
		{
			int width = (listViewNames.Width - SystemInformation.VerticalScrollBarWidth - 6)/listViewNames.Columns.Count;
			foreach (ColumnHeader column in listViewNames.Columns) {
				column.Width = width;
			}
		}

		private void SyncGeneratorsToListView()
		{
			listViewGenerators.BeginUpdate();
			listViewGenerators.Items.Clear();

			int i = 1;
			foreach (INamingGenerator namingGenerator in Generators) {
				ListViewItem item = new ListViewItem();
				item.Text = string.Format("<{0}> {1}", i, namingGenerator.Name);
				item.Tag = namingGenerator;
				listViewGenerators.Items.Add(item);
				i++;
			}

			listViewGenerators.EndUpdate();
		}

		private void AddNewNamingGenerator(INamingGenerator generator)
		{
			Generators.Add(generator);
			SyncGeneratorsToListView();
		}

		private void RemoveNamingGenerator(int index)
		{
			Generators.RemoveAt(index);
			SyncGeneratorsToListView();
		}

		private void DisplayNamingGenerator(INamingGenerator generator)
		{
			panelRuleConfig.Controls.Clear();

			NameGeneratorEditor newControl = null;
			if (generator is NumericCounter) {
				newControl = new NumericCounterEditor(generator as NumericCounter);
			}
			else if (generator is LetterCounter) {
				newControl = new LetterCounterEditor(generator as LetterCounter);
			}
			else if (generator is LetterIterator) {
				newControl = new LetterIteratorEditor(generator as LetterIterator);
			}
			else if (generator is WordIterator) {
				newControl = new WordIteratorEditor(generator as WordIterator);
			}

			if (newControl != null) {
				newControl.DataChanged += new EventHandler(NameGeneratorEditor_DataChanged);
				panelRuleConfig.Controls.Add(newControl);
			}

			buttonMoveRuleUp.Enabled = (newControl != null);
			buttonMoveRuleDown.Enabled = (newControl != null);
			buttonDeleteRule.Enabled = (newControl != null);
		}

		private void LoadNamingTemplate(INamingTemplate template)
		{
			textBoxNameFormat.Text =  SelectedGroupName + template.Format;

			Generators.Clear();
			foreach (INamingGenerator generator in template.Generators) {
				AddNewNamingGenerator(generator);
			}

			PopulateNames();
			DisplayNamingGenerator(null);
		}

		private void NameGeneratorEditor_DataChanged(object sender, EventArgs e)
		{
			PopulateNames();
		}

		private int GetRepCount()
		{
			if (OldNames != null && OldNames.Count > 0) {
				return OldNames.Count;
			}

			if (FixedCount > 0) {
				return FixedCount;
			}

			return decimal.ToInt32(numericUpDownItemCount.Value);
		}

		private void PopulateNames()
		{
			Names = new List<string>(GenerateNames());

			listViewNames.BeginUpdate();
			listViewNames.Items.Clear();

			if (OldNames != null) {
				for (int i = 0; i < OldNames.Count; i++) {
					ListViewItem item = new ListViewItem();
					item.Text = OldNames[i];
					item.SubItems.Add(Names.Count > i ? Names[i] : "-");
					listViewNames.Items.Add(item);
				}
			}
			else {
				foreach (var name in Names) {
					ListViewItem item = new ListViewItem();
					item.Text = name;
					listViewNames.Items.Add(item);
				}
			}

			listViewNames.EndUpdate();
		}


		private IEnumerable<string> GenerateNames()
		{
			return GenerateNames(1, textBoxNameFormat.Text, 0, GetRepCount());
		}

		// depth is 1-offset, as it will be the same as the one the user uses
		private IEnumerable<string> GenerateNames(int depth, string format, int currentNumber, int maxNumber)
		{
			List<string> result = new List<string>();

			if (Generators.Count < depth || currentNumber > maxNumber)
				return result;

			INamingGenerator generator = Generators[depth - 1];

			// if the generator is endless, this will be an empty list.
			List<string> names = new List<string>(generator.GenerateNames());

			for (int i = 0; i < generator.IterationsInCycle || generator.EndlessCycle; i++) {
				string substitution;
				if (generator.EndlessCycle) {
					substitution = generator.GenerateName(i);
				}
				else {
					substitution = names[i];
				}

				string newFormat = format.Replace("<" + depth + ">", substitution);

				// if this is the last generator, use the single string; otherwise, recurse so the next
				// generator can have a crack at it as well.
				if (depth >= Generators.Count) {
					result.Add(newFormat);
				}
				else {
					// if the sub-generator didn't make anything, add the name directly and treat this one as the final.
					IEnumerable<string> subResult = GenerateNames(depth + 1, newFormat, currentNumber + result.Count, maxNumber);
					if (subResult.Any())
						result.AddRange(subResult);
					else
						result.Add(newFormat);
				}

				if (currentNumber + result.Count >= maxNumber)
					break;
			}

			return result;
		}


		private void comboBoxRuleTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonAddNewRule.Enabled = comboBoxRuleTypes.SelectedIndex >= 0;
		}

		private void buttonAddNewRule_Click(object sender, EventArgs e)
		{
			if (comboBoxRuleTypes.SelectedIndex < 0) {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Select a rule type first.",
					"Warning", false, false);
				messageBox.ShowDialog();
				return;
			}

			INamingGenerator ng = (INamingGenerator) comboBoxRuleTypes.SelectedItem;
			INamingGenerator newGenerator = (INamingGenerator) Activator.CreateInstance(ng.GetType());

			AddNewNamingGenerator(newGenerator);
			listViewGenerators.Items[listViewGenerators.Items.Count - 1].Selected = true;
			int index = listViewGenerators.SelectedIndices[0];
			if (textBoxNameFormat.Text.Contains("<" + (index + 1) + ">"))
			{

			}
			else
			{
				textBoxNameFormat.Text = textBoxNameFormat.Text + string.Format(" <{0}>", index + 1);
			}
			PopulateNames();
		}

		private void buttonDeleteRule_Click(object sender, EventArgs e)
		{
			int index = listViewGenerators.SelectedIndices[0];
			if (listViewGenerators.SelectedIndices.Count <= 0)
				return;

			RemoveNamingGenerator(listViewGenerators.SelectedIndices[0]);

			DisplayNamingGenerator(null);
			if (index > 0)
			{
	//			this.listViewGenerators.Items[index - 1].BackColor = Color.DodgerBlue;
				this.listViewGenerators.Items[index - 1].Selected = true;
			}
			else
			{
				if (listViewGenerators.Items.Count != index)
				{
	//				this.listViewGenerators.Items[index].BackColor = Color.DodgerBlue;
					this.listViewGenerators.Items[index].Selected = true;
				}
			}
			if (index < listViewGenerators.Items.Count)
			{
				textBoxNameFormat.Text = textBoxNameFormat.Text.Replace(" <" + (listViewGenerators.Items.Count + 1) + ">", "");
				textBoxNameFormat.Text = textBoxNameFormat.Text.Replace("<" + (listViewGenerators.Items.Count + 1) + ">", "");
			}
			else
			{
				textBoxNameFormat.Text = textBoxNameFormat.Text.Replace(" <" + (index + 1) + ">", "");
				textBoxNameFormat.Text = textBoxNameFormat.Text.Replace("<" + (index + 1) + ">", "");
			}
			PopulateNames();
		}

		private void buttonMoveRuleUp_Click(object sender, EventArgs e)
		{
			int index = listViewGenerators.SelectedIndices[0];
			if (listViewGenerators.SelectedIndices.Count <= 0)
				return;
			if (index <= 0)
				return;

			INamingGenerator ng = Generators[index - 1];
			Generators[index - 1] = Generators[index];
			Generators[index] = ng;
			SyncGeneratorsToListView();
			PopulateNames();
			this.listViewGenerators.Items[index - 1].Selected = true;
		}

		private void buttonMoveRuleDown_Click(object sender, EventArgs e)
		{
			int index = listViewGenerators.SelectedIndices[0];
			if (listViewGenerators.SelectedIndices.Count <= 0)
				return;

			if (index >= Generators.Count - 1)
				return;

			INamingGenerator ng = Generators[index + 1];
			Generators[index + 1] = Generators[index];
			Generators[index] = ng;
			SyncGeneratorsToListView();
			PopulateNames();
			this.listViewGenerators.Items[index + 1].Selected = true;
		}

		private void listViewGenerators_Highlight(object sender, DrawListViewItemEventArgs e)
		{
			// If this item is the selected item
			if (e.Item.Selected)
			{
				// If the selected item has focus Set the colors to the normal colors for a selected item
				e.Item.ForeColor = SystemColors.HighlightText;
				e.Item.BackColor = SystemColors.Highlight;
			}
			else
			{
				// Set the normal colors for items that are not selected
				e.Item.ForeColor = listViewGenerators.ForeColor;
				e.Item.BackColor = listViewGenerators.BackColor;
			}
			e.DrawBackground();
			e.DrawText();
		}

		private void listViewNames_Resize(object sender, EventArgs e)
		{
			ResizeListviewColumns();
		}

		private void listViewGenerators_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewGenerators.SelectedIndices.Count <= 0) {
				DisplayNamingGenerator(null);
				return;
			}

			DisplayNamingGenerator((INamingGenerator) listViewGenerators.SelectedItems[0].Tag);
		}

		private void textBoxNameFormat_TextChanged(object sender, EventArgs e)
		{
			PopulateNames();
		}

		private void comboBoxTemplates_SelectedIndexChanged(object sender, EventArgs e)
		{
			INamingTemplate template = (INamingTemplate) comboBoxTemplates.SelectedItem;
			LoadNamingTemplate(template);
		}

		private void numericUpDownItemCount_ValueChanged(object sender, EventArgs e)
		{
			PopulateNames();
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;

		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
	}
}
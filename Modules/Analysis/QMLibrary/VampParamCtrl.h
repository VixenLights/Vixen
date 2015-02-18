#pragma once

#include <WrapperFiles\ManagedParameter.h>
#include <VampTrackBar.h>

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace QMLibrary {

	/// <summary>
	/// Summary for VampParamCtrl
	/// </summary>
	public ref class VampParamCtrl : public System::Windows::Forms::UserControl
	{
	private:
		Dictionary<VampTrackBar^, TextBox^>^ m_valueCtrls;
		Dictionary<TextBox^, VampTrackBar^>^ m_trackBarCtrls;
		Dictionary<ManagedParameterDescriptor^, TextBox^>^ m_paramToTextBoxMappings;

		TableLayoutPanel^ m_layoutPanel;
		int m_rows = 0;

		void trackBar_ValueChanged(Object^ sender, EventArgs^ e)
		{
			VampTrackBar^ trackBar = (VampTrackBar^)sender;
			TextBox^ partnerBox = nullptr;
			if (m_valueCtrls->TryGetValue(trackBar, partnerBox))
			{
				partnerBox->Text = trackBar->Value.ToString();
			}
		}

		void paramTextBox_Validating(Object^ sender, System::ComponentModel::CancelEventArgs^ e)
		{
			TextBox^ paramTextBox = (TextBox^)sender;
			e->Cancel = true;

			try
			{
				double paramValue = Convert::ToDouble(paramTextBox->Text);
				VampTrackBar^ trackBar = m_trackBarCtrls[paramTextBox];
				if ((trackBar->Minimum <= paramValue) && (trackBar->Maximum >= paramValue))
				{
					e->Cancel = false;
				}
				else
				{
					ManagedParameterDescriptor^ theLabel;

					bool found = false;
					for each(KeyValuePair<ManagedParameterDescriptor^, TextBox^> kvp in m_paramToTextBoxMappings)
					{
						if (kvp.Value == paramTextBox)
						{
							theLabel = kvp.Key;
							found = true;
							break;
						}
					}

					if (!found)
					{
						return;
					}

					DialogResult result =
						MessageBox::Show("Please enter a value between " +
						trackBar->Minimum +
						" and " +
						trackBar->Maximum +
						" for the " +
						theLabel->name +
						" entry", "Error", MessageBoxButtons::OKCancel);
					if (result == DialogResult::Cancel)
					{
						e->Cancel = false;
						paramTextBox->Text = trackBar->Value.ToString();
					}
				}
			}
			catch (Exception^ e) {}
		}

		void paramTextBox_KeyPress(Object^ sender, KeyPressEventArgs^ e)
		{
			if (!Char::IsControl(e->KeyChar) && !Char::IsDigit(e->KeyChar) &&
				(e->KeyChar != '.'))
			{
				e->Handled = true;
			}

			// only allow one decimal point
			if ((e->KeyChar == '.') && (((TextBox^)sender)->Text->IndexOf('.') > -1))
			{
				e->Handled = true;
			}
		}

	public:
		VampParamCtrl(void)
		{
			InitializeComponent();

			m_valueCtrls = gcnew Dictionary<VampTrackBar^, TextBox^>();
			m_trackBarCtrls = gcnew Dictionary<TextBox^, VampTrackBar^>();
			m_paramToTextBoxMappings = gcnew Dictionary<ManagedParameterDescriptor^, TextBox^>();

			m_layoutPanel = gcnew TableLayoutPanel();
			m_layoutPanel->ColumnCount = 3;
			m_layoutPanel->ColumnStyles->Add(gcnew ColumnStyle(SizeType::AutoSize));
			m_layoutPanel->ColumnStyles->Add(gcnew ColumnStyle(SizeType::AutoSize));
			m_layoutPanel->ColumnStyles->Add(gcnew ColumnStyle(SizeType::AutoSize));
			m_layoutPanel->RowStyles->Add(gcnew RowStyle(SizeType::AutoSize));
			m_layoutPanel->Anchor = (AnchorStyles::Left | AnchorStyles::Right);
			m_layoutPanel->AutoSize = true;

			Controls->Add(m_layoutPanel);
			m_rows = 0;

		}

		void InitParamControls(ManagedParameterList^ parameterDescriptors)
		{

			Drawing::Size^ offsetSize = gcnew Drawing::Size(5, 0);

			System::Collections::Generic::IEnumerator<ManagedParameterDescriptor^>^ paramEnumerator =
				parameterDescriptors->GetEnumerator();

			while (paramEnumerator->MoveNext())
			{
				ManagedParameterDescriptor^ descriptor = paramEnumerator->Current;

				m_layoutPanel->RowCount = ++m_rows;
				m_layoutPanel->Size = this->Size;

				Label^ paramLabel = gcnew Label();
				paramLabel->Text = descriptor->name + ":";
				m_layoutPanel->Controls->Add(paramLabel, 0, m_rows);

				if (descriptor->valueNames->Count != 0)
				{

				}
				else
				{
					TextBox^ paramTextBox = gcnew TextBox();
					paramTextBox->KeyPress += gcnew System::Windows::Forms::KeyPressEventHandler(this, &VampParamCtrl::paramTextBox_KeyPress);
					paramTextBox->Text = descriptor->defaultValue.ToString();
					paramTextBox->TextAlign = HorizontalAlignment::Right;
					paramTextBox->MaxLength = (int)Math::Log10(descriptor->maxValue) + 1;
					paramTextBox->Size = paramTextBox->GetPreferredSize(Drawing::Size(0, 0));
					paramTextBox->Size = paramTextBox->Size + *offsetSize;
					paramTextBox->CausesValidation = true;
					paramTextBox->Validating += gcnew System::ComponentModel::CancelEventHandler(this, &VampParamCtrl::paramTextBox_Validating);

					m_layoutPanel->Controls->Add(paramTextBox, 1, m_rows);

					if (descriptor->isQuantized)
					{
						VampTrackBar^ trackBar = gcnew VampTrackBar();
						trackBar->TickStyle = TickStyle::BottomRight;
						trackBar->Minimum = descriptor->minValue;
						trackBar->Maximum = descriptor->maxValue;
						trackBar->TickFrequency = descriptor->quantizeStep;
						trackBar->Value = descriptor->defaultValue;
						trackBar->ValueChanged += gcnew System::EventHandler(this, &VampParamCtrl::trackBar_ValueChanged);
						m_layoutPanel->Controls->Add(trackBar, 2, m_rows);

						m_valueCtrls[trackBar] = paramTextBox;
						m_trackBarCtrls[paramTextBox] = trackBar;
						m_paramToTextBoxMappings[descriptor] = paramTextBox;
					}
				}
			}
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~VampParamCtrl()
		{
			if (components)
			{
				delete components;
			}
		}

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->SuspendLayout();
			// 
			// VampParamCtrl
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->Name = L"VampParamCtrl";
			this->Size = System::Drawing::Size(337, 69);
			this->ResumeLayout(false);

		}
#pragma endregion
	};
}


/*

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using VixenModules.Analysis.Common;

namespace VixenModules.Analysis.Common
{
public partial class VampParams : UserControl
{
private Dictionary<PrecisionTrackBar, TextBox> m_valueCtrls;
private Dictionary<TextBox, PrecisionTrackBar> m_trackBarCtrls;
private Dictionary<ManagedParameterDescriptor, TextBox> m_paramToTextBoxMappings;

private TableLayoutPanel m_layoutPanel;
private int m_rows = 0;

public VampParams()
{
InitializeComponent();
m_valueCtrls = new Dictionary<PrecisionTrackBar, TextBox>();
m_trackBarCtrls = new Dictionary<TextBox, PrecisionTrackBar>();
m_paramToTextBoxMappings = new Dictionary<ManagedParameterDescriptor, TextBox>();

m_layoutPanel = new TableLayoutPanel();
m_layoutPanel.ColumnCount = 3;
m_layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
m_layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
m_layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
m_layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
m_layoutPanel.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
m_layoutPanel.AutoSize = true;

Controls.Add(m_layoutPanel);
m_rows = 0;
}

protected void trackBar_ValueChanged(object sender, EventArgs e)
{
PrecisionTrackBar trackBar = (PrecisionTrackBar) sender;
TextBox partnerBox = null;
if (m_valueCtrls.TryGetValue(trackBar, out partnerBox))
{
partnerBox.Text = trackBar.Value.ToString();
}
}

private void paramTextBox_KeyPress(object sender, KeyPressEventArgs e)
{
if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
(e.KeyChar != '.'))
{
e.Handled = true;
}

// only allow one decimal point
if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
{
e.Handled = true;
}
}

private void paramTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
{
TextBox paramTextBox = (TextBox) sender;
e.Cancel = true;
try
{
double paramValue = Convert.ToDouble(paramTextBox.Text);
PrecisionTrackBar trackBar = m_trackBarCtrls[paramTextBox];
if ((trackBar.Minimum <= paramValue) && (trackBar.Maximum >= paramValue))
{
e.Cancel = false;
}
else
{
var theLabel = m_paramToTextBoxMappings.FirstOrDefault(x => x.Value == paramTextBox).Key;

DialogResult result =
MessageBox.Show("Please enter a value between " +
trackBar.Minimum +
" and " +
trackBar.Maximum +
" for the " +
theLabel.name +
" entry","Error", MessageBoxButtons.OKCancel);
if (result == DialogResult.Cancel)
{
e.Cancel = false;
paramTextBox.Text = trackBar.Value.ToString();
}
}
}
catch (Exception) {}
}

private void InitParamControls(ICollection<ManagedParameterDescriptor> parameterDescriptors)
{

	Size offsetSize = new Size(5, 0);

	foreach(ManagedParameterDescriptor descriptor in parameterDescriptors)
	{
		m_layoutPanel.RowCount = ++m_rows;

		Label paramLabel = new Label();
		paramLabel.Text = descriptor.name + ":";
		m_layoutPanel.Controls.Add(paramLabel, 0, m_rows);

		if (descriptor.valueNames.Count != 0)
		{

		}
		else
		{
			TextBox paramTextBox = new TextBox();
			paramTextBox.KeyPress += paramTextBox_KeyPress;
			paramTextBox.Text = descriptor.defaultValue.ToString();
			paramTextBox.TextAlign = HorizontalAlignment.Right;
			paramTextBox.MaxLength = (int)Math.Log10(descriptor.maxValue) + 1;
			paramTextBox.Size = paramTextBox.GetPreferredSize(new Size(0, 0)) + offsetSize;
			paramTextBox.CausesValidation = true;
			paramTextBox.Validating += paramTextBox_Validating;

			m_layoutPanel.Controls.Add(paramTextBox, 1, m_rows);

			if (descriptor.isQuantized)
			{
				PrecisionTrackBar trackBar = new PrecisionTrackBar();
				trackBar.TickStyle = TickStyle.BottomRight;
				trackBar.Minimum = descriptor.minValue;
				trackBar.Maximum = descriptor.maxValue;
				trackBar.TickFrequency = descriptor.quantizeStep;
				trackBar.Value = descriptor.defaultValue;
				trackBar.ValueChanged += trackBar_ValueChanged;
				m_layoutPanel.Controls.Add(trackBar, 2, m_rows);

				m_valueCtrls[trackBar] = paramTextBox;
				m_trackBarCtrls[paramTextBox] = trackBar;
				m_paramToTextBoxMappings[descriptor] = paramTextBox;
			}
		}
	}
}

public void InitParameterSettingControls(ICollection<ManagedParameterDescriptor> parameterDescriptors)
{
	int startX = 0;

	InitParamControls(parameterDescriptors);
}
	}
}


*/
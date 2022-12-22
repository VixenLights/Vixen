#pragma once

#include <WrapperFiles\ManagedParameter.h>

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
		Dictionary<TrackBar^, TextBox^>^ m_valueCtrls;
		Dictionary<TextBox^, TrackBar^>^ m_trackBarCtrls;
		Dictionary<ManagedParameterDescriptor^, TextBox^>^ m_paramToTextBoxMappings;

		ToolTip^ m_toolTip = gcnew ToolTip();

		TableLayoutPanel^ m_layoutPanel;
		int m_rows = 0;

		void trackBar_ValueChanged(Object^ sender, EventArgs^ e)
		{
			TrackBar^ trackBar = (TrackBar^)sender;
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
				TrackBar^ trackBar = m_trackBarCtrls[paramTextBox];
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

			m_valueCtrls = gcnew Dictionary<TrackBar^, TextBox^>();
			m_trackBarCtrls = gcnew Dictionary<TextBox^, TrackBar^>();
			m_paramToTextBoxMappings = gcnew Dictionary<ManagedParameterDescriptor^, TextBox^>();

			m_layoutPanel = gcnew TableLayoutPanel();
			m_layoutPanel->Size = Drawing::Size(1, 1);
			m_layoutPanel->ColumnCount = 3;
			m_layoutPanel->ColumnStyles->Add(gcnew ColumnStyle(SizeType::AutoSize));
			m_layoutPanel->ColumnStyles->Add(gcnew ColumnStyle(SizeType::AutoSize));
			m_layoutPanel->ColumnStyles->Add(gcnew ColumnStyle(SizeType::AutoSize));
			m_layoutPanel->RowStyles->Add(gcnew RowStyle(SizeType::AutoSize));
			m_layoutPanel->Anchor = (AnchorStyles::Left | AnchorStyles::Right | AnchorStyles::Top);
			m_layoutPanel->AutoSize = true;

			m_layoutPanel->Location = Point(0, 0);

			Controls->Add(m_layoutPanel);
			m_rows = 0;

			m_toolTip->AutoPopDelay = 5000;
			m_toolTip->InitialDelay = 1000;
			m_toolTip->ReshowDelay = 500;
			m_toolTip->ShowAlways = true;

		}

		String^ FindParamByIdentifier(String^ identifier)
		{
			String^ retVal = nullptr;
			for (int j = 0; j < m_rows; j++)
			{
				Control^ control = m_layoutPanel->GetControlFromPosition(1, j);
				if (control != nullptr)
				{
					if (((String^)(control->Tag))->CompareTo(identifier) == 0)
					{
						retVal = (String^)(control->Text);
						break;
					}
				}
			}
			return retVal;
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
				m_toolTip->SetToolTip(paramLabel, descriptor->description);

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
					paramTextBox->Tag = gcnew String(descriptor->identifier);
					paramTextBox->Validating += gcnew System::ComponentModel::CancelEventHandler(this, &VampParamCtrl::paramTextBox_Validating);
					m_toolTip->SetToolTip(paramTextBox, descriptor->description);

					m_layoutPanel->Controls->Add(paramTextBox, 1, m_rows - 1);

					if (descriptor->isQuantized)
					{
						TrackBar^ trackBar = gcnew TrackBar();
						trackBar->TickStyle = TickStyle::BottomRight;
						trackBar->Minimum = descriptor->minValue;
						trackBar->Maximum = descriptor->maxValue;
						trackBar->TickFrequency = descriptor->quantizeStep;
						trackBar->Value = descriptor->defaultValue;
						trackBar->ValueChanged += gcnew System::EventHandler(this, &VampParamCtrl::trackBar_ValueChanged);
						m_toolTip->SetToolTip(trackBar, descriptor->description);
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
			this->Size = System::Drawing::Size(30, 18);
			this->ResumeLayout(false);

		}
#pragma endregion
	};
}
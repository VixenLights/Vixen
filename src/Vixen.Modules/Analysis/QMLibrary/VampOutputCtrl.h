#pragma once

#include <WrapperFiles\ManagedOutput.h>

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace QMLibrary {

	/// <summary>
	/// Summary for VampOutputCtrl
	/// </summary>
	public ref class VampOutputCtrl : public System::Windows::Forms::UserControl
	{
	private:
		ToolTip^ m_toolTip = gcnew ToolTip();

		TableLayoutPanel^ m_layoutPanel;
		int m_rows = 0;


	public:
		VampOutputCtrl(void)
		{
			InitializeComponent();
			m_layoutPanel = gcnew TableLayoutPanel();
			m_layoutPanel->Size = Drawing::Size(1, 1);
			m_layoutPanel->ColumnCount = 2;
			m_layoutPanel->ColumnStyles->Clear();
			m_layoutPanel->ColumnStyles->Add(gcnew ColumnStyle(SizeType::AutoSize));
			m_layoutPanel->ColumnStyles->Add(gcnew ColumnStyle(SizeType::AutoSize));

			m_layoutPanel->RowStyles->Clear();
			m_layoutPanel->RowStyles->Add(gcnew RowStyle(SizeType::AutoSize));
			m_layoutPanel->Anchor = (AnchorStyles::Left | AnchorStyles::Right | AnchorStyles::Top);
			m_layoutPanel->AutoSize = true;
			
			Controls->Add(m_layoutPanel);
			m_rows = 0;

			m_toolTip->AutoPopDelay = 5000;
			m_toolTip->InitialDelay = 1000;
			m_toolTip->ReshowDelay = 500;
			m_toolTip->ShowAlways = true;
		}

		void InitOutputControls(ManagedOutputList^ outputDescriptors)
		{
			Drawing::Size^ largestSize = gcnew Drawing::Size(0, 0);
			Drawing::Size^ offsetSize = gcnew Drawing::Size(5, 0);

			System::Collections::Generic::IEnumerator<ManagedOutputDescriptor^>^ outputEnumerator =
				outputDescriptors->GetEnumerator();

			while (outputEnumerator->MoveNext())
			{
				ManagedOutputDescriptor^ descriptor = outputEnumerator->Current;

				m_layoutPanel->RowCount = ++m_rows;
				CheckBox^ outputBox = gcnew CheckBox();
				outputBox->Checked = false;
				outputBox->Text = descriptor->name;
				m_toolTip->SetToolTip(outputBox, descriptor->description);
				m_layoutPanel->Controls->Add(outputBox, 0, m_rows);

				TextBox^ textBox = gcnew TextBox();
				textBox->Enabled = false;
				textBox->Text = descriptor->name + " Marks";
				textBox->Size = textBox->GetPreferredSize(Drawing::Size(0, 0));
				if (textBox->Size.Width > largestSize->Width) 
				{
					largestSize = textBox->Size;
				}
				m_toolTip->SetToolTip(textBox, descriptor->description);
				m_layoutPanel->Controls->Add(textBox, 1, m_rows);


			}
			this->Size = m_layoutPanel->Size;

			for (int j = 1; j <= m_rows; j++)
			{
				Control^ tmpControl = m_layoutPanel->GetControlFromPosition(1, j);
				tmpControl->Size = *largestSize;

			}
				
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~VampOutputCtrl()
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
			// VampOutputCtrl
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->Name = L"VampOutputCtrl";
			this->Size = System::Drawing::Size(44, 15);
			this->ResumeLayout(false);

		}
#pragma endregion
	};
}

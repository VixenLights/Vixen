using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QMLibrary;

namespace VixenModules.Analysis.BeatsAndBars
{
	public partial class BeatsAndBarsSettings : Form
	{

		public BeatsAndBarsSettings()
		{
			InitializeComponent();


		}

		public void Parameters(ICollection<ManagedParameterDescriptor> parameterDescriptors,
								ICollection<ManagedOutputDescriptor> outputDescriptors)
		{
			Size offsetSize = new Size(20,20);
			this.m_vampParamCtrl.InitParamControls(parameterDescriptors);
			this.m_vampOutputCtrl.InitOutputControls(outputDescriptors);


			this.m_paramsGroupBox.Size = m_vampParamCtrl.Size + offsetSize;
			this.m_outputGroupBox.Size = m_vampOutputCtrl.Size + offsetSize;

			if (m_paramsGroupBox.Size.Width > m_outputGroupBox.Size.Width)
			{
				m_outputGroupBox.Size = new Size(m_paramsGroupBox.Size.Width, m_outputGroupBox.Size.Height);
			}
			else
			{
				m_paramsGroupBox.Size = new Size(m_outputGroupBox.Size.Width, m_paramsGroupBox.Size.Height);
			}
			m_outputGroupBox.Location = 
				new Point(m_paramsGroupBox.Location.X, 
					m_paramsGroupBox.Location.X + m_paramsGroupBox.Height + offsetSize.Height);

			button1.Location = new Point(button1.Location.X, m_outputGroupBox.Location.Y + m_outputGroupBox.Height + offsetSize.Height);
		}

		public void Parameters(ICollection<ManagedParameterDescriptor> parameterDescriptors)
		{
			Size offsetSize = new Size(20, 20);
			this.m_vampParamCtrl.InitParamControls(parameterDescriptors);

			this.m_paramsGroupBox.Size = m_vampParamCtrl.Size + offsetSize;
			this.m_outputGroupBox.Size = m_vampOutputCtrl.Size + offsetSize;

			if (m_paramsGroupBox.Size.Width > m_outputGroupBox.Size.Width)
			{
				m_outputGroupBox.Size = new Size(m_paramsGroupBox.Size.Width, m_outputGroupBox.Size.Height);
			}
			else
			{
				m_paramsGroupBox.Size = new Size(m_outputGroupBox.Size.Width, m_paramsGroupBox.Size.Height);
			}
			m_outputGroupBox.Location =
				new Point(m_paramsGroupBox.Location.X,
					m_paramsGroupBox.Location.X + m_paramsGroupBox.Height + offsetSize.Height);

			button1.Location = new Point(button1.Location.X, m_outputGroupBox.Location.Y + m_outputGroupBox.Height + offsetSize.Height);
		}

		public void Outputs(ICollection<ManagedOutputDescriptor> outputDescriptors)
		{
			Size offsetSize = new Size(20, 20);
			this.m_vampOutputCtrl.InitOutputControls(outputDescriptors);


			this.m_paramsGroupBox.Size = m_vampParamCtrl.Size + offsetSize;
			this.m_outputGroupBox.Size = m_vampOutputCtrl.Size + offsetSize;

			if (m_paramsGroupBox.Size.Width > m_outputGroupBox.Size.Width)
			{
				m_outputGroupBox.Size = new Size(m_paramsGroupBox.Size.Width, m_outputGroupBox.Size.Height);
			}
			else
			{
				m_paramsGroupBox.Size = new Size(m_outputGroupBox.Size.Width, m_paramsGroupBox.Size.Height);
			}
			m_outputGroupBox.Location =
				new Point(m_paramsGroupBox.Location.X,
					m_paramsGroupBox.Location.X + m_paramsGroupBox.Height + offsetSize.Height);

			button1.Location = new Point(button1.Location.X, m_outputGroupBox.Location.Y + m_outputGroupBox.Height + offsetSize.Height);
		}


	}
}

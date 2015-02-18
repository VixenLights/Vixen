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

		public void Parameters(ICollection<ManagedParameterDescriptor> parameterDescriptors)
		{
			this.vampParamCtrl1.InitParamControls(parameterDescriptors);
		}
	}
}

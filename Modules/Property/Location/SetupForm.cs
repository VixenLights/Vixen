using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Property.Location {
	public partial class SetupForm : Form {
		public SetupForm(LocationData data) {
			InitializeComponent();
			X = data.X;
			Y = data.Y;
			Z = data.Z;
		}


		public int X {
			get { return (int)numericUpDownXPosition.Value; }
			set {
				numericUpDownXPosition.Value = value;

			}
		}
		public int Y {
			get { return (int)numericUpDownYPosition.Value; }
			set {
				numericUpDownYPosition.Value = value;
			}
		}
		public int Z {
			get { return (int)numericUpDownZPosition.Value; }
			set {
				numericUpDownZPosition.Value = value;
			}
		}

	}
}

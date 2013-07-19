using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Concurrent;
using Vixen.Sys;
using System.Threading;

namespace Common.Controls.Direct2D {
	public partial class Direct2DControlWinForm : UserControl {
		Guid DisplayID;
		Image backgroundImage = null;

		public new Image BackgroundImage {
			get {
				return backgroundImage;

			}
			set {
				backgroundImage = value;
			
			}
		}


		public Direct2DControlWinForm(Guid displayID) {
			
			DisplayID = displayID;

			InitializeComponent();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Controls.WpfPropertyGrid
{
	public class PreviewStateEventArgs: EventArgs
	{
		private readonly bool _state; 

        /// <devdoc>
		/// <para>Initializes a new instance of the <see cref='System.Windows.Controls.WpfPropertyGrid.PreviewStateEventArgs'/>
        /// class.</para> 
        /// </devdoc>
		public PreviewStateEventArgs(bool state)
		{ 
            _state = state; 
        }
 
        /// <devdoc>
        ///    <para>Indicates the state of the preview.</para>
        /// </devdoc>
        public virtual bool State { 
            get {
                return _state; 
            } 
        }
	}
}

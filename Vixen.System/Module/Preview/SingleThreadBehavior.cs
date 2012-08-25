using System;
using System.Windows.Forms;

namespace Vixen.Module.Preview {
	public class SingleThreadBehavior : IThreadBehavior {
		private Func<Form> _formInit;
		private Form _form;

		public SingleThreadBehavior(Func<Form> formInit) {
			if(formInit == null) throw new InvalidOperationException();
			_formInit = formInit;
		}

		public void Start() {
			_form = _formInit();
			_form.Show();
		}

		public void Stop() {
			_form.Close();
			_form.Dispose();
			_form = null;
		}

		public bool IsRunning {
			get { return _form != null && !_form.IsDisposed && _form.Visible; }
		}

		public void BeginInvoke(Action methodToInvoke) {
			_form.BeginInvoke(methodToInvoke);
		}
	}
}

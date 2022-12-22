namespace VixenModules.App.InstrumentationPanel
{
	using System;
	using Vixen.Module.App;
	using Vixen.Sys;

	public class InstrumentationModule : AppModuleInstanceBase
	{
		private const string ID_MENU = "Instrumentation_Main";
		private IApplication _application;
		private InstrumentationForm _form;

		public override IApplication Application
		{
			set { _application = value; }
		}

		public override void Loading()
		{
			InitializeForm();
			_AddMenu();
		}

		public override void Unloading()
		{
			if (_form != null) {
				_form.Dispose();
				_form = null;
			}

			_RemoveMenu();
		}

		private void InitializeForm()
		{
			_form = new InstrumentationForm();
			_form.Closed += _form_Closed;
		}

		private void OnMainMenuOnClick(object sender, EventArgs e)
		{
			if (_form == null) {
				InitializeForm();
			}

			_form.Show();
		}

		private void _AddMenu()
		{
			if (_application != null
			    && _application.AppCommands != null) {
				AppCommand toolsMenu = _application.AppCommands.Find("Tools");
				if (toolsMenu == null)
				{
					toolsMenu = new AppCommand("Tools", "Tools");
					_application.AppCommands.Add(toolsMenu);
				}
				var myMenu = new AppCommand(ID_MENU, "Instrumentation...");
				myMenu.Click += OnMainMenuOnClick;
				toolsMenu.Add(myMenu);
			}
		}

		private void _RemoveMenu()
		{
			if (_application != null
			    && _application.AppCommands != null) {
				_application.AppCommands.Remove(ID_MENU);
			}
		}

		private void _form_Closed(object sender, EventArgs e)
		{
			_form.Dispose();
			_form = null;
		}
	}
}
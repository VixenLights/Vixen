﻿using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.LayerMixingFilter.MaskFill
{
	public partial class MaskAndFillSetup : BaseForm
	{
		public MaskAndFillSetup(bool excludeZeroValues, bool requireMixingPartner)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			ExcludeZeroValuesValues = chkExcludeZero.Checked= excludeZeroValues;
			RequireMixingPartner = chkRequireMixingPartner.Checked = requireMixingPartner;
			
		}

		public bool ExcludeZeroValuesValues { get; private set; }

		public bool RequireMixingPartner { get; private set; }

		private void chkExcludeZero_CheckedChanged(object sender, EventArgs e)
		{
			ExcludeZeroValuesValues = chkExcludeZero.Checked;
		}
		private void chkRequireMixingPartner_CheckedChanged(object sender, EventArgs e)
		{
			RequireMixingPartner = chkRequireMixingPartner.Checked;
		}
	}
}

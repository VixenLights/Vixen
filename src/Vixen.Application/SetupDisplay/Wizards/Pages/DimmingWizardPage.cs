using Catel.Data;
using Catel.MVVM;
using Common.WPFCommon.Command;
using NAudio.Wave;
using Orc.Wizard;
using System.Windows.Input;
using Vixen.Extensions;
using Vixen.Sys.Props;
using Xceed.Wpf.Toolkit;
using VixenModules.App.Curves;
using VixenApplication.SetupDisplay.Wizards.ViewModels;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	public class DimmingWizardPage : WizardPageBase
	{

		public DimmingWizardPage()
		{
			Title = "Dimming Curve";
			Description = $"Enter dimming information";
		}

		#region Curve property
		/// <summary>
		/// Gets or sets the size of each light.
		/// </summary>
		public Curve Curve
		{
			get { return GetValue<Curve>(CurveProperty); }
			set { SetValue(CurveProperty, value); }
		}
		private static readonly IPropertyData CurveProperty = RegisterProperty<Curve>(nameof(Curve));
		#endregion

		private PropType _propType;
		public PropType PropType
		{
			set { 
				_propType = value;
				Description = $"Enter dimming information for {_propType.GetEnumDescription()}";
			}
		}

		public override ISummaryItem GetSummary()
		{
			string curveName = "None Specified";
			if (Curve != null)
			{
				if (Curve.CustomReferenceName != string.Empty)
				{
					curveName = Curve.CustomReferenceName;
				}
				else if (Curve.LibraryReferenceName != string.Empty)
				{
					curveName = Curve.LibraryReferenceName;
				}
				else if (Curve.Points.Count > 0)
				{
					curveName = "Custom";
				}
			}

			return new SummaryItem
			{
				Title = this.Title,
				Summary = curveName
			};
		}

		public IProp GetProp()
		{
			return null;
		}
	}
}

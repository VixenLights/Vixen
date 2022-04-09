using VixenModules.OutputFilter.ShutterFilter.Output;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.ShutterFilter
{
	/// <summary>
	/// Maintains a shutter filter module.
	/// </summary>
	public class ShutterFilterModule : TaggedFilterModuleBase<ShutterFilterData, ShutterFilterOutput, ShutterFilterDescriptor>
	{
		#region Public Methods

		/// <summary>
		/// Configures the filter.
		/// </summary>
		/// <returns>True if the filter was configured</returns>
		public override bool Setup()
		{
			bool okSelected = false;

			/*
			using (DimmingFilterSetup setup = new DimmingFilterSetup(Data))
			{
				if (setup.ShowDialog() == DialogResult.OK)
				{
					CreateOutput();
					okSelected = true;
				}
			}
			*/

			return okSelected;
		}

		#endregion

		#region Public Properties

		public override bool HasSetup => false;
		
		/// <summary>
		/// Open Shutter index command value.
		/// </summary>
		public byte OpenShutterIndexValue
		{
			get { return Data.OpenShutterIndexValue; }
			set { Data.OpenShutterIndexValue = value; }
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override ShutterFilterOutput CreateOutputInternal()
		{
			// Create the shutter filter output
			return new ShutterFilterOutput(Data.Tag, Data.OpenShutterIndexValue);
		}

		#endregion
	}
}
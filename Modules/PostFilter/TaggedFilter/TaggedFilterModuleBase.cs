using System.Windows.Forms;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.TaggedFilter
{
    /// <summary>
    /// Abstract base class for a tagged filter module.
    /// </summary>
    /// <typeparam name="TFilterData">Type of the filter data</typeparam>
    /// <typeparam name="TOutput">Type of the filter output</typeparam>
    /// <typeparam name="TDescriptor">Type of the filter descriptor</typeparam>
    public abstract class TaggedFilterModuleBase<TFilterData, TOutput, TDescriptor> : OutputFilterModuleInstanceBase
		where TFilterData : TaggedFilterDataBase, IModuleDataModel, ITaggedFilterData
		where TOutput : class, ITaggedFilterOutput, IDataFlowOutput<IntentsDataFlowData>, IDataFlowOutput
		where TDescriptor : IModuleDescriptor
	{
		#region Fields

		/// <summary>
		/// Output associated with the filter.
		/// </summary>
		private TOutput _output;

		/// <summary>
		/// Outputs associated with the filter in array format.
		/// </summary>
		private TOutput[] _outputsArray;

		#endregion

		#region Protected Properties

		/// <summary>
		/// Data associated with the filter.
		/// </summary>
		protected TFilterData Data { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Handles, filters, and converts intents.
		/// </summary>
		/// <param name="obj">Intents to process</param>
		public override void Handle(IntentsDataFlowData obj)
		{
			// Forward the call onto the output
			_output.ProcessInputData(obj);
		}

		/// <summary>
		/// Creates the output associated with filter.
		/// </summary>
		public void CreateOutput()
		{
			// Create the output and associated filter
			_output = CreateOutputInternal();
			
			// Convert the outputs to an array
			_outputsArray = new TOutput[] {_output};
		}

		#endregion

		#region IDataFlowComponent

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override DataFlowType InputDataType => DataFlowType.MultipleIntents;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override DataFlowType OutputDataType => DataFlowType.MultipleIntents;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override IDataFlowOutput[] Outputs => _outputsArray;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override IModuleDataModel ModuleData
		{
			get => Data;
			set
			{
				// Save off the data associated with filter
				Data = (TFilterData)value;

				// Create the output associated with the filter
				CreateOutput();
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Name
		{
			// Adding Tag to the Name makes the Filter unique in the graphical view
			get { return Descriptor.TypeName + ": " + Tag; }
		}
		
		#endregion

		#region IHasSetup

		/// <summary>
		/// Refer interface documentation.
		/// </summary>
		public override bool HasSetup => true;

		/// <summary>
		/// Refer interface documentation.
		/// </summary>
		public override bool Setup()
		{
			// Default to the OK button not being selected
			bool okSelected = false;

			// Display the Tagger filter setup dialog
			using (TaggedFilterSetup setup = new TaggedFilterSetup(Data))
			{
				// If the user selected OK then...
				if (setup.ShowDialog() == DialogResult.OK)
				{
					// Re-create the filter's output
					CreateOutput();

					// Indicate that setup completed successfully
					okSelected = true;
				}
			}

			return okSelected;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates the output associated with the tagged filter module.
		/// </summary>
		/// <returns>Configured tagged output</returns>
		protected abstract TOutput CreateOutputInternal();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the tag associated with filter module.
        /// </summary>
        public string Tag
		{
			get
			{
				return Data.Tag;
			}
			set
			{
				Data.Tag = value;
			}
		}

		#endregion
	}
}

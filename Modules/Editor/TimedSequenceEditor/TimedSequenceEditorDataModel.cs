using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;

namespace TimedSequenceEditor
{
	class TimedSequenceEditorDataModel : IModuleDataModel
	{
		// TODO: I have no idea what I should be populating these with. Ideas?
		public Guid ModuleTypeId { get; set; }
		public Guid ModuleInstanceId { get; set; }
		public IModuleDataSet ModuleDataSet { get; set; }

		public IModuleDataModel Clone()
		{
			TimedSequenceEditorDataModel newInstance = new TimedSequenceEditorDataModel();
			// TODO: copy any custom data to the new object here. Like this:
			//newInstance.LastOpened = LastOpened;
			return newInstance;
		}

	}
}

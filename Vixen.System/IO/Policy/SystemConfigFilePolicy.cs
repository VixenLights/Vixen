namespace Vixen.IO.Policy
{
	internal abstract class SystemConfigFilePolicy : IFilePolicy
	{
		public void Write()
		{
			WriteContextFlag();
			WriteIdentity();
			WriteFilterEvaluationAllowance();
			WriteDefaultUpdateInterval();
			WriteElements();
			WriteNodes();
			WriteControllers();
			//WriteSmartControllers();
			WritePreviews();
			WriteFilters();
			WriteDataFlowPatching();
			WriteDisabledDevices();
		}

		protected abstract void WriteContextFlag();
		protected abstract void WriteIdentity();
		protected abstract void WriteFilterEvaluationAllowance();
		protected abstract void WriteDefaultUpdateInterval();
		protected abstract void WriteElements();
		protected abstract void WriteNodes();
		protected abstract void WriteControllers();
		//protected abstract void WriteSmartControllers();
		protected abstract void WritePreviews();
		protected abstract void WriteFilters();
		protected abstract void WriteDataFlowPatching();
		protected abstract void WriteDisabledDevices();

		public void Read()
		{
			ReadContextFlag();
			ReadIdentity();
			ReadFilterEvaluationAllowance();
			ReadDefaultUpdateInterval();
			ReadElements();
			ReadNodes();
			ReadControllers();
			//ReadSmartControllers();
			ReadPreviews();
			ReadFilters();
			ReadDataFlowPatching();
			ReadDisabledDevices();
		}

		protected abstract void ReadContextFlag();
		protected abstract void ReadIdentity();
		protected abstract void ReadFilterEvaluationAllowance();
		protected abstract void ReadDefaultUpdateInterval();
		protected abstract void ReadElements();
		protected abstract void ReadNodes();
		protected abstract void ReadControllers();
		//protected abstract void ReadSmartControllers();
		protected abstract void ReadPreviews();
		protected abstract void ReadFilters();
		protected abstract void ReadDataFlowPatching();
		protected abstract void ReadDisabledDevices();
	}
}
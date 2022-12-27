namespace Vixen.Export
{
	public class ExportProgressStatus
	{
		/// <summary>
		/// Creates a progress status that is by default configured to report both types of updates
		/// </summary>
		public ExportProgressStatus()
		{
			StatusType = ProgressType.Both;
		}

		/// <summary>
		/// Creates a progress status with the specified updated type
		/// </summary>
		/// <param name="statusType"></param>
		public ExportProgressStatus(ProgressType statusType)
		{
			StatusType = statusType;
		}

		public ProgressType StatusType { get; set; }

		public int TaskProgressValue { get; set; }

		public string TaskProgressMessage { get; set; }

		public int OverallProgressValue { get; set; }

		public string OverallProgressMessage { get; set; }

		public enum ProgressType
		{
			Task,
			Overall,
			Both
		}
	}
}

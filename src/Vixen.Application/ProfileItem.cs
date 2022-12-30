namespace VixenApplication
{
	public class ProfileItem
	{
		private string _name = String.Empty;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _dataFolder = String.Empty;

		public string DataFolder
		{
			get { return _dataFolder; }
			set { _dataFolder = value; }
		}

		private DateTime _dateLastLoaded = DateTime.MaxValue;

		public DateTime DateLastLoaded
		{
			get { return _dateLastLoaded; }
			set { _dateLastLoaded = value; }
		}

		private int _profileNumber;

		public int ProfileNumber
		{
			get { return _profileNumber; }
			set { _profileNumber = value; }
		}


		public bool IsLocked { get; set; }

		public override string ToString()
		{
			return IsLocked ? string.Format("{0} - Locked", _name) : _name;
		}
	}
}
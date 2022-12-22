namespace Common.Preferences
{
	public interface IPreference
	{
		void SetStandardValues();

		void Save(string path);

	}
}

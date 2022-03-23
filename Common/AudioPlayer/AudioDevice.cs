namespace Common.AudioPlayer
{
    public class AudioDevice
    {
	    public AudioDevice(string deviceFriendlyName, string deviceId, bool isDefault = false)
	    {
		    IsDefault = isDefault;
		    FriendlyName = deviceFriendlyName;
		    Id = deviceId;
	    }

	    public string FriendlyName { get; set; }

	    public string Id { get; set; }

	    public bool IsDefault { get; set; }


	    #region Equality members

	    /// <inheritdoc />
	    public bool Equals(AudioDevice other)
	    {
		    if (ReferenceEquals(null, other)) return false;
		    if (ReferenceEquals(this, other)) return true;
		    return Id == other.Id;
	    }

	    /// <inheritdoc />
	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((AudioDevice) obj);
	    }

	    /// <inheritdoc />
	    public override int GetHashCode()
	    {
		    return (Id != null ? Id.GetHashCode() : 0);
	    }

	    #endregion
    }
}

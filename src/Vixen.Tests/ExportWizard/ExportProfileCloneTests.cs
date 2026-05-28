using System.Reflection;
using Vixen.Export;
using VixenModules.App.ExportWizard;
using Xunit;

namespace Vixen.Tests.ExportWizard;

public class ExportProfileCloneTests
{
	/// <summary>
	/// Creates an <see cref="ExportProfile"/> via its private parameterless constructor
	/// so that <see cref="ExportProfile.SyncronizeControllerInfo"/> is not called.
	/// Sets the list properties to empty collections so that <see cref="ExportProfile.Clone"/>
	/// does not throw on null references.
	/// </summary>
	private static ExportProfile CreateMinimalProfile()
	{
		var profile = (ExportProfile)Activator.CreateInstance(
			typeof(ExportProfile),
			nonPublic: true)!;

		typeof(ExportProfile)
			.GetProperty(nameof(ExportProfile.SequenceFiles))!
			.SetValue(profile, new List<string>());

		typeof(ExportProfile)
			.GetProperty(nameof(ExportProfile.Controllers),
				BindingFlags.Public | BindingFlags.Instance)!
			.SetValue(profile, new List<Controller>(),
				BindingFlags.NonPublic | BindingFlags.Instance,
				null, null, null);

		return profile;
	}

	[Fact]
	public void Clone_CopiesFppDirectUpload_True()
	{
		// Arrange
		var profile = CreateMinimalProfile();
		profile.FppDirectUpload = true;

		// Act
		var clone = (ExportProfile)profile.Clone();

		// Assert
		Assert.True(clone.FppDirectUpload);
	}

	[Fact]
	public void Clone_CopiesFppHostAddress()
	{
		// Arrange
		var profile = CreateMinimalProfile();
		profile.FppHostAddress = "192.168.1.10";

		// Act
		var clone = (ExportProfile)profile.Clone();

		// Assert
		Assert.Equal("192.168.1.10", clone.FppHostAddress);
	}

	[Fact]
	public void Clone_DefaultFppDirectUploadIsFalse()
	{
		// Arrange
		var profile = CreateMinimalProfile();

		// Act & Assert
		Assert.False(profile.FppDirectUpload);
	}
}

using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Moq;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Wipe;
using Xunit;
using ZedGraph;

namespace Vixen.Tests.Effects;

/// <summary>
/// Verifies that Wipe data cloning preserves serialized settings and isolates editable values.
/// </summary>
public sealed class WipeDataCloneTests
{
	private static readonly HashSet<string> SerializedFixtureMembers =
	[
		"ColorHandling",
		"ColorGradient",
		"Direction",
		"Curve",
		"PulseTime",
		"PassCount",
		"PulsePercent",
		"WipeOn",
		"WipeOff",
		"MovementCurve",
		"WipeMovement",
		"ReverseDirection",
		"ColorAcrossItemPerCount",
		"ReverseColorDirection",
		"XOffset",
		"YOffset",
		"DepthOfEffect",
		"TargetNodeSelection",
		"TargetPositioning",
		"ModuleTypeId",
		"ModuleInstanceId"
	];

	/// <summary>
	/// Verifies that cloning preserves every serialized Wipe setting and keeps its owner reference.
	/// </summary>
	/// <param name="targetNodeSelection">One of the enumeration values that specifies the target handling mode.</param>
	[Theory]
	[InlineData(TargetNodeSelection.Group)]
	[InlineData(TargetNodeSelection.Individual)]
	public void Clone_PreservesAllSerializedSettings(TargetNodeSelection targetNodeSelection)
	{
		// Arrange
		var owner = new Mock<IModuleDataSet>().Object;
		var source = CreateConfiguredData(targetNodeSelection);
		source.ModuleDataSet = owner;

		// Act
		var clone = Assert.IsType<WipeData>(source.Clone());

		// Assert
		Assert.NotSame(source, clone);
		Assert.Same(owner, clone.ModuleDataSet);
		AssertSerializedMembersAreCovered(source, clone);
	}

	/// <summary>
	/// Verifies that edits to every cloned curve and gradient point leave the source data unchanged in either direction.
	/// </summary>
	[Fact]
	public void Clone_CurvesAndGradientAreIndependentInBothDirections()
	{
		// Arrange
		var source = CreateConfiguredData(TargetNodeSelection.Individual);
		var clone = Assert.IsType<WipeData>(source.Clone());
		SetGradientAsCurrentLibraryReference(source.ColorGradient);
		SetGradientAsCurrentLibraryReference(clone.ColorGradient);
		var originalCurveY = source.Curve.Points[1].Y;
		var originalMovementY = source.MovementCurve.Points[1].Y;
		var originalGradientPosition = source.ColorGradient.Colors[1].Position;
		var cloneCurveY = clone.Curve.Points[1].Y;
		var cloneMovementY = clone.MovementCurve.Points[1].Y;
		var cloneGradientPosition = clone.ColorGradient.Colors[1].Position;

		// Act
		clone.Curve.Points[1].Y = cloneCurveY + 1;
		clone.MovementCurve.Points[1].Y = cloneMovementY + 1;
		clone.ColorGradient.Colors[1].Position = cloneGradientPosition + 0.01;

		// Assert
		Assert.Equal(originalCurveY, source.Curve.Points[1].Y);
		Assert.Equal(originalMovementY, source.MovementCurve.Points[1].Y);
		Assert.Equal(originalGradientPosition, source.ColorGradient.Colors[1].Position);
		Assert.NotSame(source.Curve, clone.Curve);
		Assert.NotSame(source.MovementCurve, clone.MovementCurve);
		Assert.NotSame(source.ColorGradient, clone.ColorGradient);

		// Act
		source.Curve.Points[1].Y = originalCurveY + 2;
		source.MovementCurve.Points[1].Y = originalMovementY + 2;
		source.ColorGradient.Alphas[1].Alpha = 0.4;

		// Assert
		Assert.Equal(cloneCurveY + 1, clone.Curve.Points[1].Y);
		Assert.Equal(cloneMovementY + 1, clone.MovementCurve.Points[1].Y);
		Assert.Equal(0.8, clone.ColorGradient.Alphas[1].Alpha);
	}

	/// <summary>
	/// Verifies that raw cloning preserves null mutable members without substituting defaults.
	/// </summary>
	[Fact]
	public void Clone_PreservesNullMutableMembers()
	{
		// Arrange
		var source = CreateConfiguredData(TargetNodeSelection.Group);
		source.Curve = null!;
		source.MovementCurve = null!;
		source.ColorGradient = null!;

		// Act
		var clone = Assert.IsType<WipeData>(source.Clone());

		// Assert
		Assert.Null(clone.Curve);
		Assert.Null(clone.MovementCurve);
		Assert.Null(clone.ColorGradient);
	}

	/// <summary>
	/// Verifies that each reference member has an explicit clone ownership classification.
	/// </summary>
	[Fact]
	public void Clone_ReferenceMemberInventoryHasNoUnclassifiedMembers()
	{
		// Arrange
		var source = CreateConfiguredData(TargetNodeSelection.Group);
		var clone = Assert.IsType<WipeData>(source.Clone());

		// Act and assert
		AssertReferenceMembersHaveExplicitOwnershipRules(source, clone);
	}

	private static WipeData CreateConfiguredData(TargetNodeSelection targetNodeSelection)
	{
		var gradient = new ColorGradient();
		gradient.Colors.Clear();
		gradient.Colors.Add(new ColorPoint(Color.Red, 0));
		gradient.Colors.Add(new ColorPoint(Color.Blue, 0.55));
		gradient.Colors.Add(new ColorPoint(Color.Green, 1));
		gradient.Alphas.Clear();
		gradient.Alphas.Add(new AlphaPoint(0.15, 0, 0));
		gradient.Alphas.Add(new AlphaPoint(0.8, 0, 0.6));
		gradient.Alphas.Add(new AlphaPoint(0.35, 0, 1));
		gradient.Title = "Wipe clone fixture";
		gradient.Gammacorrected = true;
		gradient.LibraryReferenceName = "gradient fixture";
		gradient.IsCurrentLibraryGradient = true;
		SetGradientAsCurrentLibraryReference(gradient);

		var data = new WipeData
		{
			ColorHandling = ColorHandling.ColorAcrossItems,
			ColorGradient = gradient,
			Direction = WipeDirection.DiagonalDown,
			Curve = CreateCurve(0, 12, 20, 67, 100, 91, "intensity fixture"),
			PulseTime = 1743,
			PassCount = 7,
			PulsePercent = 42.5,
			WipeOn = true,
			WipeOff = true,
			MovementCurve = CreateCurve(0, 4, 38, 73, 100, 96, "movement fixture"),
			WipeMovement = WipeMovement.Movement,
			ReverseDirection = true,
			ColorAcrossItemPerCount = false,
			ReverseColorDirection = false,
			XOffset = 12.75,
			YOffset = -8.25,
			DepthOfEffect = 2,
			TargetNodeSelection = targetNodeSelection,
			TargetPositioning = TargetPositioningType.Locations,
			ModuleTypeId = Guid.Parse("c8994b8e-10ec-48f5-8df1-356838b5eb12"),
			ModuleInstanceId = Guid.Parse("d219955a-321a-4ee3-b38f-d058f79f27c7")
		};

		return data;
	}

	private static Curve CreateCurve(double x1, double y1, double x2, double y2, double x3, double y3, string libraryReferenceName)
	{
		var points = new PointPairList
		{
			new PointPair(x1, y1),
			new PointPair(x2, y2),
			new PointPair(x3, y3)
		};
		return new Curve(points)
		{
			LibraryReferenceName = libraryReferenceName,
			IsCurrentLibraryCurve = true
		};
	}

	private static void AssertSerializedMembersAreCovered(WipeData source, WipeData clone)
	{
		var members = GetDataMembers(typeof(WipeData));
		var discoveredNames = members.Select(member => member.Name).ToHashSet(StringComparer.Ordinal);
		var uncoveredMembers = discoveredNames.Except(SerializedFixtureMembers).OrderBy(name => name).ToArray();
		var staleFixtureEntries = SerializedFixtureMembers.Except(discoveredNames).OrderBy(name => name).ToArray();
		Assert.True(
			uncoveredMembers.Length == 0 && staleFixtureEntries.Length == 0,
			$"Serialized member coverage differs. Members missing fixture/equality coverage: {string.Join(", ", uncoveredMembers)}. Fixture entries no longer serialized: {string.Join(", ", staleFixtureEntries)}.");

		foreach (var member in members)
		{
			var sourceValue = GetMemberValue(member, source);
			var cloneValue = GetMemberValue(member, clone);
			AssertMemberValueEqual(member.Name, sourceValue, cloneValue);
		}

		AssertCurveContentEqual(source.Curve, clone.Curve);
		AssertCurveContentEqual(source.MovementCurve, clone.MovementCurve);
		AssertGradientContentEqual(source.ColorGradient, clone.ColorGradient);
		Assert.Equal(source.Curve.LibraryReferenceName, clone.Curve.LibraryReferenceName);
		Assert.Equal(source.Curve.IsCurrentLibraryCurve, clone.Curve.IsCurrentLibraryCurve);
		Assert.Equal(source.MovementCurve.LibraryReferenceName, clone.MovementCurve.LibraryReferenceName);
		Assert.Equal(source.MovementCurve.IsCurrentLibraryCurve, clone.MovementCurve.IsCurrentLibraryCurve);
		Assert.Equal(source.ColorGradient.LibraryReferenceName, clone.ColorGradient.LibraryReferenceName);
		Assert.Equal(source.ColorGradient.IsCurrentLibraryGradient, clone.ColorGradient.IsCurrentLibraryGradient);
	}

	private static IEnumerable<MemberInfo> GetDataMembers(Type type)
	{
		for (var currentType = type; currentType is not null; currentType = currentType.BaseType)
		{
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			foreach (var property in currentType.GetProperties(flags))
			{
				if (property.IsDefined(typeof(DataMemberAttribute), inherit: false))
				{
					yield return property;
				}
			}

			foreach (var field in currentType.GetFields(flags))
			{
				if (field.IsDefined(typeof(DataMemberAttribute), inherit: false))
				{
					yield return field;
				}
			}
		}
	}

	private static object? GetMemberValue(MemberInfo member, object instance)
	{
		return member switch
		{
			PropertyInfo property => property.GetValue(instance),
			FieldInfo field => field.GetValue(instance),
			_ => throw new InvalidOperationException($"Unsupported serialized member {member.Name}.")
		};
	}

	private static void AssertMemberValueEqual(string memberName, object? expected, object? actual)
	{
		switch (memberName)
		{
			case nameof(WipeData.Curve):
		case nameof(WipeData.MovementCurve):
			AssertCurveContentEqual(Assert.IsType<Curve>(expected), Assert.IsType<Curve>(actual));
			break;
			case nameof(WipeData.ColorGradient):
			AssertGradientContentEqual(Assert.IsType<ColorGradient>(expected), Assert.IsType<ColorGradient>(actual));
			break;
			default:
			Assert.Equal(expected, actual);
			break;
		}
	}

	private static void AssertCurveContentEqual(Curve expected, Curve actual)
	{
		Assert.Equal(expected.Points.Select(point => (point.X, point.Y)), actual.Points.Select(point => (point.X, point.Y)));
		Assert.Equal(expected.LibraryReferenceName, actual.LibraryReferenceName);
		Assert.Equal(expected.IsCurrentLibraryCurve, actual.IsCurrentLibraryCurve);
	}

	private static void AssertGradientContentEqual(ColorGradient expected, ColorGradient actual)
	{
		SetGradientAsCurrentLibraryReference(expected);
		SetGradientAsCurrentLibraryReference(actual);
		Assert.Equal(expected.Colors, actual.Colors);
		Assert.Equal(expected.Alphas, actual.Alphas);
		Assert.Equal(expected.Title, actual.Title);
		Assert.Equal(expected.Gammacorrected, actual.Gammacorrected);
		Assert.Equal(expected.LibraryReferenceName, actual.LibraryReferenceName);
		Assert.Equal(expected.IsCurrentLibraryGradient, actual.IsCurrentLibraryGradient);
	}

	private static void SetGradientAsCurrentLibraryReference(ColorGradient gradient)
	{
		var referencedGradientField = typeof(ColorGradient).GetField("_libraryReferencedGradient", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(referencedGradientField);
		referencedGradientField.SetValue(gradient, gradient);
	}

	private static void AssertReferenceMembersHaveExplicitOwnershipRules(WipeData source, WipeData clone)
	{
		var ownershipRules = new Dictionary<string, string>(StringComparer.Ordinal)
		{
			[nameof(WipeData.Curve)] = "deep-copy",
			[nameof(WipeData.MovementCurve)] = "deep-copy",
			[nameof(WipeData.ColorGradient)] = "deep-copy",
			[nameof(ModuleDataModelBase.ModuleDataSet)] = "shared-owner"
		};
		var referenceMembers = GetInstanceProperties(typeof(WipeData))
			.Where(property => !property.PropertyType.IsValueType)
			.Select(property => (MemberName: property.Name, MemberType: property.PropertyType, GetValue: (Func<object, object?>)(instance => property.GetValue(instance))))
			.Concat(GetInstanceFields(typeof(WipeData))
				.Where(field => !field.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false) && !field.FieldType.IsValueType)
				.Select(field => (MemberName: field.Name, MemberType: field.FieldType, GetValue: (Func<object, object?>)(instance => field.GetValue(instance)))))
			.ToArray();

		foreach (var (memberName, memberType, getValue) in referenceMembers)
		{
			var sourceValue = getValue(source);
			var cloneValue = getValue(clone);
			if (memberType == typeof(string))
			{
				Assert.Equal(sourceValue, cloneValue);
				continue;
			}

			Assert.True(ownershipRules.TryGetValue(memberName, out var ownershipRule), $"Reference member '{memberName}' has no ownership classification.");
			switch (ownershipRule)
			{
				case "deep-copy":
					Assert.NotSame(sourceValue, cloneValue);
					break;
				case "shared-owner":
					Assert.Same(sourceValue, cloneValue);
					break;
				default:
					Assert.Fail($"Reference member '{memberName}' has unsupported ownership classification '{ownershipRule}'.");
					break;
			}
		}
	}

	private static IEnumerable<PropertyInfo> GetInstanceProperties(Type type)
	{
		for (var currentType = type; currentType is not null; currentType = currentType.BaseType)
		{
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			foreach (var property in currentType.GetProperties(flags))
			{
				if (property.GetIndexParameters().Length == 0 && property.GetMethod is not null)
				{
					yield return property;
				}
			}
		}
	}

	private static IEnumerable<FieldInfo> GetInstanceFields(Type type)
	{
		for (var currentType = type; currentType is not null; currentType = currentType.BaseType)
		{
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			foreach (var field in currentType.GetFields(flags))
			{
				yield return field;
			}
		}
	}
}

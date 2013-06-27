using System;

namespace Vixen.IO
{
	public interface IMigrationSegment<T> : IMigrationSegment, IEquatable<IMigrationSegment<T>>
		where T : class
	{
		T Execute(T content);
	}

	public interface IMigrationSegment : IEquatable<IMigrationSegment>
	{
		object Execute(object content);
		int FromVersion { get; }
		int ToVersion { get; }
	}
}
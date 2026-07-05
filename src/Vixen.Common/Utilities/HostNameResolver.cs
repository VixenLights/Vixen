using System.Net;
using System.Net.Sockets;

namespace Utilities
{
	/// <summary>
	/// Resolves DNS host names to IPv4 addresses. 
	/// </summary>
	public static class HostNameResolver
	{
		/// <summary>
		/// Attempts to resolve <paramref name="hostName"/> to an IPv4 address.
		/// </summary>
		/// <param name="hostName">A DNS host name, e.g. "wled-porch" or "wled-porch.local". Must not be null or empty.</param>
		/// <param name="resolvedAddress">
		/// When this method returns <see langword="true"/>, the first IPv4 (<see cref="AddressFamily.InterNetwork"/>)
		/// address found for <paramref name="hostName"/>. When this method returns <see langword="false"/>,
		/// <see langword="null"/>.
		/// </param>
		/// <returns><see langword="true"/> if an IPv4 address was found; otherwise <see langword="false"/>.</returns>
		public static bool TryResolveToIPv4(string hostName, out IPAddress resolvedAddress)
		{
			resolvedAddress = null;
			if (string.IsNullOrWhiteSpace(hostName))
			{
				return false;
			}

			IPAddress[] addresses;
			try
			{
				addresses = Dns.GetHostAddresses(hostName);
			}
			catch (SocketException)
			{
				return false;
			}

			resolvedAddress = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
			return resolvedAddress != null;
		}
	}
}

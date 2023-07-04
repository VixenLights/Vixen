using Vixen.Export.FPP;

namespace Vixen.Export
{
	/// <summary>
	/// Vixen Player FSEQ writer.  This writer includes an extra extended data
	/// header to facilitate staleness checks and improved extraction of channel data.
	/// </summary>
	public class VixenPlayerFSEQCompressedWriter : FSEQCompressedWriter
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public VixenPlayerFSEQCompressedWriter()
		{
			FileTypeDescr = "Vixen Player";
		}

		#endregion

		#region Protected Methods

		/// <inheritdoc/>
		protected override void AddOptionalVariableHeaders()
		{
			// Add the Vixen Player extended data header
			VariableHeaders.Add(new VixenPlayerVariableHeader(CreateVixenPlayerExtendedDataVariableHeader));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the Vixen Player extended data associated with the header.
		/// </summary>
		/// <returns>Vixen Player extended data</returns>
		private byte[] CreateVixenPlayerExtendedDataVariableHeader()
		{
			// Create a memory stream
			using (MemoryStream stream = new MemoryStream())
			{
				// Create a binary writer on top of the memory stream
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					string version = "1.0";

					// Write the Version #.# for Vixen Player extended data header
					writer.Write("1.0");

					// Write out the number of controller IDs
					writer.Write(ControllerIDs.Count);

					// Loop over the controller IDs
					int controllerIndex = 0;
					foreach (Guid controllerID in ControllerIDs)
					{
						// Write the controller ID
						writer.Write(controllerID.ToString());

						// Write the number of channels the controller supports
						writer.Write(ControllerChannels[controllerIndex]);

						// Move on to the next controller
						controllerIndex++;
					}

					// Write the sequence timestamp
					writer.Write(SequenceTimeStamp.ToBinary());
				}
				
				// Flush the stream
				stream.Flush();

				// Return the extended data buffer
				return stream.GetBuffer();
			}
		}

		#endregion
	}
}

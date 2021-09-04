using System.Diagnostics;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;
using VixenModules.Preview.VixenPreview.Fixtures;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Dispatches and handles intents for a moving head preview shape.
	/// </summary>
	public class MovingHeadIntentHandler : IntentStateDispatch
	{		
		#region Public Properties

		/// <summary>
		/// 
		/// </summary>
		public int PanStartPosition
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public int PanStopPosition
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public int TiltStartPosition
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public int TiltStopPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Beam length factor in percent (0-100%).
		/// </summary>
		public int BeamLength
		{
			get;			
			set;			
		}
		
		/// <summary>
		/// Moving head settings associated with the preview shape.
		/// </summary>
		public IMovingHead MovingHead { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Dispatches intents to a specific handler method.
		/// </summary>
		/// <param name="states">Collection of IIntentState to dispatch</param>
		public void Dispatch(IIntentStates states)
		{
			// If there are valid intents then...
			if (states != null)
			{
				// Loop over the intents
				foreach (IIntentState state in states)
				{
					// If the intent is a positional intent then...
					if (state is IIntentState<PositionValue>)
					{
						// Handle the position intent
						Handle((IIntentState<PositionValue>)state);
					}
					// Otherwise if the intent is a lighting intent then...
					else if (state is IIntentState<LightingValue>)
					{
						// Handle the lighting intent
						Handle((IIntentState<LightingValue>)state);
					}
					else if (state is IIntentState<RGBValue>)
					{
						// Handle the lighting intent
						Handle((IIntentState<RGBValue>)state);
					}
				}
			}
		}

		/// <summary>
		/// Handles position intents.
		/// </summary>
		/// <param name="positionIntent">Position intent to handle</param>
		public override void Handle(IIntentState<PositionValue> positionIntent)
		{
			// Determine which type of position type is in the intent
			switch (positionIntent.GetValue().PositionType)
			{
				case PositionType.Pan:
					double pan = positionIntent.GetValue().Position * (PanStopPosition - PanStartPosition) + PanStartPosition;
					while (pan > 360.0)
					{
						pan -= 360;
					}
					while (pan < -360)
					{
						pan += 360;
					}
					// Set the moving head pan angle converting to degrees
					MovingHead.PanAngle = pan;
					
					break;
				case PositionType.Tilt:

					double tilt = positionIntent.GetValue().Position * (TiltStopPosition - TiltStartPosition) + TiltStartPosition;
					while (tilt > 360.0)
					{
						tilt -= 360;
					}
					while (tilt < -360)
					{
						tilt += 360;
					}
					// Set the moving head tilt angle converting to degrees
					MovingHead.TiltAngle = tilt;
					
					break;
				default:
					Debug.Assert(false, "Unsupported Position Type");
					break;
			}
		}

		/// <summary>
		/// Handles lighting intents.
		/// </summary>
		/// <param name="lightingIntent">Lighting intent to handle</param>
		public override void Handle(IIntentState<LightingValue> lightingIntent)
		{
			// Turn on the beam
			MovingHead.OnOff = true;

			// Set the beam color of the moving head
			MovingHead.BeamColor = lightingIntent.GetValue().Color;

			// Set the intensity of the beam color
			MovingHead.Intensity = (int)(lightingIntent.GetValue().Intensity * 100.0);

			// Set the beam length taking into account a factor specified on the preview shape
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}

		public override void Handle(IIntentState<RGBValue> lightingIntent)
		{
			// Turn on the beam
			MovingHead.OnOff = true;

			// Set the beam color of the moving head
			MovingHead.BeamColor = lightingIntent.GetValue().FullColor;

			// Set the intensity of the beam color
			MovingHead.Intensity = (int)(lightingIntent.GetValue().Intensity * 100.0);

			// Set the beam length taking into account a factor specified on the preview shape
			//TODO: Better place to put this code?
			MovingHead.BeamLength = (int)BeamLength;
		}

		#endregion
	}
}

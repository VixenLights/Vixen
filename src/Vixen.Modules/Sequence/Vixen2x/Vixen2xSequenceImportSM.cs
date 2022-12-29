using System;
using System.Linq;
using Vixen.Sys;
using VixenModules.Sequence.Timed;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using Vixen.Services;
using Vixen.Module.Effect;
using VixenModules.App.Curves;
using VixenModules.App.ColorGradients;
using System.Drawing;
using ZedGraph;

namespace VixenModules.SequenceType.Vixen2x
{
	public class Vixen2xSequenceImportSM
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Current output sequence data
		/// </summary>
		private ISequence m_Sequence = null;

		private double m_eventDuration = 0.0;
		private Color m_currentColor = new Color();
		private byte m_StartingIntensity = 0;
		private byte m_currentIntensity = 0;
		private uint m_startEventPosition = 0;
		private uint m_endEventPosition = 0;
		private ElementNode m_targetNode = null;

		/// <summary>
		/// storage for the state machine that process events from the incoming sequence
		/// </summary>
		private IVixen2xImportStateMachine m_stateMachine = null;

		// event instances
		private SIgnoreEvents m_IgnoreEvents = new SIgnoreEvents();
		private SWaitingToStart m_waitingToStart = new SWaitingToStart();
		private SHoldingOneEvent m_holdingOneEvent = new SHoldingOneEvent();
		private SSingleIntensityMultipleEvents m_SingleIntensityMultipleEvents = new SSingleIntensityMultipleEvents();
		private SRampDown m_RampDown = new SRampDown();
		private SRampUp m_RampUp = new SRampUp();

		// public accessors
		public IVixen2xImportStateMachine StateMachine { get { return m_stateMachine; } set { m_stateMachine = value; } }
		public SIgnoreEvents IgnoreEvents { get { return m_IgnoreEvents; } }
		public SWaitingToStart WaitingToStart { get { return m_waitingToStart; } }
		public SHoldingOneEvent HoldingOneEvent { get { return m_holdingOneEvent; } }
		public SSingleIntensityMultipleEvents SingleIntensityMultipleEvents { get { return m_SingleIntensityMultipleEvents; } }
		public SRampDown RampDown { get { return m_RampDown; } }
		public SRampUp RampUp { get { return m_RampUp; } }

		public ISequence Sequence { get { return m_Sequence; } }
		public Color CurrentColor { get { return m_currentColor; } set { m_currentColor = value; } }
		public byte StartingIntensity { get { return m_StartingIntensity; } set { m_StartingIntensity = value; } }
		public byte CurrentIntensity { get { return m_currentIntensity; } set { m_currentIntensity = value; } }
		public uint StartEventPosition { get { return m_startEventPosition; } set { m_startEventPosition = value; } }
		public uint EndEventPosition { get { return m_endEventPosition; } set { m_endEventPosition = value; } }
		public ElementNode TargetNode { get { return m_targetNode; } }
		public double EventDuration { get { return m_eventDuration; } set { m_eventDuration = value; } }

		/// <summary>
		/// Set up the data import state machine
		/// </summary>
		public Vixen2xSequenceImportSM(ISequence sequence, double eventDuration)
		{
			Logging.Debug("Vixen2xSequenceImport - Enter");
			// init the sate machine
			StateMachine = m_IgnoreEvents;

			// save a reference to the sequence data
			m_Sequence = sequence;

			// record how long each event lasts
			m_eventDuration = eventDuration;

			Logging.Debug("Vixen2xSequenceImport - Exit");
		} // Vixen2xSequenceImport

		/// <summary>
		/// set up context to process data for an output channel
		/// </summary>
		/// <param name="ChannelId">mappings[chan].ElementNodeId</param>
		public void OpenChannel(Guid TargetChannelId, double eventDuration)
		{
			Logging.Debug("Vixen2xSequenceImport::OpenChannel - Enter. GUID '" + TargetChannelId.ToString() + "' eventDuration: " + eventDuration);

			// set the target node for the incoming data
			if (null == (m_targetNode = VixenSystem.Nodes.GetElementNode(TargetChannelId)))
			{
				Logging.Error("Vixen2xSequenceImport::OpenChannel - GUID '" + TargetChannelId.ToString() + "' eventDuration: " + eventDuration);
				StateMachine = m_IgnoreEvents;
			}
			else
			{
				// set up to process the first event
				StateMachine = m_waitingToStart;
			}
			// ms / event
			m_eventDuration = eventDuration;

			Logging.Debug("Vixen2xSequenceImport::OpenChannel - Exit");
		} // OpenChannel

		/// <summary>
		/// process a single event
		/// </summary>
		public void processEvent(uint position, Color color, byte intensity)
		{
			//			Logging.Info("Vixen2xSequenceImport::processEvent - Enter. position: " + position + ", color: " + color.ToString() + ", intensity: " + intensity);
			m_stateMachine.processEvent(this, position, color, intensity);
			//			Logging.Info("Vixen2xSequenceImport::processEvent - Exit");
		}

		/// <summary>
		/// Stop processing data for this channel
		/// </summary>
		public void closeChannel()
		{
			Logging.Debug("Vixen2xSequenceImport::closeChannel - Entry");
			m_stateMachine.closeChannel(this);
			Logging.Debug("Vixen2xSequenceImport::closeChannel - Exit");
		} // closeChannel

		/// <summary>
		/// Add a constant level effect to the destination channel
		/// </summary>
		/// <param name="intensity">period intensity</param>
		/// <param name="startEvent">number of V2 events to skip</param>
		/// <param name="endEvent">Last V2 event in the block</param>
		/// <param name="targetNode">Destination channel structure</param>
		/// <param name="v3color">Color for this effect</param>
		/// <returns></returns>
		public EffectNode GenerateSetLevelEffect(uint intensity,
												 uint startEvent,
												 uint endEvent,
												 ElementNode targetNode,
												 Color v3color)
		{
			//			Logging.Info("Vixen2xSequenceImport::GenerateSetLevelEffect - Entry");

			EffectNode effectNode = null;
			IEffectModuleInstance setLevelInstance = null;

			do
			{
				if (null == (setLevelInstance = ApplicationServices.Get<IEffectModuleInstance>(Guid.Parse("32cff8e0-5b10-4466-a093-0d232c55aac0"))))
				{
					// could not get the structure
					Logging.Error("Vixen 2 import: Could not allocate an instance of IEffectModuleInstance");
					break;
				}

				// Clone() Doesn't work! :(
				setLevelInstance.TargetNodes = new ElementNode[] { targetNode };

				// calculate how long the event lasts
				setLevelInstance.TimeSpan = TimeSpan.FromMilliseconds(EventDuration * (endEvent - startEvent + 1));

				// set the event and event starting time
				if (null == (effectNode = new EffectNode(setLevelInstance, TimeSpan.FromMilliseconds(EventDuration * startEvent))))
				{
					// could not allocate the structure
					Logging.Error("Vixen 2 import: Could not allocate an instance of EffectNode");
					break;
				}

				// set intensity and color
				effectNode.Effect.ParameterValues = new object[] { (Convert.ToDouble(intensity) / Convert.ToDouble(byte.MaxValue)), v3color };
			} while (false);

			//			Logging.Info("Vixen2xSequenceImport::GenerateSetLevelEffect - Exit");
			return effectNode;
		} // end GenerateSetLevelEffect

		/// <summary>
		/// Add a constantly increasing / deacreasing ramp
		/// </summary>
		/// <param name="eventStartValue"></param>
		/// <param name="eventEndValue"></param>
		/// <param name="startEvent"></param>
		/// <param name="endEvent"></param>
		/// <param name="targetNode"></param>
		/// <param name="v3color"></param>
		/// <returns></returns>
		public EffectNode GeneratePulseEffect( uint eventStartValue, 
											   uint eventEndValue, 
											   uint startEvent, 
											   uint endEvent,
		                                       ElementNode targetNode, 
											   Color v3color)
		{
			EffectNode effectNode = null;
			const double startX = 0.0;
			const double endX = 100.0;

			do
			{
				IEffectModuleInstance pulseInstance = ApplicationServices.Get<IEffectModuleInstance>(Guid.Parse("cbd76d3b-c924-40ff-bad6-d1437b3dbdc0"));
				if (null == pulseInstance)
				{
					Logging.Error("Vixen 2 import: Could not allocate an instance of IEffectModuleInstance");
					break;
				}
				
				// Clone() Doesn't work! :(
				pulseInstance.TargetNodes = new ElementNode[] { targetNode };
				pulseInstance.TimeSpan = TimeSpan.FromMilliseconds(EventDuration * (endEvent - startEvent + 1));

				if( null == (effectNode = new EffectNode(pulseInstance, TimeSpan.FromMilliseconds(EventDuration * startEvent))))
				{
					// could not allocate the structure
					Logging.Error("Vixen 2 import: Could not allocate an instance of EffectNode");
					break;
				}

				effectNode.Effect.ParameterValues = new Object[]
				{
					new Curve(new PointPairList(new double[] {startX, endX}, new double[] {getY(eventStartValue), getY(eventEndValue)})), new ColorGradient(v3color)
				};
			} while (false);

			return effectNode;
		} // end GeneratePulseEffect

		private double getY(uint value)
		{
			const double curveDivisor = byte.MaxValue/100.0;

			return Convert.ToDouble(value) / curveDivisor;
		} // end getY
	} // end class Vixen2xImportStateMachine

	/// <summary>
	/// Interface class for the state machine
	/// </summary>
	public interface IVixen2xImportStateMachine
	{
		void processEvent(Vixen2xSequenceImportSM parent, uint position, Color color, byte intensity);
		void closeChannel(Vixen2xSequenceImportSM parent);
	} // Vixen2xSequenceImport

	/// <summary>
	/// State to ignore the incoming events
	/// </summary>
	public class SIgnoreEvents : IVixen2xImportStateMachine
	{
		public void processEvent(Vixen2xSequenceImportSM parent, uint position, Color color, byte intensity)
		{
			// just ignore the data
		} // processEvent

		public void closeChannel(Vixen2xSequenceImportSM parent)
		{
			// set back to the waiting for the first event state
			parent.StateMachine = parent.IgnoreEvents;
		} // closeChannel
	} // SIgnoreEvents

	/// <summary>
	/// Idle state. No data is in progress
	/// </summary>
	public class SWaitingToStart : IVixen2xImportStateMachine
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public void processEvent(Vixen2xSequenceImportSM parent, uint position, Color color, byte intensity)
		{
//			Logging.Info("SWaitingToStart::processEvent - Entry");
			do
			{
				// save the starting values
				parent.CurrentColor = color;
				parent.StartingIntensity = intensity;
				parent.CurrentIntensity = intensity;
				parent.StartEventPosition = position;
				parent.EndEventPosition = position;

				// we ignore events that are close to black
				if( 5 > intensity)
				{
					// dont bother
					parent.StateMachine = parent.WaitingToStart;
					break;
				}

				// next state Wait for a scond event to arrive
				parent.StateMachine = parent.HoldingOneEvent;

			} while (false);
//			Logging.Info("SWaitingToStart::processEvent - Exit");
		} // processEvent

		public void closeChannel(Vixen2xSequenceImportSM parent)
		{
			// set back to the waiting for the first event state
			parent.StateMachine = parent.IgnoreEvents;
		} // closeChannel
	} // SWaitingToStart

	/// <summary>
	/// Received the first event, waiting for a second
	/// </summary>
	public class SHoldingOneEvent : IVixen2xImportStateMachine
	{
		public void processEvent(Vixen2xSequenceImportSM parent, uint position, Color color, byte intensity)
		{
			// is the intensity and color the same as it was?
			if ((parent.CurrentColor == color) && (parent.CurrentIntensity == intensity))
			{
				// we are in a block of similar events
				parent.StateMachine = parent.SingleIntensityMultipleEvents;

				// update the current end position
				parent.EndEventPosition = position;
			}
			// is the intensity and color the same as it was?
			else if ((parent.CurrentColor == color) && (parent.CurrentIntensity < intensity))
			{
				// we are in a block of increasing intensity events
				parent.StateMachine = parent.RampUp;

				// update the current end position
				parent.EndEventPosition = position;
				parent.CurrentIntensity = intensity;
			}
			// is the intensity and color the same as it was?
			else if ((parent.CurrentColor == color) && (parent.CurrentIntensity > intensity) && (5 < intensity))
			{
				// we are in a block of increasing intensity events
				parent.StateMachine = parent.RampDown;

				// update the current end position
				parent.EndEventPosition = position;
				parent.CurrentIntensity = intensity;
			}
			else
			{
				// something has changed. Write out the current data and start a new block
				closeChannel(parent);
				parent.StateMachine = parent.WaitingToStart;
				parent.processEvent(position, color, intensity);
			}
		} // processEvent

		public void closeChannel(Vixen2xSequenceImportSM parent)
		{
			// save the data
			EffectNode node = parent.GenerateSetLevelEffect(parent.CurrentIntensity, parent.StartEventPosition, parent.EndEventPosition, parent.TargetNode, parent.CurrentColor);
			if (node != null)
			{
				parent.Sequence.InsertData(node);
			}

			// set back to the waiting for the channel to open
			parent.StateMachine = parent.IgnoreEvents;

		} // closeChannel
	} // HoldingOneEvent

	/// <summary>
	/// Processing a block of single color single intensity
	/// </summary>
	public class SSingleIntensityMultipleEvents : IVixen2xImportStateMachine
	{
		public void processEvent(Vixen2xSequenceImportSM parent, uint position, Color color, byte intensity)
		{
			// is the intensity and color the same as it was?
			if ((parent.CurrentColor == color) && (parent.CurrentIntensity == intensity))
			{
				// we are in a block of similar events
				parent.StateMachine = parent.SingleIntensityMultipleEvents;

				// update the current end position
				parent.EndEventPosition = position;
			}
			else
			{
				// something has changed. Write out the current data and start a new block
				closeChannel(parent);
				parent.StateMachine = parent.WaitingToStart;
				parent.processEvent(position, color, intensity);
			}
		} // processEvent

		public void closeChannel(Vixen2xSequenceImportSM parent)
		{
			// save the data
			EffectNode node = parent.GenerateSetLevelEffect(parent.CurrentIntensity, parent.StartEventPosition, parent.EndEventPosition, parent.TargetNode, parent.CurrentColor);
			if (node != null)
			{
				parent.Sequence.InsertData(node);
			}

			// set back to the waiting for the channel to open
			parent.StateMachine = parent.IgnoreEvents;
		} // closeChannel
	} // SSingleIntensityMultipleEvents

	/// <summary>
	/// Handle the ramp down sequence
	/// </summary>
	public class SRampDown : IVixen2xImportStateMachine
	{
		public void processEvent(Vixen2xSequenceImportSM parent, uint position, Color color, byte intensity)
		{
			// is the intensity and color the same as it was?
			if ((parent.CurrentColor == color) && (parent.CurrentIntensity > intensity) && (5 < intensity) )
			{
				// we are in a block of increasing intensity events
				parent.StateMachine = parent.RampDown;

				// update the current end position
				parent.EndEventPosition = position;
				parent.CurrentIntensity = intensity;
			}
			else
			{
				// something has changed. Write out the current data and start a new block
				closeChannel(parent);
				parent.StateMachine = parent.WaitingToStart;
				parent.processEvent(position, color, intensity);
			}
		} // processEvent

		public void closeChannel(Vixen2xSequenceImportSM parent)
		{
			// save the data
			EffectNode node = parent.GeneratePulseEffect(parent.StartingIntensity, parent.CurrentIntensity, parent.StartEventPosition, parent.EndEventPosition, parent.TargetNode, parent.CurrentColor);
			if (node != null)
			{
				parent.Sequence.InsertData(node);
			}

			// set back to the waiting for the channel to open
			parent.StateMachine = parent.IgnoreEvents;
		} // closeChannel
	} // SRampDown

	/// <summary>
	/// Handle the  ramp up sequence
	/// </summary>
	public class SRampUp : IVixen2xImportStateMachine
	{
		public void processEvent(Vixen2xSequenceImportSM parent, uint position, Color color, byte intensity)
		{
			// is the intensity and color the same as it was?
			if ((parent.CurrentColor == color) && (parent.CurrentIntensity < intensity))
			{
				// we are in a block of increasing intensity events
				parent.StateMachine = parent.RampUp;

				// update the current end position
				parent.EndEventPosition = position;
				parent.CurrentIntensity = intensity;
			}
			else
			{
				// something has changed. Write out the current data and start a new block
				closeChannel(parent);
				parent.StateMachine = parent.WaitingToStart;
				parent.processEvent(position, color, intensity);
			}
		} // processEvent

		public void closeChannel(Vixen2xSequenceImportSM parent)
		{
			// save the data
			EffectNode node = parent.GeneratePulseEffect(parent.StartingIntensity, parent.CurrentIntensity, parent.StartEventPosition, parent.EndEventPosition, parent.TargetNode, parent.CurrentColor);
			if (node != null)
			{
				parent.Sequence.InsertData(node);
			}

			// set back to the waiting for the channel to open
			parent.StateMachine = parent.IgnoreEvents;
		} // closeChannel
	} // SRampUp
} // VixenModules.SequenceType.Vixen2x

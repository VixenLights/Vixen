using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution.Context
{
	public class ShowContext : LiveContext
	{

		public string _info;
		SystemClock _startTime = new SystemClock();

		/// <summary>
		/// Initializes a new instance of the ShowContext class. 
		/// </summary>
		/// <param name="name">Specifies the name of the Show.</param>
		/// <param name="guid">Specifies the GUID of the Show.</param>
		public ShowContext(string name, string guid) : base(name)
		{
			Id = Guid.Parse(guid);
			_info = guid;
			_startTime.Start();
		}

		/// <summary>
		/// Initializes a new instance of the ShowContext class. 
		/// </summary>
		/// <param name="name">Specifies the name of the Show.</param>
		/// <param name="guid">Specifies the GUID of the Show.</param>
		public ShowContext(string name, Guid guid) : base(name)
		{
			Id = guid;
			_info = guid.ToString();
			_startTime.Start();
		}

		/// <summary>
		/// Returns the timing criteria, based upon the System Clock.
		/// </summary>
		protected override ITiming _SequenceTiming
		{
			get { return _startTime; }
		}
	}
}
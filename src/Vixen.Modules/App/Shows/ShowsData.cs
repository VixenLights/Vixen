using Common.Broadcast;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.Shows
{
	[DataContract]
	public class ShowsData : ModuleDataModelBase
	{
		// We make this static so it is always available
		// There can only be one instance of this module, anyway
		static protected List<Show> _shows;

		public ShowsData()
		{
		}

		static public List<Show> ShowList
		{
			get { return _shows; }
		}

		[DataMember]
		public List<Show> Shows 
		{ 
			get
			{
				return _shows != null ? _shows : _shows = new List<Show>();
			} 
			set
			{
				_shows = value;
			} 
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			Broadcast.Subscribe<string>(this, "RetrieveShowList", SendShowList);
		}

		static public Show GetShow(Guid id)
		{
			foreach (Show show in _shows)
			{
				if (show.ID.Equals(id))
				{
					return show;
				}
			}
			return null;
		}

		public Show AddShow()
		{
			Show show = new Show();
			show.Name = "New Show";
			Shows.Add(show);
			return show;
		}

		public override IModuleDataModel Clone()
		{
			ShowsData newData = (ShowsData)MemberwiseClone();
			//newData.ScheduledItems = ScheduledItems.ToList();
			return newData;
		}

		/// <summary>
		/// Create a listing of all the shows to transmit via the RESTful API 
		/// </summary>
		/// <param name="x"></param>
		private void SendShowList(string name)
		{
			List<Presentation> _webShowList = new List<Presentation>();

			foreach (Show show in ShowList)
			{
				Presentation showItem = new Presentation() {
					Name = show.Name,
					Info = show.ID.ToString()
				};

				_webShowList.Add(showItem);
			}

			Broadcast.Publish<List<Presentation>>("GetShowList", _webShowList);
		}
	}
}
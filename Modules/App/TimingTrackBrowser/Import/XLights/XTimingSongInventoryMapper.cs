using System;
using System.Collections.Generic;
using Catel.Logging;
using NLog;
using VixenModules.App.TimingTrackBrowser.Model.ExternalVendorInventory;
using VixenModules.App.TimingTrackBrowser.Model.InternalVendorInventory;
using Category = VixenModules.App.TimingTrackBrowser.Model.ExternalVendorInventory.Category;
using LogManager = NLog.LogManager;
using Song = VixenModules.App.TimingTrackBrowser.Model.InternalVendorInventory.Song;
using Vendor = VixenModules.App.TimingTrackBrowser.Model.ExternalVendorInventory.Vendor;

namespace VixenModules.App.TimingTrackBrowser.Import.XLights
{
	public class XTimingSongInventoryMapper
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		public SongInventory Map(MusicVendorInventory musicVendorInventory)
		{
			SongInventory mi = new SongInventory();

			Dictionary<string, List<Song>> songMap = MapSongs(musicVendorInventory.Songs);

			var cat = MapCategories(musicVendorInventory.Categories, songMap);
			mi.Vendor = MapVendor(musicVendorInventory.Vendor);
			mi.Inventory = cat;
			mi.DateTime = DateTime.Now;

			return mi;
		}

		private static Model.InternalVendorInventory.Vendor MapVendor(Vendor vendor)
		{
            Model.InternalVendorInventory.Vendor v = new Model.InternalVendorInventory.Vendor();
			v.Name = vendor.Name;

            if (!string.IsNullOrEmpty(vendor.Website))
            {
                v.Website = new Uri(vendor.Website);
            }
			
			if (!string.IsNullOrEmpty(vendor.LogoLink))
			{
				v.Logo = new Uri(vendor.LogoLink);
			}

			return v;
		}

		private static Dictionary<string, List<Song>> MapSongs(List<Model.ExternalVendorInventory.Song> songs)
		{
			Dictionary<string, List<Song>> songMap = new Dictionary<string, List<Song>>();

			foreach (var song in songs)
			{
                try
                {
                    var s = new Song
                    {
                        Artist = song.Artist.Trim(),
                        CategoryId = song.CategoryId.Trim(),
                        Creator = song.Creator.Trim(),
                        Title = song.Title.Trim()
                    };

                    if (!string.IsNullOrEmpty(song.Weblink))
                    {
                        if (Uri.TryCreate(song.Weblink, UriKind.Absolute, out var link))
                        {
                            s.SongLink = link;
                        }
                        else
                        {
							Logging.Info($"Song link for {s.Title} by {s.Artist} could not be parsed: {song.Weblink}");
                        }
                    }
                    if (!string.IsNullOrEmpty(song.Download))
                    {
						if (Uri.TryCreate(song.Download, UriKind.Absolute, out var link))
                        {
                            s.TimingLink = link;
                        }
                        else
                        {
                            Logging.Info($"Timing link for {s.Title} by {s.Artist} could not be parsed: {song.Download}");
                        }
					}

                    
                    if (songMap.TryGetValue(s.CategoryId, out var catProducts))
                    {
                        catProducts.Add(s);
                    }
                    else
                    {
                        songMap.Add(s.CategoryId, new List<Song>(new[] { s }));
                    }
                    
				}
                catch (Exception e)
                {
                    Logging.Error(e, "An error occured paring song.");
                }


			}

			return songMap;
		}

		private static List<Model.InternalVendorInventory.Category> MapCategories(List<Category> categories, Dictionary<string, List<Song>> songMap)
		{
			List<Model.InternalVendorInventory.Category> cList = new List<Model.InternalVendorInventory.Category>();
			foreach (var category in categories)
			{
				Model.InternalVendorInventory.Category c = new Model.InternalVendorInventory.Category();
				c.Name = category.Name;
				c.Id = category.Id;
				if (songMap.TryGetValue(c.Id, out var songs))
				{
					c.Songs = songs;
				}

				c.Categories = MapCategories(category.Categories, songMap);
				cList.Add(c);
			}

            foreach (Model.InternalVendorInventory.Category c in cList)
            {
                SortCategory(c);
            }

			return cList;
		}

        private static void SortCategory(Model.InternalVendorInventory.Category category)
        {
            category.Songs.Sort();
			foreach (Model.InternalVendorInventory.Category c in category.Categories)
            {
                SortCategory(c);
            }
		}
	}
}

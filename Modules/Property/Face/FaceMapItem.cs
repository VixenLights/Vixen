using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Sys;
using VixenModules.App.LipSyncApp;

namespace VixenModules.Property.Face
{
	public class FaceMapItem
	{
		public FaceMapItem()
		{
			PhonemeList = PhonemeList = new Dictionary<string, Boolean>();
		}

		public FaceMapItem(ElementNode node)
		{
			PhonemeList = PhonemeList = new Dictionary<String, Boolean>();
			Node = node;
			DefaultColor = Color.White;
		}

		public FaceMapItem Clone()
		{
			FaceMapItem retVal = new FaceMapItem();
			retVal.Node = Node;
			retVal.PhonemeList = PhonemeList = new Dictionary<String, Boolean>();
			retVal.DefaultColor = DefaultColor;
			retVal.FaceComponents = new Dictionary<FaceComponent, bool>(FaceComponents);
			return retVal;
		}


		public Dictionary<string, Boolean> PhonemeList{ get; set; }

		public Dictionary<FaceComponent, bool> FaceComponents { get; set; }

		public Color DefaultColor { get; set; }

		public Guid ElementGuid => Node.Id;

		public ElementNode Node { get; set; }

		public override string ToString()
		{
			return Node.Name;
		}

	}

   
}
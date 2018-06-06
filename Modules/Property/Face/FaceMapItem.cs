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
			PhonemeList = PhonemeList = new Dictionary<PhonemeType, Boolean>();
		}

		public FaceMapItem(ElementNode node)
		{
			PhonemeList = PhonemeList = new Dictionary<PhonemeType, Boolean>();
			Node = node;
			ElementColor = Color.White;
		}

		public FaceMapItem Clone()
		{
			FaceMapItem retVal = new FaceMapItem();
			retVal.Node = Node;
			retVal.PhonemeList = PhonemeList = new Dictionary<PhonemeType, Boolean>();
			retVal.ElementColor = ElementColor;
			retVal.FaceComponent = FaceComponent;
			return retVal;
		}


		public Dictionary<PhonemeType, Boolean> PhonemeList{ get; set; }

		public FaceComponent FaceComponent { get; set; }

		public Color ElementColor { get; set; }

		public Guid ElementGuid => Node.Id;

		public ElementNode Node { get; set; }

		public override string ToString()
		{
			return Node.Name;
		}

	}

   
}
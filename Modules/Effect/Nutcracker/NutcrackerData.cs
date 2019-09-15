using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.Nutcracker
{
	[DataContract]
	[KnownType(typeof (SerializableFont)),
	 KnownType(typeof (System.Drawing.FontStyle)),
	 KnownType(typeof (System.Drawing.GraphicsUnit))]
	public class NutcrackerData : ICloneable
	{
		public NutcrackerData()
		{
			Text_Font = new SerializableFont(new Font("Arial", 8));
		}

		public IElementNode[] TargetNodes;

		[DataMember] public NutcrackerEffects.PreviewType PreviewType = NutcrackerEffects.PreviewType.Tree180;

		[DataMember] public NutcrackerEffects.Effects CurrentEffect = NutcrackerEffects.Effects.Bars;
		[DataMember] public int Speed = 5;
		[DataMember] public int PixelSize = 2;
		[DataMember] public NutcrackerEffects.StringOrientations StringOrienation = NutcrackerEffects.StringOrientations.Vertical;

		[DataMember] public Palette Palette = new Palette();
		[DataMember] public bool FitToTime = false;

		// Bars
		[DataMember] public int Bars_PaletteRepeat = 1;
		[DataMember] public int Bars_Direction = 1;
		[DataMember] public bool Bars_Highlight = false;
		[DataMember] public bool Bars_3D = false;

		// Butterfly
		[DataMember] public int Butterfly_Colors = 1;
		[DataMember] public int Butterfly_Style = 1;
		[DataMember] public int Butterfly_BkgrdChunks = 1;
		[DataMember] public int Butterfly_BkgrdSkip = 2;
		[DataMember] public int Butterfly_Direction = 0;

		// ColorWash
		[DataMember] public int ColorWash_Count = 1;
		[DataMember] public bool ColorWash_FadeHorizontal = false;
		[DataMember] public bool ColorWash_FadeVertical = false;

		// Fire
		[DataMember] public int Fire_Height = 50;
		[DataMember] public int Fire_Hue = 0;

		// Garlands
		[DataMember] public int Garland_Type = 1;
		[DataMember] public int Garland_Spacing = 1;

		// Life
		[DataMember] public int Life_CellsToStart = 50;
		[DataMember] public int Life_Type = 1;

		// Meteors
		[DataMember] public int Meteor_Colors = 1;
		[DataMember] public int Meteor_Count = 10;
		[DataMember] public int Meteor_TrailLength = 8;

		// Fireworks
		[DataMember] public int Fireworks_Explosions = 5;
		[DataMember] public int Fireworks_Particles = 20;
		[DataMember] public int Fireworks_Velocity = 50;
		[DataMember] public int Fireworks_Fade = 50;

		// Snowflakes
		[DataMember] public int Snowflakes_Max;
		[DataMember] public int Snowflakes_Type;

		// Snowstorm
		[DataMember] public int Snowstorm_MaxFlakes = 10;
		[DataMember] public int Snowstorm_TrailLength = 10;

		// Spirals
		[DataMember] public int Spirals_PaletteRepeat = 1;
		[DataMember] public int Spirals_Direction = 1;
		[DataMember] public int Spirals_Rotation = 20;
		[DataMember] public int Spirals_Thickness = 60;
		[DataMember] public bool Spirals_Blend = false;
		[DataMember] public bool Spirals_3D = false;

		// Twinkles
		[DataMember] public int Twinkles_Count = 50;
		[DataMember] public int Twinkles_Steps = 30;
		[DataMember] public bool Twinkles_Strobe = false;

		// Text
		[DataMember] public int Text_Top = 5;
		[DataMember] public int Text_Left = 5;
		[DataMember] public string Text_Line1 = string.Empty;
		[DataMember] public string Text_Line2 = string.Empty;
		[DataMember] public string Text_Line3 = string.Empty;
		[DataMember] public string Text_Line4 = string.Empty;
		[DataMember] public int Text_Direction = 0;
		[DataMember] public bool Text_CenterStop = false;

		[DataMember(IsRequired = false)]
		public SerializableFont Text_Font { get; set; }

		[DataMember] public int Text_TextRotation = 0;

		// Picture
		[DataMember] public string Picture_FileName = string.Empty;
		[DataMember] public int Picture_Direction = 0;
		[DataMember] public int Picture_GifSpeed = 1;
		[DataMember] public bool Picture_ScaleToGrid = false;
		[DataMember] public int Picture_ScalePercent = 10;

		// Spirograph
		[DataMember] public int Spirograph_ROuter = 20;
		[DataMember] public int Spirograph_RInner = 10;
		[DataMember] public int Spirograph_Distance = 30;
		[DataMember] public bool Spirograph_Animate = false;

		// Tree
		[DataMember] public int Tree_Branches = 5;

		// Movie
		[DataMember] public string Movie_DataPath = string.Empty;
		[DataMember] public int Movie_PlaybackSpeed = 0; // -100=Slow, 0=Normal, 100=Fast
		[DataMember] public int Movie_MovementDirection = 0;

		// PictureTile
		[DataMember] public string PictureTile_FileName = string.Empty;
		[DataMember] public int PictureTile_Direction = 0;
		[DataMember] public double PictureTile_Scaling = 100.0;
		[DataMember] public bool PictureTile_ReplaceColor = false;
		[DataMember] public bool PictureTile_UseSaturation = false;
		[DataMember] public int PictureTile_ColorReplacementSensitivity = 0;

		//Curtain
		[DataMember] public int Curtain_Edge = 0;
		[DataMember] public int Curtain_Effect = 0;
		[DataMember] public int Curtain_SwagWidth = 0;
		[DataMember] public bool Curtain_Repeat = false;

		// Glediator
		[DataMember]
		public string Glediator_FileName;

		


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (PreviewType.ToString() == string.Empty)
				PreviewType = NutcrackerEffects.PreviewType.Tree180;

			if (Palette == null)
				Palette = new Palette();
			if (Fire_Height < 1)
				Fire_Height = 50;
			if (Fire_Hue < 0)
			{
				Fire_Hue = 0;
			}
			if (Meteor_Colors < 0)
				Meteor_Colors = 1;
			if (Meteor_Count < 1)
				Meteor_Count = 10;
			if (Meteor_TrailLength < 1)
				Meteor_TrailLength = 8;

			if (Spirals_PaletteRepeat == 0)
				Spirals_PaletteRepeat = 1;

			if (Twinkles_Count < 2)
				Twinkles_Count = 10;
			if (Twinkles_Steps < 2)
				Twinkles_Count = 30;

			if (Text_Line1 == null)
				Text_Line1 = string.Empty;
			if (Text_Line2 == null)
				Text_Line2 = string.Empty;
			if (Text_Line3 == null)
				Text_Line3 = string.Empty;
			if (Text_Line4 == null)
				Text_Line4 = string.Empty;

			if (Text_Font == null) {
				Text_Font = new SerializableFont(new Font("Arial", 8));
			}

			if (Picture_FileName == null)
				Picture_FileName = string.Empty;
			if (Picture_GifSpeed < 1)
				Picture_GifSpeed = 1;
			if (Picture_ScalePercent < 1)
			{
				Picture_ScalePercent = 10;
			}

			if (Spirograph_ROuter < 1)
				Spirograph_ROuter = 20;
			if (Spirograph_RInner < 1)
				Spirograph_RInner = 10;
			if (Spirograph_Distance < 1)
				Spirograph_Distance = 30;

			if (Tree_Branches == 0)
				Tree_Branches = 5;

			if (PixelSize == 0)
				PixelSize = 3;

			if (Movie_DataPath == null)
				Movie_DataPath = string.Empty;

			if (PictureTile_Scaling == 0.0) {
				PictureTile_Scaling = 100.0;
			}

			if (Curtain_SwagWidth < 1)
			{
				Curtain_SwagWidth = 3;
			}
		}

		public object Clone()
		{
			Object data = MemberwiseClone();
			var ndata = data as NutcrackerData;
			//Override the non value types
			ndata.Palette = new Palette{Colors = Palette.Colors.ToArray(), ColorsActive = Palette.ColorsActive.ToArray()};
			ndata.Text_Font = new SerializableFont(Text_Font.FontValue);
			return ndata;
		}
	}
}
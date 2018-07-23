using System.ComponentModel;

namespace VixenModules.Effect.Shapes
{
	public enum ShapeType
	{
		[Description("None")]
		None,
		[Description("Bounce")]
		Bounce,
		[Description("Wrap")]
		Wrap
	}

	public enum FadeType
	{
		[Description("None")]
		None,
		[Description("In")]
		In,
		[Description("Out")]
		Out,
		[Description("Random")]
		Random
	}
	public enum SizeMode
	{
		[Description("Shrink")]
		In,
		[Description("Grow")]
		Out,
		[Description("Random")]
		Random
	}

	public enum GeometricShapesList
	{
		[Description("Random")]
		Random,
		[Description("Arrow")]
		Arrow,
		[Description("Circle")]
		Circle,
		[Description("Cross")]
		Cross,
		[Description("Ellipse")]
		Ellipse,
		[Description("Polygon")]
		Polygon,
		[Description("Rectangle")]
		Rectangle,
		[Description("Square")]
		Square,
		[Description("Star")]
		Star,
		[Description("Star-Concave")]
		ConcaveStar,
		[Description("Star-NonIntersecting")]
		NonIntersectingStar,
		[Description("Star-North")]
		NorthStar,
		[Description("Triangle")]
		Triangle,
		[Description("Heart")]
		Heart
	}

	public enum ChristmasShapesList
	{
		[Description("Random")]
		Random,
		[Description("Bauble")]
		Bauble,
		[Description("Bauble2")]
		Bauble2,
		[Description("Bow")]
		Bow,
		[Description("Candy Cane")]
		CandyCane,
		[Description("Gingerbread Man")]
		GingerBreadMan,
		[Description("Gingerbread Man2")]
		GingerBreadMan2,
		[Description("Mistletoe")]
		Mistletoe,
		[Description("Present")]
		Present,
		[Description("Santa's Hat")] 
		SantasHat,
		[Description("Sleigh")]
		Sleigh,
		[Description("SnowFlake")]
		SnowFlake,
		[Description("SnowFlake2")]
		SnowFlake2,
		[Description("SnowMan")]
		SnowMan,
		[Description("SnowMan2")]
		SnowMan2,
		[Description("Stocking")]
		Stocking,
		[Description("Tree")]
		Tree,
		[Description("Tree2")]
		Tree2,
		[Description("Reindeer")]
		Reindeer,
		[Description("Wreath")]
		Wreath
	}

	public enum HalloweenShapesList
	{
		[Description("Random")]
		Random,
		[Description("Coffin")]
		Coffin,
		[Description("Cross")]
		Cross,
		[Description("Cross2")]
		Cross2,
		[Description("Ghost")]
		Ghost,
		[Description("Ghost2")]
		Ghost2,
		[Description("Hand")]
		Hand,
		[Description("Knife")]
		Knife,
		[Description("Skull")]
		Skull,
		[Description("Haunted House")]
		HauntedHouse,
		[Description("Pumpkin")]
		Pumpkin,
		[Description("Skull & Bone")] 
		SkullandBone,
		[Description("Skull & Bone1")]
		SkullandBone1,
		[Description("Spider")]
		Spider,
		[Description("Tombstone")]
		Tombstone,
		[Description("Tombstone2")]
		Tombstone2,
		[Description("Web")]
		Web
	}

	public enum ShapeList
	{
		[Description("Geometric")]
		GeometricShapes,
		[Description("Christmas")]
		ChristmasShapes,
		[Description("Halloween")]
		HalloweenShapes,
		[Description("File")]
		File
	}
}
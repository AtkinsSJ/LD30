using UnityEngine;

public enum GoodType {
	BalloonAnimal,
	Gorilla,
	Hotdog,
	Kryptonite,
	Sock,
	Toast,
	VacuumCleaner,
	Wheel,
	SIZE
}

public struct Good {
	public static Good[] GOODS = {
		new Good(GoodType.BalloonAnimal,	"Balloon Animal",		"Balloon Animals",		 20),
		new Good(GoodType.Gorilla,			"Gorilla",				"Gorillas",				 30),
		new Good(GoodType.Hotdog,			"Hotdog",				"Hotdogs",				  8),
		new Good(GoodType.Kryptonite,		"Chunk of Kryptonite",	"Chunks of Kryptonite",	100),
		new Good(GoodType.Sock,				"Sock",					"Socks",				  5),
		new Good(GoodType.Toast,			"Slice of Toast",		"Slices of Toast",		 10),
		new Good(GoodType.VacuumCleaner,	"Vacuum Cleaner",		"Vacuum Cleaners",		 70),
		new Good(GoodType.Wheel,			"Wheel",				"Wheels",				 50),
	};

	public readonly GoodType type;
	public readonly string name;
	public readonly string pluralName;
	private int averagePrice;

	Good(GoodType myType, string myName, string myPluralName, int myAveragePrice) {
		type = myType;
		name = myName;
		pluralName = myPluralName;
		averagePrice = myAveragePrice;
	}

	public int GenerateBuyPrice() {
		return (int)Random.Range(averagePrice / 2, averagePrice * 1.2f);
	}

	public int GenerateSellPrice() {
		return (int)Random.Range(averagePrice * 0.8f, averagePrice * 2);
	}
}

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
		new Good(GoodType.BalloonAnimal, "Balloon Animal", "Balloon Animals"),
		new Good(GoodType.Gorilla, "Gorilla", "Gorillas"),
		new Good(GoodType.Hotdog, "Hotdog", "Hotdogs"),
		new Good(GoodType.Kryptonite, "Chunk of Kryptonite", "Chunks of Kryptonite"),
		new Good(GoodType.Sock, "Sock", "Socks"),
		new Good(GoodType.Toast, "Slice of Toast", "Slices of Toast"),
		new Good(GoodType.VacuumCleaner, "Vacuum Cleaner", "Vacuum Cleaners"),
		new Good(GoodType.Wheel, "Wheel", "Wheels"),
	};

	public readonly GoodType type;
	public readonly string name;
	public readonly string pluralName;

	Good(GoodType myType, string myName, string myPluralName) {
		type = myType;
		name = myName;
		pluralName = myPluralName;
	}
}

[System.Flags]
public enum TileTargetType {
	None = 0,
	Ally = 1,
	Enemy = 2,
	Unit = Ally | Enemy,
	Ground = 4,
	Wall = 8,
	Any = Ground | Wall,
}

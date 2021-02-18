
using Muc.Numerics;

public enum TileDir {
	Right,
	DownRight,
	DownLeft,
	Left,
	UpLeft,
	UpRight,
}

public static class TileDirExtensions {
	public static TileDir Invert(this TileDir dir) {
		return dir.Offset(3);
	}

	public static TileDir Offset(this TileDir dir, int offset) {
		return (TileDir)(int)new CircularInt((int)dir + offset, 6);
	}
}
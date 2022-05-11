
[System.Flags]
public enum UnitTargetType {
	None = 0,
	Self = 1 << 1,
	Ally = 1 << 2,
	Enemy = 1 << 3,
}

public static class UnitTargetTypeExtension {

	public static bool TargetIsCompatible(this UnitTargetType tt, Unit unit, Unit target) {

		if ((tt & UnitTargetType.Self) != 0 && unit == target) return true;
		if ((tt & UnitTargetType.Ally) != 0 && unit != target && unit.team == target.team) return true;
		if ((tt & UnitTargetType.Enemy) != 0 && unit.team != target.team) return true;
		return false;
	}

}
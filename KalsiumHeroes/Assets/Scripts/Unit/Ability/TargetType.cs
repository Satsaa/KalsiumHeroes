
[System.Flags]
public enum TargetType {
  None = 0,
  Ground = 1 << 0,
  Self = 1 << 1,
  Ally = 1 << 2,
  Enemy = 1 << 3,
  Neutral = 1 << 4,
}


[System.Flags]
public enum EffectShape {
  None = 0,
  Target = 1 << 0,
  Radius = 1 << 1,
  Line = 1 << 2,
  Cone = 1 << 3,
}

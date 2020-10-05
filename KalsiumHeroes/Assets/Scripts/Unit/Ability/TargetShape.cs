
[System.Flags]
public enum TargetShape {
  None = 0,
  Target = 1 << 0,
  Area = 1 << 1,
  Line = 1 << 2,
  Cone = 1 << 3,
}

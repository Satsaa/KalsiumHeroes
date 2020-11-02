
using System.Collections.Generic;
using UnityEngine;

public class ArrayTest : MonoBehaviour {

  public int @int;
  public Structy structy;
  public Classy classy;
  public int[] ints;
  public uint[] uints;
  public ulong[] ulongs;
  public long[] longs;
  public sbyte[] sbytes;
  public decimal[] decimals;
  public float[] floats;
  public ScriptableObject[] sos;
  public List<ScriptableObject> sosList;
  public List<Structy> structies;
  public List<Classy> classies;

}

[System.Serializable]
public struct Structy {
  public int foo;
  public List<int> bars;
}

[System.Serializable]
public struct Classy {
  public int foo;
  public List<int> bars;
}
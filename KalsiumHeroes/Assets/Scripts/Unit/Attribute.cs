
using UnityEngine;

public class Attribute<T> {

  public T baseValue;
  public T value;

  public Attribute() { }

  public Attribute(T baseAndValue) {
    this.baseValue = baseAndValue = value;
  }
  public Attribute(T baseValue, T value) {
    this.baseValue = baseValue;
    this.value = value;
  }
}
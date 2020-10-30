
using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class AttributeLabelsAttribute : PropertyAttribute {

  public readonly string primaryLabel;
  public readonly string secondaryLabel;

  public AttributeLabelsAttribute(string primaryLabel, string secondaryLabel) {
    this.primaryLabel = primaryLabel;
    this.secondaryLabel = secondaryLabel;
  }
}
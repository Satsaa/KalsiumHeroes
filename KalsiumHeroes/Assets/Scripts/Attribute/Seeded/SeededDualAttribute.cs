
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using System.Security;

[Serializable]
public class SeededDualAttribute<T> : SeededAttribute<T> {

	[SerializeField]
	[FormerlySerializedAs(nameof(other))]
	[Tooltip("Secondary value")]
	protected T _other;
	public virtual T other => otherAlterers.Aggregate(_other, (current, alt) => alt(current));

	protected HashSet<Func<T, T>> otherAlterers = new HashSet<Func<T, T>>();


	public SeededDualAttribute() { }
	public SeededDualAttribute(T value, T other) : base(value) {
		this._other = other;
	}


	/// <summary> Adds or removes a function that alters what the value property returns. </summary>
	public bool ConfigureOtherAlterer(bool add, Func<T, T> alterer) {
		if (add) return otherAlterers.Add(alterer);
		else return otherAlterers.Remove(alterer);
	}

	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Primary: return !_value.Equals(value);
			case AttributeProperty.Secondary: return !_other.Equals(other);
			case AttributeProperty.Enabled: return false;
			default: return false;
		}
	}

	public override string GetEditorLabel(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Primary: return "Value";
			case AttributeProperty.Secondary: return "Other";
			case AttributeProperty.Enabled: return "";
			default: return "Unknown";
		}
	}

}

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using System.Security;

[Serializable]
public class DualAttribute<T> : Attribute<T> {

	[SerializeField]
	[Tooltip("Secondary value")]
	protected T _other;

	public virtual T other {
		get => otherAlterers.Aggregate(_other, (current, alt) => alt(current));
		set => _other = value;
	}

	protected HashSet<Func<T, T>> otherAlterers = new HashSet<Func<T, T>>();


	public DualAttribute() { }
	public DualAttribute(T value, T other) : base(value) {
		_other = other;
	}


	/// <summary> Sets value to other. </summary>
	public void ResetValue() => value = other;

	/// <summary> If value is larger than other, sets value to other. T must be IComparable. </summary>
	public void LimitValue() {
		if (((IComparable)value).CompareTo((IComparable)other) > 0) value = other;
	}

	/// <summary> Clamps value between min and other. T must be IComparable. </summary>
	public void ClampValue(T min = default(T)) {
		if (((IComparable)value).CompareTo((IComparable)min) < 0) {
			value = min;
		} else {
			LimitValue();
		}
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
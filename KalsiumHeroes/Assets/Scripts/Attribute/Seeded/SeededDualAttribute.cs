﻿
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
	public virtual T other => otherAlterers.Values.Aggregate(_other, (current, alt) => alt(current));

	protected Dictionary<object, Func<T, T>> otherAlterers = new Dictionary<object, Func<T, T>>();


	public SeededDualAttribute() { }
	public SeededDualAttribute(T value, T other) : base(value) {
		this._other = other;
	}


	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Enabled: return false;
			case AttributeProperty.Primary: return !_value.Equals(value);
			case AttributeProperty.Secondary: return !_other.Equals(other);
			default: return false;
		}
	}

	public override string Editor_DefaultLabel(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Enabled: return "";
			case AttributeProperty.Primary: return "Value";
			case AttributeProperty.Secondary: return "Other";
			default: return "Unknown";
		}
	}
	public override bool Editor_OnlyShowAlteredInPlay(AttributeProperty attributeProperty) => true;

	/// <summary> Registers a function that alters what the other property returns. </summary>
	public void RegisterSecondaryAlterer(Func<T, T> alterer) {
		if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
		var keyObject = new object();
		otherAlterers.Add(keyObject, alterer);
		keyTarget.Add(keyObject, this);
	}

	/// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
	public override void RemoveAlterer(object key) {
		if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
		alterers.Remove(key);
		otherAlterers.Remove(key);
	}

}
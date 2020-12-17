
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using System.Security;

[Serializable]
public class Attribute<T> : AttributeBase {

	[SerializeField]
	[FormerlySerializedAs(nameof(value))]
	[Tooltip("Primary value")]
	protected T _value;

	public virtual T value {
		get => alterers.Aggregate(_value, (current, alt) => alt(current));
		set => _value = value;
	}

	protected HashSet<Func<T, T>> alterers = new HashSet<Func<T, T>>();


	public Attribute() { }
	public Attribute(T value) {
		this._value = value;
	}


	/// <summary> Adds or removes a function that alters what the value property returns. </summary>
	public bool ConfigureAlterer(bool add, Func<T, T> alterer) {
		if (add) return alterers.Add(alterer);
		else return alterers.Remove(alterer);
	}

	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Primary: return !_value.Equals(value);
			case AttributeProperty.Secondary: return false;
			case AttributeProperty.Enabled: return false;
			default: return false;
		}
	}

	public override string GetEditorLabel(AttributeProperty attributeProperty) => "";

}

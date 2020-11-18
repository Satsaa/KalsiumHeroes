
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
		get => alterers.Values.Aggregate(_value, (current, alt) => alt(current));
		set => _value = value;
	}

	protected Dictionary<object, Func<T, T>> alterers = new Dictionary<object, Func<T, T>>();

	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Enabled: return false;
			case AttributeProperty.Primary: return !_value.Equals(value);
			case AttributeProperty.Secondary: return false;
			default: return false;
		}
	}

	public override string Editor_DefaultLabel(AttributeProperty attributeProperty) => "";

	public Attribute() { }
	public Attribute(T value) {
		this._value = value;
	}


	/// <summary> Registers a function that alters what the value property returns. </summary>
	public void RegisterAlterer(Func<T, T> alterer) {
		if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
		var keyObject = new object();
		alterers.Add(keyObject, alterer);
		keyTarget.Add(keyObject, this);
	}

	/// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
	public override void RemoveAlterer(object key) {
		if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
		alterers.Remove(key);
	}

}

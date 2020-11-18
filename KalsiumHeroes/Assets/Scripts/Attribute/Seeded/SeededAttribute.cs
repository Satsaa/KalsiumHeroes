
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using System.Security;


[Serializable]
public class SeededAttribute<T> : AttributeBase {

	[SerializeField]
	[FormerlySerializedAs(nameof(value))]
	[Tooltip("Seed value")]
	protected T _value;
	public virtual T value => alterers.Values.Aggregate(_value, (current, alt) => alt(current));

	protected Dictionary<object, Func<T, T>> alterers = new Dictionary<object, Func<T, T>>();


	public SeededAttribute() { }
	public SeededAttribute(T value) {
		_value = value;
	}


	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Enabled: return false;
			case AttributeProperty.Primary: return !_value.Equals(value);
			case AttributeProperty.Secondary: return false;
			default: return false;
		}
	}

	public override string Editor_DefaultLabel(AttributeProperty attributeProperty) => "";

	public override bool Editor_OnlyShowAlteredInPlay(AttributeProperty attributeProperty) => true;

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
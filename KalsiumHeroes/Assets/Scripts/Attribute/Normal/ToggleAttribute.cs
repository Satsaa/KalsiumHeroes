
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using System.Security;

[Serializable]
public class ToggleAttribute<T> : Attribute<T> {

	[SerializeField]
	[FormerlySerializedAs(nameof(enabled))]
	[Tooltip("Attribute is enabled?")]
	private bool _enabled;

	public virtual bool enabled {
		get => enabledAlterers.Values.Aggregate(_enabled, (current, alt) => alt(current));
		set => _enabled = value;
	}

	protected Dictionary<object, Func<bool, bool>> enabledAlterers = new Dictionary<object, Func<bool, bool>>();


	public ToggleAttribute(bool enabled = true) {
		_enabled = enabled;
	}
	public ToggleAttribute(T value, bool enabled = true) : base(value) {
		_enabled = enabled;
	}


	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Enabled: return !_enabled.Equals(enabled);
			case AttributeProperty.Primary: return !_value.Equals(value);
			case AttributeProperty.Secondary: return false;
			default: return false;
		}
	}

	public override string Editor_DefaultLabel(AttributeProperty attributeProperty) => "";

	internal void RegisterEnabledAlterer(Func<bool, bool> alterer) {
		if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
		var keyObject = new object();
		enabledAlterers.Add(keyObject, alterer);
		keyTarget.Add(keyObject, this);
	}

	/// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
	public override void RemoveAlterer(object key) {
		if (!allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
		alterers.Remove(key);
		enabledAlterers.Remove(key);
	}
}
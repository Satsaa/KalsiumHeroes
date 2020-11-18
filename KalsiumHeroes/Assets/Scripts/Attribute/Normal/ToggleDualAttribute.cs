
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using System.Security;

[Serializable]
public class ToggleDualAttribute<T> : DualAttribute<T> {

	[SerializeField]
	[FormerlySerializedAs(nameof(enabled))]
	[Tooltip("Attribute is enabled?")]
	private bool _enabled;

	public virtual bool enabled {
		get => enabledAlterers.Values.Aggregate(_enabled, (current, alt) => alt(current));
		set => _enabled = value;
	}


	protected Dictionary<object, Func<bool, bool>> enabledAlterers = new Dictionary<object, Func<bool, bool>>();

	public ToggleDualAttribute(bool enabled = true) {
		_enabled = enabled;
	}
	public ToggleDualAttribute(T value, T other, bool enabled = true) : base(value, other) {
		_enabled = enabled;
	}


	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Enabled: return !_enabled.Equals(enabled);
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

	internal void RegisterEnabledAlterer(Func<bool, bool> alterer) {
		if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
		var keyObject = new object();
		enabledAlterers.Add(keyObject, alterer);
		AttributeBase.keyTarget.Add(keyObject, this);
	}

	/// <summary> Internal use only. Attribute alterers are removed automatically. </summary>
	public override void RemoveAlterer(object key) {
		if (!AttributeBase.allow) throw new SecurityException("Configuring alterers is only allowed inside the RegisterAttributeAlterers function!");
		alterers.Remove(key);
		otherAlterers.Remove(key);
		enabledAlterers.Remove(key);
	}
}
﻿
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System;
using System.Security;

[Serializable]
public class SeededToggleAttribute<T> : SeededAttribute<T> {

	[SerializeField]
	[FormerlySerializedAs(nameof(enabled))]
	[Tooltip("Attribute is enabled?")]
	private bool _enabled;
	public virtual bool enabled => enabledAlterers.Aggregate(_enabled, (current, alt) => alt(current));

	protected HashSet<Func<bool, bool>> enabledAlterers = new HashSet<Func<bool, bool>>();


	public SeededToggleAttribute(bool enabled = true) {
		_enabled = enabled;
	}
	public SeededToggleAttribute(T value, bool enabled = true) : base(value) {
		_enabled = enabled;
	}


	/// <summary> Adds or removes a function that alters what the value property returns. </summary>
	public bool ConfigureEnabledAlterer(bool add, Func<bool, bool> alterer) {
		if (add) return enabledAlterers.Add(alterer);
		else return enabledAlterers.Remove(alterer);
	}

	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Primary: return !_value.Equals(value);
			case AttributeProperty.Secondary: return false;
			case AttributeProperty.Enabled: return !_enabled.Equals(enabled);
			default: return false;
		}
	}

	public override string GetEditorLabel(AttributeProperty attributeProperty) => "";

}
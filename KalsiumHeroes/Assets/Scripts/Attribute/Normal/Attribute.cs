
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Attribute<T> : AttributeBase {

	[field: SerializeField, Tooltip("Primary value"), UnityEngine.Serialization.FormerlySerializedAs("_value")]
	public T rawValue { get; protected set; }

	public Muc.Data.Event onValueChanged = new();

	private List<Alterer<T>> valueAlterers = new();
	private bool valueCached;
	private T cachedValue;

	public T value {
		get => valueCached ? cachedValue : cachedValue = valueAlterers.Aggregate(rawValue, (v, alt) => alt.Apply(v));
		set { rawValue = value; valueCached = false; onValueChanged?.Invoke(); }
	}


	public Attribute(T value = default) {
		rawValue = value;
	}


	/// <summary> Adds or removes Alterers. When add is false all Alterers are removed by the creator regardless of the alterers argument.</summary>
	public Alterer<T, T2> ConfigureValueAlterer<T2>(bool add, Object creator, Func<T, T2, T> applier, Func<T2> updater, IEnumerable<Muc.Data.Event> updateEvents = null) {
		Alterer<T, T2> res = null;
		if (add) {
			res = new(creator, updater, applier, () => onValueChanged?.Invoke());
			valueAlterers.Add(res);
			if (updateEvents != null) {
				foreach (var upEvent in updateEvents) {
					upEvent.ConfigureListener(add, res.Update);
				}
			}
		} else {
			if (updateEvents != null) {
				foreach (var alt in valueAlterers.Where(v => v.creator == creator)) {
					foreach (var upEvent in updateEvents) {
						upEvent.ConfigureListener(add, alt.Update);
					}
				}
			}
			valueAlterers.RemoveAll(v => v.creator == creator);
		}
		valueCached = false;
		onValueChanged?.Invoke();
		return res;
	}

	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Primary: return !rawValue.Equals(value);
			case AttributeProperty.Secondary: return false;
			case AttributeProperty.Enabled: return false;
			default: return false;
		}
	}

	public override string GetEditorLabel(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Primary: return "Value";
			case AttributeProperty.Secondary: return "Other";
			case AttributeProperty.Enabled: return "Enabled";
			default: return "Unknown";
		}
	}

}

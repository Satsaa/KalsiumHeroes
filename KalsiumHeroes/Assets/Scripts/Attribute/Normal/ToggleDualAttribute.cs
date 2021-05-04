
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ToggleDualAttribute<T> : DualAttribute<T> {

	[field: SerializeField, Tooltip("Attribute is enabled?"), UnityEngine.Serialization.FormerlySerializedAs("_enabled")]
	public bool rawEnabled { get; protected set; }

	public Muc.Data.Event onEnabledChanged = new();

	[SerializeField]
	private List<Alterer<bool>> enabledAlterers = new();
	private bool enabledCached;
	private bool cachedEnabled;

	public bool enabled {
		get => enabledCached ? cachedEnabled : cachedEnabled = enabledAlterers.Aggregate(rawEnabled, (v, alt) => alt.Apply(v));
		set { rawEnabled = value; enabledCached = false; onEnabledChanged?.Invoke(); }
	}


	public ToggleDualAttribute(T value = default, T other = default, bool enabled = true) : base(value, other) {
		rawEnabled = enabled;
	}


	/// <summary> Adds or removes Alterers. When add is false all Alterers are removed by the creator regardless of the alterers argument.</summary>
	public Alterer<bool, T2> ConfigureEnabledAlterer<T2>(bool add, Object creator, Func<bool, T2, bool> applier, Func<T2> updater, params Muc.Data.Event[] updateEvents) {
		Alterer<bool, T2> res = null;
		if (add) {
			res = new(creator, updater, applier, () => onEnabledChanged?.Invoke());
			enabledAlterers.Add(res);
			if (updateEvents != null) {
				foreach (var upEvent in updateEvents) {
					upEvent.ConfigureListener(add, res.Update);
				}
			}
		} else {
			if (updateEvents != null) {
				foreach (var alt in enabledAlterers.Where(v => v.creator == creator)) {
					foreach (var upEvent in updateEvents) {
						upEvent.ConfigureListener(add, alt.Update);
					}
				}
			}
			enabledAlterers.RemoveAll(v => v.creator == creator);
		}
		enabledCached = false;
		onEnabledChanged?.Invoke();
		return res;
	}

	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Primary: return !rawValue.Equals(value);
			case AttributeProperty.Secondary: return false;
			case AttributeProperty.Enabled: return !rawEnabled.Equals(enabled);
			default: return false;
		}
	}

}
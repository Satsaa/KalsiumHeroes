
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ToggleDualAttribute<T> : DualAttribute<T> {

	public ToggleDualAttribute(T value = default, T other = default, bool enabled = true) : base(value, other) { rawEnabled = enabled; }
	public ToggleDualAttribute(bool enabled = true) : base(default, default) { rawEnabled = enabled; }

	[field: SerializeField, Tooltip("Attribute is enabled?"), UnityEngine.Serialization.FormerlySerializedAs("_enabled")]
	public bool rawEnabled { get; private set; } = true;

	private Muc.Data.Event _onEnabledChanged;
	public Muc.Data.Event onEnabledChanged => _onEnabledChanged ??= new();

	private List<Alterer<bool>> _enabledAlterers;
	private List<Alterer<bool>> enabledAlterers => _enabledAlterers ??= new(); // !! maybe can remove _enabledAlterers ??=

	private bool enabledCached;
	private bool cachedEnabled;

	[field: SerializeField] public bool enabledHasChanged { get; private set; }
	[field: SerializeField] private bool _startEnabled;
	public bool startEnabled => enabledHasChanged ? _startEnabled : _startEnabled = rawEnabled;


	public virtual bool enabled {
		get {
			UpdateEnabled();
			return cachedEnabled;
		}
		set {
			if (rawEnabled.Equals(value)) return;
			if (!enabledHasChanged) {
				_startEnabled = rawEnabled;
				enabledHasChanged = true;
			}
			rawEnabled = value;
			UpdateEnabled(true);
		}
	}

	private void UpdateEnabled(bool force = false) {
		if (!force && enabledCached)
#if UNITY_EDITOR
			if (Application.isPlaying)
#endif
				return;
		var prev = cachedEnabled;
		cachedEnabled = enabledAlterers.Aggregate(rawEnabled, (v, alt) => alt.Apply(v));
		enabledCached = true;
		if (!prev.Equals(cachedEnabled)) {
			if (!enabledHasChanged) {
				_startEnabled = rawEnabled;
				enabledHasChanged = true;
			}
			onEnabledChanged?.Invoke();
			onChanged?.Invoke();
		}
	}


	/// <summary> Adds or removes Alterers. When add is false all Alterers are removed by the creator regardless of the alterers argument.</summary>
	public Alterer<bool, T2> ConfigureEnabledAlterer<T2>(bool add, Object creator, Func<bool, T2, bool> applier, Func<T2> updater, params Muc.Data.Event[] updateEvents) {
		return ConfigureEnabledAlterer(add, creator, applier, updater, updateEvents as IEnumerable<Muc.Data.Event>);
	}

	/// <summary> Adds or removes Alterers. When add is false all Alterers are removed by the creator regardless of the other arguments.</summary>
	public Alterer<bool, T2> ConfigureEnabledAlterer<T2>(bool add, Object creator, Func<bool, T2, bool> applier, Func<T2> updater, IEnumerable<Muc.Data.Event> updateEvents = null) {
		Alterer<bool, T2> res = null;
		if (add) {
			res = new(creator, updater, applier, () => UpdateEnabled(true));
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
			UpdateEnabled(true);
		}
		return res;
	}


	public sealed override bool HasEnabled() => true;

	public sealed override bool GetEnabled() => enabled;

	public sealed override bool GetRawEnabled() => rawEnabled;

}
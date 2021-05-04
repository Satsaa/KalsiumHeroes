
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class DualAttribute<T> : Attribute<T> {

	[field: SerializeField, Tooltip("Secondary value"), UnityEngine.Serialization.FormerlySerializedAs("_other")]
	public T rawOther { get; protected set; }

	public Muc.Data.Event onOtherChanged = new();

	private List<Alterer<T>> otherAlterers = new();
	private bool otherCached;
	private T cachedOther;

	public T other {
		get => otherCached ? cachedOther : cachedOther = otherAlterers.Aggregate(rawOther, (v, alt) => alt.Apply(v));
		set { rawOther = value; otherCached = false; onOtherChanged?.Invoke(); }
	}


	public DualAttribute(T value = default, T other = default) : base(value) {
		rawOther = other;
	}


	/// <summary> Adds or removes Alterers. When add is false all Alterers are removed by the creator regardless of the alterers argument.</summary>
	public Alterer<T, T2> ConfigureOtherAlterer<T2>(bool add, Object creator, Func<T, T2, T> applier, Func<T2> updater, params Muc.Data.Event[] updateEvents) {
		Alterer<T, T2> res = null;
		if (add) {
			res = new(creator, updater, applier, () => onOtherChanged?.Invoke());
			otherAlterers.Add(res);
			if (updateEvents != null) {
				foreach (var upEvent in updateEvents) {
					upEvent.ConfigureListener(add, res.Update);
				}
			}
		} else {
			if (updateEvents != null) {
				foreach (var alt in otherAlterers.Where(v => v.creator == creator)) {
					foreach (var upEvent in updateEvents) {
						upEvent.ConfigureListener(add, alt.Update);
					}
				}
			}
			otherAlterers.RemoveAll(v => v.creator == creator);
		}
		otherCached = false;
		onOtherChanged?.Invoke();
		return res;
	}

	public override bool HasAlteredValue(AttributeProperty attributeProperty) {
		switch (attributeProperty) {
			case AttributeProperty.Primary: return !rawValue.Equals(value);
			case AttributeProperty.Secondary: return !rawOther.Equals(other);
			case AttributeProperty.Enabled: return false;
			default: return false;
		}
	}


	/// <summary> Sets value to other. </summary>
	public void ResetValue() => value = other;

	/// <summary> If value is larger than other, sets value to other. T must be IComparable. </summary>
	public void LimitValue() {
		if (((IComparable)value).CompareTo((IComparable)other) > 0) value = other;
	}

	/// <summary> Clamps value between min and other. T must be IComparable. </summary>
	public void ClampValue(T min = default(T)) {
		if (((IComparable)value).CompareTo((IComparable)min) < 0) {
			value = min;
		} else {
			LimitValue();
		}
	}

}
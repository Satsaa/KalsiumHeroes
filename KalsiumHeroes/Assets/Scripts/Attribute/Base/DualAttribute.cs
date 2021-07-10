
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class DualAttribute<T> : Attribute<T> {

	public DualAttribute(T value = default, T other = default) : base(value) { rawOther = other; }

	[field: SerializeField, Tooltip("Secondary value"), UnityEngine.Serialization.FormerlySerializedAs("_other")]
	public T rawOther { get; private set; }

	private Muc.Data.Event _onOtherChanged;
	public Muc.Data.Event onOtherChanged => _onOtherChanged ??= new();

	private List<Alterer<T>> _otherAlterers;
	private List<Alterer<T>> otherAlterers => _otherAlterers ??= new(); // !! maybe can remove _otherAlterers ??=

	private bool otherCached;
	private T cachedOther;

	[field: SerializeField] public bool otherHasChanged { get; private set; }
	[field: SerializeField] private T _startOther;
	public T startOther => otherHasChanged ? _startOther : _startOther = rawOther;


	public virtual T other {
		get {
			UpdateOther();
			return cachedOther;
		}
		set {
			if (rawOther.Equals(value)) return;
			if (!otherHasChanged) {
				_startOther = rawOther;
				otherHasChanged = true;
			}
			rawOther = value;
			UpdateOther(true);
		}
	}

	private void UpdateOther(bool force = false) {
		if (!force && otherCached)
#if UNITY_EDITOR
			if (Application.isPlaying)
#endif
				return;
		var prev = cachedOther;
		cachedOther = otherAlterers.Aggregate(rawOther, (v, alt) => alt.Apply(v));
		otherCached = true;
		if (!prev.Equals(cachedOther)) {
			if (!otherHasChanged) {
				_startOther = rawOther;
				otherHasChanged = true;
			}
			onOtherChanged?.Invoke();
			onChanged?.Invoke();
		}
	}


	/// <summary> Adds or removes Alterers. When add is false all Alterers are removed by the creator regardless of the alterers argument.</summary>
	public Alterer<T, T2> ConfigureOtherAlterer<T2>(bool add, Object creator, Func<T, T2, T> applier, Func<T2> updater, params Muc.Data.Event[] updateEvents) {
		return ConfigureOtherAlterer(add, creator, applier, updater, updateEvents as IEnumerable<Muc.Data.Event>);
	}

	/// <summary> Adds or removes Alterers. When add is false all Alterers are removed by the creator regardless of the other arguments.</summary>
	public Alterer<T, T2> ConfigureOtherAlterer<T2>(bool add, Object creator, Func<T, T2, T> applier, Func<T2> updater, IEnumerable<Muc.Data.Event> updateEvents = null) {
		Alterer<T, T2> res = null;
		if (add) {
			res = new(creator, updater, applier, () => UpdateOther(true));
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
			UpdateOther(true);
		}
		return res;
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


	public override int valueCount => 2;

	public override IEnumerable<T> GetValues() {
		yield return value;
		yield return other;
	}

	public override T GetValue(int valueIndex) {
		if (valueIndex >= valueCount) throw new IndexOutOfRangeException(nameof(valueIndex));
		return valueIndex == 0 ? value : other;
	}

	public sealed override bool HasOther() => true;

	public sealed override T GetOther() => other;

	public sealed override T GetRawOther() => rawOther;

	public override string Format(bool isSource) => String.Format(Lang.GetStr("Format_Fraction", "{0}/{1}"), value, other);

}
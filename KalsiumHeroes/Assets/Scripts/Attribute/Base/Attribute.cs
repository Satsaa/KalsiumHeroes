
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Attribute<T> : Attribute {

	public Attribute(T value = default) { rawValue = value; }
	protected Attribute() { }

	[field: SerializeField, Tooltip("Primary value"), UnityEngine.Serialization.FormerlySerializedAs("_value")]
	public T rawValue { get; private set; }

	private Muc.Data.Event _onValueChanged;
	public Muc.Data.Event onValueChanged => _onValueChanged ??= new();

	private List<Alterer<T>> _valueAlterers;
	private List<Alterer<T>> valueAlterers => _valueAlterers ??= new(); // !! maybe can remove _valueAlterers ??=

	private bool valueCached;
	private T cachedValue;

	[field: SerializeField] public bool valueHasChanged { get; private set; }
	[field: SerializeField] private T _startValue;
	public T startValue => valueHasChanged ? _startValue : _startValue = rawValue;


	public virtual T value {
		get {
			UpdateValue();
			return cachedValue;
		}
		set {
			if (rawValue.Equals(value)) return;
			if (!valueHasChanged) {
				_startValue = rawValue;
				valueHasChanged = true;
			}
			rawValue = value;
			UpdateValue(true);
		}
	}

	private void UpdateValue(bool force = false) {
		if (!force && valueCached)
#if UNITY_EDITOR
			if (Application.isPlaying)
#endif
				return;
		var prev = cachedValue;
		cachedValue = valueAlterers.Aggregate(rawValue, (v, alt) => alt.Apply(v));
		valueCached = true;
		if (!prev.Equals(cachedValue)) {
			if (!valueHasChanged) {
				_startValue = rawValue;
				valueHasChanged = true;
			}
			onValueChanged?.Invoke();
			onChanged?.Invoke();
		}
	}


	/// <summary> Adds or removes Alterers. When add is false all Alterers are removed by the creator regardless of the alterers argument.</summary>
	public Alterer<T, T2> ConfigureValueAlterer<T2>(bool add, Object creator, Func<T, T2, T> applier, Func<T2> updater, params Muc.Data.Event[] updateEvents) {
		return ConfigureValueAlterer(add, creator, applier, updater, updateEvents as IEnumerable<Muc.Data.Event>);
	}

	/// <summary> Adds or removes Alterers. When add is false all Alterers are removed by the creator regardless of the other arguments.</summary>
	public Alterer<T, T2> ConfigureValueAlterer<T2>(bool add, Object creator, Func<T, T2, T> applier, Func<T2> updater, IEnumerable<Muc.Data.Event> updateEvents = null) {
		Alterer<T, T2> res = null;
		if (add) {
			res = new(creator, updater, applier, () => UpdateValue(true));
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
			UpdateValue(true);
		}
		return res;
	}


	public override int valueCount => 1;

	public virtual IEnumerable<T> GetValues() {
		yield return value;
	}

	public virtual T GetValue(int valueIndex) {
		if (valueIndex >= valueCount) throw new IndexOutOfRangeException(nameof(valueIndex));
		return value;
	}

	public sealed override IEnumerable<object> GetObjectValues() => GetValues().Cast<object>();
	public sealed override object GetObjectValue(int valueIndex) => GetValue(valueIndex);

	public sealed override bool HasValue() => true;

	public T GetValue() => value;
	public virtual T GetOther() => throw new NotSupportedException();

	public T GetRawValue() => rawValue;
	public virtual T GetRawOther() => throw new NotSupportedException();

	public sealed override bool HasAlteredValue() => !GetRawValue().Equals(GetValue());
	public sealed override bool HasAlteredOther() => !GetRawOther().Equals(GetOther());
	public sealed override bool HasAlteredEnabled() => !GetRawEnabled().Equals(GetEnabled());

	public sealed override object GetObjectValue() => GetValue();
	public sealed override object GetObjectOther() => GetOther();
	public sealed override object GetObjectEnabled() => GetEnabled();

	public sealed override object GetObjectRawValue() => GetRawValue();
	public sealed override object GetObjectRawOther() => GetRawOther();
	public sealed override object GetObjectRawEnabled() => GetRawEnabled();

	public override string Format(bool isSource)
		=> value is bool boolVal
			? Lang.GetStr(boolVal ? "True" : "False", boolVal.ToString())
			: String.Format(Lang.GetStr("Format_SingleNumber", "{0}"), value);

}

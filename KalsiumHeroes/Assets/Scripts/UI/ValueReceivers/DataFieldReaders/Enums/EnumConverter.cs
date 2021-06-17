
using System;
using UnityEngine;
using UnityEngine.Events;
using Muc.Data;

public abstract class EnumConverter<T, TParam> : DataFieldReader<T> where T : Enum {

	[SerializeField] TParam defaultParam;
	[SerializeField] SerializedDictionary<T, TParam> parameters;
	[SerializeField, HideInInspector] SerializedType<T> enumType;

	[SerializeField, Tooltip("Invoked when the attribute changes in any way or the reader is initialized. The name of the enum value is returned.")]
	protected UnityEvent<TParam> onUpdate;

	protected override void OnReceive() => Handle();
	protected override void OnValue(T value) => Handle();
	protected override void OnOther(T value) => Handle();
	protected override void OnEnabled(bool enabled) => Handle();

	protected virtual void Handle() {
		var current = selector.GetValue(data);
		if (parameters.TryGetValue(current, out var parameter)) {
			onUpdate.Invoke(parameter);
		} else {
			onUpdate.Invoke(defaultParam);
		}
	}

}
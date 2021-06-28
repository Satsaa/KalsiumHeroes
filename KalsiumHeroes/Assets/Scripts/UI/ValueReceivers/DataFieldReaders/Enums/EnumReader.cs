
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using Muc.Data;

public abstract class EnumReader<T> : DataFieldReader<T> where T : Enum {

	[SerializeField] SerializedDictionary<T, string> msgIds;

	[SerializeField, Tooltip("Invoked when the attribute changes in any way or the reader is initialized. The name of the enum value is returned.")]
	protected UnityEvent<string> onUpdate;

	protected override void OnReceive() => Handle();
	protected override void OnValue(T value) => Handle();
	protected override void OnOther(T value) => Handle();
	protected override void OnEnabled(bool enabled) => Handle();

	protected virtual void Handle() {
		var current = selector.GetValue(data);
		if (msgIds.TryGetValue(current, out var msgId)) {
			onUpdate.Invoke(Lang.GetText(msgId));
		} else {
			var name = current?.GetType().GetEnumName(current) ?? "None";
			onUpdate.Invoke(name);
		}
	}

}

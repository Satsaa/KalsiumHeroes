
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;

[Serializable]
public class Attribute<T> : Attribute, IAttribute, IIdentifiable, ISerializationCallbackReceiver {

	[Serializable]
	public class ValueContainer : ValueContainer<T> {
		public static implicit operator T(ValueContainer v) => v.value;
		public ValueContainer() : base() { }
		public ValueContainer(T raw) : base(raw) { }
	}

	#region General

	protected Attribute() { }
	public Attribute(T value) => InitValues(true, value);

	public virtual string identifier => null;

	[SerializeField] ValueContainer[] _values;
	public IReadOnlyList<ValueContainer> values => _values;

	public virtual int count => 1;
	public T this[int index] {
		get => _values[index].value;
		set => _values[index].value = value;
	}

	public ValueContainer value => values[0];

	#endregion


	#region Other

	protected ValueContainer[] CheckArray() {
		if (_values == null) {
			_values = new ValueContainer[count];
			for (int i = 0; i < _values.Length; i++) _values[i] = new();
			Debug.LogWarning("Was null");
		} else if (_values.Length != count) {
			Array.Resize(ref _values, count);
			for (int i = 0; i < _values.Length; i++) _values[i] ??= new();
			Debug.LogWarning("Was wrong size");
		}
		return _values;
	}

	protected void InitValues(bool enabled, params T[] values) => InitValues(enabled, values as IReadOnlyList<T>);
	protected void InitValues(bool enabled, IReadOnlyList<T> values) {
		if (_values == null) {
			_values = new ValueContainer[count];
			for (int i = 0; i < _values.Length; i++) _values[i] = values == null || values.Count <= i ? new() : new(values[i]);
		} else if (_values.Length != count) {
			Array.Resize(ref _values, count);
			for (int i = 0; i < _values.Length; i++) _values[i] ??= values == null || values.Count <= i ? new() : new(values[i]);
		}
		(this as IAttribute).SetEnabled(new(enabled));
	}

	public virtual string Format(bool isSource) {
		switch (count) {
			case 0:
				return Lang.GetStr(((IAttribute)this).GetEnabled()?.value ?? true ? "True" : "False");
			case 1:
				return this[0] is bool boolVal
					? Lang.GetStr(boolVal ? "True" : "False", boolVal.ToString())
					: this[0].ToString();
			case 2:
				return String.Format(Lang.GetStr("Format_Fraction", "{0}/{1}"), this[0], this[1]);
			case 3:
				return String.Format(Lang.GetStr("Format_TripleFraction", "{0}/{1}/{3}"), this[0], this[1], this[2]);
			case 4:
				return String.Format(Lang.GetStr("Format_QuadrupleFraction", "{0}/{1}/{3}/{4}"), this[0], this[1], this[2], this[3]);
			default:
				return String.Join(Lang.GetStr("MultiValueDeliminator", "/"), values);
		}
	}

	public virtual string TooltipText(IAttribute source) {
		if (String.IsNullOrEmpty(identifier)) return null;
		return DefaultTooltip(source);
	}

	protected string DefaultTooltip(IAttribute source, string overrideDisplayName = null) {
		if (Lang.HasStr($"{identifier}_Tooltip")) {
			if (source.GetEnabled() != null) return Lang.GetStr($"{identifier}_Tooltip", source.GetValues().Select(v => v.value).ToArray());
			return Lang.GetStr($"{identifier}_Tooltip", source.GetValues().Select(v => v.value).Append(source.GetEnabled()).ToArray());
		}
		return $"{overrideDisplayName ?? Lang.GetStr($"{identifier}_DisplayName")}{Lang.GetStr("LabelValueDeliminator", ": ")}{Format(source == this)}";
	}

	#endregion


	#region ISerializationCallbackReceiver

	void ISerializationCallbackReceiver.OnBeforeSerialize() => CheckArray();
	void ISerializationCallbackReceiver.OnAfterDeserialize() => CheckArray();

	#endregion


	#region IAttribute

	int IAttribute.count => count;
	string IAttribute.identifier => identifier;

	IReadOnlyList<IValueContainer> IAttribute.GetValues() => values;
	IValueContainer IAttribute.GetValue(int index) => values[index];
	EnabledContainer IAttribute.GetEnabled() => null;
	EnabledContainer IAttribute.SetEnabled(EnabledContainer value) => null;

	#endregion

}

[Serializable]
public class ToggleAttribute<T> : Attribute<T>, IAttribute {

	protected ToggleAttribute() { }
	public ToggleAttribute(T value, bool enabled = true) => InitValues(enabled, value);
	public ToggleAttribute(bool enabled) => InitValues(enabled);

	[field: SerializeField] public EnabledContainer enabled { get; private set; }
	EnabledContainer IAttribute.GetEnabled() => enabled;
	EnabledContainer IAttribute.SetEnabled(EnabledContainer value) => enabled = value;

}

#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using Attribute = global::Attribute;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CustomPropertyDrawer(typeof(Attribute<>), true)]
	public class AttributeDrawer : PropertyDrawer {

		static string rawField { get; } = GetBackingFieldName("raw");
		static string enabledField { get; } = GetBackingFieldName("enabled");

		static GUIContent[] baseLabels { get; } = new GUIContent[] {
			new("A", "First value"),
			new("B", "Second value"),
			new("C", "Third value"),
			new("D", "Fourth value"),
			new("E", "Fifth value")
		};

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {


			using (PropertyScope(position, label, property, out label)) {

				var values = property.FindPropertyRelative("_values");
				var enabled = property.FindPropertyRelative(enabledField);

				var size = values.arraySize;
				var props = new SerializedProperty[size];
				for (int i = 0; i < size; i++) props[i] = values.GetArrayElementAtIndex(i).FindPropertyRelative(rawField);

				var toggleWidth = 15 + spacing;

				using (LabelWidthScope(v => enabled == null ? v : v - toggleWidth))
					position = Prefix(position, label);

				if (enabled != null) {
					var pos = position;
					pos.width = toggleWidth;
					PropertyField(pos, GUIContent.none, enabled.FindPropertyRelative(rawField));
					position.xMin = pos.xMax;
				}

				MultiPropertyField(position, baseLabels, props);

			}
		}

	}

}
#endif
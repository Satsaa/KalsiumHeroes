
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public abstract class Attribute : IIdentifiable {

	public Muc.Data.Event onChanged;

	public virtual bool HasValue() => false;
	public virtual bool HasOther() => false;
	public virtual bool HasEnabled() => false;

	public abstract int valueCount { get; }

	public abstract IEnumerable<object> GetObjectValues();
	public abstract object GetObjectValue(int valueIndex);

	public virtual bool GetEnabled() => true;

	public virtual bool GetRawEnabled() => throw new NotSupportedException();

	public virtual object GetObjectValue() => throw new NotSupportedException();
	public virtual object GetObjectOther() => throw new NotSupportedException();
	public virtual object GetObjectEnabled() => throw new NotSupportedException();

	public virtual object GetObjectRawValue() => throw new NotSupportedException();
	public virtual object GetObjectRawOther() => throw new NotSupportedException();
	public virtual object GetObjectRawEnabled() => throw new NotSupportedException();

	public virtual bool HasAlteredValue() => throw new NotSupportedException();
	public virtual bool HasAlteredOther() => throw new NotSupportedException();
	public virtual bool HasAlteredEnabled() => throw new NotSupportedException();

	public virtual string GetValueLabel() => "Value";
	public virtual string GetOtherLabel() => "Other";
	public virtual string GetEnabledLabel() => "Enabled";

	public virtual string identifier => null;
	public abstract string Format(bool isSource);
	public virtual string TooltipText(Attribute source) {
		if (String.IsNullOrEmpty(identifier)) return null;
		return DefaultTooltip(source);
	}

	protected string DefaultTooltip(Attribute source, string overrideDisplayName = null) {
		if (Lang.HasStr($"{identifier}_Tooltip")) {
			return Lang.GetStr($"{identifier}_Tooltip", source.GetObjectValues().Append(source.GetEnabled()).ToArray());
		}
		return $"{overrideDisplayName ?? Lang.GetStr($"{identifier}_DisplayName")}{Lang.GetStr("LabelValueDeliminator", ": ")}{Format(source == this)}";
	}

}


#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using Attribute = global::Attribute;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CustomPropertyDrawer(typeof(Attribute), true)]
	public class AttributeDrawer : PropertyDrawer {

		public float alteredWidth => expandAltered ? 0.5f : 0.8f;
		public bool expandAltered = false;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			using (PropertyScope(position, label, property, out label))
			using (RestoreLabelWidthScope())
			using (RestoreFieldWidthScope()) {

				var enabledProperty = property.FindPropertyRelative(GetBackingFieldName(nameof(ToggleAttribute<int>.rawEnabled)));
				var valueProperty = property.FindPropertyRelative(GetBackingFieldName(nameof(Attribute<int>.rawValue)));
				var otherProperty = property.FindPropertyRelative(GetBackingFieldName(nameof(DualAttribute<int>.rawOther)));

				var fieldInfo = GetFieldInfo(property);
				var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

				var noLabel = label.text is "" && label.image is null;
				var pos = position;
				pos.width = noLabel ? 0 : labelWidth;
				if (!noLabel) EditorGUI.LabelField(pos, label);

				using (IndentScope(v => 0)) {

					var obj = GetValues<Attribute>(property).First();
					var objType = obj.GetType();
					var isRepaint = UnityEngine.Event.current.type == EventType.Repaint;
					var prevExpandAltered = expandAltered;
					var wasExpanded = false;

					// Enabled boolean
					if (enabledProperty != null) {
						pos.xMin = pos.xMax + spacing;
						pos.width = 15 + spacing;
						if (Application.isPlaying) {
							using (DisabledScope()) {
								var prop = objType.GetProperty("enabled");
								var val = (bool)prop.GetValue(obj);
								EditorGUI.Toggle(pos, val);
							}
						} else {
							EditorGUI.PropertyField(pos, enabledProperty, GUIContent.none);
							if (obj.HasAlteredEnabled()) {
								using (DisabledScope()) {
									pos.x = pos.xMax;
									var prop = objType.GetProperty("enabled");
									var val = (bool)prop.GetValue(obj);
									EditorGUI.Toggle(pos, val);
								}
							}
						}
					}

					// Primary value
					if (valueProperty != null) {
						var valueLabel = new GUIContent(labelAttribute?.primaryLabel ?? obj.GetValueLabel());
						labelWidth = GUI.skin.label.CalcSize(valueLabel).x - spacing;
						pos.xMin = pos.xMax + spacing;
						pos.width = (position.xMax - pos.xMin) / 2 - spacing;
						if (otherProperty == null) pos.xMax = position.xMax;
						if (Application.isPlaying) {
							using (DisabledScope()) {
								var prop = objType.GetProperty("value");
								var val = prop.GetValue(obj);
								EditorGUI.TextField(pos, val.ToString());
								if (isRepaint) {
									wasExpanded |= pos.Contains(UnityEngine.Event.current.mousePosition);
									GUI.changed |= prevExpandAltered != wasExpanded;
								}
							}
						} else {
							var origMax = pos.xMax;
							if (obj.HasAlteredValue()) pos.width *= alteredWidth;
							EditorGUI.PropertyField(pos, valueProperty, valueLabel);

							if (obj.HasAlteredValue()) {
								using (DisabledScope()) {
									pos.xMin = pos.xMax;
									pos.xMax = origMax;
									var prop = objType.GetProperty("value");
									var val = prop.GetValue(obj);
									EditorGUI.TextField(pos, val.ToString());
									if (isRepaint) {
										wasExpanded |= pos.Contains(UnityEngine.Event.current.mousePosition);
										GUI.changed |= prevExpandAltered != wasExpanded;
									}
								}
							}
						}
					}

					// Secondary value
					if (otherProperty != null) {
						var baseLabel = new GUIContent(labelAttribute?.secondaryLabel ?? obj.GetOtherLabel());
						labelWidth = GUI.skin.label.CalcSize(baseLabel).x - spacing;
						pos.xMin = pos.xMax + spacing;
						pos.xMax = position.xMax;
						if (Application.isPlaying) {
							using (DisabledScope()) {
								var prop = objType.GetProperty("other");
								var val = prop.GetValue(obj);
								EditorGUI.TextField(pos, val.ToString());
								if (isRepaint) {
									wasExpanded |= pos.Contains(UnityEngine.Event.current.mousePosition);
									GUI.changed |= prevExpandAltered != wasExpanded;
								}
							}
						} else {
							var origMax = pos.xMax;
							if (obj.HasAlteredOther()) pos.width *= alteredWidth;
							EditorGUI.PropertyField(pos, otherProperty, baseLabel);

							if (obj.HasAlteredOther()) {
								using (DisabledScope()) {
									pos.xMin = pos.xMax;
									pos.xMax = origMax;
									var prop = objType.GetProperty("other");
									var val = prop.GetValue(obj);
									EditorGUI.TextField(pos, val.ToString());
									if (isRepaint) {
										wasExpanded |= pos.Contains(UnityEngine.Event.current.mousePosition);
										GUI.changed |= prevExpandAltered != wasExpanded;
									}
								}
							}
						}
					}

					if (isRepaint) {
						expandAltered = wasExpanded;
					}
				}

			}

		}

	}

}
#endif
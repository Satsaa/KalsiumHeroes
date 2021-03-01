
#if UNITY_EDITOR
using UnityEditor;
using static Muc.Editor.EditorUtil;
using static Muc.Editor.PropertyUtil;
using UnityEngine;
using System.Linq;


[CustomPropertyDrawer(typeof(AttributeBase), true)]
public class AttributeBaseDrawer : PropertyDrawer {

	public float alteredWidth => expandAltered ? 0.5f : 0.8f;
	public bool expandAltered = false;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		using (PropertyScope(position, label, property, out label))
		using (RestoreLabelWidthScope())
		using (RestoreFieldWidthScope()) {

			var enabledProperty = property.FindPropertyRelative("_enabled");
			var valueProperty = property.FindPropertyRelative("_value");
			var otherProperty = property.FindPropertyRelative("_other");

			var fieldInfo = GetFieldInfo(property);
			var labelAttribute = fieldInfo?.GetCustomAttributes(typeof(AttributeLabelsAttribute), false).FirstOrDefault() as AttributeLabelsAttribute;

			var noLabel = label.text is "" && label.image is null;
			var pos = position;
			pos.width = noLabel ? 0 : labelWidth;
			if (!noLabel) EditorGUI.LabelField(pos, label);

			using (IndentScope(v => 0)) {

				var obj = GetValues<AttributeBase>(property).First();
				var objType = obj.GetType();
				var isRepaint = UnityEngine.Event.current.type == EventType.Repaint;
				var prevExpandAltered = expandAltered;
				var wasExpanded = false;

				// Enabled boolean
				if (enabledProperty != null) {
					var attributeProperty = AttributeBase.AttributeProperty.Enabled;
					pos.xMin = pos.xMax + spacing;
					pos.width = 15 + spacing;
					if (Application.isPlaying && obj.DisplayAlteredInPlay(attributeProperty)) {
						using (DisabledScope()) {
							var prop = objType.GetProperty("enabled");
							var val = (bool)prop.GetValue(obj);
							EditorGUI.Toggle(pos, val);
						}
					} else {
						EditorGUI.PropertyField(pos, enabledProperty, GUIContent.none);
						if (obj.HasAlteredValue(attributeProperty)) {
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
					var attributeProperty = AttributeBase.AttributeProperty.Primary;
					var valueLabel = new GUIContent(labelAttribute?.primaryLabel ?? obj.GetEditorLabel(attributeProperty));
					labelWidth = GUI.skin.label.CalcSize(valueLabel).x - spacing;
					pos.xMin = pos.xMax + spacing;
					pos.width = (position.xMax - pos.xMin) / 2 - spacing;
					if (otherProperty == null) pos.xMax = position.xMax;
					if (Application.isPlaying && obj.DisplayAlteredInPlay(attributeProperty)) {
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
						if (obj.HasAlteredValue(attributeProperty)) pos.width *= alteredWidth;
						EditorGUI.PropertyField(pos, valueProperty, valueLabel);

						if (obj.HasAlteredValue(attributeProperty)) {
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
					var attributeProperty = AttributeBase.AttributeProperty.Secondary;
					var baseLabel = new GUIContent(labelAttribute?.secondaryLabel ?? obj.GetEditorLabel(attributeProperty));
					labelWidth = GUI.skin.label.CalcSize(baseLabel).x - spacing;
					pos.xMin = pos.xMax + spacing;
					pos.xMax = position.xMax;
					if (Application.isPlaying && obj.DisplayAlteredInPlay(attributeProperty)) {
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
						if (obj.HasAlteredValue(attributeProperty)) pos.width *= alteredWidth;
						EditorGUI.PropertyField(pos, otherProperty, baseLabel);

						if (obj.HasAlteredValue(attributeProperty)) {
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
#endif
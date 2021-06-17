
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public struct ToggleValue<T> {

	public T value;
	public bool enabled;

}

#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(ToggleValue<>), true)]
	public class ToggleValueDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			using (PropertyScope(position, label, property, out label)) {

				var noLabel = label.text is "" && label.image is null;
				var pos = position;
				pos.width = noLabel ? 0 : labelWidth;
				if (!noLabel) EditorGUI.LabelField(pos, label);

				using (IndentScope(v => 0)) {

					var value = property.FindPropertyRelative(nameof(ToggleValue<int>.value));
					var enabled = property.FindPropertyRelative(nameof(ToggleValue<int>.enabled));

					// Enabled
					pos.xMin = pos.xMax + spacing;
					pos.width = 15 + spacing;
					EditorGUI.PropertyField(pos, enabled, GUIContent.none);

					pos.xMin = pos.xMax + spacing;
					pos.xMax = position.xMax;
					EditorGUI.PropertyField(pos, value, GUIContent.none);
				}

			}
		}

	}
}
#endif
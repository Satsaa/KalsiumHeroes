
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ValuePropagator : MonoBehaviour {

	[SerializeField] Object value;

	public void Propagate() {
		ValueReceiver.SendValue(gameObject, value);
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
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;
	using UnityEditor.SceneManagement;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(ValuePropagator))]
	public class ValuePropagatorEditor : Editor {

		ValuePropagator t => (ValuePropagator)target;

		SerializedProperty value;

		void OnEnable() {
			value = serializedObject.FindProperty(nameof(value));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawDefaultInspector();

			using (DisabledScope(!Application.isPlaying || !value.objectReferenceValue || PrefabStageUtility.GetPrefabStage(t.gameObject) != null)) {
				if (GUILayout.Button("Propagate Value")) {
					t.Propagate();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif



using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class SendValueStopper : MonoBehaviour {

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
	[CustomEditor(typeof(SendValueStopper), true)]
	public class SendValueStopperEditor : Editor {

		SendValueStopper t => (SendValueStopper)target;

		// SerializedProperty property;

		void OnEnable() {
			// property = serializedObject.FindProperty(nameof(property));
		}

		public override void OnInspectorGUI() {
			EditorGUILayout.LabelField("Stops SendValue from parent GameObjects.");
		}
	}
}
#endif


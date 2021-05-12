
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Test : MonoBehaviour {

	void Start() { }

	public void DoTest() {
		Debug.Log($"TryGetComponent<Test>(out var comp) = {TryGetComponent<Test>(out var comp)}, comp = {comp}");
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

	[CanEditMultipleObjects]
	[CustomEditor(typeof(Test), true)]
	public class TestEditor : Editor {

		Test t => (Test)target;

		public override void OnInspectorGUI() {
			serializedObject.Update();

			if (GUILayout.Button(nameof(Test.DoTest))) {
				t.DoTest();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
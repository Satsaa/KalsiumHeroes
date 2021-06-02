
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Test : MonoBehaviour {

	public Attribute<float> appeal;
	public Attribute<int> health;
	public string defense = "lol";

	public AttributeSelector<float> test;
	public NumericAttributeSelector numTest;

	void Start() { }

	public void DoTest1() {
		Debug.Log($"test.GetValue(this) => {test.GetValue(this)}");
		Debug.Log($"test.GetOther(this) => {test.GetOther(this)}");
		Debug.Log($"test.GetEnabled(this) => {test.GetEnabled(this)}");
	}

	public void DoTest2() {
		Debug.Log($"numTest.GetValue(this) => {numTest.GetValue(this)}");
		Debug.Log($"numTest.GetOther(this) => {numTest.GetOther(this)}");
		Debug.Log($"numTest.GetEnabled(this) => {numTest.GetEnabled(this)}");
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

			DrawDefaultInspector();

			if (GUILayout.Button(nameof(Test.DoTest1))) {
				t.DoTest1();
			}

			if (GUILayout.Button(nameof(Test.DoTest2))) {
				t.DoTest2();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
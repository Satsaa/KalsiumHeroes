
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Serialization;
using System.Reflection;

public class Test : MonoBehaviour {

	public DataObjectData data;
	public Unit unit;

	public MaxAttribute<float> newAtt;
	public ToggleMaxAttribute<float> newTogAtt;

	public Attribute<float> appeal;
	public Attribute<int> health;
	public string str = "lol";

	public AttributeSelector<float> test;
	public NumericAttributeSelector numTest;
	public DataFieldName fieldTest;
	public DataFieldSelector<float> floatFieldTest;
	public NumericDataFieldSelector numericFieldTest;

	void Start() { }

	public void DoTest1() {
		var message = GameSerializer.Serialize(Game.game);
		System.IO.File.WriteAllText("C:/Users/sampp/Desktop/output.json", message);
	}

	public void DoTest2() {
		var json = System.IO.File.ReadAllText("C:/Users/sampp/Desktop/output.json");
		GameSerializer.Deserialize(json, Game.game);
	}

	public void DoTest3() {
		Debug.Log(Game.instance.ar1.value);
		Debug.Log(Game.instance.ar2.value);
		Debug.Log(Game.instance.arC.value);
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

		protected override void OnHeaderGUI() {
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawDefaultInspector();

			Space();

			if (ButtonField(new GUIContent("a label", "a tooltip"), new("Test"))) {
				t.DoTest1();
			}

			using (IndentScope(1)) {

				if (ButtonField(new GUIContent("a label", "a tooltip"), new("Test"))) {
					t.DoTest2();
				}

				using (IndentScope(1)) {
					if (ButtonField(new GUIContent("a label", "a tooltip"), new("Test"))) {
						t.DoTest3();
					}

				}
			}
			/* 
			Space();

			var rect = new Rect();
			var data = serializedObject.FindProperty("data");
			var prop = serializedObject.FindProperty("fieldTest");
			var label = new GUIContent("Label");

			EditorGUILayout.TextField(label, "EditorGUILayout");
			using (IndentScope(1)) {
				EditorGUILayout.TextField(label, "EditorGUILayout");
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					EditorGUI.TextField(rect, label, "EditorGUILayout");
				}
			}

			ButtonField(label, new("Text"));
			using (IndentScope(1)) {
				ButtonField(label, new("Text"));
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					ButtonField(rect, label, new("Text"));
				}
			}

			DropdownField(label, new("Text"));
			using (IndentScope(1)) {
				DropdownField(label, new("Text"));
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					DropdownField(rect, label, new("Text"));
				}
			}

			HelpBoxField(label, "WARNING!", MessageType.Warning);
			using (IndentScope(1)) {
				HelpBoxField(label, "WARNING!", MessageType.Warning);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					HelpBoxField(rect, label, "WARNING!", MessageType.Warning);
				}
			}

			Field(label, "TEst");
			using (IndentScope(1)) {
				Field(label, "TEst");
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, "TEst");
				}
			}

			Field("TEst");
			using (IndentScope(1)) {
				Field("TEst");
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, "TEst");
				}
			}


			ComponentHeader(true, targets);
			using (IndentScope(1)) {
				ComponentHeader(true, targets);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					ComponentHeader(rect, true, targets);
				}
			}

			Foldout(prop);
			using (IndentScope(1)) {
				Foldout(prop);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Foldout(rect, prop);
				}
			}

			Label(label);
			using (IndentScope(1)) {
				Label(label);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Label(rect, label);
				}
			}

			Field(label, 100);
			using (IndentScope(1)) {
				Field(label, 100);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, 100);
				}
			}

			PasswordField(label, "ass");
			using (IndentScope(1)) {
				PasswordField(label, "ass");
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					PasswordField(rect, label, "ass");
				}
			}

			Field(label, new Rect(0, 0, 10, 10));
			using (IndentScope(1)) {
				Field(label, new Rect(0, 0, 10, 10));
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, new Rect(0, 0, 10, 10));
				}
			}

			Field(label, new RectInt(0, 0, 10, 10));
			using (IndentScope(1)) {
				Field(label, new RectInt(0, 0, 10, 10));
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, new RectInt(0, 0, 10, 10));
				}
			}

			Field(label, new Vector2Int(0, 0));
			using (IndentScope(1)) {
				Field(label, new Vector2Int(0, 0));
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, new Vector2Int(0, 0));
				}
			}

			Field(label, new Vector4(0, 0, 10, 10));
			using (IndentScope(1)) {
				Field(label, new Vector4(0, 0, 10, 10));
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, new Vector4(0, 0, 10, 10));
				}
			}

			Field(label, new Color());
			using (IndentScope(1)) {
				Field(label, new Color());
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, new Color());
				}
			}

			Field(label, AnimationCurve.EaseInOut(0, 0, 1, 1));
			using (IndentScope(1)) {
				Field(label, AnimationCurve.EaseInOut(0, 0, 1, 1));
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, AnimationCurve.EaseInOut(0, 0, 1, 1));
				}
			}

			Field(label, new Bounds());
			using (IndentScope(1)) {
				Field(label, new Bounds());
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, new Bounds());
				}
			}

			Field(label, prop.isExpanded);
			using (IndentScope(1)) {
				Field(label, prop.isExpanded);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, prop.isExpanded);
				}
			}

			SliderField(label, 5, 0, 10);
			using (IndentScope(1)) {
				SliderField(label, 5, 0, 10);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					SliderField(rect, label, 5, 0, 10);
				}
			}

			var min = 0.5f;
			var max = 2.5f;
			MinMaxSliderField(label, ref min, ref max, 0, 10);
			using (IndentScope(1)) {
				MinMaxSliderField(label, ref min, ref max, 0, 10);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					MinMaxSliderField(rect, label, ref min, ref max, 0, 10);
				}
			}

			PropertyField<DataObjectData>(label, data);
			using (IndentScope(1)) {
				PropertyField<DataObjectData>(label, data);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					PropertyField<DataObjectData>(rect, label, data);
				}
			}

			Field(label, AbilityType.WeaponSkill);
			using (IndentScope(1)) {
				Field(label, AbilityType.WeaponSkill);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, AbilityType.WeaponSkill);
				}
			}

			Field(label, UnitTargetType.Ally);
			using (IndentScope(1)) {
				Field(label, UnitTargetType.Enemy);
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					Field(rect, label, UnitTargetType.Ally | UnitTargetType.Enemy);
				}
			}

			TagField(label, "MainCamera");
			using (IndentScope(1)) {
				TagField(label, "MainCamera");
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					TagField(rect, label, "MainCamera");
				}
			}

			MultiPropertyField(label, new GUIContent[] { new("X"), new("Y"), new("Width") }, new SerializedProperty[] { data, prop, data });
			using (IndentScope(1)) {
				MultiPropertyField(label, new GUIContent[] { new("X"), new("Y"), new("Width") }, new SerializedProperty[] { data, prop, data });
				using (IndentScope(1)) {
					rect = EditorGUILayout.GetControlRect();
					MultiPropertyField(rect, label, new GUIContent[] { new("X"), new("Y"), new("Width") }, new SerializedProperty[] { data, prop, data });
				}
			}
			*/

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Menu : MonoBehaviour {

	[field: SerializeField, Tooltip("Used to compare Menus. Many settings define their behaviour by comparing types of Menus. Some menus can extends the comparison beyond just this.")]
	public string type { get; private set; }

	[field: SerializeField, Tooltip("Will this Menu close the previous chain of Menus that are replaceable?")]
	public bool replace { get; private set; }

	[field: SerializeField, Tooltip("Will this Menu be closed when a Replacing Menu is added after this?")]
	public bool replaceable { get; private set; }

	[field: SerializeField, Tooltip("Disallow multiple consecutive Menus of this type? The newest one is used.")]
	public bool collapse { get; private set; }

	[field: SerializeField, Tooltip("Reuse this Menu for upcoming Menus of the same type? No per-menu data is stored.")]
	public bool reuse { get; private set; }

	[field: SerializeField, Tooltip("Don't destroy the last instance of a Menu of this type, complementing the reuse toggle.")]
	public bool persist { get; private set; }

	[field: SerializeField, Tooltip("Don't hide the Menu when it is not at the top?")]
	public bool alwaysVisible { get; private set; }

	/// <summary> True if the Menu should be destroyed after the hide animation. </summary>
	[field: SerializeField, HideInInspector]
	public bool destroy { get; set; }


	public static bool CompareType(Menu a, Menu b) {
		return a.CompareType(b) && b.CompareType(a);
	}

	protected virtual bool CompareType(Menu other) {
		return this.type == other.type;
	}

	public virtual void OnHide() {
		if (TryGetComponent<Animator>(out var animator)) {
			animator.SetTrigger("Hide");
		}
	}

	public virtual void OnShow() {
		if (TryGetComponent<Animator>(out var animator)) {
			animator.SetTrigger("Show");
		}
	}

	public void FinalizeHide() {
		if (destroy) Destroy(gameObject);
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
	[CustomEditor(typeof(global::Menu), true)]
	public class MenuEditor : Editor {

		global::Menu t => (global::Menu)target;

		SerializedProperty reuse;

		void OnEnable() {
			reuse = serializedObject.FindProperty($"<{nameof(global::Menu.reuse)}>k__BackingField");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			if (reuse.boolValue) {
				DrawPropertiesExcluding(serializedObject, "m_Script");
			} else {
				DrawPropertiesExcluding(serializedObject, "m_Script", $"<{nameof(global::Menu.persist)}>k__BackingField");
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
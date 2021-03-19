
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Menus : Singleton<Menus> {

	public new static Transform transform => instance.gameObject.transform;

	public List<Menu> menus;
	public List<Menu> cached;


	/// <summary>
	/// Removes the top-most instance of target menu.
	/// Due to reusage of Menus this may result in unexpected behaviour.
	/// Use the Hide(index) for removal of a specific instance of a Menu.
	/// </summary>
	public static void Hide(Menu target) => instance._Hide(target);

	/// <summary> Removes the Menu at index. </summary>
	public static void Hide(int index) => instance._Hide(index);

	/// <summary> Removes the last/newest Menu. </summary>
	public static void Pop() => instance._Hide(instance.menus.Count - 1);

	/// <summary> Shows a menu representing the source Menu and optionally runs an initializer on it before it is activated. </summary>
	public static Menu Show(Menu source, Action<Menu> initializer = null) => instance._Show(source, initializer);


	protected void _Hide(Menu target) {
		var mi = menus.FindLastIndex(v => v == target);
		if (mi != -1) {
			Hide(mi);
		} else {
			throw new ArgumentException($"Parameter was not found in the {nameof(Menu)} list.", nameof(target));
		}
	}

	protected void _Hide(int index, bool collapse = true) {
		var menu = menus[index];
		var destroy = true;

		// Persist for reuse
		if (destroy && menu.reuse && menu.persist) {
			for (int i = 0; i < menus.Count; i++) {
				var other = menus[i];
				if (i != index && Menu.CompareType(menu, other)) {
					goto skipPersist;
				}
			}
			destroy = false;
			if (!cached.Contains(menu)) cached.Add(menu);
		}
	skipPersist:

		// Check if reused
		if (destroy && menu.reuse) {
			for (int i = 0; i < menus.Count; i++) {
				var other = menus[i];
				if (i != index && other.reuse && Menu.CompareType(menu, other)) {
					destroy = false;
					goto skipReuse;
				}
			}
		}
	skipReuse:

		menu.destroy = destroy;
		menus.RemoveAt(index);
		menu.OnHide();

		if (collapse) TryCollapse(index);

		var last = menus.LastOrDefault();
		if (last != null && !last.alwaysVisible) {
			last.OnShow();
		}
	}

	protected Menu _Show(Menu source, Action<Menu> initializer = null) {
		Menu instance = null;

		// reuse
		if (source.reuse) {
			var ci = cached.FindIndex(v => Menu.CompareType(source, v));
			if (ci != -1) {
				instance = cached[ci];
				cached.RemoveAt(ci);
				initializer?.Invoke(instance);
			} else {
				var mi = menus.FindIndex(v => v.reuse && Menu.CompareType(source, v));
				if (mi != -1) {
					instance = menus[mi];
					initializer?.Invoke(instance);
				}
			}
		}

		// default instance
		if (instance == null) {
			instance = Instantiate(source, transform);
			initializer?.Invoke(instance);
		}

		menus.Add(instance);
		instance.OnShow();

		while (TryCollapse(menus.Count - 1, false) || TryReplace(menus.Count - 1, false)) { }

		if (menus.Count >= 2) {
			var before = menus[menus.Count - 2];
			if (!before.alwaysVisible) {
				before.OnHide(); // No destroy...
			}
		}

		return instance;
	}

	protected bool TryCollapse(int index, bool propagate = true) {
		if (menus.Count > index && index > 0) {
			var after = menus[index];
			var before = menus[index - 1];
			if (Menu.CompareType(after, before) && after.collapse && before.collapse) {
				_Hide(index - 1, propagate);
				return true;
			}
		}
		return false;
	}

	protected bool TryReplace(int index, bool propagate = true) {
		if (menus.Count > index && index > 0) {
			var menu = menus[index];
			if (menu.replaceable) {
				_Hide(index, propagate);
				return true;
			}
		}
		return false;
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
	[CustomEditor(typeof(Menus), true)]
	public class MenusEditor : Editor {

		Menus t => (Menus)target;

		// SerializedProperty property;

		void OnEnable() {
			// property = serializedObject.FindProperty(nameof(property));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawDefaultInspector();

			if (GUILayout.Button("Pop")) {
				Menus.Pop();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
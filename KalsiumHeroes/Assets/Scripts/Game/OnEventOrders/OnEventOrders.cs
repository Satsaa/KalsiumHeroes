
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Components.Extended;
using Muc.Data;

public class OnEventOrders : Singleton<OnEventOrders> {

	public List<SerializedType<IOnEvent>> order;

	private bool valid = false;
	private Dictionary<Type, int> cache;

	public int GetPirority<T>() where T : IOnEvent {
		if (!valid) BuildCache();
		return cache[typeof(T)];
	}

	public void BuildCache() {
		valid = true;
		cache = new Dictionary<Type, int>();
		for (int i = 0; i < order.Count; i++) {
			var current = order[i];
			cache[current] = i;
		}
	}

	public void InvalidateCache() {
		valid = false;
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

	[CustomEditor(typeof(OnEventOrders), true)]
	public class OnEventOrdersEditor : Editor {

		OnEventOrders t => (OnEventOrders)target;

		void OnEnable() {

		}

		public override void OnInspectorGUI() {

			EditorGUI.BeginChangeCheck();
			DrawDefaultInspector();
			if (EditorGUI.EndChangeCheck()) {
				t.InvalidateCache();
			}

			if (GUILayout.Button("Refresh")) {
				serializedObject.Update();
				var types = GetDataObjectTypes();
				foreach (var type in types) {
					if (t.order.All(v => v != type)) {
						t.order.Add(new SerializedType<IOnEvent>(type));
					}
				}
				t.order.RemoveAll(v => v == null);
				t.order = t.order.Distinct().ToList();
				t.InvalidateCache();
				serializedObject.ApplyModifiedProperties();
			}

		}


		private static IEnumerable<Type> GetDataObjectTypes() {
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				var name = assembly.GetName().Name;
				var firstDot = name.IndexOf('.');
				var rootName = firstDot == -1 ? name : name.Substring(0, firstDot);
				switch (rootName) {
					case "System": continue;
					case "UnityEngine": continue;
					case "UnityEditor": continue;
					case "mscorlib": continue;
					case "Unity": continue;
					case "Mono": continue;
					default:
						if (
							name.StartsWith("com.unity") ||
							name == "nunit.framework" ||
							name == "ICSharpCode.NRefactory"
						) {
							continue;
						}
						break;
				}
				var types = assembly.GetTypes();
				foreach (var type in types) {
					if (type.IsClass && !type.IsAbstract && !type.IsGenericType && typeof(IOnEvent).IsAssignableFrom(type)) {
						yield return type;
					}
				}
			}
		}


	}

}
#endif
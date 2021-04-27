

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Data;

[DisallowMultipleComponent]
public class Library : MonoBehaviour {

	[SerializeField] List<DataObjectData> sources;
	public SerializedDictionary<string, DataObjectData> dict => _dict == null ? _dict ??= BuildDict() : _dict.Count == sources.Count ? _dict : _dict ??= BuildDict();
	private SerializedDictionary<string, DataObjectData> _dict;


	private SerializedDictionary<string, DataObjectData> BuildDict() {
		var res = new SerializedDictionary<string, DataObjectData>();
		sources.RemoveAll(v => v == null);
		foreach (var source in sources) {
			Debug.Assert(source.identifier != "", source);
			Debug.Assert(!res.ContainsKey(source.identifier), source);
			res.Add(source.identifier, source);
		}
		return res;
	}

	public bool TryGetById<T>(string id, out T result) where T : DataObjectData {
		if (dict.TryGetValue(id, out var res) && res is T _result) {
			result = _result;
			return true;
		}
		result = default;
		return false;
	}

	public T GetById<T>(string id) where T : DataObjectData {
		return (T)dict[id];
	}

	public IEnumerable<T> GetByType<T>() where T : DataObjectData {
		foreach (var v in dict.Values) {
			if (v is T r) yield return r;
		}
	}

	public void SetSources(List<DataObjectData> sources) {
		this.sources = sources;
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
	[CustomEditor(typeof(Library), true)]
	public class LibraryEditor : Editor {

		Library t => (Library)target;

		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawDefaultInspector();

			if (GUILayout.Button("Add All Sources In Project")) {
				var datas = new List<DataObjectData>();
				var guids = AssetDatabase.FindAssets($"t:{nameof(DataObjectData)}");
				foreach (var guid in guids) {
					var path = AssetDatabase.GUIDToAssetPath(guid);
					var data = AssetDatabase.LoadAssetAtPath<DataObjectData>(path);
					datas.Add(data);
				}
				t.SetSources(datas);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
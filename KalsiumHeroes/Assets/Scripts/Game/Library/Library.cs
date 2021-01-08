using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains a list of data sources
/// </summary>
[CreateAssetMenu(fileName = nameof(Library), menuName = "DataSources/" + nameof(Library))]
public class Library : ScriptableObject {

	public string version = "0.0.0";

	public List<DataComponentData> sources;

	private Dictionary<string, DataComponentData> _dict;
	public Dictionary<string, DataComponentData> dict {
		get {
			return _dict ??= BuildDict();
		}
	}

	private Dictionary<string, DataComponentData> BuildDict() {
		var res = new Dictionary<string, DataComponentData>();
		foreach (var source in sources) {
			Debug.Assert(source.identifier != "");
			Debug.Assert(!res.ContainsKey(source.identifier));
			res.Add(source.identifier, source);
		}
		return res;
	}

	public T GetData<T>(string id) where T : DataComponentData {
		var item = dict[id];
		return item as T;
	}
}

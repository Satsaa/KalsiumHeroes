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

	[Tooltip("Library specific DataObjects.")]
	public List<DataObjectData> sources;
	[Tooltip("Library specific tooltips. They are linked like tt_lib_ID.")]
	public SerializedDictionary<string, Tooltip> tooltips;

	private Dictionary<string, DataObjectData> _dict;
	public Dictionary<string, DataObjectData> dict => _dict ??= BuildDict();

	private Dictionary<string, DataObjectData> BuildDict() {
		var res = new Dictionary<string, DataObjectData>();
		foreach (var source in sources) {
			Debug.Assert(source.identifier != "");
			Debug.Assert(!res.ContainsKey(source.identifier));
			res.Add(source.identifier, source);
		}
		return res;
	}

	public T GetData<T>(string id) where T : DataObjectData {
		var item = dict[id];
		return item as T;
	}
}

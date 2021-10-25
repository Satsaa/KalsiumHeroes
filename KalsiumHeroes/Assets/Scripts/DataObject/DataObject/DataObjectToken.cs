
using System;
using System.Linq;
using System.Reflection;
using Muc.Editor;
using Newtonsoft.Json.Linq;
using Serialization;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class DataObjectToken : RefTokenAttribute {

	public override Object CreateObject(JToken jToken) {
		if (jToken is JObject jObject) {
			var identifier = (string)jObject[nameof(DataObject.identifier)];
			var isSource = (bool)jObject[nameof(DataObject.isSource)]; // !!! Yeah this aint gonna work (isSource is a getter)
			var source = App.library.GetById(identifier);
			if (isSource) return source;
			return ScriptableObject.CreateInstance(source.GetType());
		}
		return base.CreateObject(jToken);
	}

}

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
			var identifier = (string)jObject[nameof(DataObjectData.identifier)];
			var isSource = (bool)jObject[nameof(DataObjectData.isSource)];
			var source = App.library.GetById(identifier);
			if (isSource) return source;
			else return Object.Instantiate(source);
		}
		return base.CreateObject(jToken);
	}

}
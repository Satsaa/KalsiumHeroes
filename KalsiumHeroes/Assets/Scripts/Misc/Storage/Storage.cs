
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json.Linq;

public static class Storage {

	public readonly static JsonSerializer serializer = new() {
		ContractResolver = new CollectionClearingContractResolver()
	};

	public static void SaveObject(string path, object obj) {
		path = $"{Application.persistentDataPath}/{path}.json";

		// serialize JSON directly to a file
		using (StreamWriter file = File.CreateText(path)) {
			serializer.Serialize(file, obj);
		}
	}

	public static void LoadObject(string path, object target) {
		path = $"{Application.persistentDataPath}/{path}.json";

		if (!File.Exists(path)) return;

		// deserialize JSON directly from a file
		using (StreamReader file = File.OpenText(path)) {
			serializer.Populate(file, target);
		}
	}

	public static void DeleteObject(string path) {
		path = $"{Application.persistentDataPath}/{path}.json";
		File.Delete(path);
	}

}

public class CollectionClearingContractResolver : DefaultContractResolver {

	static void ClearGenericCollectionCallback<T>(object o, StreamingContext c) {
		var collection = o as ICollection<T>;
		if (collection == null || collection is Array || collection.IsReadOnly)
			return;
		collection.Clear();
	}

	static SerializationCallback ClearListCallback = (o, c) => {
		var collection = o as IList;
		if (collection == null || collection is Array || collection.IsReadOnly)
			return;
		collection.Clear();
	};

	protected override JsonArrayContract CreateArrayContract(Type objectType) {
		var contract = base.CreateArrayContract(objectType);
		if (!objectType.IsArray) {
			if (typeof(IList).IsAssignableFrom(objectType)) {
				contract.OnDeserializingCallbacks.Add(ClearListCallback);
			} else if (GetCollectItemTypes(objectType).Count() == 1) {
				var method = typeof(CollectionClearingContractResolver).GetMethod("ClearGenericCollectionCallback", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
				var generic = method.MakeGenericMethod(contract.CollectionItemType);
				contract.OnDeserializingCallbacks.Add((SerializationCallback)Delegate.CreateDelegate(typeof(SerializationCallback), generic));
			}
		}

		return contract;
	}

	public static IEnumerable<Type> GetInterfacesAndSelf(Type type) {
		if (type == null)
			throw new ArgumentNullException();
		if (type.IsInterface)
			return new[] { type }.Concat(type.GetInterfaces());
		else
			return type.GetInterfaces();
	}

	public static IEnumerable<Type> GetCollectItemTypes(Type type) {
		foreach (var intType in GetInterfacesAndSelf(type)) {
			if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(ICollection<>)) {
				yield return intType.GetGenericArguments()[0];
			}
		}
	}

	public static void Populate<T>(JToken value, T target) where T : class {
		Populate(value, target, null);
	}

	public static void Populate<T>(JToken value, T target, JsonSerializerSettings settings) where T : class {
		using (var sr = value.CreateReader()) {
			JsonSerializer.CreateDefault(settings).Populate(sr, target);
		}
	}
}

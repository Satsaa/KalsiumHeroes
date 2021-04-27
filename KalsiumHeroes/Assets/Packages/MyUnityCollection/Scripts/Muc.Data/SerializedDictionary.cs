

// License: MIT
// Original Author: Erik Eriksson (2020)
// Original Code: https://github.com/upscalebaby/generic-serializable-dictionary

namespace Muc.Data {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver {
		// Internal
		[SerializeField]
		List<SerializedDictionaryKeyValuePair<TKey, TValue>> list = new List<SerializedDictionaryKeyValuePair<TKey, TValue>>();
		[SerializeField]
		Dictionary<TKey, int> indexByKey = new Dictionary<TKey, int>();
		[SerializeField, HideInInspector]
		Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

#pragma warning disable 0414
		[SerializeField, HideInInspector]
		bool keyCollision;
#pragma warning restore 0414

		// Since lists can be serialized natively by unity no custom implementation is needed
		public void OnBeforeSerialize() { }

		// Fill dictionary with list pairs and flag key-collisions .
		public void OnAfterDeserialize() {
			dict.Clear();
			indexByKey.Clear();
			keyCollision = false;

			for (int i = 0; i < list.Count; i++) {
				var key = list[i].key;
				if (key != null && !ContainsKey(key)) {
					dict.Add(key, list[i].value);
					indexByKey.Add(key, i);
				} else {
					keyCollision = true;
				}
			}
		}

		// IDictionary
		public TValue this[TKey key] {
			get => dict[key];
			set {
				dict[key] = value;

				if (indexByKey.ContainsKey(key)) {
					var index = indexByKey[key];
					list[index] = new SerializedDictionaryKeyValuePair<TKey, TValue>(key, value);
				} else {
					list.Add(new SerializedDictionaryKeyValuePair<TKey, TValue>(key, value));
					indexByKey.Add(key, list.Count - 1);
				}
			}
		}

		public ICollection<TKey> Keys => dict.Keys;
		public ICollection<TValue> Values => dict.Values;

		public void Add(TKey key, TValue value) {
			dict.Add(key, value);
			list.Add(new SerializedDictionaryKeyValuePair<TKey, TValue>(key, value));
			indexByKey.Add(key, list.Count - 1);
		}

		public bool ContainsKey(TKey key) => dict.ContainsKey(key);

		public bool Remove(TKey key) {
			if (dict.Remove(key)) {
				var index = indexByKey[key];
				list.RemoveAt(index);
				indexByKey.Remove(key);
				return true;
			} else {
				return false;
			}
		}

		public bool TryGetValue(TKey key, out TValue value) => dict.TryGetValue(key, out value);

		// ICollection
		public int Count => dict.Count;
		public bool IsReadOnly { get; set; }

		public void Add(KeyValuePair<TKey, TValue> pair) {
			Add(pair.Key, pair.Value);
		}

		public void Clear() {
			dict.Clear();
			list.Clear();
			indexByKey.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> pair) {
			TValue value;
			if (dict.TryGetValue(pair.Key, out value)) {
				return EqualityComparer<TValue>.Default.Equals(value, pair.Value);
			} else {
				return false;
			}
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			if (array == null)
				throw new ArgumentException("The array cannot be null.");
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
			if (array.Length - arrayIndex < dict.Count)
				throw new ArgumentException("The destination array has fewer elements than the collection.");

			foreach (var pair in dict) {
				array[arrayIndex] = pair;
				arrayIndex++;
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> pair) {
			TValue value;
			if (dict.TryGetValue(pair.Key, out value)) {
				bool valueMatch = EqualityComparer<TValue>.Default.Equals(value, pair.Value);
				if (valueMatch) {
					return Remove(pair.Key);
				}
			}
			return false;
		}

		// IEnumerable
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dict.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();
	}
}


#if UNITY_EDITOR
namespace Muc.Data {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(SerializedDictionary<,>), true)]
	internal class SerializedDictionaryDrawer : PropertyDrawer {

		public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label) {
			// Draw list.
			var list = property.FindPropertyRelative("list");
			string fieldName = ObjectNames.NicifyVariableName(fieldInfo.Name);
			var currentPos = new Rect(lineHeight, pos.y, pos.width, lineHeight);
			EditorGUI.PropertyField(currentPos, list, new GUIContent(fieldName), true);

			// Draw key collision warning.
			var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
			if (keyCollision) {
				currentPos.y += EditorGUI.GetPropertyHeight(list, true) + spacing;
				var entryPos = new Rect(lineHeight, currentPos.y, pos.width, lineHeight * 2f);
				EditorGUI.HelpBox(entryPos, "Duplicate keys will not be serialized.", MessageType.Warning);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			float totHeight = 0f;

			// Height of KeyValue list.
			var listProp = property.FindPropertyRelative("list");
			totHeight += EditorGUI.GetPropertyHeight(listProp, true);

			// Height of key collision warning.
			bool keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
			if (keyCollision) {
				totHeight += lineHeight * 2f + spacing;
			}

			return totHeight;
		}

	}
}
#endif


namespace Muc.Data {

	using System;

	[Serializable]
	internal struct SerializedDictionaryKeyValuePair<TKey, TValue> {

		public TKey key;
		public TValue value;

		public SerializedDictionaryKeyValuePair(TKey key, TValue value) {
			this.key = key;
			this.value = value;
		}

		public override string ToString() {
			return $"[{key.ToString()},{value.ToString()}]";
		}

	}

}

#if UNITY_EDITOR
namespace Muc.Data {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(SerializedDictionaryKeyValuePair<,>), true)]
	public class SerializedKeyValuePairDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			var propPos = position;
			propPos.xMin = 0;

			using (PropertyScope(propPos, label, property, out label)) {
				var key = property.FindPropertyRelative(nameof(SerializedDictionaryKeyValuePair<string, string>.key));
				var value = property.FindPropertyRelative(nameof(SerializedDictionaryKeyValuePair<string, string>.value));

				position.xMin -= 5 + spacing;

				var keyRect = position;
				keyRect.width /= 2;

				var valueRect = keyRect;
				valueRect.x += keyRect.width;

				keyRect.width -= 2;
				valueRect.width += 2;

				using (LabelWidthScope(10)) EditorGUI.PropertyField(keyRect, key, new GUIContent("K", "Key"));
				using (LabelWidthScope(14)) EditorGUI.PropertyField(valueRect, value, new GUIContent(" V", "Value"));
			}
		}

	}
}
#endif
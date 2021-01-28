
namespace Muc.Collections {

	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Like a normal List but enumeration doesn't throw if the collection is changed, and the enumerated objects don't change.
	/// </summary>
	[Serializable]
	public class SafeList<T> : ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T> {

		public SafeList() { list = new List<T>(); }
		public SafeList(IEnumerable<T> collection) { list = new List<T>(collection); }
		public SafeList(int capacity) { list = new List<T>(capacity); }

		[SerializeField]
		private List<T> list;
		private List<T> enumerationTarget;

		public T this[int index] { get => list[index]; set => list[index] = value; }

		public int Count => list.Count;

		bool ICollection<T>.IsReadOnly => ((ICollection<T>)list).IsReadOnly;

		private void UpdateIfRequired() {
			if (enumerationTarget == null || enumerationTarget.Count != this.Count) {
				enumerationTarget = list.ToList();
			}
		}

		public void Add(T item) {
			list.Add(item);
			enumerationTarget = null;
		}

		public bool Remove(T item) {
			var res = list.Remove(item);
			if (res) enumerationTarget = null;
			return res;
		}

		public void RemoveAt(int index) {
			list.RemoveAt(index);
			enumerationTarget = null;
		}

		public void Clear() {
			list.Clear();
			enumerationTarget = null;
		}

		public int IndexOf(T item) => list.IndexOf(item);
		public void Insert(int index, T item) => list.Insert(index, item);

		public bool Contains(T item) => list.Contains(item);
		public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<T> GetEnumerator() {
			UpdateIfRequired();
			return enumerationTarget.GetEnumerator();
		}

	}
}

#if UNITY_EDITOR
namespace Muc.Collections {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(SafeList<>), true)]
	internal class SafeListDrawer : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var prop = property.FindPropertyRelative("list");
			return EditorGUI.GetPropertyHeight(prop, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			using (PropertyScope(position, label, property, out label)) {
				var prop = property.FindPropertyRelative("list");
				EditorGUI.PropertyField(position, prop, new GUIContent(label));
			}
		}

	}
}
#endif

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;

[Serializable]
public class EventStackItemContainer<T> where T : EventStackItem {
	public T item;
	public int width;
	public int targetPosition;
	public EventStackItemContainer(T item, int targetPosition) {
		this.item = item;
		this.targetPosition = targetPosition;
		this.width = item.width;
	}
}

public class EventStack : EventStack<EventStackItem> { }

public class EventStack<T> : MonoBehaviour where T : EventStackItem {

	[SerializeField]
	protected List<EventStackItemContainer<T>> _stack;
	public IReadOnlyList<EventStackItemContainer<T>> stack => _stack;

	protected void Awake() {
		_stack.RemoveAll(v => v.item == null);
		var pos = 0;
		for (int i = 0; i < stack.Count; i++) {
			var cont = stack[i];
			cont.width = cont.item.width;
			cont.targetPosition = pos;
			pos += cont.width;
		}
	}

	protected void Update() {
		_stack.RemoveAll(v => v.item == null);
		var pos = 0;
		for (int i = 0; i < stack.Count; i++) {
			var cont = stack[i];

			cont.item.canvas.sortingOrder = -i;

			var newWidth = cont.item.width;
			var widthDiff = newWidth - cont.width;
			if (widthDiff != 0) {
				InstantPush(i + 1, widthDiff);
			}
			cont.width = newWidth;

			if (pos != cont.targetPosition) {
				cont.item.StartReposition(pos - cont.targetPosition);
				cont.targetPosition = pos;
			}
			pos += newWidth;
		}
	}

	public void Add(T item) {
		Insert(_stack.Count, item);
	}

	public void Insert(int index, T item) {
		item.transform.SetParent(transform);
		item.transform.localPosition = item.transform.localPosition.SetX(CalculateTargetPosition(index));
		var cont = new EventStackItemContainer<T>(item, Mathf.RoundToInt(item.transform.localPosition.x));
		_stack.Insert(index, cont);
		item.OnAdd();
	}

	public void Reposition(int oldIndex, int newIndex) {
		var cont = _stack[oldIndex];
		_stack.RemoveAt(oldIndex);
		_stack.Insert(newIndex, cont);
	}

	private void InstantPush(int startIndex, int offset) {
		for (int i = startIndex; i < _stack.Count; i++) {
			var cont = _stack[i];
			if (cont.item != null) {
				cont.item.rt.localPosition += new Vector3(offset, 0, 0);
				cont.targetPosition += offset;
			}
		}
	}


	private int CalculateTargetPosition(int index) {
		var pos = 0;
		for (int i = 0; i < index; i++) {
			pos += _stack[i].item.width;
		}
		return pos;
	}

	public int MaxX() {
		return _stack.Aggregate(0, (acc, cur) => !cur.item ? acc : Mathf.Max(acc, Mathf.RoundToInt(cur.item.rt.localPosition.x + cur.item.width)));
	}

	public void RemoveAt(int index) {
		_stack[index].item.StartRemove();
	}

}

#if UNITY_EDITOR
namespace EventStackEditor {

	using System.Reflection;
	using System.Linq;

	using UnityEngine;
	using UnityEditor;
	using UnityEngine.UI;
	using static Muc.Editor.EditorUtil;

	[CustomEditor(typeof(EventStack<>))]
	internal class EventStackEditor : Editor {

		public int insertIndex;

		public int destroyIndex;

		public int repositionIndex1;
		public int repositionIndex2;

		private EventStack<EventStackItem> t => (EventStack<EventStackItem>)target;

		public override void OnInspectorGUI() {
			using (HorizontalScope()) {
				insertIndex = EditorGUILayout.IntField(insertIndex);
				if (GUILayout.Button($"Insert item at {EndOffArraysize(insertIndex)}")) {
					var last = t.stack.Last();
					var newItem = Instantiate(last.item);
					newItem.GetComponentInChildren<Image>().color = new Color(Random.value, Random.value, Random.value);
					t.Insert(EndOffArraysize(insertIndex), newItem);
				}
			}
			using (HorizontalScope()) {
				destroyIndex = EditorGUILayout.IntField(destroyIndex);
				if (GUILayout.Button($"Destroy item at {EndOffArraysize(destroyIndex)}")) {
					t.RemoveAt(EndOffArraysize(destroyIndex));
				}
			}
			using (HorizontalScope()) {
				repositionIndex1 = EditorGUILayout.IntField(repositionIndex1);
				repositionIndex2 = EditorGUILayout.IntField(repositionIndex2);
				if (GUILayout.Button($"Reposition item at {EndOffArraysize(repositionIndex1)} to {EndOffArraysize(repositionIndex2)}")) {
					t.Reposition(EndOffArraysize(repositionIndex1), EndOffArraysize(repositionIndex2));
				}
			}
			DrawDefaultInspector();
		}

		private int EndOffArraysize(int index) {
			if (index >= 0) return index;
			return t.stack.Count + index;
		}
	}

}
#endif
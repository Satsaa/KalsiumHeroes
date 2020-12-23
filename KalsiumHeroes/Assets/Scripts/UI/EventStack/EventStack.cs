
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;

public class EventStack : MonoBehaviour {

	public List<EventStackItem> stack;

	public AnimationCurve expandCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public float expandTime = 1;
	public float speed = 1;

	protected virtual void Update() {
		var currentPos = 0f;
		stack.RemoveAll(v => !v);
		var maxX = 0f;
		for (int i = 0; i < stack.Count; i++) {
			var item = stack[i];
			item.time += Time.deltaTime;
			item.canvas.sortingOrder = -i;
			var rt = item.transform as RectTransform;
			rt.localScale = rt.localScale.SetX(expandCurve.Evaluate(Mathf.Clamp01(item.time / expandTime)));
			var width = item.fullWidth;
			item.targetPosition = currentPos;
			var sign = Mathf.Sign(rt.localPosition.x - item.targetPosition);
			item.velocity -= speed * sign * Time.deltaTime;
			if (Mathf.Sign(item.velocity) == sign) item.velocity = 0;
			rt.localPosition += new Vector3(item.velocity * Time.deltaTime, 0, 0);
			if (sign != Mathf.Sign(rt.localPosition.x - item.targetPosition)) {
				if (i == 0) item.OnReachZero(this);
				item.velocity = 0;
				rt.localPosition = rt.localPosition.SetX(item.targetPosition);
			}
			currentPos += width;
			maxX = Mathf.Max(maxX, rt.localPosition.x + rt.rect.max.x);
		}
	}

	public void Add(EventStackItem item) {
		item.transform.SetParent(transform);
		var pos = 0f;
		var last = stack.LastOrDefault();
		if (last != null) pos = last.transform.localPosition.x + last.fullWidth;
		item.transform.localPosition = item.transform.localPosition.SetX(pos);
		item.time = 0;
		stack.Add(item);
	}

	public void Insert(EventStackItem item, int index) {
		var pos = 0f;
		var clamped = Mathf.Clamp(index, 0, stack.Count - 1);
		if (clamped > 0) {
			var prev = stack[index - 1];
			if (prev != null) pos = prev.transform.localPosition.x + prev.fullWidth;
		}
		item.transform.localPosition = item.transform.localPosition.SetX(pos);
		item.time = 0;
		stack.Insert(index, item);
	}

	public void Reposition(int oldIndex, int newIndex) {
		var item = stack[oldIndex];
		stack.RemoveAt(oldIndex);
		stack.Insert(newIndex, item);
	}

	public void Remove(int index) {
		stack.RemoveAt(index);
	}

}

#if UNITY_EDITOR
namespace EventStackEditor {

	using System.Reflection;
	using System.Linq;

	using UnityEngine;
	using UnityEditor;
	using UnityEngine.UI;

	[CustomEditor(typeof(EventStack))]
	internal class EventStackEditor : Editor {

		private EventStack t => (EventStack)target;

		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if (GUILayout.Button("Clone last")) {
				var last = t.stack.Last();
				var newItem = Instantiate(last, t.transform);
				newItem.GetComponentInChildren<Image>().color = new Color(Random.value, Random.value, Random.value);
				t.Add(newItem);
			}
			if (GUILayout.Button("Insert at 1")) {
				var last = t.stack.Last();
				var newItem = Instantiate(last, t.transform);
				newItem.GetComponentInChildren<Image>().color = new Color(Random.value, Random.value, Random.value);
				t.Insert(newItem, 1);
			}
			if (GUILayout.Button("Destroy all but 2")) {
				for (int i = 0; i < t.stack.Count; i++) {
					if (i > 1) {
						Destroy(t.stack[i].gameObject);
					}
				}
			}
		}
	}

}
#endif
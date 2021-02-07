
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;

[RequireComponent(typeof(Windows))]
[DefaultExecutionOrder(-1)]
public class Tooltips : MonoBehaviour {

	public static Tooltips instance => _instance;
	private static Tooltips _instance;

	[Serializable]
	class StackItem {
		public string id;
		public bool windowed;
		public GameObject gameObject;
		public StackItem(string id, GameObject gameObject) { this.id = id; this.gameObject = gameObject; }
	}
	[SerializeField] SerializedStack<StackItem> stack;
	[SerializeField] SerializedDictionary<string, GameObject> tooltips;
	[SerializeField] int frameBuffer;
	[SerializeField] int framesLeft;
	[SerializeField] GameObject windowPrefab;

	private void OnValidate() => Awake();
	private void Awake() {
		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {nameof(Tooltips)} GameObjects. Exterminating.");
			ObjectUtil.Destroy(this);
			return;
		}

		_instance = this;

	}

	void Update() {
		if (stack.Any()) {
			if (IsTopHovered()) {
				framesLeft = frameBuffer;
			} else {
				if (framesLeft <= 0) {
					Pop();
				}
				framesLeft--;
			}
		}
	}

	void Pop() {
		Prune();
		var popped = stack.Pop();
		if (!popped.windowed) Destroy(popped.gameObject);
	}

	void Prune() {
		if (!stack.Any()) return;
		while (!stack.Peek().gameObject) {
			stack.Pop();
		}
	}

	StackItem Peek() {
		Prune();
		return stack.Peek();
	}

	void Push(StackItem item) {
		Prune();
		stack.Push(item);
	}

	bool Any() {
		Prune();
		return stack.Any();
	}

	public bool Windowize() {
		if (!Any()) return false;
		var top = Peek();
		if (top.windowed) return false;
		top.windowed = true;
		var go = Instantiate(windowPrefab, top.gameObject.transform.position, top.gameObject.transform.rotation, Windows.instance.transform);
		Windows.instance.MoveToTop(go.transform);
		var window = go.GetComponent<Window>();
		var tt = top.gameObject.GetComponentInChildren<Tooltip>();
		tt.MoveContent(window.content.contentParent);
		top.gameObject = go;
		return true;
	}

	public bool Upkeep(string id, GameObject creator, Rect creatorRect) {
		var tt = tooltips[id];
		if (!Any() || Peek().id != id) {
			if (Any() && !IsTopHovered()) return false;
			framesLeft = frameBuffer;
			return true;
		} else {
			framesLeft = frameBuffer;
		}
		return false;
	}

	public bool Show(string id, GameObject creator, Rect creatorRect) {
		var tt = tooltips[id];
		if (!Any() || Peek().id != id) {
			if (Any() && !IsTopHovered()) return false;
			framesLeft = frameBuffer;
			var go = Instantiate(tt, Windows.instance.transform);
			Windows.instance.MoveToTop(go.transform);
			Push(new StackItem(id, go));
			var rt = (RectTransform)go.transform;
			var x = creatorRect.center.x;
			var y = creatorRect.yMax + rt.pivot.y * rt.rect.height;
			rt.position = rt.position.SetX(x).SetY(y);
			var ssRect = rt.ScreenRect();
			if (ssRect.yMax > Screen.height) { // Clips screen top?
				var bx = creatorRect.center.x;
				var by = creatorRect.yMin - (1 - rt.pivot.y) * rt.rect.height;
				rt.position = rt.position.SetX(bx).SetY(by);
			}
			return true;
		} else {
			framesLeft = frameBuffer;
		}
		return false;
	}

	private List<RaycastResult> raycasts = new List<RaycastResult>();
	private bool IsTopHovered() {
		if (Any()) {
			var e = EventSystem.current;
			if (e.IsPointerOverGameObject()) {
				var eData = new PointerEventData(e) {
					position = Input.mousePosition
				};
				e.RaycastAll(eData, raycasts);
				var parent = raycasts.First().gameObject.transform;
				var top = Peek();
				while (parent != null) {
					if (parent == top.gameObject.transform) {
						return true;
					}
					parent = parent.parent;
				}
			}
		}
		return false;
	}
}
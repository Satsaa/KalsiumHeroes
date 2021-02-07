
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;

[RequireComponent(typeof(Windows))]
public class Tooltips : MonoBehaviour {

	public static Tooltips instance => _instance;
	private static Tooltips _instance;

	[Serializable]
	class StackItem {
		public string id;
		public GameObject gameObject;
		public bool isWindow;
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

	private List<RaycastResult> raycasts = new List<RaycastResult>();
	void Update() {
		if (stack.Any()) {
			var top = stack.Peek();
			var cur = EventSystem.current;
			if (cur.IsPointerOverGameObject()) {
				var ptrData = new PointerEventData(cur);
				ptrData.position = Input.mousePosition;
				cur.RaycastAll(ptrData, raycasts);
				if (raycasts.Any()) {
					var isTopStack = false;
					var parent = raycasts.First().gameObject.transform;
					while (parent != null) {
						if (parent == top.gameObject.transform) {
							isTopStack = true;
							break;
						}
						parent = parent.parent;
					}
					if (isTopStack) {
						framesLeft = frameBuffer;
					} else {
						TryPop();
					}
				}
			} else {
				TryPop();
			}
			framesLeft--;
		}
	}

	void TryPop() {
		if (framesLeft <= 0) {
			Pop();
		}
	}
	void Pop() {
		var popped = stack.Pop();
		if (popped.isWindow) {

		} else {
			Destroy(popped.gameObject);
		}

	}

	public bool Windowize() {
		if (!stack.Any()) return false;
		var top = stack.Peek();
		if (top.isWindow) return false;
		top.isWindow = true;
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
		Debug.DrawLine(new Vector3(creatorRect.xMin, creatorRect.yMin, 0), new Vector3(creatorRect.xMin, creatorRect.yMax, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMin, creatorRect.yMin, 0), new Vector3(creatorRect.xMax, creatorRect.yMin, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMax, creatorRect.yMax, 0), new Vector3(creatorRect.xMin, creatorRect.yMax, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMax, creatorRect.yMax, 0), new Vector3(creatorRect.xMax, creatorRect.yMin, 0), Color.red);
		if (!stack.Any() || stack.Peek().id != id) {
			if (stack.Any()) {
				// Allow creating tooltips only from the top tooltip
				var top = stack.Peek();
				var isTopStack = false;
				var parent = creator.transform;
				while (parent != null) {
					if (parent == top.gameObject.transform) {
						isTopStack = true;
						break;
					}
					parent = parent.parent;
				}
				if (!isTopStack) return false;
			}

			framesLeft = frameBuffer;
			return true;
		} else {
			framesLeft = frameBuffer;
		}
		return false;
	}

	public bool Show(string id, GameObject creator, Rect creatorRect) {
		var tt = tooltips[id];
		Debug.DrawLine(new Vector3(creatorRect.xMin, creatorRect.yMin, 0), new Vector3(creatorRect.xMin, creatorRect.yMax, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMin, creatorRect.yMin, 0), new Vector3(creatorRect.xMax, creatorRect.yMin, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMax, creatorRect.yMax, 0), new Vector3(creatorRect.xMin, creatorRect.yMax, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMax, creatorRect.yMax, 0), new Vector3(creatorRect.xMax, creatorRect.yMin, 0), Color.red);
		if (!stack.Any() || stack.Peek().id != id) {
			if (stack.Any()) {
				// Allow creating tooltips only from the top tooltip
				var top = stack.Peek();
				var isTopStack = false;
				var parent = creator.transform;
				while (parent != null) {
					if (parent == top.gameObject.transform) {
						isTopStack = true;
						break;
					}
					parent = parent.parent;
				}
				if (!isTopStack) return false;
			}

			framesLeft = frameBuffer;
			var go = Instantiate(tt, Windows.instance.transform);
			Windows.instance.MoveToTop(go.transform);
			stack.Push(new StackItem(id, go));
			var rt = (RectTransform)go.transform;
			var x = creatorRect.center.x;
			var y = creatorRect.yMax + rt.pivot.y * rt.rect.height;
			rt.position = rt.position.SetX(x).SetY(y);
			var ssRect = rt.ScreenRect();
			if (ssRect.yMax > Screen.height) {
				// Position below instead
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

}
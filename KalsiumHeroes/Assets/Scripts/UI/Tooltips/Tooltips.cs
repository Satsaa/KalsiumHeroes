
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class Tooltips : MonoBehaviour {

	public static Tooltips instance => _instance;
	private static Tooltips _instance;

	[Serializable]
	class StackItem {
		public string id;
		public GameObject gameObject;
		public StackItem(string id, GameObject gameObject) { this.id = id; this.gameObject = gameObject; }
	}
	[SerializeField] SerializedStack<StackItem> stack;
	[SerializeField] SerializedDictionary<string, GameObject> tooltips;
	[SerializeField] int frameBuffer;
	[SerializeField] int framesLeft;

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
		Destroy(stack.Pop().gameObject);
	}

	public bool Show(string id, GameObject creator) {
		var tt = tooltips[id];
		if (!stack.Any() || stack.Peek().id != id) {
			if (stack.Any()) {
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
			var go = Instantiate(tt, transform);
			stack.Push(new StackItem(id, go));
			var rt = (RectTransform)go.transform;
			var x = Input.mousePosition.x;
			var y = Input.mousePosition.y + rt.sizeDelta.y / 2;
			rt.position = rt.position.SetX(x).SetY(y);
			return true;
		} else {
			framesLeft = frameBuffer;
		}
		return false;
	}

}
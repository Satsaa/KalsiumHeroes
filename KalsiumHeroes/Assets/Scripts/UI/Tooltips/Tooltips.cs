
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using Muc.Time;

[RequireComponent(typeof(Windows))]
[DefaultExecutionOrder(-1)]
public class Tooltips : MonoBehaviour {

	public static Tooltips instance => _instance;
	private static Tooltips _instance;

	[SerializeField, HideInInspector] SerializedStack<Tooltip> tts;
	[SerializeField] SerializedDictionary<string, Tooltip> tooltips;
	[SerializeField] Window windowPrefab;

	[SerializeField] TooltipAnimator animatorPrefab;

	private FrameTimeout hideFrameDelay = new FrameTimeout(2, true);
	[SerializeField] Timeout hideDelay = new Timeout(0.5f, true);
	[SerializeField] Timeout showDelay = new Timeout(0.5f, true);
	[SerializeField] Timeout quickSwitchDelay = new Timeout(0.1f, true);

	string lastPing;
	int lastPingFrame = -1;

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
		if (Time.frameCount - lastPingFrame > 1) {
			// Nothing hovered
			showDelay.Reset(true);
			quickSwitchDelay.Reset(true);
		}
		if (tts.Any()) {
			GetHovered(out var hovered);
			if (IsTopHovered()) {
				hideFrameDelay.Reset(true);
				hideDelay.Reset(true);
			} else {
				hideDelay.paused = false;
				hideFrameDelay.paused = false;
				if (hideDelay.expired) {
					if (hideFrameDelay.expired) {
						showDelay.Reset(true);
						if (hovered) {
							while (tts.Count > 0 && hovered.index <= Peek().index - 1 &&
								!(hovered.index == Peek().index - 1 && Peek().id == lastPing)
							) {
								Pop();
							}
						} else {
							while (tts.Count > 0 && !(tts.Count == 1 && Time.frameCount - lastPingFrame <= 1 && Peek().id == lastPing)) {
								Pop();
							}
						}
						return;
					}
				}
			}
		}
	}

	void Pop() {
		Prune();
		var popped = tts.Pop();
		if (popped.gameObject.GetComponentInParent<Window>()) {
			popped.index = -1;
		} else {
			var animator = popped.gameObject.GetComponentInParent<TooltipAnimator>();
			if (animator) animator.Hide();
		}
	}

	void Prune() {
		while (tts.Any() && !tts.Peek()) {
			tts.Pop();
		}
	}

	Tooltip Peek() {
		Prune();
		return tts.Peek();
	}

	void Push(Tooltip item) {
		Prune();
		tts.Push(item);
	}

	bool Any() {
		Prune();
		return tts.Any();
	}

	public bool Windowize() {
		if (!Any()) return false;
		var top = Peek();
		if (top.gameObject.GetComponentInParent<Window>()) return false;
		var animator = top.GetComponentInParent<TooltipAnimator>();
		if (animator) animator.FinishAnims();
		var window = Instantiate(windowPrefab, top.gameObject.transform.position, top.gameObject.transform.rotation, Windows.instance.transform);
		Windows.instance.MoveToTop(window.gameObject.transform);
		top.MoveContent(window.content.contentParent);
		top.root = window.gameObject;
		window.gameObject.transform.Translate(0, 10, 0);
		if (animator) Destroy(animator.gameObject);
		return true;
	}

	public bool Ping(string id, GameObject creator, Rect creatorRect) {
		GetTooltipPrefab(id);
		lastPing = id;
		lastPingFrame = Time.frameCount;
		if (!Any() || Peek().creator != creator || Peek().id != id) {
			if (Any() && !IsTopHovered()) return false;
			hideFrameDelay.Reset(true);
			hideDelay.Reset(true);
			return true;
		} else {
			hideFrameDelay.Reset(true);
			hideDelay.Reset(true);
			showDelay.Reset(true);
		}
		return false;
	}

	public bool Show(string id, GameObject creator, Rect creatorRect) {
		Debug.DrawLine(new Vector3(creatorRect.xMin, creatorRect.yMin, 0), new Vector3(creatorRect.xMin, creatorRect.yMax, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMin, creatorRect.yMin, 0), new Vector3(creatorRect.xMax, creatorRect.yMin, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMax, creatorRect.yMax, 0), new Vector3(creatorRect.xMin, creatorRect.yMax, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMax, creatorRect.yMax, 0), new Vector3(creatorRect.xMax, creatorRect.yMin, 0), Color.red);
		var isTop = GetHovered(out var hovered) ? hovered.index == tts.Count - 1 : tts.Count - 1 == -1;
		if (!isTop && (!Any() || Peek().id != id)) {
			// Quick switching
			quickSwitchDelay.paused = false;
			if (quickSwitchDelay.expired) {
				if (hovered) {
					while (tts.Count > hovered.index + 1) {
						Pop();
					}
				} else {
					while (tts.Count > 0) {
						Pop();
					}
				}
				showDelay.Reset(true);
			} else {
				Ping(id, creator, creatorRect);
				return false;
			}
		} else {
			// Normal delay test
			showDelay.paused = false;
			if (showDelay.expired) {
				showDelay.Reset(true);
			} else {
				Ping(id, creator, creatorRect);
				return false;
			}
		}
		hideFrameDelay.Reset(true);
		hideDelay.Reset(true);
		var animator = Instantiate(animatorPrefab, creatorRect.center, Quaternion.identity, Windows.instance.transform);
		var tt = Instantiate(GetTooltipPrefab(id), animator.transform);
		Windows.instance.MoveToTop(animator.transform);
		tt.id = id;
		tt.index = tts.Count;
		tt.root = tt.gameObject;
		tt.creator = creator;
		Push(tt);
		var rt = (RectTransform)tt.transform;
		rt.position = new Vector3(creatorRect.center.x, creatorRect.yMax + rt.pivot.y * rt.rect.height);
		if (rt.ScreenRect().yMax > Screen.height) { // Clips screen top?
			var bx = creatorRect.center.x;
			var by = creatorRect.yMin - (1 - rt.pivot.y) * rt.rect.height;
			animator.transform.Translate(0, creatorRect.height / -2f, 0);
			rt.position = rt.position.SetX(bx).SetY(by);
		} else {
			var ws = rt.position;
			animator.transform.Translate(0, creatorRect.height / 2f, 0);
			rt.position = ws;
		}
		animator.Show();
		return true;
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
					if (parent == top.root.transform) {
						return true;
					}
					parent = parent.parent;
				}
			}
		}
		return false;
	}

	private bool GetHovered(out Tooltip tooltip) {
		if (Any()) {
			var e = EventSystem.current;
			if (e.IsPointerOverGameObject()) {
				var eData = new PointerEventData(e) {
					position = Input.mousePosition
				};
				e.RaycastAll(eData, raycasts);
				var parent = raycasts.First().gameObject.transform;
				tooltip = parent.GetComponentInParent<Tooltip>();
				if (tooltip) {
					return true;
				}
			}
		}
		tooltip = default;
		return false;
	}

	private Tooltip GetTooltipPrefab(string id) {
		if (id.StartsWith("lib_")) {
			id = id.Substring(4);
			return Game.library.tooltips[id];
		}
		return tooltips[id];
	}
}
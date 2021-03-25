
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using Muc.Time;

[DefaultExecutionOrder(-1)]
public class Tooltips : Singleton<Tooltips> {

	[SerializeField, HideInInspector] List<Tooltip> tts;
	[SerializeField] SerializedDictionary<string, Tooltip> tooltips;

	[SerializeField] TooltipAnimator animatorPrefab;

	private FrameTimeout hideFrameDelay = new FrameTimeout(2, true);
	[SerializeField] Timeout hideDelay = new Timeout(0.5f, true);
	[SerializeField] Timeout showDelay = new Timeout(0.5f, true);
	[SerializeField] Timeout quickSwitchDelay = new Timeout(0.1f, true);

	string lastPing;
	int lastPingFrame = -1;


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
							while (
								tts.Count > 0 && hovered.index <= tts.Last().index - 1 &&
								!(hovered.index == tts.Last().index - 1 && tts.Last().id == lastPing)
							) {
								Pop();
							}
						} else {
							while (tts.Count > 0 && !(tts.Count == 1 && Time.frameCount - lastPingFrame <= 1 && tts.Last().id == lastPing)) {
								Pop();
							}
						}
						return;
					}
				}
			}
		}
	}

	public void RemoveListing(Tooltip tooltip) {
		tts.Remove(tooltip);
	}

	private Tooltip Pop() {
		var popped = tts.Last();
		tts.RemoveAt(tts.Count - 1);
		popped.index = -1;
		popped.OnHide();
		return popped;
	}

	public void InvokeOnCreatorClicked(Rect creatorRect) {
		if (!tts.Any()) return;
		var top = tts.Last();
		if (top) top.OnCreatorClicked(creatorRect);
	}

	public bool Ping(string id, GameObject creator, Rect creatorRect) {
		GetTooltipPrefab(id);
		lastPing = id;
		lastPingFrame = Time.frameCount;
		if (!tts.Any() || tts.Last().creator != creator || tts.Last().id != id) {
			if (tts.Any() && !IsTopHovered()) return false;
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

	public bool Show(string id, GameObject creator, Rect creatorRect, Action<Tooltip> initializer = null) {
		Debug.DrawLine(new Vector3(creatorRect.xMin, creatorRect.yMin, 0), new Vector3(creatorRect.xMin, creatorRect.yMax, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMin, creatorRect.yMin, 0), new Vector3(creatorRect.xMax, creatorRect.yMin, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMax, creatorRect.yMax, 0), new Vector3(creatorRect.xMin, creatorRect.yMax, 0), Color.red);
		Debug.DrawLine(new Vector3(creatorRect.xMax, creatorRect.yMax, 0), new Vector3(creatorRect.xMax, creatorRect.yMin, 0), Color.red);
		var isTop = GetHovered(out var hovered) ? hovered.index == tts.Count - 1 : tts.Count - 1 == -1;
		if (!isTop && (!tts.Any() || tts.Last().creator != creator || tts.Last().id != id)) {
			// Quick switching
			quickSwitchDelay.paused = false;
			if (quickSwitchDelay.expired) {
				if (hovered) {
					while (tts.Count > hovered.index + 2) {
						Pop();
					}
					if (tts.Last().id != id || tts.Last().creator != creator) {
						Pop();
					} else {
						return false;
					}
				} else {
					while (tts.Count > 1) {
						Pop();
					}
					if (tts.Last().id != id || tts.Last().creator != creator) {
						Pop();
					} else {
						return false;
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
		var animator = Instantiate(animatorPrefab, creatorRect.center, Quaternion.identity, transform);
		var tt = Instantiate(GetTooltipPrefab(id), animator.transform);
		tt.id = id;
		tt.index = tts.Count;
		tt.root = tt.gameObject;
		tt.creator = creator;
		tts.Add(tt);
		var rt = (RectTransform)tt.transform;
		if (initializer != null) {
			initializer(tt);
		}
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
		rt.position = new Vector3(creatorRect.center.x, creatorRect.yMax + rt.pivot.y * rt.rect.height * rt.lossyScale.y);
		var screenRect = rt.ScreenRect();
		if (screenRect.yMax > Screen.height) { // Clips screen top?
			var bx = creatorRect.center.x;
			var by = creatorRect.yMin - (1 - rt.pivot.y) * screenRect.height;
			animator.transform.Translate(0, creatorRect.height / -2f, 0);
			rt.position = rt.position.SetX(bx).SetY(by);
		} else {
			var ws = rt.position;
			animator.transform.Translate(0, creatorRect.height / 2f, 0);
			rt.position = ws;
		}
		tt.OnShow(animator);
		return true;
	}

	private List<RaycastResult> raycasts = new List<RaycastResult>();
	private bool IsTopHovered() {
		if (tts.Any()) {
			var e = EventSystem.current;
			if (e.IsPointerOverGameObject()) {
				var eData = new PointerEventData(e) {
					position = App.input.pointer
				};
				e.RaycastAll(eData, raycasts);
				var parent = raycasts.First().gameObject.transform;
				var top = tts.Last();
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
		if (tts.Any()) {
			var e = EventSystem.current;
			if (e.IsPointerOverGameObject()) {
				var eData = new PointerEventData(e) {
					position = App.input.pointer
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
		return tooltips[id];
	}

}
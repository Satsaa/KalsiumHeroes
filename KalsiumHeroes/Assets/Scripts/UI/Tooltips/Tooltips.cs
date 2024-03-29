
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using Muc.Time;
using Muc.Components.Extended;
using Muc.Data;

[DefaultExecutionOrder(-1)]
public class Tooltips : UISingleton<Tooltips> {

	[field: SerializeField] public Window defaultWindowPrefab { get; protected set; }
	[SerializeField] Timeout hideDelay = new(0.5f, true);
	[SerializeField] Timeout showDelay = new(0.5f, true);
	[SerializeField] Timeout quickSwitchDelay = new(0.1f, true);
	[SerializeField] SerializedDictionary<string, Tooltip> tooltips;

	[SerializeField, HideInInspector] List<Tooltip> tts;
	FrameTimeout hideFrameDelay = new(2, true);
	string lastPing;
	int lastPingFrame = -1;

	protected virtual void Update() {
		if (Game.game && Game.targeting.targeting) {
			while (tts.Count > 0) Pop();
			return;
		}
		if (Time.frameCount - lastPingFrame > 1) {
			// Nothing hovered
			showDelay.Reset(true);
			quickSwitchDelay.Reset(true);
		}
		if (tts.Count > 0) {
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
								!(hovered.index == tts.Last().index - 1 && tts.Last().query == lastPing)
							) {
								Pop();
							}
						} else {
							while (tts.Count > 0 && !(tts.Count == 1 && Time.frameCount - lastPingFrame <= 1 && tts.Last().query == lastPing)) {
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

	public bool TooltipExists(string tooltipId) {
		return tooltips.ContainsKey(tooltipId);
	}

	private Tooltip Pop() {
		var popped = tts.Last();
		tts.RemoveAt(tts.Count - 1);
		if (popped) {
			popped.index = -1;
			popped.OnHide();
		}
		return popped;
	}

	//TODO: Only works for top tooltip
	public void InvokeOnCreatorClicked(Rect innerRect) {
		if (tts.Count == 0) return;
		var top = tts.Last();
		if (top) top.OnCreatorClicked(innerRect);
	}

	/// <summary>
	/// Pings a Tooltip so it doesn't get automatically hidden.
	/// </summary>
	/// <param name="query">Tooltip query.</param>
	/// <param name="creator">The RectTransform of the UI object that is creating the Tooltip.</param>
	/// <param name="innerRect">A Rect in the space of <b>creator</b> that represents an area where the Tooltip is created from.</param>
	/// <returns>True if the ping succeeds.</returns>
	public bool Ping(string query, RectTransform creator, Rect innerRect) {
		if (Game.game && Game.targeting.targeting) return false;
		lastPing = query;
		lastPingFrame = Time.frameCount;
		if (tts.Count == 0 || tts.Last().creator != creator || tts.Last().query != query) {
			if (tts.Count > 0 && !IsTopHovered()) return false;
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

	/// <summary>
	/// Shows a Tooltip after called in update for a sufficient duration and other checks pass.
	/// </summary>
	/// <param name="query">Query string for the Tooltip. Format: "{tooltip_id}[:{data_id}]..."</param>
	/// <param name="creator">The RectTransform of the UI object that is creating the Tooltip.</param>
	/// <param name="innerRect">A Rect in the space of <b>creator</b> that represents an area where the Tooltip is created from. Use RectTransform.rect to use the whole object.</param>
	/// <param name="canvasCamera">Camera of the associated Canvas, if any.</param>
	/// <param name="initializer">Optional intializer function ran before layouting.  </param>
	/// <returns>True when the specific Tooltip was shown.</returns>
	public bool Show(string query, RectTransform creator, Rect innerRect, Camera canvasCamera, Action<Tooltip> initializer = null) {
		if (Game.game && Game.targeting.targeting) return false;

		var creatorCenter = creator.transform.TransformPoint(innerRect.center);
		var creatorMin = creator.transform.TransformPoint(innerRect.min);
		var creatorMax = creator.transform.TransformPoint(innerRect.max);
		var creatorTop = creator.transform.TransformPoint(innerRect.center + new Vector2(0, innerRect.height / 2));
		var creatorBot = creator.transform.TransformPoint(innerRect.center + new Vector2(0, -innerRect.height / 2));

#if UNITY_EDITOR
		{ // Draw a rectangle around the innerRect
			var creatorXMinYMax = creator.transform.TransformPoint(new Vector2(innerRect.xMin, innerRect.yMax));
			var creatorXMaxYMin = creator.transform.TransformPoint(new Vector2(innerRect.xMax, innerRect.yMin));

			Debug.DrawLine(creatorMin, creatorXMinYMax, Color.blue);
			Debug.DrawLine(creatorMin, creatorXMaxYMin, Color.blue);
			Debug.DrawLine(creatorMax, creatorXMinYMax, Color.blue);
			Debug.DrawLine(creatorMax, creatorXMaxYMin, Color.blue);
		}
#endif

		Parse(query, out var id, out var values);

		var isTop = GetHovered(out var hovered) ? hovered.index == tts.Count - 1 : tts.Count - 1 == -1;
		if (!isTop && (tts.Count == 0 || tts.Last().creator != creator || tts.Last().query != query)) {
			// Quick switching
			quickSwitchDelay.paused = false;
			if (quickSwitchDelay.expired) {
				if (hovered) {
					while (tts.Count > hovered.index + 2) {
						Pop();
					}
					if (tts.Last().query != query || tts.Last().creator != creator) {
						Pop();
					} else {
						return false;
					}
				} else {
					while (tts.Count > 1) {
						Pop();
					}
					if (tts.Last().query != query || tts.Last().creator != creator) {
						Pop();
					} else {
						return false;
					}
				}
				showDelay.Reset(true);
			} else {
				Ping(query, creator, innerRect);
				return false;
			}
		} else {
			// Normal delay test
			showDelay.paused = false;
			if (showDelay.expired) {
				showDelay.Reset(true);
			} else {
				Ping(query, creator, innerRect);
				return false;
			}
		}
		hideFrameDelay.Reset(true);
		hideDelay.Reset(true);
		var tt = InstantiateTooltip(id, values);
		tt.query = query;
		tt.index = tts.Count;
		tt.creator = creator;
		tts.Add(tt);
		var rt = (RectTransform)tt.transform;
		initializer?.Invoke(tt);
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

		rt.position = creatorTop;
		rt.pivot = rt.pivot.SetY(0);

		var topScreenPos = RectTransformUtility.WorldToScreenPoint(canvasCamera, rt.TransformPoint(rt.rect.max)).y;
		if (topScreenPos > Screen.height) {
			rt.position = creatorBot;
			rt.pivot = rt.pivot.SetY(1);
		}

		tt.OnShow();
		return true;
	}

	public bool Hide(string query, RectTransform creator) {
		if (tts.Count > 0 && tts.Last().creator == creator && tts.Last().query == query) {
			Pop();
			return true;
		}
		return false;
	}

	private bool IsTopHovered() {
		if (tts.Count > 0 && CustomInputModule.IsPointerOverUI()) {
			var hovered = CustomInputModule.GetHoveredGameObject();
			var parent = hovered.transform;
			var top = tts.Last();
			while (parent != null) {
				if (parent == top.transform) {
					return true;
				}
				parent = parent.parent;
			}
		}
		return false;
	}

	private bool GetHovered(out Tooltip tooltip) {
		if (tts.Count > 0 && CustomInputModule.IsPointerOverUI()) {
			var hovered = CustomInputModule.GetHoveredGameObject();
			var parent = hovered.transform;
			tooltip = parent.GetComponentInParent<Tooltip>();
			if (tooltip) {
				return true;
			}
		}
		tooltip = default;
		return false;
	}

	private Tooltip GetTooltip(string query, out string tooltipId, out string[] values) {
		Parse(query, out tooltipId, out values);
		tooltips.TryGetValue(query, out var res);
		return res;
	}

	private Tooltip InstantiateTooltip(string id, string[] parameters) {
		var res = Instantiate(tooltips[id], transform);
		foreach (var param in parameters) {
			ValueReceiver.SendValue(res.gameObject, ParseParamValue(param));
		}
		return res;
	}

	private static object ParseParamValue(string param) {
		switch (param[0]) {
			case '#':
				if (!int.TryParse(param[1..], out var instanceId)) {
					throw new ArgumentException(MakeThrowString("The instance id is not a valid integer."), "query");
				}
				var obj = ObjectUtil.FindObjectFromInstanceID(instanceId);
				if (obj is null) {
					throw new ArgumentException(MakeThrowString("The instance id does not result in a valid Object."), "query");
				}
				return obj;
			case '@':
				return Lang.GetStr(param[1..]);
			default:
				switch (param) {
					case "true":
						return true;
					case "false":
						return false;
					default:
						if (param.Contains('.')) {
							return float.Parse(param, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
						} else if (param.All(c => c >= '0' && c <= '9')) {
							return int.Parse(param, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
						} else {
							var dataObject = App.library.GetById<KalsiumObject>(param);
							if (dataObject == null) throw new ArgumentException(MakeThrowString($"The identifier specified {nameof(KalsiumObject)} was not found."), "query");
							return dataObject;
						}
				}
		}
		string MakeThrowString(string reason) => $"Invalid tooltip query parameter: \"${param}\". {reason}";
	}

	private void Parse(string query, out string tooltipId, out string[] parameters) {

		var tokens = new List<string>();

		var token = "";
		var dashing = false;

		foreach (var current in query) {
			if (dashing) {
				dashing = false;
				token += current;
			} else if (current == '\\') {
				dashing = true;
				continue;
			} else {
				if (current == ':') {
					tokens.Add(token);
					token = "";
					continue;
				}
				token += current;
			}
		}

		tokens.Add(token);

		tooltipId = tokens[0];
		parameters = tokens.Skip(1).ToArray();

	}

}
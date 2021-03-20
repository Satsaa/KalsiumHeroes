using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

/// <summary>
/// The main purpose of this is to prevent activating buttons that are not raycastable.
/// </summary>
public class CustomInputModule : StandaloneInputModule {

	public override void Process() {
		if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
			return;

		bool usedEvent = SendUpdateEventToSelectedObject();

		// case 1004066 - touch / mouse events should be processed before navigation events in case
		// they change the current selected gameobject and the submit button is a touch / mouse button.

		// touch needs to take precedence because of the mouse emulation layer
		if (!ProcessTouchEvents() && input.mousePresent)
			ProcessMouseEvent();

		if (eventSystem.sendNavigationEvents) {
			if (!usedEvent)
				usedEvent |= SendMoveEventToSelectedObject();

			if (!usedEvent)
				usedEvent |= CustomSendSubmitEventToSelectedObject();
		}
	}


	private static List<RaycastResult> raycasts = new List<RaycastResult>();

	public static bool IsDirectlyRaycastable(GameObject gameObject) {
		var e = EventSystem.current;
		if (gameObject.transform is RectTransform rt) {
			var screenRect = rt.ScreenRect();

			var eData = new PointerEventData(e) {
				position = screenRect.center,
			};
			e.RaycastAll(eData, raycasts);
			if (!raycasts.Any()) return false;
			var current = raycasts.First().gameObject.transform;
			while (current) {
				if (current == gameObject.transform) {
					return true;
				}
				current = current.transform.parent;
			}
			return false;
		}
		return false;
	}


	/// <summary>
	/// Calculate and send a submit event to the current selected object.
	/// </summary>
	/// <returns>If the submit event was used by the selected object.</returns>
	protected bool CustomSendSubmitEventToSelectedObject() {
		if (eventSystem.currentSelectedGameObject == null)
			return false;

		var data = GetBaseEventData();

		// mouse0 check so you cant click and enter at same frame, submitting twice
		if (input.GetButtonDown(submitButton) && !Input.GetKey(KeyCode.Mouse0) && IsDirectlyRaycastable(eventSystem.currentSelectedGameObject))
			ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

		if (input.GetButtonDown(cancelButton))
			ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
		return data.used;
	}


	// Copied privates

	private bool ShouldIgnoreEventsOnNoFocus() {
#if UNITY_EDITOR
		return !UnityEditor.EditorApplication.isRemoteConnected;
#else
            return true;
#endif
	}

	private bool ProcessTouchEvents() {
		for (int i = 0; i < input.touchCount; ++i) {
			Touch touch = input.GetTouch(i);

			if (touch.type == TouchType.Indirect)
				continue;

			bool released;
			bool pressed;
			var pointer = GetTouchPointerEventData(touch, out pressed, out released);

			ProcessTouchPress(pointer, pressed, released);

			if (!released) {
				ProcessMove(pointer);
				ProcessDrag(pointer);
			} else
				RemovePointerData(pointer);
		}
		return input.touchCount > 0;
	}
}


using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using Muc.Time;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour {

	public KeyCode key = KeyCode.Mouse0;
	new public Camera camera;
	public Vector3 dragOffset;

	public UnityEvent onDragStart;
	public UnityEvent onDrag;
	public UnityEvent onDragEnd;

	bool dragging;
	public Vector3 startPos { get; private set; }
	public Vector2 startScreenPos { get; private set; }
	public float startScreenDist { get; private set; }

	[SerializeField, HideInInspector] protected bool resetCalled;

	protected void Reset() {
		onDrag ??= new UnityEvent();
#if UNITY_EDITOR
		UnityEditor.Events.UnityEventTools.AddPersistentListener(onDrag, OnDrag);
#else
		onDrag.AddListener(OnDrag);
#endif
	}

	protected void Awake() {
		if (!resetCalled) {
			Reset();
		}
		if (!camera) Debug.Assert(camera = Camera.main, this);
	}

	protected void OnMouseOver() {
		if (!dragging && Input.GetKeyDown(key) && !EventSystem.current.currentSelectedGameObject) {
			dragging = true;
			startPos = transform.position;
			startScreenPos = Input.mousePosition;
			startScreenDist = Vector3.Distance(camera.transform.position, transform.position);
			onDragStart?.Invoke();
		}
	}

	protected void Update() {
		if (dragging) {
			onDrag?.Invoke();
			if (!Input.GetKey(key)) {
				dragging = false;
				onDragEnd?.Invoke();
			}
		}
	}

	public virtual void OnDrag() {
		var ray = camera.ScreenPointToRay(Input.mousePosition);
		var endPos = ray.origin + ray.direction * startScreenDist;
		transform.position = endPos + dragOffset;
	}

}
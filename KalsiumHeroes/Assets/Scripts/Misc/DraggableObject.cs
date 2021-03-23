
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using Muc.Time;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour {

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
		if (!resetCalled) Reset();
		Debug.Assert(camera, this);
	}

	protected void OnMouseOver() {
		if (!dragging && App.input.primaryDown && !EventSystem.current.currentSelectedGameObject) {
			dragging = true;
			startPos = transform.position;
			startScreenPos = App.input.pointer;
			startScreenDist = Vector3.Distance(camera.transform.position, transform.position);
			onDragStart?.Invoke();
		}
	}

	protected void Update() {
		if (dragging) {
			onDrag?.Invoke();
			if (!App.input.primary) {
				dragging = false;
				onDragEnd?.Invoke();
			}
		}
	}

	public virtual void OnDrag() {
		var ray = camera.ScreenPointToRay(App.input.pointer);
		var endPos = ray.origin + ray.direction * startScreenDist;
		transform.position = endPos + dragOffset;
	}

}
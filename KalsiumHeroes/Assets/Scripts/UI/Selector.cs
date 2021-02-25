using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Animator))]
public class Selector : UIBehaviour {

	public RectTransform rectTransform => (RectTransform)transform;

	[SerializeField] Animator animator;
	[SerializeField] Image image;

	private GameObject prevSelected;
	private bool prevSelectedWasNull;

	new protected void Awake() {
		base.Awake();
		if (!animator) Debug.Assert(animator = GetComponent<Animator>());
		if (!image) Debug.Assert(image = GetComponent<Image>());
		UpdateVars();
	}

	protected void Update() {
		var eventSystem = EventSystem.current;
		var rectTransform = this.rectTransform;
		var changed = prevSelected != eventSystem.currentSelectedGameObject;
		if (eventSystem.currentSelectedGameObject) {
			if (changed) {
				animator.Play("Select", -1, 0);
			}
			if (eventSystem.currentSelectedGameObject.TryGetComponent<RectTransform>(out var rt)) {
				var screenRect = rt.ScreenRect();
				rectTransform.position = screenRect.center;
				rectTransform.sizeDelta = screenRect.size;
				rectTransform.localScale = rt.lossyScale;
				rectTransform.rotation = rt.rotation;
				if (eventSystem.currentSelectedGameObject.TryGetComponent<Image>(out var img)) {
					image.sprite = img.sprite;
					image.type = img.type;
				}
				UpdateVars();
			}
		} else {
			if (changed || !prevSelectedWasNull) {
				animator.SetTrigger("Hide");
			}
		}
		prevSelected = eventSystem.currentSelectedGameObject;
		prevSelectedWasNull = prevSelected == null;
	}

	protected override void OnRectTransformDimensionsChange() {
		base.OnRectTransformDimensionsChange();
		if (!image) return;
		UpdateVars();
	}

	protected void UpdateVars() {
		image.material.SetVector("Size", rectTransform.sizeDelta);
		image.material.SetVector("Position", rectTransform.localPosition);
		image.material.SetVector("Scale", rectTransform.lossyScale);
	}
}

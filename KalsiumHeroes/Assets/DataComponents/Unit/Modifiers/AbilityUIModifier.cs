using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class AbilityUIModifier : UnitModifier {

	public Canvas canvas;
	public Camera cam;
	[Tooltip("World space offset")]
	public Vector3 wsOffset;
	[Tooltip("Screen space offs et")]
	public Vector2 ssOffset;


	[Space()]

	[Tooltip("Will be moved, and disabled or enabled depending on the turn.")]
	public GameObject parent;
	[Tooltip("CanvasGroup Component (probably on the parent object). Fades out the abilities when casting.")]
	public CanvasGroup group;
	[Tooltip("A prefab with AbilityIcon component.")]
	public GameObject abilityPrefab;
	[Tooltip("A prefab with PassiveIcon component.")]
	public GameObject passivePrefab;

	public float padding;

	[SerializeField, HideInInspector] private List<AbilityIcon> aIcons = default;
	[SerializeField, HideInInspector] private List<PassiveIcon> pIcons = default;

	[SerializeField, HideInInspector] float width;
	[SerializeField, HideInInspector] float height;

	[SerializeField, Range(0, 1)] float fadeAlpha = 0.15f;
	[SerializeField] float alphaFadeSpeed = 5f;
	[SerializeField, HideInInspector] float currentAlpha = 1;
	[SerializeField, HideInInspector] float targetAlpha = 1;

	[SerializeField, HideInInspector] bool hibernated;
	Action nextFrame;

	void LateUpdate() {
		if (!hibernated) {
			if (true) {
				var sign = Mathf.Sign(targetAlpha - currentAlpha);
				currentAlpha += alphaFadeSpeed * Time.deltaTime * sign;
				if (sign > 0) currentAlpha = Mathf.Clamp(currentAlpha, 0, targetAlpha);
				else currentAlpha = Mathf.Clamp(currentAlpha, targetAlpha, 1);
				group.alpha = currentAlpha;
			}
			RefreshPosition();
			nextFrame?.Invoke();
			nextFrame = null;
		}
	}

	new void OnValidate() {
		base.OnValidate();
		if (!group && parent) group = parent.GetComponent<CanvasGroup>();
	}

	new void Awake() {
		if (!canvas) canvas = GetComponentInChildren<Canvas>();
		if (!cam) cam = Camera.main;
		if (!cam) FindObjectOfType<Camera>();
		base.Awake();
		foreach (var ability in unit.modifiers.Get<Ability>()) AddIcon(ability);
		foreach (var passive in unit.modifiers.Get<Passive>()) AddIcon(passive);
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	static void OnReloadScripts() {
		if (!Application.isPlaying) return;
		foreach (var uiMod in Game.dataComponents.Get<AbilityUIModifier>()) uiMod.RefreshValues();
	}
#endif

	void AddIcon(Ability ability) {
		var icon = Instantiate(abilityPrefab).GetComponent<AbilityIcon>();
		icon.transform.SetParent(parent.transform);
		aIcons.Add(icon);
		icon.ability = ability;
		icon.abilityText.text = ability.data.displayName;
		icon.cooldownText.text = "";
		icon.chargeText.text = "";
		icon.fgImage.enabled = true;
		icon.bgImage.enabled = false;
		RefreshLayout();
		RefreshValues();
	}
	void AddIcon(Passive passive) {
		var icon = Instantiate(passivePrefab).GetComponent<PassiveIcon>();
		pIcons.Add(icon);
		icon.passive = passive;
		icon.abilityText.text = passive.data.displayName;
		RefreshLayout();
	}


	void RemoveIcon(Ability ability) {
		var index = aIcons.FindIndex(v => v.ability == ability);
		Destroy(aIcons[index]);
		aIcons.RemoveAt(index);
		RefreshLayout();
	}
	void RemoveIcon(Passive passive) {
		var index = pIcons.FindIndex(v => v.passive == passive);
		Destroy(pIcons[index]);
		pIcons.RemoveAt(index);
		RefreshLayout();
	}


	public void RefreshLayout() {
		var total = aIcons.Count + pIcons.Count;
		if (total == 0) return;

		var compAbilties = unit.GetComponents<Ability>();
		var compPassives = unit.GetComponents<Passive>();
		aIcons.Sort((a, b) =>
				Array.FindIndex(compAbilties, v => v == a.ability).CompareTo(
				Array.FindIndex(compAbilties, v => v == b.ability)
		));
		pIcons.Sort((a, b) =>
				Array.FindIndex(compPassives, v => v == a.passive).CompareTo(
				Array.FindIndex(compPassives, v => v == b.passive)
		));

		var rts = aIcons.Select(v => v.GetComponent<RectTransform>())
				.Concat(pIcons.Select(v => v.GetComponent<RectTransform>()));

		var unpaddedDistance = rts.Aggregate(0f, (acc, rt) => acc + rt.rect.width);
		height = rts.Aggregate(0f, (acc, rt) => Mathf.Max(acc, rt.rect.height));
		width = unpaddedDistance + (total - 1) * padding;
		var centerToCenterWidth = width - (rts.First().rect.width / 2f + rts.Last().rect.width / 2f);
		var startX = centerToCenterWidth / -2f;

		var currentX = startX;
		foreach (var rt in rts) {
			rt.localPosition = new Vector3(currentX, 0, rt.localPosition.z);
			currentX += rt.rect.width + padding;
		}
	}

	public void RefreshPosition() {
		if (!cam) return;
		parent.transform.position = cam.WorldToScreenPoint(transform.position + wsOffset).Add(ssOffset);

		// Clamp left
		var minX = parent.transform.position.x - width / 2f;
		if (minX < 0) {
			parent.transform.position = parent.transform.position.SetX(width / 2f);
		} else {
			// Clamp right
			var maxX = parent.transform.position.x + width / 2f;
			if (maxX > canvas.pixelRect.width) {
				parent.transform.position = parent.transform.position.SetX(canvas.pixelRect.width - width / 2f);
			}
		}
		// Clamp top
		var minY = parent.transform.position.y - height / 2f;
		if (minY < 0) {
			parent.transform.position = parent.transform.position.SetY(height / 2f);
		} else {
			// Clamp bottom
			var maxY = parent.transform.position.y + height / 2f;
			if (maxY > canvas.pixelRect.height) {
				parent.transform.position = parent.transform.position.SetY(canvas.pixelRect.height - height / 2f);
			}
		}
	}


	bool ShowCharges(AbilityData abilityData) => abilityData.charges.other > 1 && abilityData.cooldown.other >= 1;
	string GetChargeText(AbilityData abilityData) => ShowCharges(abilityData) ? abilityData.charges.value.ToString() : "";

	public void RefreshValues() {

		foreach (var icon in aIcons) {
			var (ability, abilityButton, abilityText, cooldownText, chargeText, fgImage, bgImage) = icon;
			var abilityData = ability.abilityData;


			chargeText.text = GetChargeText(abilityData);
			abilityButton.onClick.RemoveAllListeners();
			if (Game.events.finished && ability.IsReady()) {
				fgImage.fillAmount = 1;
				cooldownText.text = "";
				fgImage.enabled = true;
				bgImage.enabled = false;
				abilityButton.onClick.AddListener(() => {
					if (ability.IsReady()) {
						Game.targeting.TryStartTargeter(ability.GetTargeter());
					}
				});
			} else {
				if (abilityData.cooldown.other > 0 && abilityData.charges.value <= 0) {
					cooldownText.text = abilityData.cooldown.value > 0 ? abilityData.cooldown.value.ToString() : "";
					fgImage.fillAmount = 1 - (float)abilityData.cooldown.value / (float)abilityData.cooldown.other;
					fgImage.enabled = true;
					bgImage.enabled = true;
				} else {
					cooldownText.text = "";
					fgImage.fillAmount = 1;
					fgImage.enabled = false;
					bgImage.enabled = true;
				}
			}
		}

	}


	void Wake() {
		hibernated = false;
		parent.SetActive(true);
		RefreshPosition();
		nextFrame += RefreshValues;
	}

	void Hibernate() {
		hibernated = true;
		parent.SetActive(false);
	}


	private void OnEventStart() {
		targetAlpha = fadeAlpha;
		if (Game.rounds.current == unit) RefreshValues();
	}
	private void OnEventFinish() {
		targetAlpha = 1;
		if (Game.rounds.current == unit) RefreshValues();
	}

	private void OnTargeterStart() {
		targetAlpha = fadeAlpha;
		if (Game.rounds.current == unit) {
			foreach (var icon in aIcons) icon.abilityButton.enabled = false;
		}
	}
	private void OnTargeterEnd() {
		targetAlpha = 1;
		if (Game.rounds.current == unit) {
			foreach (var icon in aIcons) icon.abilityButton.enabled = true;
		}
	}


	protected override void OnLoadNonpersistent() {
		base.OnLoadNonpersistent();
		Game.events.onStart += OnEventStart;
		Game.events.onFinish += OnEventFinish;
		Game.targeting.onTargeterEnd += OnTargeterEnd;
		Game.targeting.onTargeterStart += OnTargeterStart;
	}

	protected override void OnUnloadNonpersistent() {
		base.OnLoadNonpersistent();
		Game.events.onStart -= OnEventStart;
		Game.events.onFinish -= OnEventFinish;
		Game.targeting.onTargeterEnd -= OnTargeterEnd;
		Game.targeting.onTargeterStart -= OnTargeterStart;
	}


	public override void OnAdd(UnitModifier modifier) {
		base.OnAdd(modifier);
		switch (modifier) {
			case Ability ability: AddIcon(ability); break;
			case Passive passive: AddIcon(passive); break;
		}
	}

	public override void OnRemove(UnitModifier modifier) {
		base.OnRemove(modifier);
		switch (modifier) {
			case Ability ability: AddIcon(ability); break;
			case Passive passive: AddIcon(passive); break;
		}
	}


	public override void OnBaseAbilityCast(Ability ability) {
		base.OnBaseAbilityCast(ability);
		RefreshValues();
	}

	public override void OnAbilityCast(Ability ability) {
		base.OnAbilityCast(ability);
		RefreshValues();
	}


	public override void OnTurnStart() {
		base.OnTurnStart();
		Wake();
	}

	public override void OnTurnEnd() {
		base.OnTurnEnd();
		Hibernate();
	}

}

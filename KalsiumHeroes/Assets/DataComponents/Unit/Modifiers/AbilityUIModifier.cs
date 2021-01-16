using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class AbilityUIModifier : UnitModifier,
		IOnAnimationEventStart,
		IOnAnimationEventEnd,
		IOnTargeterStart,
		IOnTargeterEnd,
		IOnAbilityCastStart_Unit,
		IOnGameStart,
		IOnGameEnd,
		IOnTurnStart_Unit,
		IOnTurnEnd_Unit {

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
		base.Awake();
		if (!canvas) canvas = GetComponentInChildren<Canvas>();
		if (!cam) cam = Camera.main;
		if (!cam) FindObjectOfType<Camera>();
		foreach (var ability in unit.modifiers.Get<Ability>()) AddIcon(ability);
		foreach (var passive in unit.modifiers.Get<Passive>()) AddIcon(passive);
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	static void OnReloadScripts() {
		if (!Application.isPlaying) return;
		foreach (var uiMod in Game.dataComponents.Get<AbilityUIModifier>().Where(v => v)) uiMod.RefreshValues();
	}
#endif

	void AddIcon(Ability ability) {
		var icon = Instantiate(abilityPrefab).GetComponent<AbilityIcon>();
		icon.transform.SetParent(parent.transform);
		aIcons.Add(icon);
		icon.ability = ability;
		icon.abilityText.text = ability.unitModifierData.displayName;
		icon.cooldownText.text = "";
		icon.chargeText.text = "";
		icon.fgMask.gameObject.SetActive(true);
		icon.bgImage.enabled = false;
		RefreshLayout();
		RefreshValues();
	}
	void AddIcon(Passive passive) {
		var icon = Instantiate(passivePrefab).GetComponent<PassiveIcon>();
		pIcons.Add(icon);
		icon.passive = passive;
		icon.abilityText.text = passive.unitModifierData.displayName;
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

		var compAbilties = unit.modifiers.Get<Ability>().ToArray();
		var compPassives = unit.modifiers.Get<Passive>().ToArray();
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

	bool ShowEnergy(AbilityData abilityData) => abilityData.energyCost.value != 0;
	string GetEnergyText(AbilityData abilityData) => ShowEnergy(abilityData) ? abilityData.energyCost.value.ToString() : "";

	public void RefreshValues() {

		foreach (var icon in aIcons) {
			var (ability, abilityButton, abilityText, cooldownText, chargeText, energyText, fgMask, bgImage) = icon;
			var abilityData = ability.abilityData;


			chargeText.text = GetChargeText(abilityData);
			energyText.text = GetEnergyText(abilityData);
			abilityButton.onClick.RemoveAllListeners();
			if (Game.events.finished && ability.IsReady()) {
				fgMask.fillAmount = 1;
				cooldownText.text = "";
				fgMask.gameObject.SetActive(true);
				bgImage.enabled = false;
				abilityButton.onClick.AddListener(() => {
					if (ability.IsReady()) {
						Game.targeting.TryStartTargeter(ability.GetTargeter());
					}
				});
			} else {
				if (abilityData.cooldown.other > 0 && abilityData.charges.value <= 0) {
					cooldownText.text = abilityData.cooldown.value > 0 ? abilityData.cooldown.value.ToString() : "";
					fgMask.fillAmount = 1 - (float)abilityData.cooldown.value / (float)abilityData.cooldown.other;
					fgMask.gameObject.SetActive(true);
					bgImage.enabled = true;
				} else {
					cooldownText.text = "";
					fgMask.fillAmount = 1;
					fgMask.gameObject.SetActive(false);
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

	public void OnAnimationEventStart(EventHandler handler) {
		targetAlpha = fadeAlpha;
		if (Game.rounds.current == unit) RefreshValues();
	}
	public void OnAnimationEventEnd() {
		targetAlpha = 1;
		if (Game.rounds.current == unit) RefreshValues();
	}

	public void OnTargeterStart(Targeter targeter) {
		targetAlpha = fadeAlpha;
		if (Game.rounds.current == unit) {
			foreach (var icon in aIcons) icon.abilityButton.enabled = false;
		}
	}
	public void OnTargeterEnd() {
		targetAlpha = 1;
		if (Game.rounds.current == unit) {
			foreach (var icon in aIcons) icon.abilityButton.enabled = true;
		}
	}

	public void OnAbilityCastStart(Ability ability) {
		RefreshValues();
	}


	public void OnTurnStart() {
		Wake();
	}

	public void OnTurnEnd() {
		Hibernate();
	}

	public void OnGameStart() {
		if (Game.rounds.current == unit)
			Wake();
	}

	public void OnGameEnd() {
		Destroy(this);
	}
}

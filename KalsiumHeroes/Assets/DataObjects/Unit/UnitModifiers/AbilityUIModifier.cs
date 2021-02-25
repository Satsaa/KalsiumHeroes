using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class AbilityUIModifier : UnitModifier,
		IOnLateUpdate,
		IOnAnimationEventStart,
		IOnAnimationEventEnd,
		IOnTargeterStart,
		IOnTargeterEnd,
		IOnAbilityCastStart_Unit,
		IOnDeath_Unit,
		IOnGameStart,
		IOnGameEnd,
		IOnTurnStart_Unit,
		IOnTurnEnd_Unit {


	public new AbilityUIModifierData data => (AbilityUIModifierData)_data;
	public override Type dataType => typeof(AbilityUIModifierData);

	[SerializeField, HideInInspector] Camera cam;
	[SerializeField, HideInInspector] Canvas canvas;

	[SerializeField, HideInInspector] private AbilityButtonsUI ui;
	[SerializeField, HideInInspector] private List<AbilityIcon> aIcons = new List<AbilityIcon>();
	[SerializeField, HideInInspector] private List<PassiveIcon> pIcons = new List<PassiveIcon>();

	[SerializeField, HideInInspector] float width;
	[SerializeField, HideInInspector] float height;

	[SerializeField, HideInInspector] float currentAlpha = 1;
	[SerializeField, HideInInspector] float targetAlpha = 1;

	[SerializeField, HideInInspector] bool hibernated;
	Action nextFrame;

	void IOnLateUpdate.OnLateUpdate() {
		if (!hibernated) {
			var sign = Mathf.Sign(targetAlpha - currentAlpha);
			currentAlpha += data.alphaFadeSpeed * Time.deltaTime * sign;
			if (sign > 0) currentAlpha = Mathf.Clamp(currentAlpha, 0, targetAlpha);
			else currentAlpha = Mathf.Clamp(currentAlpha, targetAlpha, 1);
			ui.group.alpha = currentAlpha;

			RefreshPosition();
			nextFrame?.Invoke();
			nextFrame = null;
		}
	}

	protected override void OnCreate() {
		base.OnCreate();
		Debug.Assert(ui = container.GetComponentInChildren<AbilityButtonsUI>());
		Debug.Assert(cam = Camera.main);
		foreach (var ability in unit.modifiers.Get<Ability>()) AddIcon(ability);
		foreach (var passive in unit.modifiers.Get<Passive>()) AddIcon(passive);
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	static void OnReloadScripts() {
		if (Application.isPlaying && Game.instance) {
			foreach (var uiMod in Game.dataObjects.Get<AbilityUIModifier>().Where(v => v)) uiMod.RefreshValues();
		}
	}
#endif

	void AddIcon(Ability ability) {
		var icon = ObjectUtil.Instantiate(data.abilityPrefab, ui.parent.transform).GetComponent<AbilityIcon>();
		aIcons.Add(icon);
		icon.SetAbility(ability);
		RefreshLayout();
		RefreshValues();
	}
	void AddIcon(Passive passive) {
		var icon = ObjectUtil.Instantiate(data.passivePrefab, ui.parent.transform).GetComponent<PassiveIcon>();
		pIcons.Add(icon);
		icon.SetPassive(passive);
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
		width = unpaddedDistance + (total - 1) * data.padding;
		var centerToCenterWidth = width - (rts.First().rect.width / 2f + rts.Last().rect.width / 2f);
		var startX = centerToCenterWidth / -2f;

		var currentX = startX;
		foreach (var rt in rts) {
			rt.localPosition = new Vector3(currentX, 0, rt.localPosition.z);
			currentX += rt.rect.width + data.padding;
		}
	}

	public void RefreshPosition() {
		if (!cam) return;
		var pos = cam.WorldToScreenPoint(master.transform.position + data.wsOffset).Add(data.ssOffset);

		// Clamp
		var ssRect = ui.areaConstraint.ScreenRect();

		// Clamp left
		var minX = pos.x - width / 2f;
		if (minX < ssRect.xMin) {
			pos = pos.SetX(ssRect.xMin + width / 2f);
		} else {
			// Clamp right
			var maxX = pos.x + width / 2f;
			if (maxX > ssRect.xMax) {
				pos = pos.SetX(ssRect.xMax - width / 2f);
			}
		}
		// Clamp top
		var minY = pos.y - height / 2f;
		if (minY < ssRect.yMin) {
			pos = pos.SetY(ssRect.yMin + height / 2f);
		} else {
			// Clamp bottom
			var maxY = pos.y + height / 2f;
			if (maxY > ssRect.yMax) {
				pos = pos.SetY(ssRect.yMax - height / 2f);
			}
		}

		ui.parent.transform.position = pos;

	}


	bool ShowCharges(AbilityData abilityData) => abilityData.charges.other > 1 && abilityData.cooldown.other >= 1;
	string GetChargeText(AbilityData abilityData) => ShowCharges(abilityData) ? abilityData.charges.value.ToString() : "";

	bool ShowEnergy(AbilityData abilityData) => abilityData.energyCost.value != 0;
	string GetEnergyText(AbilityData abilityData) => ShowEnergy(abilityData) ? abilityData.energyCost.value.ToString() : "";

	public void RefreshValues() {

		foreach (var icon in aIcons) {
			var abilityData = icon.ability.data;

			icon.chargeText.text = GetChargeText(abilityData);
			icon.energyText.text = GetEnergyText(abilityData);
			icon.button.onClick.RemoveAllListeners();
			if (Game.events.finished && icon.ability.IsReady()) {
				icon.mask.fillAmount = 1;
				icon.cooldownText.text = "";
				icon.mask.gameObject.SetActive(true);
				icon.sprite.enabled = false;
				icon.button.onClick.AddListener(() => {
					if (icon.ability.IsReady()) {
						switch (icon.ability) {
							case TargetAbility targ:
								Game.targeting.TryStartTargeter(targ.GetTargeter());
								break;
							case NoTargetAbility notarg:
								notarg.PostDefaultAbilityEvent();
								break;
						}
					}
				});
			} else {
				if (abilityData.cooldown.other > 0 && abilityData.charges.value <= 0) {
					icon.cooldownText.text = abilityData.cooldown.value > 0 ? abilityData.cooldown.value.ToString() : "";
					icon.mask.fillAmount = 1 - (float)abilityData.cooldown.value / (float)abilityData.cooldown.other;
					icon.mask.gameObject.SetActive(true);
					icon.sprite.enabled = true;
				} else {
					icon.cooldownText.text = "";
					icon.mask.fillAmount = 1;
					icon.mask.gameObject.SetActive(false);
					icon.sprite.enabled = true;
				}
			}
		}

	}


	void Wake() {
		hibernated = false;
		ui.parent.SetActive(true);
		RefreshPosition();
		nextFrame += RefreshValues;
	}

	void Hibernate() {
		hibernated = true;
		if (ui != null) ui.parent.SetActive(false);
	}


	public override void OnCreate(UnitModifier modifier) {
		base.OnCreate(modifier);
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
		targetAlpha = data.fadeAlpha;
		ui.group.blocksRaycasts = false;
		if (Game.rounds.current == unit) RefreshValues();
	}
	public void OnAnimationEventEnd() {
		targetAlpha = 1;
		ui.group.blocksRaycasts = true;
		if (Game.rounds.current == unit) RefreshValues();
	}

	public void OnTargeterStart(Targeter targeter) {
		targetAlpha = data.fadeAlpha;
		ui.group.blocksRaycasts = false;
		if (Game.rounds.current == unit) {
			foreach (var icon in aIcons) icon.button.enabled = false;
		}
	}
	public void OnTargeterEnd() {
		targetAlpha = 1;
		ui.group.blocksRaycasts = true;
		if (Game.rounds.current == unit) {
			foreach (var icon in aIcons) icon.button.enabled = true;
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
		Remove();
	}

	public void OnDeath() {
		Remove();
	}
}

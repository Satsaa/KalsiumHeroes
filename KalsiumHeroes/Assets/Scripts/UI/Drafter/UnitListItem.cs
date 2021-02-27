using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitListItem : UIBehaviour {

	public RectTransform rectTransform => (RectTransform)transform;

	[SerializeField] TMP_Text displayName;
	[SerializeField] Image sprite;
	[SerializeField] Toggle toggle;

	[SerializeField, HideInInspector] Drafter drafter;
	[SerializeField, HideInInspector] UnitData unitData;
	[SerializeField, HideInInspector] bool unitEnabled;

	new protected void Awake() {
		base.Awake();
		if (!toggle) Debug.Assert(toggle = GetComponentInChildren<Toggle>(), this);
	}

	public void Init(Drafter drafter, UnitData unitData, bool enabled) {
		this.drafter = drafter;
		this.unitData = unitData;
		this.unitEnabled = enabled;
		sprite.sprite = unitData.portrait;
		displayName.text = unitData.displayName;
		toggle.isOn = unitEnabled;
	}

	public void SetUnitDrafted(bool enabled) {
		if (unitEnabled == enabled) return;
		if (unitData) {
			if (unitEnabled) {
				drafter.RemoveFromDraft(unitData.identifier);
			} else {
				drafter.AddToDraft(unitData.identifier);
			}
		}
		unitEnabled = enabled;
	}

}

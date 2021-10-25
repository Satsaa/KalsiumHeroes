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
	[SerializeField, HideInInspector] Unit unit;
	[SerializeField, HideInInspector] bool unitEnabled;

	new protected void Awake() {
		base.Awake();
		if (!toggle) Debug.Assert(toggle = GetComponentInChildren<Toggle>(), this);
	}

	public void Init(Drafter drafter, Unit unit, bool enabled) {
		this.drafter = drafter;
		this.unit = unit;
		this.unitEnabled = enabled;
		sprite.sprite = unit.sprite.value;
		displayName.text = Lang.GetStr($"{unit.identifier}_DisplayName");
		toggle.isOn = unitEnabled;
	}

	public void SetUnitDrafted(bool enabled) {
		if (unitEnabled == enabled) return;
		if (unit) {
			if (unitEnabled) {
				drafter.RemoveFromDraft(unit);
			} else {
				drafter.AddToDraft(unit);
			}
		}
		unitEnabled = enabled;
	}

}

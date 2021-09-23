using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Drafter : UIBehaviour {

	public RectTransform rectTransform => (RectTransform)transform;


	[SerializeField] UnitListItem itemPrefab;
	[SerializeField] Transform itemParent;

	[SerializeField] GameMode mode;
	[SerializeField] List<UnitData> selection;

	[FormerlySerializedAs("submitMessageBox")]
	[SerializeField] PopupPreset submitPopup;

	[SerializeField] List<UnitListItem> items;

	new protected void Start() {
		base.Start();
		if (mode) SetGameMode(mode);
	}

	public void SetGameMode(int index) => SetGameMode(App.gameModes[index]);
	public void SetGameMode(GameMode mode) {
		this.mode = mode;
		ResetUnits();
	}

	public void ResetUnits() {
		foreach (var item in items) {
			Destroy(item.gameObject);
		}
		items.Clear();
		foreach (var unitData in App.library.GetByType<UnitData>()) {
			var item = Instantiate(itemPrefab, itemParent);

			item.Init(this, unitData, selection.Contains(unitData));
			items.Add(item);
		}
	}

	public void AddToDraft(UnitData unitData) {
		if (selection.Contains(unitData)) {
			Debug.LogWarning("Already selected");
		} else {
			selection.Add(unitData);
		}
	}

	public void RemoveFromDraft(UnitData unitData) {
		if (!selection.Remove(unitData)) {
			Debug.LogWarning("Already removed");
		}
	}

	public void Submit() {
		if (Validate(out var failMessage)) {
			mode.draft = selection.Select(v => v.identifier).ToArray();
			submitPopup.Show(Lang.GetStr("Popup_DraftSubmitSuccess_Title"));
		} else {
			submitPopup.Show(failMessage);
		}
	}

	public bool Validate(out string failMessage) {
		return mode.ValidateDraft(selection, out failMessage);
	}
}

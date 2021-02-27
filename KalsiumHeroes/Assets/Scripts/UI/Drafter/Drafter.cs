using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drafter : UIBehaviour {

	public RectTransform rectTransform => (RectTransform)transform;


	[SerializeField] UnitListItem itemPrefab;
	[SerializeField] Transform itemParent;

	[SerializeField] GameMode mode;
	[SerializeField] List<string> draft;
	[SerializeField] List<UnitListItem> items;

	[SerializeField] MessageBoxPreset submitMessageBox;
	[SerializeField] TextSource submitSuccessMessage;

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

			item.Init(this, unitData, draft.Contains(unitData.identifier));
			items.Add(item);
		}
	}

	public void AddToDraft(string unitId) {
		if (draft.Contains(unitId)) {
			Debug.LogWarning($"Already selected");
		} else {
			draft.Add(unitId);
		}
	}

	public void RemoveFromDraft(string unitId) {
		if (!draft.Remove(unitId)) {
			Debug.LogWarning($"Already removed");
		}
	}

	public void Submit() {
		if (Validate(out var failReason)) {
			mode.draft = draft.ToArray();
			submitMessageBox.Show(message: submitSuccessMessage);
		} else {
			submitMessageBox.Show(message: failReason);
		}
	}

	public bool Validate(out TextSource failReason) {
		if (mode.ValidateDraft(draft, out failReason)) {
			return true;
		} else {
			return false;
		}
	}
}

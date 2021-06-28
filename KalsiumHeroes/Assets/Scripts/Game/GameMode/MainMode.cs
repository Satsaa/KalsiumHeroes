using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains a list of data sources
/// </summary>
[CreateAssetMenu(fileName = nameof(MainMode), menuName = "KalsiumHeroes/" + nameof(MainMode))]
public class MainMode : GameMode {

	public int minDraftCount = 1;
	public int maxDraftCount = 10;
	public int maxDraftCost = 50;

	public override bool ValidateDraft(IEnumerable<UnitData> unitDatas, out string failMessage) {
		var count = unitDatas.Count();
		if (count < minDraftCount) {
			failMessage = Lang.GetText("DRAFT_MIN_COUNT_NOT_REACHED");
			return false;
		}
		if (count > maxDraftCount) {
			failMessage = Lang.GetText("DRAFT_MAX_COUNT_EXCEEDED");
			return false;
		}

		int cost = 0;
		foreach (var unitData in unitDatas) {
			cost += unitData.draftCost;
		}

		if (cost > maxDraftCost) {
			failMessage = Lang.GetText("DRAFT_MAX_COST_EXCEEDED");
			return false;
		}

		failMessage = default;
		return true;
	}

}


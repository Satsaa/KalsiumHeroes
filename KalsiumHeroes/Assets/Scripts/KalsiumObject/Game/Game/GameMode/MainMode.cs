using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MainMode), menuName = "KalsiumHeroes/" + nameof(MainMode))]
public class MainMode : GameMode {

	public int minDraftCount = 1;
	public int maxDraftCount = 10;
	public int maxDraftCost = 50;

	public override bool ValidateDraft(IEnumerable<Unit> draftUnits, out string failMessage) {
		var count = draftUnits.Count();
		if (count < minDraftCount) {
			failMessage = Lang.GetStr("Draft_MinCountNotReached");
			return false;
		}
		if (count > maxDraftCount) {
			failMessage = Lang.GetStr("Draft_MaxCountExceeded");
			return false;
		}

		int cost = 0;
		foreach (var unitData in draftUnits) {
			cost += unitData.draftCost;
		}

		if (cost > maxDraftCost) {
			failMessage = Lang.GetStr("Draft_MaxCostExceeded");
			return false;
		}

		failMessage = default;
		return true;
	}

}


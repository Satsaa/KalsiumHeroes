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

	[SerializeField] TextSource maxCostExceeded;
	[SerializeField] TextSource maxCountExceeded;
	[SerializeField] TextSource minCountNotReached;
	[SerializeField] TextSource unitMissing;

	public override bool ValidateDraft(List<string> unitIds, out TextSource reason) {
		if (unitIds.Count < minDraftCount) {
			reason = minCountNotReached;
			return false;
		}
		if (unitIds.Count > maxDraftCount) {
			reason = maxCountExceeded;
			return false;
		}

		int cost = 0;
		foreach (var unitId in unitIds) {
			if (App.library.TryGetById<UnitData>(unitId, out var unitData)) {
				cost += unitData.draftCost;
			} else {
				reason = unitMissing;
				return false;
			}
		}

		if (cost > maxDraftCost) {
			reason = maxCostExceeded;
			return false;
		}

		reason = default;
		return true;
	}

}


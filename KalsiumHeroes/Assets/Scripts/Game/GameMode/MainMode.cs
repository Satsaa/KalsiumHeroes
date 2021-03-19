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

	public override bool ValidateDraft(IEnumerable<UnitData> unitDatas, out TextSource reason) {
		var count = unitDatas.Count();
		if (count < minDraftCount) {
			reason = minCountNotReached;
			return false;
		}
		if (count > maxDraftCount) {
			reason = maxCountExceeded;
			return false;
		}

		int cost = 0;
		foreach (var unitData in unitDatas) {
			cost += unitData.draftCost;
		}

		if (cost > maxDraftCost) {
			reason = maxCostExceeded;
			return false;
		}

		reason = default;
		return true;
	}

}


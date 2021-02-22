
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;


public class DisplayName : MonoBehaviour, IContainerComponent {

	void IContainerComponent.SetMaster(Master master) {
		var displayName = master switch {
			Unit unit => unit.data.displayName,
			Tile tile => tile.data.displayName,
			Edge edge => edge.data.displayName,
			_ => "UNSUPPORTED",
		};
		if (TryGetComponent<Text>(out var text)) {
			text.text = displayName;
		}
		if (TryGetComponent<TMPro.TMP_Text>(out var tmp_text)) {
			tmp_text.text = displayName;
		}
	}

}
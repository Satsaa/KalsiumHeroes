
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class LangText : TMPro.TextMeshProUGUI {

	public string strId {
		get => _strId;
		set { _strId = value; UpdateText(); }
	}

	[SerializeField] string _strId { get; set; }

	protected override void Awake() {
		UpdateText();
		base.Awake();
	}

	void UpdateText() {
		text = Lang.GetText(strId);
	}
}
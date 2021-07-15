
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Components;

public class CombatLog : VirtualLayoutGroup, IOnCombatLog {

	public RectTransform logItem;

	protected override void Awake() {
		base.Awake();
		Game.hooks.Hook(this);
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		if (Game.game) Game.hooks.Unhook(this);
	}

	public virtual void OnCombatLog(string str) {
		Debug.Log($"Adding: {str}");
		var roundItem = AnimatedItem.CreateInstance<AnimatedItem>();
		roundItem.prefab = logItem;
		roundItem.calculateSize = true;
		roundItem.createOnInit = true;
		base.Insert(0, roundItem);
		roundItem.item.gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = str;
	}

}
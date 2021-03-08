
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;

[RequireComponent(typeof(EventStack))]
public class RoundStackController : MonoBehaviour, IOnGameStart, IOnRoundStart {

	[HideInInspector] public RectTransform rt;
	[HideInInspector] public EventStack es;
	public GameObject roundStackPrefab;
	public GameObject roundStackItemPrefab;
	public int total;

	protected void Awake() {
		rt = GetComponent<RectTransform>();
		es = GetComponent<EventStack>();
		Game.onEvents.Add(this);
	}

	protected void Update() {
		rt.sizeDelta = rt.sizeDelta.SetX(es.MaxX());
	}

	void OnDestroy() {
		if (Game.game) Game.onEvents.Remove(this);
	}

	public void OnGameStart() {
		for (int i = 0; i < total; i++) {
			var round = Game.rounds.round + i;
			CreateItem(round);
		}
	}

	public void OnRoundStart() {
		if (Game.rounds.round > 1) {
			var round = Game.rounds.round + total - 1;
			CreateItem(round);
		}
	}

	void CreateItem(int round) {
		if (roundStackItemPrefab) {
			var item = ObjectUtil.Instantiate(roundStackItemPrefab, transform);
			var comp = item.GetComponent<RoundItem>();
			comp.round = round;
			es.Add(comp);
		}
		var go = ObjectUtil.Instantiate(roundStackPrefab, transform);
		var mod = go.GetComponent<RoundStack>().round = round;
		es.Add(go.GetComponent<EventStackItem>());
	}


}
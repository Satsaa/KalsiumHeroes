
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventStack))]
public class RoundStackController : Modifier {

	public EventStack es;
	public GameObject roundStackPrefab;
	public GameObject roundStackItemPrefab;
	public int total;

	new void Awake() {
		base.Awake();
		es = GetComponent<EventStack>();
	}

	public override void OnGameStart() {
		Game.onAfterEvent += () => {
			for (int i = 0; i < total; i++) {
				var round = Game.rounds.round + i;
				CreateItem(round);
			}
		};
	}

	public override void OnRoundStart() {
		if (Game.rounds.round > 1) {
			Game.onAfterEvent += () => {
				var round = Game.rounds.round + total - 1;
				CreateItem(round);
			};
		}
	}

	void CreateItem(int round) {
		if (roundStackItemPrefab) {
			var item = Instantiate(roundStackItemPrefab, transform);
			var comp = item.GetComponent<RoundStackItem>();
			comp.Init(round);
			es.Add(comp);
		}
		var go = Instantiate(roundStackPrefab, transform);
		var mod = go.GetComponent<RoundStack>().round = round;
		es.Add(go.GetComponent<EventStackItem>());
	}


}
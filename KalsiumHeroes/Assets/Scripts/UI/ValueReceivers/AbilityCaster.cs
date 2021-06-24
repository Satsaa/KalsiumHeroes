
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AbilityCaster : ValueHooker<Ability>, IOnAbilityCastStart_Global, IOnAbilityCastEnd_Global, IOnAnimationEventStart, IOnAnimationEventEnd {

	public void Cast() {
		if (target && Game.rounds.unit == target.unit) {
			if (Game.targeting.targeting) {
				if (!Game.targeting.TryCancel()) return;
			}
			if (Game.events.animating && Game.events.handler is EventHandler<GameEvents.Ability>) {
				Game.events.TryEndEvent();
			}
			if (!Game.events.animating && target.IsReady()) {
				switch (target) {
					case TargetAbility targ:
						Game.targeting.TryStartTargeter(targ.GetTargeter());
						break;
					case NoTargetAbility notarg:
						notarg.PostDefaultAbilityEvent();
						break;
				}
			}
		}
	}

	protected override void ReceiveValue(Ability target) {
		this.target = target;
		Hook(target.unit);
	}

	protected Ability target;
	protected bool casting;

	public void OnAbilityCastStart(Ability ability) {

	}

	public void OnAbilityCastEnd(Ability ability) {

	}

	public void OnAnimationEventStart(EventHandler handler) {

	}

	public void OnAnimationEventEnd() {

	}

}
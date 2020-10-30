using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnAbility : Ability {

  public override bool EventIsFinished() => true;
  public override bool SkipEvent() => true;
  public override void StartEvent(Events.Ability data) => Debug.LogWarning("End turn ability asked to animate.");

  public override Targeter GetTargeter() {
    return new InstantTargeter(
      onComplete: t => Game.client.PostEvent(new Events.Turn())
    );
  }
}

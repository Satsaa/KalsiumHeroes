using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnAbility : Ability {

  public override bool EventIsFinished() {
    return true;
  }

  public override bool SkipEvent() {
    return true;
  }

  public override void StartEvent(Events.Ability data) {
    throw new System.Exception("This ability is not animateable. It sends an end turn event and no ability event should refer to this ability.");
  }

  public override TargetingSequence GetTargetingSequence() {
    return new InstantTargetingSequence(
      onComplete: (seq) => {
        Debug.Log("Posting end turn event.");
        Game.client.PostEvent(new Events.Turn());
      },
      onCancel: (seq) => {
        Debug.Log("We got cancelled? :(");
      }
    );
  }
}

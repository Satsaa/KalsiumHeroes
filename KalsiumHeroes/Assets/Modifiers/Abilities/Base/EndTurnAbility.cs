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

  public override Targeter GetTargeter() {
    return new InstantTargeter(
      onComplete: OnTargeterComplete,
      onCancel: OnTargeterCancel
    );
  }

  protected override void OnTargeterComplete(Targeter targeter) {
    PostEvent(null);
  }

  protected override void PostEvent(GameHex hex) {
    Game.client.PostEvent(new Events.Turn());
  }
}

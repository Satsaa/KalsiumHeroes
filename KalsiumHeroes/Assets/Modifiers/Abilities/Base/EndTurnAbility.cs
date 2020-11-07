using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnAbility : Ability {

  public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) => default;

  public override Targeter GetTargeter() {
    return new InstantTargeter(
      onComplete: t => Game.client.PostEvent(new Events.Turn())
    );
  }
}

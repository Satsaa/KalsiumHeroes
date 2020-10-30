using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothingAbility : Ability {

  public override bool EventIsFinished() => true;
  public override bool SkipEvent() => true;
  public override void StartEvent(Events.Ability data) { }

}

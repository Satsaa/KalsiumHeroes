using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbility : Ability {

  [SerializeField, HideInInspector] float animTime;
  [SerializeField, HideInInspector] bool animating;
  [SerializeField, HideInInspector] GameHex start;
  [SerializeField, HideInInspector] GameHex target;
  [SerializeField, HideInInspector] GameHex[] path;

  void Update() {
    if (animating) UpdateAnim();
  }

  public override Targeter GetTargeter() {
    return new MoveAbilityTargeter(unit, this,
      onComplete: (seq) => {
        Debug.Log("Move sequence complete! Posting event.");
        Game.client.PostEvent(new Events.Ability() {
          ability = data.identifier,
          target = seq.selection[0].hex.pos,
          unit = unit.hex.hex.pos
        });
      },
      onCancel: (seq) => {
        Debug.Log("We got cancelled :(");
      }
    );
  }

  #region Anim

  void UpdateAnim() {
    animTime += Time.deltaTime;
    if (animTime > path.Length - 1) {
      EndEvent();
      return;
    }
    var current = Mathf.FloorToInt(animTime);
    var startPos = path[current].center;
    var targetPos = path[current + 1].center;
    unit.transform.position = Vector3.Lerp(startPos, targetPos, animTime % 1);
  }

  void EndEvent() {
    unit.transform.position = target.center;
    animating = false;
  }

  public override bool EventIsFinished() {
    return !animating;
  }

  public override bool SkipEvent() {
    if (animating) EndEvent();
    return true;
  }

  public override void StartEvent(Events.Ability data) {
    start = Game.grid.hexes[data.unit];
    target = Game.grid.hexes[data.target];
    Debug.Log("Handling move ability event!");
    if (target.unit || target.blocked) {
      Debug.LogError("Target hex is occupied!");
    } else {
      Game.grid.CheapestPath(start, target, out var path);
      this.path = path;
      animating = true;
      animTime = 0;
      unit.MovePosition(target, false);
    }
  }

  #endregion

}

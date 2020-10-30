using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbility : Ability {

  [SerializeField, HideInInspector] float animTime;
  [SerializeField, HideInInspector] bool animating;
  [SerializeField, HideInInspector] GameHex start;
  [SerializeField, HideInInspector] GameHex target;
  [SerializeField, HideInInspector] GameHex[] path;

  [SerializeField, HideInInspector] float remainingMovement;

  Predicate<GameHex> passablePredicate = h => !h.blocked && !h.unit;

  void Update() {
    if (animating) UpdateAnim();
  }

  public override bool IsReady() {
    return remainingMovement > 0 && base.IsReady();
  }

  protected override IEnumerable<GameHex> GetTargets_GetRangeTargets(GameHex hex) {
    var res = Game.grid.GetCostField(hex, maxCost: remainingMovement, passable: (h => !h.blocked && !h.unit)).costs.Keys;
    return res;
  }

  public override void OnTurnStart() {
    remainingMovement = unit.data.movement.value;
    base.OnTurnStart();
  }

  public override Targeter GetTargeter() {
    return new PathConfirmAbilityTargeter(unit, this,
      onComplete: t => PostDefaultAbilityEvent(t.selection[0])
    ) { passable = passablePredicate };
  }


  #region Anim

  void UpdateAnim() {
    animTime += Time.deltaTime * 3;
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
      Game.grid.CheapestPath(start, target, out var path, out var field, passablePredicate);
      var cost = field.scores[field.closest];
      remainingMovement -= cost;
      this.path = path;
      animating = true;
      animTime = 0;
      unit.MovePosition(target, false);
    }
  }

  #endregion

}

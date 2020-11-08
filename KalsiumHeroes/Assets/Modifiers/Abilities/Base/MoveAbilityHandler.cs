﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbilityHandler : EventHandler<Events.Ability> {

  public MoveAbility creator;

  [SerializeField, HideInInspector] float animTime;
  [SerializeField, HideInInspector] bool animating;
  [SerializeField, HideInInspector] GameHex start;
  [SerializeField, HideInInspector] GameHex target;
  [SerializeField, HideInInspector] GameHex[] path;

  public MoveAbilityHandler(Events.Ability data, MoveAbility creator) : base(data) {
    this.creator = creator;
    start = Game.grid.hexes[data.unit];
    target = Game.grid.hexes[data.target];
    Debug.Log("Handling move ability event!");
    if (target.unit || target.blocked) {
      Debug.LogError("Target hex is occupied!");
    } else {
      Game.grid.CheapestPath(start, target, out var path, out var field, MoveAbility.passablePredicate);
      var cost = field.scores[field.closest];
      creator.remainingMovement -= cost;
      this.path = path;
      animating = true;
      animTime = 0;
      creator.unit.MovePosition(target, false);
    }
  }

  public override void Update() {
    animTime += Time.deltaTime * 3;
    if (animTime > path.Length - 1) {
      End();
      return;
    }
    var current = Mathf.FloorToInt(animTime);
    var startPos = path[current].center;
    var targetPos = path[current + 1].center;
    creator.unit.transform.position = Vector3.Lerp(startPos, targetPos, animTime % 1);
  }

  public override bool EventHasEnded() {
    return !animating;
  }

  public override bool End() {
    creator.unit.transform.position = target.center;
    animating = false;
    return true;
  }

}
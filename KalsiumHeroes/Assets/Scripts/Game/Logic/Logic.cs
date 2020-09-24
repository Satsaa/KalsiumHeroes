
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Logic {

  public void OnMove(GameEvents.Move data) {
    Debug.Log($"({nameof(Logic)}) {nameof(OnMove)} called.");

    var source = Game.grid.hexes[data.unit];
    var target = Game.grid.hexes[data.target];

    // Move unit (not visually)
    // E.G.
    // target.logicContent.unit = source.logicContent.unit; // Move the unit to its new hex
    // source.logicContent.unit = null; // Remove the unit from its old hex
  }

  public void OnAbility(GameEvents.Ability data) {
    Debug.Log($"({nameof(Logic)}) {nameof(OnAbility)} called.");
  }

  public void OnHealth(GameEvents.Health data) {
    Debug.Log($"({nameof(Logic)}) {nameof(OnHealth)} called.");
  }

  public void OnDamage(GameEvents.Damage data) {
    Debug.Log($"({nameof(Logic)}) {nameof(OnDamage)} called.");
  }

  public void OnHeal(GameEvents.Heal data) {
    Debug.Log($"({nameof(Logic)}) {nameof(OnHeal)} called.");
  }

  public void OnCreate(GameEvents.Create data) {
    Debug.Log($"({nameof(Logic)}) {nameof(OnCreate)} called.");
  }

  public void OnRemove(GameEvents.Remove data) {
    Debug.Log($"({nameof(Logic)}) {nameof(OnRemove)} called.");
  }

}
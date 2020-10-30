using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class RoundManager {

  [SerializeField, HideInInspector] List<Unit> units = new List<Unit>();

  public Unit current {
    get {
      var res = units.LastOrDefault();
      if (!res) {
        Gather();
        res = units.LastOrDefault();
      }
      return res;
    }
  }

  [field: SerializeField] public int round { get; private set; }

  public void Next() {
    if (units.Count <= 1) {
      foreach (var modifier in current.modifiers) modifier.OnTurnEnd();
      round++;
      Gather();
      foreach (var modifier in Game.modifiers.GetModifiers()) modifier.OnRoundStart();
      foreach (var modifier in current.modifiers) modifier.OnTurnStart();
      return;
    }
    foreach (var modifier in current.modifiers) modifier.OnTurnEnd();
    units.RemoveAt(units.Count - 1);
    foreach (var modifier in current.modifiers) modifier.OnTurnStart();
  }


  void Gather() {
    units = Game.grid.hexes.Values.Where(v => v.unit != null).Select(v => v.unit).ToList();
    Sort();
  }

  public void Sort() {
    units.Sort((a, b) => b.data.speed.value - a.data.speed.value);
  }

  public void OnGameStart() {
    round++;
    Gather();
    foreach (var modifier in Game.modifiers.GetModifiers()) modifier.OnRoundStart();
    foreach (var modifier in current.modifiers) modifier.OnTurnStart();
  }
}
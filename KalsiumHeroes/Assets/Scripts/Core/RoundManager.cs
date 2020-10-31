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

  public void Sort() {
    units.Sort((a, b) => b.unitData.speed.value - a.unitData.speed.value);
  }

  void Gather() {
    units = Game.grid.hexes.Values.Where(v => v.unit != null).Select(v => v.unit).ToList();
    Sort();
  }

  public void Next() {
    if (units.Count <= 1) {
      OnTurnEnds();
      round++;
      Gather();
      OnRoundStarts();
      OnTurnStarts();
      return;
    }
    OnTurnEnds();
    units.RemoveAt(units.Count - 1);
    OnTurnStarts();
  }

  public void OnGameStart() {
    round++;
    Gather();
    OnRoundStarts();
    OnTurnStarts();
  }

  private void OnRoundStarts() { foreach (var modifier in Game.modifiers.GetModifiers()) modifier.OnRoundStart(); }
  private void OnTurnEnds() { foreach (var modifier in current.modifiers) modifier.OnTurnEnd(); }
  private void OnTurnStarts() { foreach (var modifier in current.modifiers) modifier.OnTurnStart(); }
}
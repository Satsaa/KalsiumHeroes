using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoundManager {

  [SerializeField, HideInInspector] List<Unit> units = new List<Unit>();

  public Unit current => units.Count <= 0 ? null : units[units.Count - 1];
  [field: SerializeField] public int round { get; private set; }

  public void Next() {
    if (units.Count <= 1) {
      NextTurn();
      return;
    }
    units.RemoveAt(units.Count - 1);
    if (units.Count <= 0) {
      NextTurn();
      return;
    }
  }

  public void Sort() {
    units.Sort((a, b) => b.GetSpeed() - a.GetSpeed());
  }

  private void NextTurn() {
    round++;
    units = Game.grid.hexes.Values.Where(v => v.unit != null).Select(v => v.unit).ToList();
    Sort();
  }
}
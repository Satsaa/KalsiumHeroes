using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
      round++;
      Gather();
      return;
    }
    units.RemoveAt(units.Count - 1);
  }


  void Gather() {
    units = Game.grid.hexes.Values.Where(v => v.unit != null).Select(v => v.unit).ToList();
    Sort();
  }

  public void Sort() {
    units.Sort((a, b) => b.GetSpeed() - a.GetSpeed());
  }
}
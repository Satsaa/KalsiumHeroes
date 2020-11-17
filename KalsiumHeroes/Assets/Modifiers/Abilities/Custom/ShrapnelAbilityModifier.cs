using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrapnelAbilityModifier : Modifier {

  public ShrapnelAbilityData casterData;

  [HideInInspector] public IEnumerable<GameHex> aoe;
  [HideInInspector] public GameHex target;

  public override void OnTurnStart() {
    base.OnTurnStart();

    foreach (var hex in aoe) {
      if (hex.unit) hex.unit.Damage(casterData.damage.value, casterData.damageType);
    }

    Debug.Log("We DID DAMGE?");
    Destroy(this);
  }

}

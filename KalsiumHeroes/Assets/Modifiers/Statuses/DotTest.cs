using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTest : StatusEffect {

  public DotTestData dotTestData => (DotTestData)data;

  public override void OnTurnEnd() {
    unit.Damage(dotTestData.damage.value, dotTestData.damageType);
    base.OnTurnEnd();
  }
}

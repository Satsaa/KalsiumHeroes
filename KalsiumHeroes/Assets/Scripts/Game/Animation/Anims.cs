
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Anims {

  [SerializeReference] public List<object> stack = new List<object>();
  [SerializeField] public GameEvent first => (GameEvent)stack[0];

  IAnimsProvider provider = null;

  public bool TryRunNextAnim() {
    if (provider == null || provider.IsFinished()) {
      first.InvokeAnims();
      stack.RemoveAt(0);
      return true;
    } else {
      // Animation is currently running...
      return false;
    }
  }


  public void AnimateMove(GameEvents.Move data) {
    Debug.Log($"({nameof(Anims)}) {nameof(AnimateMove)} called.");

    var source = Game.grid.hexes[data.unit];
    var target = Game.grid.hexes[data.target];

    // Use IAnimsProvider for animation implementation
    // Something like...

    /*
    AnimsUnit unit = source.animsContent.unit;
    IAnimsProvider<GameEvents.Move> provider = unit.moveAnim;
    this.provider = provider;
    provider.StartAnim(data);
    */
  }

  public void AnimateAbility(GameEvents.Ability data) {
    Debug.Log($"({nameof(Anims)}) {nameof(AnimateAbility)} called.");
  }

  public void AnimateHealth(GameEvents.Health data) {
    Debug.Log($"({nameof(Anims)}) {nameof(AnimateHealth)} called.");
  }

  public void AnimateDamage(GameEvents.Damage data) {
    Debug.Log($"({nameof(Anims)}) {nameof(AnimateDamage)} called.");
  }

  public void AnimateHeal(GameEvents.Heal data) {
    Debug.Log($"({nameof(Anims)}) {nameof(AnimateHeal)} called.");
  }

  public void AnimateCreate(GameEvents.Create data) {
    Debug.Log($"({nameof(Anims)}) {nameof(AnimateCreate)} called.");
  }

  public void AnimateRemove(GameEvents.Remove data) {
    Debug.Log($"({nameof(Anims)}) {nameof(AnimateRemove)} called.");
  }

}

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Muc.Extensions;

public abstract class ControlFlow {
  List<ControlFlowSegment> segments;

  public abstract bool Next();
  public abstract bool Valid();
}

public abstract class ControlFlowSegment {

  public HashSet<GameHex> targets = new HashSet<GameHex>();
  public HashSet<GameHex> selection = new HashSet<GameHex>();

  public List<Dictionary<GameHex, Color>> highlights = new List<Dictionary<GameHex, Color>>();


  public abstract bool IsCompleted();
  public abstract void RefreshTargets();
  public abstract void RefreshHighlights();

}


public class AbilityTargetControlSegment : ControlFlowSegment {

  public Ability ability;

  public override bool IsCompleted() {
    return false;
  }

  public override void RefreshTargets() {

    // Get range
    // Game.grid.GetRange();

    if ((ability.targetType & TargetType.Ground) != 0) {

    } else { // Ground targeting is all-encompassing
      if ((ability.targetType & TargetType.Self) != 0) {

      }
      if ((ability.targetType & TargetType.Ally) != 0) {

      }
      if ((ability.targetType & TargetType.Enemy) != 0) {

      }
    }
  }

  public override void RefreshHighlights() {

  }

}
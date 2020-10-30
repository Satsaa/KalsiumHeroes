using System.Collections.Generic;

using UnityEngine;
using System;
using System.Linq;

public class Targeting : MonoBehaviour {

  private RoundManager rm => Game.rounds;
  private Events e => Game.events;
  private bool finished => e.finished;

  private GameHex prevHoverHex;

  Targeter seq;
  [SerializeField] new Camera camera;


  public bool TryStartSequence(Targeter sequence) {
    if (seq != null) return false;
    seq = sequence;
    Refresh();
    seq.RefreshTargets();
    return true;
  }


  void Start() {
    if (camera == null) camera = Camera.main;
  }

  void Update() {
    if (seq != null) {
      if (!TryComplete()) {
        var hex = Game.grid.RaycastHex(camera.ScreenPointToRay(Input.mousePosition));
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
          if (hex == null) {
            TryCancel();
            return;
          }
          if (seq.Select(hex)) {
            if (!TryComplete()) {
              Refresh();
            }
          } else {
            TryCancel();
          }
        } else {
          if (prevHoverHex != hex && hex != null) {
            prevHoverHex = hex;
            seq.Hover(hex);
            Refresh();
          }
        }
      }
    }
  }


  bool TryComplete() {
    if (seq.IsCompleted()) {
      Complete();
      return true;
    }
    return false;
  }

  void Complete() {
    seq.onComplete(seq);
    Remove();
  }


  bool TryCancel() {
    if (seq.Cancel()) {
      Cancel();
      return true;
    }
    return false;
  }

  void Cancel() {
    if (seq.onCancel != null) seq.onCancel(seq);
    Remove();
  }


  void Remove() {
    seq = null;
    prevHoverHex = null;
    ClearHighlights();
  }


  void Refresh() {
    seq.RefreshHighlights();

    ClearHighlights();

    foreach (var kv in seq.highlights) {
      var hex = kv.Key;
      var color = kv.Value;
      hex.highlighter.Highlight(color);
    }
  }

  void ClearHighlights() {
    foreach (var kv in Game.grid.hexes) {
      var hex = kv.Value;
      hex.highlighter.ResetHighlight();
    }
  }

}

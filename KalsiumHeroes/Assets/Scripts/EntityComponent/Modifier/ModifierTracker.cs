
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to track the modifiers in the scene. It enables efficient enumeration of modifiers as GameObject.FindObjectsOfType is not used.
/// </summary>
[System.Serializable]
public class ModifierTracker {

  [SerializeField] private HashSet<Modifier> modifiers = new HashSet<Modifier>();
  [SerializeField] private HashSet<Ability> abilities = new HashSet<Ability>();
  [SerializeField] private HashSet<StatusEffect> statuses = new HashSet<StatusEffect>();

  public IEnumerable<T> GetModifiers<T>(bool includeInactive = false) {
    foreach (var modifier in EnumerateModifiers(includeInactive)) {
      if (modifier is T res) yield return res;
    }
  }
  public IEnumerable<Modifier> GetModifiers(bool includeInactive = false) {
    return EnumerateModifiers(includeInactive);
  }
  public IEnumerable<Ability> GetAbilities(bool includeInactive = false) {
    return EnumerateAbilities(includeInactive);
  }
  public IEnumerable<StatusEffect> GetStatuses(bool includeInactive = false) {
    return EnumerateStatuses(includeInactive);
  }

  /// <summary> Finds all loaded Modifiers and their subtypes </summary>
  public void ResetModifiers() {
    modifiers.Clear();
    abilities.Clear();
    statuses.Clear();

    var sceneItems = GameObject.FindObjectsOfType<Modifier>(true);
    modifiers.UnionWith(sceneItems);
    abilities.UnionWith(sceneItems.Where(v => v is Ability).Cast<Ability>());
    statuses.UnionWith(sceneItems.Where(v => v is StatusEffect).Cast<StatusEffect>());
  }

  /// <summary> Adds the Modifier to the pool of modifiers </summary>
  public void RegisterModifier<T>(T modifier) where T : Modifier {
    modifiers.Add(modifier);
    if (modifier is Ability ability) abilities.Add(ability);
    if (modifier is StatusEffect status) statuses.Add(status);
  }

  private IEnumerable<Modifier> EnumerateModifiers(bool includeInactive) {
    var toRemove = new List<Modifier>();
    foreach (var mod in modifiers) {
      if (mod == null) {
        toRemove.Add(mod);
      } else {
        if (includeInactive || mod.isActiveAndEnabled) {
          yield return mod;
        }
      }
    }
    foreach (var remove in toRemove) {
      modifiers.Remove(remove);
    }
  }

  private IEnumerable<Ability> EnumerateAbilities(bool includeInactive) {
    var toRemove = new List<Ability>();
    foreach (var ability in abilities) {
      if (ability == null) {
        toRemove.Add(ability);
      } else {
        if (includeInactive || ability.isActiveAndEnabled) {
          yield return ability;
        }
      }
    }
    foreach (var remove in toRemove) {
      abilities.Remove(remove);
    }
  }
  private IEnumerable<StatusEffect> EnumerateStatuses(bool includeInactive) {
    var toRemove = new List<StatusEffect>();
    foreach (var status in statuses) {
      if (status == null) {
        toRemove.Add(status);
      } else {
        if (includeInactive || status.isActiveAndEnabled) {
          yield return status;
        }
      }
    }
    foreach (var remove in toRemove) {
      statuses.Remove(remove);
    }
  }
}
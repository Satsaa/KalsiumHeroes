
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to cache the modifiers in the scene. It enables efficient enumeration of modifiers as GameObject.FindObjectsOfType is not used.
/// </summary>
[System.Serializable]
public class ModifierCache {

  [SerializeField] private HashSet<Modifier> modifiers = new HashSet<Modifier>();
  [SerializeField] private HashSet<Ability> abilities = new HashSet<Ability>();
  [SerializeField] private HashSet<Passive> passives = new HashSet<Passive>();
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
  public void BuildCache() {
    modifiers.Clear();
    abilities.Clear();
    statuses.Clear();

    var units = GameObject.FindObjectsOfType<Unit>(true);
    modifiers.UnionWith(units.SelectMany(v => v.modifiers));
    abilities.UnionWith(units.SelectMany(v => v.abilities));
    statuses.UnionWith(units.SelectMany(v => v.statuses));
  }


  /// <summary> Adds the Modifier to the pool of modifiers. Modifiers are removed during enumeration if they are null. </summary>
  public void RegisterModifier<T>(T modifier) where T : Modifier {
    modifiers.Add(modifier);
    if (modifier is Ability ability) abilities.Add(ability);
    if (modifier is StatusEffect status) statuses.Add(status);
    if (modifier is Passive passive) passives.Add(passive);
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
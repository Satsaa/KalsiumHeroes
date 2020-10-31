


#if UNITY_EDITOR
using UnityEditor;
using Muc.Editor;
using static Muc.Editor.EditorUtil;
#endif

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = nameof(AbilityData), menuName = "DataSources/" + nameof(AbilityData))]
public class AbilityData : UnitModifierData {

  [Header("Ability Data")]
  [Tooltip("Type of the ability.")]
  public AbilityType abilityType;

  [Tooltip("Passive abilities cannot be cast.")]
  public Attribute<bool> passive = new Attribute<bool>(false);

  [Tooltip("Types of valid targets.")]
  public TargetType targetType;

  [Tooltip("Cast range of the ability.")]
  public ToggleAttribute<int> range = new ToggleAttribute<int>(1, true);

  [Tooltip("How the range is determined.")]
  public RangeMode rangeMode;

  [Tooltip("Only directly visible hexes are valid in range?")]
  public Attribute<bool> requiresVision = new Attribute<bool>(false);

  [AttributeLabels("Current", "Max")]
  [Tooltip("How many turns it takes for this ability to gain a charge.")]
  public DualAttribute<int> cooldown = new DualAttribute<int>(0, 0);

  [AttributeLabels("Current", "Max")]
  [Tooltip("How many charges does the ability have.")]
  public DualAttribute<int> charges = new DualAttribute<int>(1, 1);

  [Tooltip("How many times can the ability be cast in total.")]
  public ToggleAttribute<int> uses = new ToggleAttribute<int>(false);

}


#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(AbilityData), true)]
public class AbilityDataEditor : Editor {

  SerializedProperty passive;
  SerializedProperty script;

  AbilityData t => (AbilityData)target;
  IEnumerable<AbilityData> ts => targets.Cast<AbilityData>();

  void OnEnable() {
    passive = serializedObject.FindProperty(nameof(AbilityData.passive));
    script = serializedObject.FindProperty("m_Script");
  }

  public override void OnInspectorGUI() {
    serializedObject.Update();

    if (ts.All(t => t.passive.value)) {
      using (DisabledScope(v => true)) EditorGUILayout.PropertyField(script);
      DrawPropertiesExcluding(
        serializedObject,
        nameof(AbilityData.targetType),
        nameof(AbilityData.range),
        nameof(AbilityData.rangeMode),
        nameof(AbilityData.requiresVision),
        nameof(AbilityData.cooldown),
        nameof(AbilityData.charges),
        nameof(AbilityData.uses),
        "m_Script"
      );
    } else {
      DrawDefaultInspector();
    }
    serializedObject.ApplyModifiedProperties();
  }
}
#endif
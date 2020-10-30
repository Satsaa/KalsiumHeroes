
using Muc.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(UnitModifierData), menuName = "DataSources/" + nameof(UnitModifierData))]
public class UnitModifierData : ScriptableObject {

  [Tooltip("Name displayed to users")]
  public string displayName;
  [Tooltip("String identifier of this Ability")]
  public string identifier;
  [Tooltip("Description displayed to users")]
  public string description;
  [Tooltip("Displayed image")]
  public Sprite sprite;

}

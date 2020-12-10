


#if UNITY_EDITOR
using UnityEditor;
using Muc.Editor;
using static Muc.Editor.EditorUtil;
#endif

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = nameof(PassiveData), menuName = "DataSources/" + nameof(PassiveData))]
public class PassiveData : UnitModifierData {

	[Tooltip("Passives are currently Modifiers which are shown as abilities.")]
	public string passivesAreJustModifiers;
}

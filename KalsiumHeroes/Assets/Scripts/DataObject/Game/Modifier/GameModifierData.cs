
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Components.Extended;
using System.Threading.Tasks;
using Muc.Editor;
using Serialization;
using Object = UnityEngine.Object;
using UnityEngine.AddressableAssets;

public abstract class GameModifierData : ModifierData {

	public override Type createTypeConstraint => typeof(GameModifier);


}
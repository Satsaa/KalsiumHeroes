
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

public abstract class GameModifier : Modifier {

	new public GameModifierData source => (GameModifierData)_source;
	new public GameModifierData data => (GameModifierData)_data;
	public override Type dataType => typeof(GameModifierData);

}
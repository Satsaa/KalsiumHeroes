
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Serialization;
using System.Reflection;
using Muc.Editor;

[CreateAssetMenu(fileName = nameof(TestItem), menuName = "KalsiumHeroes/" + nameof(TestItem))]
public class TestItem : ScriptableObject {

	[ShowEditor]
	public TestItem innerTestItem;

}

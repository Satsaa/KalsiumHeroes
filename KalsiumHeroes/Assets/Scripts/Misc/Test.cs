
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
using Muc.Data;
using Muc.Time;

public class Test : MonoBehaviour {

	public ToggleValue<string> toggleValue;
	public Interval interval;
	public SceneReference sceneReference;

	[SerializeField] private SceneReference gameScene;
	[SerializeField, ShowEditor] GameEvents _events;
}

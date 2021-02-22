
using System;
using System.Collections.Generic;
using System.Linq;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

/// <summary>
/// Master is automatically set when the container is instantiated for a Modifier.
/// </summary>
public interface IContainerComponent {
	public void SetMaster(Master master);
}
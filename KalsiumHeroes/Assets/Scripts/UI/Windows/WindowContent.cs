
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Rendering;

[ExecuteAlways]
[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(RectTransform))]
public class WindowContent : UIBehaviour {

	public RectTransform rectTransform => (RectTransform)transform;

	public void LateUpdate() {


	}

}
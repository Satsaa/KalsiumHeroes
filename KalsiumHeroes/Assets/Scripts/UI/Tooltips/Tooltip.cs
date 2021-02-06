
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

	public virtual void MoveContent(Transform parent) {
		Destroy(GetComponent<Image>());
		transform.SetParent(parent);
	}
}
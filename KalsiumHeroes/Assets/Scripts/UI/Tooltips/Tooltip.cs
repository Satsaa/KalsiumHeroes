
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

	public int index;
	public string id;
	public GameObject creator;

	public virtual void MoveContent(Transform parent) {
		transform.SetParent(parent);
		Destroy(GetComponent<Image>());
	}
}
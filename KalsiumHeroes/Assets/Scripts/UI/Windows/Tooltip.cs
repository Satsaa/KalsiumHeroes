
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

	[HideInInspector] public int index;
	[HideInInspector] public string id;
	[HideInInspector] public GameObject root;
	[HideInInspector] public GameObject creator;

	/// <summary> Removes any background of the tooltip and reparents the content. </summary>
	public virtual void MoveContent(Transform newParent) {
		transform.SetParent(newParent);
		Destroy(GetComponent<Image>());
	}
}
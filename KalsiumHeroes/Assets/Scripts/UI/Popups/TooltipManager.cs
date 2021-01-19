
using System.Linq;
using System.Collections.Generic;
using Muc.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TooltipManager : MonoBehaviour {

	public SerializedStack<Tooltip> tooltips;
	public SerializedStack<int> tooltipsTest = new SerializedStack<int>(new int[] { 0, 2, 4, 6, 8 });

	public void ShowTooltip() {
		var go = new GameObject("Tooltip");
		var tt = go.AddComponent<Tooltip>();
		tooltips.Push(tt);
	}
}

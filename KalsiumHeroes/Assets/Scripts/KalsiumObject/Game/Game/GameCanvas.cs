
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
using UnityEngine.UIElements;

/// <summary> Game handler. Literally the thing that makes the game work. </summary>
[RequireComponent(typeof(Canvas))]
public class GameCanvas : MonoBehaviour {

	[Serializable]
	protected class OrderedItem {
		public RectTransform rt;
		public Transform distanceFrom;
		public OrderedItem(RectTransform rt, Transform distanceFrom) { this.rt = rt; this.distanceFrom = distanceFrom; }
	}

	[field: SerializeField] public Canvas canvas { get; protected set; }
	[field: SerializeField] public RectTransform worldElements { get; protected set; }

	[SerializeField] List<OrderedItem> orderedItems;

	protected virtual void OnValidate() {
		canvas = GetComponent<Canvas>();
	}

	protected virtual void LateUpdate() {
		orderedItems.Sort(
			(a, b) => (b.distanceFrom.position - Camera.main.transform.position).sqrMagnitude
			.CompareTo((a.distanceFrom.position - Camera.main.transform.position).sqrMagnitude)
		);
		for (int i = 0; i < orderedItems.Count; i++) {
			orderedItems[i].rt.SetSiblingIndex(i);
		}

	}

	public void AddOrderedElement(RectTransform rt, Transform distanceFrom) {
		orderedItems.Add(new(rt, distanceFrom));
	}

	public bool RemoveOrderedElement(RectTransform rt) {
		var index = orderedItems.FindIndex(v => v.rt = rt);
		if (index >= 0) {
			orderedItems.RemoveAt(index);
		}
		return index >= 0;
	}

}

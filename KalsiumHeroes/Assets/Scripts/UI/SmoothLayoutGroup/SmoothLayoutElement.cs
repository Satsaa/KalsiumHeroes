
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using Muc.Components.Extended;
using Muc.Components;
using static Muc.Components.VirtualLayoutGroup;

public class SmoothLayoutElement : VirtualLayoutElement {

	public class SmoothData : Data {

		public override VirtualLayoutElement CreateElement(Storage storage, VirtualLayoutGroup group) {
			if (group is SmoothLayoutGroup smooth && smooth.animatingStorages.ContainsKey(storage)) {
				return smooth.animatingStorages[storage];
			}
			return InstantiateWithComponent<SmoothLayoutElement>(group);
		}

	}

	public override void OnHide(VirtualLayoutGroup group) {
		if (!(group is SmoothLayoutGroup smooth) || !smooth.animations.ContainsKey(this)) {
			base.OnHide(group);
		}
	}

}


namespace Muc.Components {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Object = UnityEngine.Object;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using UnityEngine.Pool;
	using Muc.Components.Extended;
	using static VirtualLayoutGroup;

#if (MUC_HIDE_COMPONENTS || MUC_HIDE_GENERAL_COMPONENTS)
	[AddComponentMenu("")]
#else
	[AddComponentMenu("MyUnityCollection/General/" + nameof(VirtualLayoutElement))]
#endif
	public class VirtualLayoutElement : MonoBehaviour {

		[Serializable]
		public class Data {

			public RectTransform prefab;

			public virtual VirtualLayoutElement CreateElement(Storage storage, VirtualLayoutGroup group) {
				return InstantiateWithComponent<VirtualLayoutElement>(group);
			}

			protected T InstantiateWithComponent<T>(VirtualLayoutGroup group) where T : VirtualLayoutElement {
				var go = Instantiate(prefab, group.transform).gameObject;
				if (go.TryGetComponent<T>(out var res)) {
					return res;
				}
				return go.AddComponent<T>();
			}
		}

		public virtual void UpdateData(Data data) {
		}

		public virtual void OnHide(VirtualLayoutGroup group) {
			Destroy(gameObject);
		}

	}

}


namespace Muc.Systems.Camera {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

#if (MUC_HIDE_COMPONENTS || MUC_HIDE_SYSTEM_COMPONENTS)
  [AddComponentMenu("")]
#else
	[AddComponentMenu("MyUnityCollection/" + nameof(Muc.Systems.Camera) + "/" + nameof(MyUnityCameraZoom))]
#endif
	[RequireComponent(typeof(MyUnityCamera))]
	public class MyUnityCameraZoom : MonoBehaviour {

		[Serializable]
		public class Alternative {
			[Tooltip("Press this key to change the multiplier.")]
			public KeyCode key = KeyCode.None;

			[Min(0), Tooltip("Distance multiplier multiplier when pressing the key.")]
			public float multiplier = 0.2f;
		}

		[Min(0), Tooltip("Distance multiplier.")]
		public float multiplier = 1.2f;

		[Min(0), Tooltip("Minimum change in distance.")]
		public float minStep = 0.1f;

		[Tooltip("Minimum and maximum or distance.")]
		public Vector2 range = new Vector2(1.2f, 25f);

		[Tooltip("Make the strength of scroll affect the amount of zoom.")]
		public bool multiplyByDelta = false;

		[Tooltip("Define other multipliers when other keys are pressed. First activated alternative is used.")]
		public Alternative[] alternatives;


		MyUnityCamera pc;

		void Start() {
			pc = gameObject.GetComponent<MyUnityCamera>();
		}

		void Update() {
			if (Input.mouseScrollDelta.y != 0) {
				var delta = Input.mouseScrollDelta.y;
				if (delta < 0) {
					if (pc.distance <= range.y) {
						var v = (multiplyByDelta ? delta : 1) * Mathf.Max(pc.distance * multiplier - pc.distance, minStep);
						foreach (var alternative in alternatives) {
							if (Input.GetKey(alternative.key)) {
								v *= alternative.multiplier;
								break;
							}
						}
						pc.distance += v;
						pc.distance = Mathf.Min(pc.distance, range.y);
					}
				} else {
					if (pc.distance >= range.x) {
						float v = (multiplyByDelta ? delta : 1) * Mathf.Max(pc.distance - pc.distance / multiplier, minStep);
						foreach (var alternative in alternatives) {
							if (Input.GetKey(alternative.key)) {
								v *= alternative.multiplier;
								break;
							}
						}
						pc.distance -= v;
						pc.distance = Mathf.Max(pc.distance, range.x);
					}
				}
			}
		}
	}

}

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
using static SmoothLayoutElement;

// Smooth movement of items

public class SmoothLayoutGroup : VirtualLayoutGroup {

	[Serializable]
	public class Animation {

		public float prevEval;
		public float time;
		public float change;

		public Animation(float change) {
			this.change = change;
		}
	}
	[SerializeField] public float animationDuration = 1f;
	[SerializeField] public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	[SerializeField, HideInInspector] public Dictionary<SmoothLayoutElement, List<Animation>> animations = new();
	[SerializeField, HideInInspector] public Dictionary<SmoothLayoutElement, Storage> storages = new();
	[SerializeField, HideInInspector] public Dictionary<Storage, SmoothLayoutElement> animatingStorages = new();

	protected virtual void Update() {
		if (animations.Count == 0) return;
		var toRemove = ListPool<KeyValuePair<SmoothLayoutElement, List<Animation>>>.Get();
		foreach (var kv in animations) {
			var element = kv.Key;
			var anims = kv.Value;
			var removes = 0;
			foreach (var anim in anims) {
				anim.time = Mathf.Clamp01(anim.time + Time.deltaTime / animationDuration);
				var eval = movementCurve.Evaluate(anim.time) * anim.change;
				var movement = eval - anim.prevEval;
				anim.prevEval = eval;
				element.transform.localPosition += new Vector3(vertical ? 0 : movement, vertical ? movement : 0);
				if (anim.time >= 1) {
					removes++;
				}
			}
			for (int i = 0; i < removes; i++) {
				anims.RemoveRange(0, removes);
			}
			if (anims.Count == 0) {
				toRemove.Add(kv);
			}
		}
		if (toRemove.Any()) {
			UpdateVisibility(1);
			UpdateVisibility(-1);
			foreach (var kv in toRemove) {
				var element = kv.Key;
				var anims = kv.Value;
				animations.Remove(element);
				var storage = storages[element];
				storages.Remove(element);
				animatingStorages.Remove(storage);
				if (storage.element != element || storage.index < visI || storage.index > visI + visCount - 1) {
					element.OnHide(this);
				} else if (storage.element) {
					// Reset position just in case
					PlaceElement(storage);
				}
			}
		}
		ListPool<KeyValuePair<SmoothLayoutElement, List<Animation>>>.Release(toRemove);
	}

	public override Storage InsertElement(int index, RectTransform prefab) => InsertElement(index, new SmoothData() { prefab = prefab });
	public virtual Storage InsertElement(int index, SmoothData data) => base.InsertElement(index, data);
	[Obsolete] new public virtual Storage InsertElement(int index, VirtualLayoutElement.Data data) => base.InsertElement(index, data);

	public override Storage AddElement(RectTransform prefab) => AddElement(new SmoothData() { prefab = prefab });
	public virtual Storage AddElement(SmoothData data) => base.AddElement(data);
	[Obsolete] new public virtual Storage AddElement(VirtualLayoutElement.Data data) => base.AddElement(data);

	protected override void ApplyElementMovement(Storage storage, float change) {
		if (storage.element is SmoothLayoutElement) {
			StartAnimation(storage, change);
		} else {
			PlaceElement(storage);
		}
	}

	protected void StartAnimation(Storage storage, float change) {
		var element = (SmoothLayoutElement)storage.element;
		element.transform.SetAsLastSibling();
		if (!animations.TryGetValue(element, out var anims)) {
			animations[element] = anims = new();
			storages[element] = storage;
			animatingStorages.Add(storage, element);
		}
		anims.Add(new(change));
	}

	protected override void Show(Storage storage) {
		if (animatingStorages.TryGetValue(storage, out var element)) {
			storage.element = element;
		} else {
			base.Show(storage);
		}
	}

	protected override void Hide(Storage storage) {
		base.Hide(storage);
	}

}

#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;
	using UnityEngine.UI;
	using static Muc.Components.VirtualLayoutGroup;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(SmoothLayoutGroup), true)]
	public class SmoothLayoutGroupEditor : Editor {

		[Serializable]
		public class CustomData : SmoothData {
			public Color color;
			public override VirtualLayoutElement CreateElement(Storage storage, VirtualLayoutGroup group) {
				var res = base.CreateElement(storage, group);
				res.GetComponentInChildren<Graphic>().color = color;
				return res;
			}
		}

		SmoothLayoutGroup t => (SmoothLayoutGroup)target;

		// SerializedProperty property;

		void OnEnable() {
			// property = serializedObject.FindProperty(nameof(property));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawDefaultInspector();

			if (GUILayout.Button("Move element at 30 to 25")) {
				t.MoveElement(30, 25);
			}
			if (GUILayout.Button("Move element at 25 to 30")) {
				t.MoveElement(25, 30);
			}
			if (GUILayout.Button("Insert test element at 30")) {
				t.InsertElement(30, new CustomData() {
					prefab = t.testPrefab,
					color = Color.Lerp(Color.green, Color.blue, UnityEngine.Random.value),
				});
			}
			if (GUILayout.Button("Add test element")) {
				t.AddElement(new CustomData() {
					prefab = t.testPrefab,
					color = Color.Lerp(Color.green, Color.red, UnityEngine.Random.value),
				});
			}
			if (GUILayout.Button("Add 100 test elements")) {
				for (int i = 0; i < 100; i++) {
					t.AddElement(new CustomData() {
						prefab = t.testPrefab,
						color = Color.Lerp(Color.green, Color.red, UnityEngine.Random.value),
					});
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif

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
	using static VirtualLayoutElement;

#if (MUC_HIDE_COMPONENTS || MUC_HIDE_GENERAL_COMPONENTS)
	[AddComponentMenu("")]
#else
	[AddComponentMenu("MyUnityCollection/General/" + nameof(VirtualLayoutGroup))]
#endif
	[DefaultExecutionOrder(5)]
	public class VirtualLayoutGroup : ExtendedUIBehaviour, ILayoutElement {

		public class Storage : ScriptableObject {

			[field: SerializeReference] public Data data { get; protected internal set; }
			[field: SerializeField] public VirtualLayoutElement element { get; set; }
			[field: SerializeField] public int index { get; protected internal set; }
			[field: SerializeField] public float start { get; protected internal set; }
			[field: SerializeField] public float size { get; protected internal set; }
			public float end => start + size;

		}

		public RectTransform testPrefab;

		[field: SerializeField] public bool vertical { private set; get; } = false;

		[SerializeField] protected float spacing = 0;

		[SerializeField] protected List<Storage> items;
		[SerializeField] protected int visI;
		[SerializeField] protected int visCount = 0;
		[SerializeField] protected int dir = 0;

		[SerializeField] protected float prevPos = 0;

		public virtual void UpdateVisibility(int direction = 0) {
			if (!items.Any()) return;
			float startPos = 0;
			float endPos = 0;
			if (visCount > 0) {
				startPos = items[visI].start;
				endPos = items[visI + visCount - 1].end;
			}
			var vis = GetVisibleArea();
			var midPos = vis.x + (vis.y - vis.x) / 2;
			if (direction == 0 && prevPos == vis.x) return;
			dir = direction != 0 ? direction : prevPos < vis.x ? 1 : -1;
			prevPos = vis.x;

			// Visibility completely changed
			if (vis.x > endPos || vis.y < startPos) {
				DoFull();
				if (visCount == 0) {
					if (vis.y < startPos) {
						visI = 0;
						visI = 0;
					} else {
						visI = items.Count;
					}
				}
				return;
			}
			ShowNew();
			HideOld();

			void DoFull() {
				for (int i = visI; i <= visI + visCount && i < items.Count; i++) {
					Hide(items[i]);
				}

				var count = 0;
				var first = -1;
				for (int i = dir == 1 ? visI : visI + visCount - 1; dir == 1 ? i < items.Count : i >= 0; i += dir) {
					var item = items[i];
					if (Visibility(item.start, item.end) == 0) {
						Show(item);
						if (first == -1) first = i;
						count++;
					} else if (first != -1) {
						break;
					}
				}
				if (first == -1) {
					visI = dir == 1 ? items.Count - 1 : 0;
				} else {
					visI = dir == 1 ? first : first - count + 1;
				}
				visCount = count;
				if (visI <= -1) {
					Debug.LogWarning("visI invalid");
				}
			}

			void ShowNew() {
				for (int i = dir == 1 ? visI + visCount : visI - 1; dir == 1 ? i < items.Count : i >= 0; i += dir) {
					var item = items[i];
					if (Visibility(item.start, item.end) == 0) {
						Show(item);
						if (dir == -1) visI--;
						visCount++;
					} else {
						break;
					}
				}
			}

			void HideOld() {
				for (int i = dir == 1 ? visI : visI + visCount - 1; dir == 1 ? i < items.Count : i >= 0; i += dir) {
					var item = items[i];
					if (Visibility(item.start, item.end) != 0) {
						Hide(item);
						if (dir == 1) visI++;
						visCount--;
					} else {
						break;
					}
				}
			}
		}

		protected override void OnRectTransformDimensionsChange() {
			base.OnRectTransformDimensionsChange();
			UpdateVisibility();
		}

		public Vector2 GetVisibleArea() {
			var parent = (RectTransform)transform.parent;
			return new(
				vertical ? parent.rect.yMin - transform.localPosition.y : parent.rect.xMin - transform.localPosition.x,
				vertical ? parent.rect.yMax - transform.localPosition.y : parent.rect.xMax - transform.localPosition.x
			);
		}

		/// <summary> Returns -1 if before the visible part, 0 if inside, and 1 if after. </summary>
		public int Visibility(float start, float end) {
			var visibility = GetVisibleArea();
			if (end < visibility.x) return -1;
			if (start > visibility.y) return 1;
			return 0;
		}

		public float GetSize(RectTransform rt) {
			var constraint = vertical ? rectTransform.rect.width : rectTransform.rect.height;
			var rect = rt.rect;
			return vertical ? rect.height / (rect.width / constraint) : rect.width / (rect.height / constraint);
		}

		public virtual void MoveElement(int from, int to) {
			if (from == to) return;
			if (from < 0 || from > items.Count || to < 0 || to > items.Count) throw new IndexOutOfRangeException();

			var item = items[from];
			items.RemoveAt(from);
			items.Insert(to, item);

			if (from < to) {
				// 3 -> 7
				// 1,2,3,4,5,6,7,8,9
				// 1,2[4,5,6,7]3,8,9 <-
				for (int i = from; i < to; i++) {
					var other = items[i];
					other.index--;
					if (!other.element && Visibility(other.start - item.size, other.end - item.size) == 0) {
						other.element = other.data.CreateElement(other, this);
						PlaceElement(other);
					}
					other.start -= item.size;
					if (other.element) {
						ApplyElementMovement(other, -item.size);
					}
				}
			} else {

				// 7 -> 3
				// 1,2,3,4,5,6,7,8,9
				// 1,2,7[3,4,5,6]8,9 ->
				for (int i = to + 1; i <= from; i++) {
					var other = items[i];
					other.index++;
					if (!other.element && Visibility(other.start + item.size, other.end + item.size) == 0) {
						other.element = other.data.CreateElement(other, this);
						PlaceElement(other);
					}
					other.start += item.size;
					if (other.element) {
						ApplyElementMovement(other, item.size);
					}
				}
			}

			var newPos = to == 0 ? 0 : items[to - 1].end;
			if (!item.element && Visibility(newPos, newPos + item.size) == 0) {
				item.element = item.data.CreateElement(item, this);
				PlaceElement(item);
			}
			if (item.element) ApplyElementMovement(item, newPos - item.start);
			item.start = newPos;
			item.index = to;

			UpdateVisibility(1);
			UpdateVisibility(-1);

		}

		protected virtual void ApplyElementMovement(Storage storage, float change) {
			PlaceElement(storage);
		}

		public virtual Storage InsertElement(int index, RectTransform prefab) => InsertElement(index, new Data() { prefab = prefab });
		public virtual Storage InsertElement(int index, Data data) {
			if (index < 0 || index > items.Count) throw new IndexOutOfRangeException();
			var res = CreateStorage(index, data, default);
			var size = res.size = GetSize(data.prefab);

			var pos = res.start = index == 0 || !items.Any() ? 0 : items[index - 1].end;
			if (Visibility(res.start, res.end) == 0) {
				Show(res);
				visCount++;
			}

			items.Insert(index, res);

			for (int i = index + 1; i < items.Count; i++) {
				var item = items[i];
				item.start += size;
				item.index++;
				if (item.element) {
					if (index < visI) {
						PlaceElement(item);
					} else {
						ApplyElementMovement(item, size);
					}
				}
			}
			if (index < visI) {
				rectTransform.localPosition -= new Vector3(vertical ? 0 : size, vertical ? size : 0);
			}

			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			return res;
		}

		public virtual Storage AddElement(RectTransform prefab) => AddElement(new Data() { prefab = prefab });
		public virtual Storage AddElement(Data data) {
			var res = CreateStorage(items.Count, data, default);
			var size = res.size = GetSize(data.prefab);
			var pos = res.start = items.Any() ? items.Last().end : 0;
			if (visI + visCount - 1 == items.Count || Visibility(res.start, res.end) == 0) {
				Show(res);
				visCount++;
			}
			items.Add(res);
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			return res;
		}

		protected virtual void Show(Storage storage) {
			if (!storage.element) {
				storage.element = storage.data.CreateElement(storage, this);
				PlaceElement(storage);
			}
		}

		protected virtual void Hide(Storage storage) {
			if (storage.element) {
				storage.element.UpdateData(storage.data);
				storage.element.OnHide(this);
				storage.element = null;
			}
		}

		protected Storage CreateStorage(int index, Data data, VirtualLayoutElement element) {
			var res = Storage.CreateInstance<Storage>();
			res.data = data;
			res.index = index;
			res.element = element;
			return res;
		}

		protected void PlaceElement(Storage storage) {
			var rt = (RectTransform)storage.element.transform;
			rt.SetInsetAndSizeFromParentEdge(vertical ? RectTransform.Edge.Top : RectTransform.Edge.Left, storage.start, storage.size);
			rt.anchorMin = new(vertical ? 0 : rt.anchorMin.x, vertical ? rt.anchorMin.y : 0);
			rt.anchorMax = new(vertical ? 1 : rt.anchorMax.x, vertical ? rt.anchorMax.y : 1);
			rt.offsetMin = new(vertical ? 0 : rt.offsetMin.x, vertical ? rt.offsetMin.y : 0);
			rt.offsetMax = new(vertical ? 0 : rt.offsetMax.x, vertical ? rt.offsetMax.y : 0);
		}

		float ILayoutElement.minWidth => -1;
		float ILayoutElement.preferredWidth => vertical ? -1 : items.Any() ? items.Last().end : -1;
		float ILayoutElement.flexibleWidth => -1;
		float ILayoutElement.minHeight => -1;
		float ILayoutElement.preferredHeight => !vertical ? -1 : items.Any() ? items.Last().end : 0;
		float ILayoutElement.flexibleHeight => -1;
		int ILayoutElement.layoutPriority => 0;
		void ILayoutElement.CalculateLayoutInputHorizontal() { }
		void ILayoutElement.CalculateLayoutInputVertical() { }

	}
}


#if UNITY_EDITOR
namespace Muc.Components.Editor {

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
	[CustomEditor(typeof(VirtualLayoutGroup), true)]
	public class VirtualLayoutGroupEditor : Editor {

		[Serializable]
		public class CustomData : VirtualLayoutElement.Data {
			public Color color;
			public override VirtualLayoutElement CreateElement(Storage storage, VirtualLayoutGroup group) {
				var res = base.CreateElement(storage, group);
				res.GetComponentInChildren<Graphic>().color = color;
				return res;
			}
		}

		VirtualLayoutGroup t => (VirtualLayoutGroup)target;

		// SerializedProperty property;

		void OnEnable() {
			// property = serializedObject.FindProperty(nameof(property));
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawDefaultInspector();

			if (GUILayout.Button("Insert test element at 10")) {
				t.InsertElement(10, new CustomData() {
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
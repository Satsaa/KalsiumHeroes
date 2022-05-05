using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Muc.Data;

public class TextQueue : MonoBehaviour {

	[Serializable]
	public class QueueItem {
		public readonly string text;
		public readonly float minLinger;
		public readonly float maxLinger;
		public readonly TextQueueText textPrefab;
		public readonly bool cacheable;
		public TextQueueText instance;

		public QueueItem(string text, float minLinger, float maxLinger, TextQueueText textPrefab, bool cacheable) {
			this.text = text;
			this.minLinger = minLinger;
			this.maxLinger = maxLinger;
			this.textPrefab = textPrefab;
			this.cacheable = cacheable;
		}
	}

	[Tooltip("Prefab for default text instances")]
	public TextQueueText defaultTextPrefab;

	[Tooltip("Amount of movement")]
	public Vector3 offset = Vector3.down;

	public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(1, 1, 0, 0);

	[Tooltip("Default duration of animations")]
	public float fadeDuration = 0.5f;

	[Tooltip("Maximum duration of one text being shown before being automatically fade out")]
	public float maxLinger = 2f;

	[Tooltip("Minmum duration of one text being shown before being automatically fade out")]
	public float minLinger = 0f;

	[SerializeField]
	private List<QueueItem> queue;

	[SerializeReference]
	private QueueItem current = null;

	[SerializeField] float fadeInTime = -1;
	[SerializeField] float lingerTime = -1;
	[SerializeField] float fadeOutTime = -1;

	public List<TextQueueText> defaultPool;
	public SerializedDictionary<TextQueueText, List<TextQueueText>> customPool;

	void Update() {
		if (queue.Any() && (current == null || Time.time - lingerTime >= current.minLinger)) {
			var upcoming = queue.First();
			if (fadeInTime == -1) {
				fadeInTime = Time.time;
				upcoming.instance = GetInstance(upcoming);
				upcoming.instance.OnShow(upcoming.text);
			}
			var t = Mathf.Max(0, (Time.time - fadeInTime) / fadeDuration);
			if (t < 1) {
				var p = offset - moveCurve.Evaluate(t) * offset;
				var a = fadeInCurve.Evaluate(t);
				upcoming.instance.OnFadeIn(t, p, a);
			} else if (current == null) {
				queue.RemoveAt(0);
				current = upcoming;
				lingerTime = Time.time;
				fadeInTime = -1;
				fadeOutTime = -1;
				var hasMinLingered = Time.time - lingerTime >= current.minLinger;
				var hasMaxLingered = Time.time - lingerTime >= current.maxLinger;
				upcoming.instance.OnLingerStart(hasMinLingered && (hasMaxLingered || queue.Count > 0));
			}
		}
		if (current != null) {
			var hasMinLingered = Time.time - lingerTime >= current.minLinger;
			var hasMaxLingered = Time.time - lingerTime >= current.maxLinger;
			if (hasMinLingered && (hasMaxLingered || queue.Count > 0)) {
				if (fadeOutTime == -1) {
					fadeOutTime = Time.time;
					current.instance.OnFadeOutStart();
				}
				var t = Mathf.Max(0, (Time.time - fadeOutTime) / fadeDuration);
				if (t < 1) {
					var p = moveCurve.Evaluate(t) * -offset;
					var a = fadeOutCurve.Evaluate(1 - t);
					current.instance.OnFadeOut(t, p, a);
				} else {
					lingerTime = -1;
					fadeOutTime = -1;
					current.instance.OnHide(TryPool(current));
					current = null;
					Update(); // Make next item come quickly
				}
			} else {
				var t = Mathf.Max(0, (Time.time - lingerTime) / (queue.Count > 0 ? current.minLinger : current.maxLinger));
				current.instance.OnLinger(t);
			}
		}
	}

	protected TextQueueText GetInstance(QueueItem item) {
		if (item.cacheable) {
			if (item.textPrefab == defaultTextPrefab) {
				if (defaultPool.Any()) {
					var res = defaultPool[^1];
					defaultPool.RemoveAt(defaultPool.Count - 1);
					return res;
				}
			} else {
				if (customPool.TryGetValue(item.textPrefab, out var pool)) {
					if (pool.Any()) {
						var res = pool[^1];
						pool.RemoveAt(pool.Count - 1);
						return res;
					}
				}
			}
		}
		return Instantiate(item.textPrefab, transform);
	}

	protected bool TryPool(QueueItem item) {
		if (item.cacheable) {
			if (item.textPrefab == defaultTextPrefab) {
				defaultPool.Add(item.instance);
				return true;
			} else {
				if (!customPool.TryGetValue(item.textPrefab, out var pool)) {
					pool = new();
				}
				pool.Add(item.instance);
				return true;
			}
		}
		return false;
	}

	/// <summary> Add a new text to the queue </summary>
	/// <param name="text"></param>
	/// <param name="minLinger"></param>
	/// <param name="maxLinger"></param>
	/// <param name="textPrefab"></param>
	public void QueueText(string text, float minLinger = -1, float maxLinger = -1, TextQueueText textPrefab = null, bool cacheable = true) {
		queue.Add(new(text, minLinger <= 0 ? this.minLinger : minLinger, minLinger <= 0 ? this.maxLinger : maxLinger, textPrefab != null ? textPrefab : defaultTextPrefab, cacheable));
	}

}

#if UNITY_EDITOR
namespace Editors {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.UI;
	using Object = UnityEngine.Object;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TextQueue), true)]
	public class TextQueueEditor : Editor {

		new TextQueue target => (TextQueue)base.target;
		new IEnumerable<TextQueue> targets => base.targets.Cast<TextQueue>();

		int i = 0;

		public override void OnInspectorGUI() {

			DrawDefaultInspector();

			if (ButtonField(new("Test"))) {
				target.QueueText($"Test {i++}");
				target.QueueText($"Test {i++}");
			}
		}
	}
}
#endif
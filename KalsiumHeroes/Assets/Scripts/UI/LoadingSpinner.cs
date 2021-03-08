
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class LoadingSpinner : MonoBehaviour {

	[SerializeField] float decay = 1f;
	[SerializeField] TMP_Text _text;
	[SerializeField, HideInInspector] Animator animator;
	public string text { get => _text.text; set => _text.text = value; }

	[SerializeField] Image blade1;
	[SerializeField] Image blade2;
	[SerializeField] Image blade3;
	[SerializeField] Image blade4;
	[SerializeField] Image blade5;
	[SerializeField] Image blade6;
	IEnumerable<Image> GetBlades() {
		yield return blade1;
		yield return blade2;
		yield return blade3;
		yield return blade4;
		yield return blade5;
		yield return blade6;
	}

	void Start() {
		animator = GetComponent<Animator>();
		_text = GetComponent<TMP_Text>();
		foreach (var blade in GetBlades()) {
			var color = blade.color;
			color.a = 0;
			blade.color = color;
		}
	}

	void Update() {
		foreach (var blade in GetBlades()) {
			var color = blade.color;
			color.a -= decay * Time.deltaTime;
			blade.color = color;
		}
	}

	/// <summary> When the task finishes the spinner is hidden. </summary>
	public async void SetTask(Task task) {
		try {
			await task;
			Hide();
		} catch (System.Exception) {
			Hide();
			throw;
		}
	}

	public void Hide() {
		animator.SetTrigger("Hide");
	}

	public void ResetAlpha1() {
		var color = blade1.color;
		color.a = 1;
		blade1.color = color;
	}

	public void ResetAlpha2() {
		var color = blade1.color;
		color.a = 1;
		blade2.color = color;
	}

	public void ResetAlpha3() {
		var color = blade1.color;
		color.a = 1;
		blade3.color = color;
	}

	public void ResetAlpha4() {
		var color = blade1.color;
		color.a = 1;
		blade4.color = color;
	}

	public void ResetAlpha5() {
		var color = blade1.color;
		color.a = 1;
		blade5.color = color;
	}

	public void ResetAlpha6() {
		var color = blade1.color;
		color.a = 1;
		blade6.color = color;
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

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LoadingSpinner), true)]
	public class SpinBladesEditor : Editor {

		LoadingSpinner t => (LoadingSpinner)target;

		public override void OnInspectorGUI() {
			serializedObject.Update();

			DrawDefaultInspector();

			if (Application.isPlaying) {
				if (GUILayout.Button("Hide")) {
					t.Hide();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
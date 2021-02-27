using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceDisplay : MonoBehaviour {

	public Text fps;

	void Awake() => gameObject.SetActive(true);
	void Update() => fps.text = $"{Mathf.RoundToInt(1f / Time.smoothDeltaTime)} FPS";

}
using System.Collections;
using System.Collections.Generic;
using Muc.Systems.Camera;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class CameraDisabler : MonoBehaviour {

	[SerializeField, HideInInspector] new Camera camera;

	protected void Awake() {
		camera = GetComponent<Camera>();
	}

	protected void OnEnable() {
		foreach (var camera in FindObjectsOfType<Camera>(true)) {
			if (camera != this.camera) {
				camera.gameObject.SetActive(false);
			}
		}
	}

	protected void OnDisable() {
		foreach (var camera in FindObjectsOfType<Camera>(true)) {
			if (camera != this.camera) {
				camera.gameObject.SetActive(true);
			}
		}
	}

}

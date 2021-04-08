using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DynamicShadowDistance : MonoBehaviour {

	public UniversalRenderPipelineAsset pipeline;
	public Camera cam;
	public float planeDebth = -8;
	public float distanceStepInterveal = 5;

	void Start() {
		if (!cam) cam = GetComponent<Camera>();
		if (pipeline == null) {
			if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urpAsset) {
				pipeline = urpAsset;
			} else {
				Debug.LogWarning("Incompatible pipeline. Only URP pipeline is supported.");
			}
		}
	}

	void Update() {
		if (pipeline != null) {
			var ray = cam.ScreenPointToRay(new Vector3(cam.pixelWidth, cam.pixelHeight, 0));
			var plane = new Plane(Vector3.up, new Vector3(0, planeDebth, 0));
			if (plane.Raycast(ray, out var dist)) {
				Debug.DrawRay(ray.origin, ray.direction * dist, Color.green);
				pipeline.shadowDistance = dist - dist % distanceStepInterveal;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Unit))]
public class UnitUICanvasOrderer : MonoBehaviour {

  public Camera cam;
  public Canvas canvas;

  void OnValidate() => Init();
  void Reset() => Init();
  void Start() => Init();

  void Init() {
    if (!cam) cam = Camera.main;
    if (!canvas) canvas = GetComponentInChildren<Canvas>();
  }

  void LateUpdate() {
    var distance = Vector3.Distance(transform.position, cam.transform.position);
    canvas.sortingOrder = (int)(distance * -25);
  }
}

using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Unit))]
public class UnitStatsUI : MonoBehaviour {

  public Camera cam;
  [HideInInspector] public Unit unit;

  [Header("General")]

  [Tooltip("World space offset")]
  public Vector3 wsOffset;
  [Tooltip("Screen space offs et")]
  public Vector2 ssOffset;
  [Tooltip("Canvas element which is moved")]
  public RectTransform uiTransform;

  [Header("Health")]
  public RectTransform hpBar;
  public Text hpText;
  public float hpFullWidth = 100;
  [SerializeField, HideInInspector] RectTransform hpFgRt;


  void OnValidate() => Init();
  void Reset() => Init();
  void Start() => Init();

  void Init() {
    if (!unit) unit = GetComponent<Unit>();
    if (!cam) cam = Camera.main;
    if (!cam) FindObjectOfType<Camera>();
    if (hpBar) hpFgRt = hpBar.GetComponent<RectTransform>();
  }

  void LateUpdate() {
    if (uiTransform) uiTransform.transform.position = cam.WorldToScreenPoint(transform.position + wsOffset).Add(ssOffset);
    if (hpBar && hpFgRt) hpFgRt.sizeDelta = new Vector2((unit.unitData.health.value / unit.unitData.health.other * hpFullWidth), hpFgRt.sizeDelta.y);
    if (hpText) hpText.text = $"{Mathf.Ceil(unit.unitData.health.value)}/{Mathf.Ceil(unit.unitData.health.other)}";
  }
}

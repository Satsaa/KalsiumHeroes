using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Unit))]
public class UnitAbilityUI : MonoBehaviour {

  public Camera cam;
  [HideInInspector] public Unit unit;

  [Header("General")]

  [Tooltip("World space offset")]
  public Vector3 wsOffset;
  [Tooltip("Screen space offs et")]
  public Vector2 ssOffset;
  [Tooltip("Canvas element which is moved")]
  public RectTransform uiTransform;

  [Header("Ability Buttons")]

  [Tooltip("A UI element with a Button and an Image component")]
  public GameObject prefab;
  public GameObject parent;

  public Color disabledColor = Color.grey;
  public Color enabledColor = Color.white;

  public float distance = 74;
  public List<GameObject> items;

  void OnValidate() => Init();
  void Reset() => Init();
  void Start() => Init();

  void Init() {
    if (!unit) unit = GetComponent<Unit>();
    if (!cam) cam = Camera.main;
    if (!cam) FindObjectOfType<Camera>();
  }

  void LateUpdate() {
    if (uiTransform) uiTransform.transform.position = cam.WorldToScreenPoint(transform.position + wsOffset).Add(ssOffset);

    if (prefab && parent) {

      var abilities = unit.abilities;
      if (items.Any(v => !v)) {
        foreach (var item in items) {
          if (item) {
            if (Application.isEditor) DestroyImmediate(item);
            else Destroy(item);
          }
        }
        items.Clear();
      }

      var abilitiesChanged = abilities.Length != items.Count;

      if (abilitiesChanged) {
        while (abilities.Length > items.Count) {
          items.Add(Instantiate(prefab, parent.transform));
        }
        while (abilities.Length < items.Count) {
          var last = items[items.Count - 1];
          items.RemoveAt(items.Count - 1);
          Destroy(last);
        }
      }

      for (int i = 0; i < abilities.Length; i++) {
        var ability = abilities[i];
        var item = items[i];
        var image = item.GetComponent<Image>();
        var button = item.GetComponent<Button>();

        if (abilitiesChanged) {
          image.sprite = ability.data.sprite;
          item.GetComponentInChildren<Text>().text = ability.data.displayName;
        }

        button.onClick.RemoveAllListeners();
        if (unit == Game.rounds.current && Game.events.finished && ability.IsReady()) {
          image.color = enabledColor;
          button.onClick.AddListener(() => {
            Game.targeting.TryStartSequence(ability.GetTargeter());
          });
        } else {
          image.color = disabledColor;
        }
      }

      var minX = -distance * (abilities.Length - 1) / 2f;
      for (int i = 0; i < items.Count; i++) {
        var item = items[i];
        var rt = item.GetComponent<RectTransform>();

        var pos = rt.localPosition;
        pos.x = minX + i * distance;
        rt.localPosition = pos;
      }
    }
  }
}

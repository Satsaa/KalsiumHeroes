using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;

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

  [Tooltip("A prefab with AbilityButton component")]
  public GameObject prefab;
  public GameObject parent;

  public Color disabledColor = Color.grey;
  public Color enabledColor = Color.white;

  public float distance = 74;
  public List<AbilityButton> items;

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
      var passives = unit.passives;

      if (items.Any(v => !v)) {
        foreach (var item in items) {
          if (item) {
            if (Application.isEditor) DestroyImmediate(item);
            else Destroy(item);
          }
        }
        items.Clear();
      }

      var total = abilities.Length + passives.Length;
      var totalChanged = total != items.Count;

      if (totalChanged) {
        while (total > items.Count) {
          items.Add(Instantiate(prefab, parent.transform).GetComponent<AbilityButton>());
        }
        while (total < items.Count) {
          var last = items[items.Count - 1];
          items.RemoveAt(items.Count - 1);
          Destroy(last);
        }
      }

      for (int i = 0; i < abilities.Length; i++) {
        var ability = abilities[i];
        var item = items[i];

        if (!ability.data) continue;

        if (totalChanged) {
          item.fgImage.sprite = item.bgImage.sprite = ability.abilityData.sprite;
          item.bgImage.color = disabledColor;
          item.abilityText.text = ability.data.displayName;
          item.cooldownText.text = ability.abilityData.cooldown.value > 0 ? ability.abilityData.cooldown.value.ToString() : "";
          item.fgImage.fillAmount = 1;
        }

        if (unit != Game.rounds.current) {
          uiTransform.gameObject.SetActive(false);
        } else {
          uiTransform.gameObject.SetActive(true);
          item.chargeText.text = GetChargeText(ability.abilityData);
          item.abilityButton.onClick.RemoveAllListeners();
          if (Game.events.finished && ability.IsReady()) {
            item.fgImage.fillAmount = 1;
            item.cooldownText.text = "";
            item.fgImage.color = enabledColor;
            item.abilityButton.onClick.AddListener(() => { Game.targeting.TryStartSequence(ability.GetTargeter()); });
          } else {
            if (ability.abilityData.cooldown.other > 0 && ability.abilityData.charges.value <= 0) {
              item.fgImage.fillAmount = 1 - (float)ability.abilityData.cooldown.value / (float)ability.abilityData.cooldown.other;
              item.cooldownText.text = ability.abilityData.cooldown.value > 0 ? ability.abilityData.cooldown.value.ToString() : "";
              item.fgImage.color = enabledColor;
            } else {
              item.fgImage.color = disabledColor;
              item.fgImage.fillAmount = 1;
              item.cooldownText.text = "";
            }
          }
        }
      }

      for (int i = 0; i < passives.Length; i++) {
        var passive = passives[i];
        var item = items[i];

        if (!passive.data) continue;

        if (totalChanged) {
          item.fgImage.sprite = item.bgImage.sprite = passive.passiveData.sprite;
          item.bgImage.color = enabledColor;
          item.abilityText.text = passive.data.displayName;
          item.cooldownText.text = "";
          item.fgImage.fillAmount = 1;
        }

        if (unit != Game.rounds.current) {
          uiTransform.gameObject.SetActive(false);
        } else {
          uiTransform.gameObject.SetActive(true);
        }
      }

      // Layout
      if (totalChanged) {
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

  public bool ShowCharges(AbilityData abilityData) {
    if (abilityData.charges.other <= 1) return false;
    if (abilityData.cooldown.other <= 1) return false;
    return true;
  }

  public string GetChargeText(AbilityData abilityData) {
    return ShowCharges(abilityData) ? abilityData.charges.value.ToString() : "";
  }
}

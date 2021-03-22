using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMProColorSwitcher : MonoBehaviour
{
    TextMeshProUGUI text;
    Button parentButton;

    public Color normalColor;
    public Color hoverColor;
    public Color pressColor;
    public Color disabledColor;
    private Color targetColor;

    public AnimationCurve transitionCurve;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (transform.parent != null) {
            if (transform.parent.GetComponent<Button>() != null) parentButton = transform.parent.GetComponent<Button>();
        }
        if (parentButton != null && !parentButton.interactable) text.color = disabledColor;
        else text.color = normalColor;
        targetColor = text.color;
    }

    void Update()
    {
        if (parentButton.interactable) {
            if (text.color == disabledColor) targetColor = normalColor;
            if (text.color != targetColor) {
                ChangeColor();
            }
        } else if (targetColor != disabledColor) {
            targetColor = disabledColor;
            ChangeColor();
        }
    }

    void ChangeColor() {
        text.color = targetColor;
    }

    public void HoverColor() {
        targetColor = hoverColor;
    }

    public void PressColor() {
        targetColor = pressColor;
    }

    public void NormalColor() {
        targetColor = normalColor;
    }
}

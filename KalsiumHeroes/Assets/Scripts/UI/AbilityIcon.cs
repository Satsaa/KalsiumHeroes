using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour {

  [HideInInspector]
  public Ability ability;

  public Button abilityButton;
  public Text abilityText;
  public Text cooldownText;
  public Text chargeText;

  public Image fgImage;
  public Image bgImage;

}

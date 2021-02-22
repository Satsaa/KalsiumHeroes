using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassiveIcon : MonoBehaviour {

	[HideInInspector]
	public Passive passive;

	public TMP_Text displayName;
	public Image sprite;

	public void SetPassive(Passive passive) {
		this.passive = passive;
		displayName.text = passive.data.displayName ?? passive.data.identifier.Replace("_", " ");
		sprite.sprite = passive.data.sprite;
	}
}


using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AbilityButtonsUI : MonoBehaviour {

	[Tooltip("Follows the unit and is disabled or enabled depending on the turn.")]
	public GameObject parent;

	[Tooltip("CanvasGroup Component. Fades out the abilities when casting.")]
	public CanvasGroup group;

	[Tooltip("The UI will be clamped inside this RectTransform.")]
	public RectTransform areaConstraint;

}
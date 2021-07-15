
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;
using Muc.Components.Extended;
using System.Reflection;

[RequireComponent(typeof(TooltipAnimator))]
public class TextTooltip : Tooltip, IValueReceiver {

	public TMPro.TMP_Text text;

	bool IValueReceiver.TryHandleValue(object value) {
		if (value is String str) {
			text.text = str;
			return true;
		}
		return false;
	}
}

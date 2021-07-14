
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
public class GeneratedTooltip : Tooltip, IValueReceiver {

	public TMPro.TMP_Text text;

	protected List<Attribute> listened; // todo

	public void Generate(DataObjectData data, DataObjectData source) {
		var title = Lang.GetStr($"{data.identifier}_DisplayName");
		var hasDesc = Lang.TryGetStr($"{data.identifier}_Description", out var desc);
		var hasLore = Lang.TryGetStr($"{data.identifier}_Lore", out var lore);

		text.text = "<line-height=115%>";
		text.text += $"<style=title>{title}</style>\n";
		if (hasDesc) text.text += $"</line-height><style=normal>{desc}</style><line-height=115%>\n";

		var props = data.GetType()
			.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
			.Where(v => typeof(Attribute).IsAssignableFrom(v.FieldType))
			.OrderBy(field => field.MetadataToken);
		var pairs = props.Select(v => (data: v.GetValue(data) as IAttribute, source: v.GetValue(source) as IAttribute));
		foreach (var pair in pairs) {
			var tooltip = pair.data.TooltipText(pair.source);
			if (!String.IsNullOrEmpty(tooltip)) text.text += $"{tooltip}\n";
		}

		text.text += $"</line-height>";

		if (hasLore) text.text += $"<style=quote>{lore}</style>\n";

	}

	bool IValueReceiver.TryHandleValue(object value) {
		if (value is DataObject obj) {
			Generate(obj.data, obj.source);
			return true;
		}
		if (value is DataObjectData data) {
			Generate(data, data);
			return true;
		}
		return false;
	}
}
